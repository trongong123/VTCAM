namespace EQX.Core.Common
{
    public interface IBackupable<T>
    {
        void Save();
        T Load();
    }
}
