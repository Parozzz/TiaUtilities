using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Siemens.Engineering;
using Siemens.Engineering.Compiler;
using Siemens.Engineering.Library.Types;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;

namespace FCFBConverter.Utility
{
    public class Block
    {
        private PlcBlock _block;
        private ExitState _state;
        public string BlockName { get; }
        
        public bool IsChangeable
        {
            get { return _state == ExitState.IsChangeable; }
        }

        public bool ChangeSuccessful
        {
            get { return _state == ExitState.Successful; }
        }

        public Block(PlcBlock plcBlock)
        {
            _block = plcBlock;
            BlockName = plcBlock.Name;
            SetChangeableState();
        }

        private static string RemoveInvalidFileNameChars(string name)
        {
            Path.GetInvalidFileNameChars().ToList().ForEach(c => name = name.Replace(c.ToString(), "_"));
            return name;
        }

        private void SetChangeableState()
        {
            if (_block.IsKnowHowProtected)
            {
                _state = ExitState.BlockIsKnowHowProtected;
                return;
            }

            if (_block.ProgrammingLanguage != ProgrammingLanguage.SCL && _block.ProgrammingLanguage != ProgrammingLanguage.STL &&
                _block.ProgrammingLanguage != ProgrammingLanguage.LAD && _block.ProgrammingLanguage != ProgrammingLanguage.FBD)
            {
                _state = ExitState.ProgrammingLanguageNotSupported;
                return;
            }

            if (!_block.IsConsistent)
            {
                try
                {
                    if (_block.GetService<ICompilable>().Compile().State == CompilerResultState.Error)
                    {
                        _state = ExitState.CouldNotCompile;
                        return;
                    }
                }
                catch
                {
                    _state = ExitState.CouldNotCompile;
                    return;
                }
            }

            if (_block.GetService<LibraryTypeInstanceInfo>() != null)
            {
                _state = ExitState.IsLibraryType;
                return;
            }

            _state = ExitState.IsChangeable;
        }

        public void ChangeBlockType(Settings settings)
        {
            var dirPath = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), AppDomain.CurrentDomain.FriendlyName);
            var dir = Directory.CreateDirectory(dirPath);
            var filePath = Path.Combine(dir.FullName, RemoveInvalidFileNameChars(BlockName) + ".xml");

            var blockGroup = (PlcBlockGroup) _block.Parent;

            //Export selected block as SimaticML file
            if (Util.ExportBlock(_block, filePath) != true)
            {
                _state = ExitState.CouldNotExport;
                return;
            }

            //Make changes to the exported SimaticML file
            bool changeSuccessful = false;
            if (_block is FB)
            {
                changeSuccessful = XmlEdit.FBtoFC(filePath, settings.StaticVariables);
            }
            else if (_block is FC)
            {
                changeSuccessful = XmlEdit.FCtoFB(filePath, settings.RemoveReturnValue);
            }

            if (!changeSuccessful)
            {
                _state = ExitState.XmlEditingError;
                return;
            }

            //Import edited SimaticML file
            IList<PlcBlock> importedBlocks;
            try
            {
                importedBlocks = blockGroup.Blocks.Import(new FileInfo(filePath), ImportOptions.Override, SWImportOptions.IgnoreMissingReferencedObjects | SWImportOptions.IgnoreStructuralChanges | SWImportOptions.IgnoreUnitAttributes);
            }
            catch(Exception ex)
            {
                Trace.TraceError("Exception during import:" + Environment.NewLine + ex);
                _state = ExitState.CouldNotImport;
                return;
            }
            finally
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch
                {
                    // Silently ignore file operations
                }
            }

            _block = importedBlocks.First();

            //Remove IDBs if selected
            if (settings.RemoveIDBs && _block is FC)
            {
                Util.RemoveIDBs(_block as FC);
            }

            //Open blocks in editors
            if (settings.OpenBlocksInEditor)
            {
                _block.ShowInEditor();
            }

            _state = ExitState.Successful;
        }

        public string GetStateText()
        {
            switch (_state)
            {
                case ExitState.BlockIsKnowHowProtected:
                    return "The block is know-how protected.";
                case ExitState.ProgrammingLanguageNotSupported:
                    return "The programming language of the block is not supported.";
                case ExitState.CouldNotCompile:
                    return "The block could not be compiled.";
                case ExitState.CouldNotExport:
                    return "The block could not be exported.";
                case ExitState.CouldNotImport:
                    return "The block could not be imported.";
                case ExitState.IsChangeable:
                    return "The block type is changeable.";
                case ExitState.XmlEditingError:
                    return "Error during editing of SimaticML file";
                case ExitState.IsLibraryType:
                    return "Library types are not supported.";
                case ExitState.Successful:
                    return "The block type was changed successfully.";
                default:
                    return "";
            }
        }

        public string GetActionText()
        {
            switch (_state)
            {
                case ExitState.BlockIsKnowHowProtected:
                    return "Remove the know-how protection.";
                case ExitState.ProgrammingLanguageNotSupported:
                    return "Change the programming language of the block.";
                case ExitState.CouldNotCompile:
                    return "Compile the block without errors.";
                case ExitState.CouldNotExport:
                    return "Please report this issue for further investigation.";
                case ExitState.CouldNotImport:
                    return "Please report this issue for further investigation.";
                case ExitState.IsChangeable:
                    return "No action required.";
                case ExitState.XmlEditingError:
                    return "Please report this issue for further investigation.";
                case ExitState.IsLibraryType:
                    return "Terminate the library type connection.";
                case ExitState.Successful:
                    return "No action required.";
                default:
                    return "";
            }
        }
    }
}