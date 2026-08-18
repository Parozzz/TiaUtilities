using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiaUtilities.Utility.Extensions
{
    public static class ControlExtensions
    {
        public static T? GetTag<T>(this Control control) => control.Tag is T t? t : default;

        public static T GetTagNotNull<T>(this Control control) => Validate.NotNull(GetTag<T>(control));
    }
}
