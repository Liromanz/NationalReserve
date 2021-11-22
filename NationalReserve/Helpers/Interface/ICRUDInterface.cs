namespace NationalReserve.Helpers.Interface
{
    interface ICRUDInterface
    {
        void CreateAsync();
        void ReadAsync();
        void UpdateAsync();
        void DeleteAsync();
    }
}
