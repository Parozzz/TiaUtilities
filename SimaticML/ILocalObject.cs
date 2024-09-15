namespace SimaticML
{
    public interface ILocalObject
    {
        //LocalObjectData GetLocalObjectData();
        void UpdateLocalUId(IDGenerator localIDGeneration);
        void SetUId(uint uid);
        uint GetUId();
    }
}
