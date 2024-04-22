using Newtonsoft.Json;
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
            ADDRESS = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("Address"));
            IO_NAME = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("IOName"));
            DB_NAME = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("DBName"));
            VARIABLE = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("Variable"));
            COMMENT = GridDataColumn.GetFromReflection(COLUMN_COUNT++, type.GetProperty("Comment"));

            COLUMN_LIST = new List<GridDataColumn>();
            foreach (var field in type.GetFields())
            {
                if (field.IsStatic && field.FieldType == typeof(GridDataColumn))
                {
                    COLUMN_LIST.Add((GridDataColumn)field.GetValue(null));
                }
            }
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
            if (string.IsNullOrEmpty(this.Address) || this.IsEmpty())
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
                if (config.MemoryType == IOMemoryTypeEnum.DB)
                {
                    prefix = this.DBName + "." + (this.GetMemoryArea() == SimaticMemoryArea.INPUT ? config.PrefixInputDB : config.PrefixOutputDB);
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
            if (string.IsNullOrEmpty(Address))
            {
                return 0;
            }

            int firstDigit = 0;
            for (var x = 0; x < Address.Length; x++)
            {
                var c = Address[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            var substring = Address.Substring(firstDigit);
            return uint.Parse(substring.Split('.')[1]);
        }

        public uint GetAddressByte()
        {
            if (string.IsNullOrEmpty(Address))
            {
                return 0;
            }

            int firstDigit = 0;
            for (var x = 0; x < Address.Length; x++)
            {
                var c = Address[x];
                if (Char.IsDigit(c))
                {
                    firstDigit = x;
                    break;
                }
            }

            var substring = Address.Substring(firstDigit);
            return uint.Parse(substring.Split('.')[0]);
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
