

namespace MFarm.Save
{
    public interface ISaveable
    {

        string guid { get; }
        void RegisterSaveable()
        {
            SaveLoadManager.Instance.RegisterSaveable(this);
        }
        GameSaveData GenerateSaveDate();

        void RestoreData(GameSaveData saveData);
    }
}

