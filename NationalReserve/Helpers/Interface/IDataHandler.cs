namespace NationalReserve.Helpers.Interface
{
    interface IDataHandler
    {
        void ReadAsync();
        void AddObject();
        void PhysicalDelete();
        void LogicalDelete();
        void LogicalRecover();
        void SaveAsync();
        void ExportTable();
        void ImportTable();
        string ValidationErrorMessage();
    }
}
