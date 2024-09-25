namespace TiaUtilities
{
    public interface ISaveable<T>
    {
        public T CreateSave();
        public void LoadSave(T save);
    }
}
