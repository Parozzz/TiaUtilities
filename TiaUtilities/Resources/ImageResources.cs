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

        public static Image ALIAS_GENERATOR { get => GetImage("AliasGenerator.png"); }
        public static Image ALARM_GENERATOR { get => GetImage("AlarmGenerator.png"); }
        public static Image DUPLICATE_DB { get => GetImage("DuplicateDB.png"); }

        public static Image A_TO_Z_72773 { get => GetImage("a-to-z-72773.png"); }
        public static Image ADD_501366_007435 { get => GetImage("add-501366-007435.png"); }
        public static Image CLOSE_193002 { get => GetImage("close-193002.png"); }
        public static Image CLOSE_193002_FF001C { get => GetImage("close-193002-FF001C.png"); }
        public static Image EDIT_562275 { get => GetImage("edit-562275.png"); }

        public static Image DOUBLE_ARROW_DOWN_3134107 { get => GetImage("double-arrow-down-3134107.png"); }
        public static Image DOUBLE_ARROW_UP_3134107 { get => GetImage("double-arrow-up-3134107.png"); }

        public static Image DRAG_8187494 { get => GetImage($"drag-8187494.png"); }

        private static Image GetImage(string imageName)
        {
            return Image.FromFile($"{FOLDER}/{imageName}");
        }
    }
}
