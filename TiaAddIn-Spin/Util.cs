﻿
/* Modifica senza merge dal progetto 'AddIn_V16'
Prima:
using System;
using System.Diagnostics;
using System.IO;
using Siemens.Engineering;
Dopo:
using Siemens.Engineering;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using System;
*/

/* Modifica senza merge dal progetto 'AddIn_V17'
Prima:
using System;
using System.Diagnostics;
using System.IO;
using Siemens.Engineering;
Dopo:
using Siemens.Engineering;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using System;
*/

/* Modifica senza merge dal progetto 'AddIn_V18'
Prima:
using System;
using System.Diagnostics;
using System.IO;
using Siemens.Engineering;
Dopo:
using Siemens.Engineering;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using System;
*/

/* Modifica senza merge dal progetto 'AddIn_V19'
Prima:
using System;
using System.Diagnostics;
using System.IO;
using Siemens.Engineering;
Dopo:
using Siemens.Engineering;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Tags;
using System;
*/
using Siemens.Engineering;
using Siemens.Engineering.SW.Blocks;


/* Modifica senza merge dal progetto 'AddIn_V16'
Prima:
using System.Diagnostics;
using System;
Dopo:
using System;
using System.Diagnostics;
*/

/* Modifica senza merge dal progetto 'AddIn_V17'
Prima:
using System.Diagnostics;
using System;
Dopo:
using System;
using System.Diagnostics;
*/

/* Modifica senza merge dal progetto 'AddIn_V18'
Prima:
using System.Diagnostics;
using System;
Dopo:
using System;
using System.Diagnostics;
*/

/* Modifica senza merge dal progetto 'AddIn_V19'
Prima:
using System.Diagnostics;
using System;
Dopo:
using System;
using System.Diagnostics;
*/
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace SpinAddin.Utility
{
    public static class Util
    {
        public static bool IsItalian()
        {
            return CultureInfo.InstalledUICulture.EnglishName.ToLowerInvariant().Contains("italy");
        }


        public static Form CreateForm()
        {
            Form form = new Form { Opacity = 1, ShowIcon = true, TopMost = true, StartPosition = FormStartPosition.CenterScreen, WindowState = FormWindowState.Normal };
            form.Show();
            form.TopMost = true;
            form.Activate();
            form.TopMost = false;
            return form;
        }

        public static DialogResult ShowMessageBox(string text, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            return MessageBox.Show(Util.CreateForm(), text, caption, buttons);
        }

        public static DialogResult ShowInputDialog(ref string input)
        {
            var size = new Size(250, 70);
            Form inputBox = new Form
            {
                Opacity = 1,
                ShowIcon = true,
                TopMost = true,
                StartPosition = FormStartPosition.CenterScreen,
                WindowState = FormWindowState.Normal,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ClientSize = size,
                Text = "Input"
            };

            var textBox = new TextBox
            {
                Size = new Size(size.Width - 10, 23),
                Location = new Point(5, 5),
                Text = input
            };
            inputBox.Controls.Add(textBox);

            var okButton = new Button
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new Size(75, 23),
                Text = "&OK",
                Location = new Point(size.Width - 80 - 80, 39)
            };
            inputBox.Controls.Add(okButton);

            var cancelButton = new Button
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new Size(75, 23),
                Text = "&Cancel",
                Location = new Point(size.Width - 80, 39)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        public static void RemoveIDBs(FC fc)
        {
            IEngineeringObject parent = fc;
            while (!(parent is PlcBlockSystemGroup))
            {
                parent = parent.Parent;
            }
            var blockSystemGroup = parent as PlcBlockSystemGroup;

            RemoveIDBs(blockSystemGroup.Blocks, fc.Name);
            foreach (PlcBlockUserGroup group in blockSystemGroup.Groups)
            {
                RemoveIDBs(group, fc.Name);
            }
        }

        private static void RemoveIDBs(PlcBlockComposition blocks, string blockName)
        {
            for (var i = 0; i < blocks.Count; i++)
            {
                if (blocks[i] is InstanceDB dB && dB.InstanceOfName == blockName)
                {
                    try
                    {
                        blocks[i].Delete();
                    }
                    catch
                    {
                        // Ignore failed deletion
                    }
                    i--;
                }
            }
        }

        private static void RemoveIDBs(PlcBlockGroup group, string blockName)
        {
            RemoveIDBs(group.Blocks, blockName);
            foreach (PlcBlockUserGroup blockGroup in group.Groups)
            {
                RemoveIDBs(blockGroup, blockName);
            }
        }

        public static Form GetForegroundWindow()
        {
            // Workaround for Add-In Windows to be shown in foreground of TIA Portal
            var form = new Form { Opacity = 0, ShowIcon = false };
            form.Show();
            form.TopMost = true;
            form.Activate();
            form.TopMost = false;
            return form;
        }

        public static string DesktopFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        public static void ShowMessage(string message, string caption)
        {
            MessageBox.Show(Util.CreateForm(), message, caption);
        }

        public static bool ShowExceptionMessage(Exception ex)
        {
            string message = "Message: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace;
            string caption = "An exception occoured while executing Spin Addin!";

            return MessageBox.Show(Util.CreateForm(), message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK;
        }
    }

    public static class ExportUtil
    {
        public delegate void ExportDelegate(FileInfo fileInfo, ExportOptions options);

        public delegate String NameDelegate(Object obj);

        public static bool Export(ExportDelegate exportDelegate, string filePath)
        {
            return Export(exportDelegate, filePath, ExportOptions.WithReadOnly);
        }

        public static bool Export(ExportDelegate exportDelegate, string filePath, ExportOptions options)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                exportDelegate(new FileInfo(filePath), options);
            }
            catch (Exception ex)
            {
                return Util.ShowExceptionMessage(ex); //If it return true it means that the error is ignored and will continue.
            }

            return true;
        }
    }
}