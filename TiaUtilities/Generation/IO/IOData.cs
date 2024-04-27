﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiaXmlReader.Generation.IO;
using TiaXmlReader.SimaticML;
using TiaXmlReader.Generation;
using TiaXmlReader.SimaticML.Enums;
using System.ComponentModel.DataAnnotations;
using TiaXmlReader.Localization;
using System.Drawing;
using System.Windows.Forms;
using TiaXmlReader.Generation.GridHandler.Data;

namespace TiaXmlReader.Generation.IO
{
    public class IOData : IGridData<IOConfiguration>
    {
        public static int COLUMN_COUNT = 0;
        //THESE IS THE ORDER IN WHICH THEY APPEAR!
        public static readonly GridDataColumn ADDRESS;
        public static readonly GridDataColumn IO_NAME;
        public static readonly GridDataColumn DB_NAME;
        public static readonly GridDataColumn VARIABLE;
        public static readonly GridDataColumn COMMENT;
        public static readonly List<GridDataColumn> COLUMN_LIST;

        static IOData()
        {
            var type = typeof(IOData);
            ADDRESS = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.Address));
            IO_NAME = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.IOName), "ioName");
            DB_NAME = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.DBName), "dbName");
            VARIABLE = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.Variable));
            COMMENT = GridDataColumn.GetFromReflection(type, COLUMN_COUNT++, nameof(IOData.Comment));

            COLUMN_LIST = GridDataColumn.GetStaticColumnList(type);
            COLUMN_LIST.Sort((x, y) => x.ColumnIndex.CompareTo(y.ColumnIndex));
        }

        [JsonProperty]
        [Display(Description = "DATA_ADDRESS", ResourceType = typeof(Localization.IO.IOGenerationLocalization))]
        public string Address { get; set; }

        [JsonProperty]
        [Display(Description = "DATA_IONAME", ResourceType = typeof(Localization.IO.IOGenerationLocalization))]
        public string IOName { get; set; }

        [JsonProperty]
        [Display(Description = "DATA_DBNAME", ResourceType = typeof(Localization.IO.IOGenerationLocalization))]
        public string DBName { get; set; }

        [JsonProperty]
        [Display(Description = "DATA_VARIABLE", ResourceType = typeof(Localization.IO.IOGenerationLocalization))]
        public string Variable { get; set; }

        [JsonProperty]
        [Display(Description = "DATA_COMMENT", ResourceType = typeof(Localization.IO.IOGenerationLocalization))]
        public string Comment { get; set; }

        public object this[int i]
        {
            get
            {
                if (i < 0 || i >= COLUMN_LIST.Count)
                {
                    throw new InvalidOperationException("Invalid index for get square bracket operator in IOData");
                }

                return COLUMN_LIST[i].PropertyInfo.GetValue(this);
            }
        }

        public GridDataPreview GetPreview(int column, IOConfiguration config)
        {
            if (string.IsNullOrEmpty(this.Address) || this.IsEmpty() || SimaticTagAddress.FromAddress(this.Address) == null)
            {
                return null;
            }

            if (column == IO_NAME)
            {
                if (string.IsNullOrEmpty(config.DefaultIoName) && string.IsNullOrEmpty(this.IOName))
                {
                    return null;
                }

                return new GridDataPreview()
                {
                    DefaultValue = config.DefaultIoName,
                    Value = this.IOName
                };
            }
            else if (column == VARIABLE)
            {
                if (string.IsNullOrEmpty(config.DefaultVariableName) && string.IsNullOrEmpty(this.Variable))
                {
                    return null;
                }

                var prefix = "";
                if (!string.IsNullOrEmpty(this.DBName))
                {
                    prefix = this.DBName + ".";
                }
                else if (config.MemoryType == IOMemoryTypeEnum.DB)
                {
                    prefix = config.DBName + "." + (this.GetMemoryArea() == SimaticMemoryArea.INPUT ? config.PrefixInputDB : config.PrefixOutputDB);
                }
                else if (config.MemoryType == IOMemoryTypeEnum.MERKER)
                {
                    prefix = this.GetMemoryArea() == SimaticMemoryArea.INPUT ? config.PrefixInputMerker : config.PrefixOutputMerker;
                }

                return new GridDataPreview()
                {
                    Prefix = prefix,
                    DefaultValue = config.DefaultVariableName,
                    Value = this.Variable
                };
            }


            return null;
        }

        public void LoadDefaults(IOConfiguration config)
        {
            if (string.IsNullOrEmpty(IOName))
            {
                IOName = config.DefaultIoName;
            }

            if (string.IsNullOrEmpty(Variable))
            {
                Variable = config.DefaultVariableName;
            }
        }

        public void ParsePlaceholders(GenerationPlaceholders placeholders)
        {
            Address = placeholders.Parse(Address);
            IOName = placeholders.Parse(IOName);
            Variable = placeholders.Parse(Variable);
            DBName = placeholders.Parse(DBName);
            Comment = placeholders.Parse(Comment);
        }

        public void Clear()
        {
            this.Address = this.IOName = this.DBName = this.Variable = this.Comment = "";
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(IOName) && string.IsNullOrEmpty(DBName) && string.IsNullOrEmpty(Variable);
        }

        public SimaticMemoryArea GetMemoryArea()
        {
            return SimaticMemoryAreaUtil.GetFromAddress(Address);
        }

        public uint GetAddressBit()
        {
            var tag = GetTagAddress();
            return tag == null ? 0 : tag.bitOffset;
        }

        public uint GetAddressByte()
        {
            var tag = GetTagAddress();
            return tag == null ? 0 : tag.byteOffset;
        }

        public SimaticTagAddress GetTagAddress()
        {
            return SimaticTagAddress.FromAddress(Address);
        }

        public IOData Clone()
        {
            return new IOData()
            {
                Address = this.Address,
                IOName = this.IOName,
                DBName = this.DBName,
                Variable = this.Variable,
                Comment = this.Comment
            };
        }
    }
}