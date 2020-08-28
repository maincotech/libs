namespace Maincotech.Data
{
    public interface IDatabaseInitializer
    {
        void CreateDataBase(string dbName);

        void Initialize(DbHelper context);
    }
}