using System.Runtime.CompilerServices;

namespace TiaUtilities
{
    public interface ICleanable
    {
        public bool IsDirty();

        public void Wash();
    }
}
