using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Resources
{
    public static class ImageResources
    {
        private const string FOLDER = "Resources/Images";

        public static Image ALIAS_GENERATOR { get => Image.FromFile($"{FOLDER}/AliasGenerator.png"); }
        public static Image ALARM_GENERATOR { get => Image.FromFile($"{FOLDER}/AlarmGenerator.png"); }
        public static Image DUPLICATE_DB { get => Image.FromFile($"{FOLDER}/DuplicateDB.png"); }

        public static Image CLOSE_193002 { get => Image.FromFile($"{FOLDER}/close-193002.png"); }
        public static Image CLOSE_193002_FF001C { get => Image.FromFile($"{FOLDER}/close-193002-FF001C.png"); }
        public static Image EDIT_562275 { get => Image.FromFile($"{FOLDER}/edit-562275.png"); }

        public static Image DOUBLE_ARROW_DOWN_3134107 { get => Image.FromFile($"{FOLDER}/double-arrow-down-3134107.png"); }
        public static Image DOUBLE_ARROW_UP_3134107 { get => Image.FromFile($"{FOLDER}/double-arrow-up-3134107.png"); }

        public static Image DRAG_8187494 { get => Image.FromFile($"{FOLDER}/drag-8187494.png"); }
    }
}
