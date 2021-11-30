namespace NationalReserve.Helpers.Interface
{
    interface IOneToOneHandler
    {
        void CreateAsync();
        void ReadOneAsync(int id);
        void UpdateAsync();
        void DeleteAsync();
        string ValidationErrorMessage();
    }
}
