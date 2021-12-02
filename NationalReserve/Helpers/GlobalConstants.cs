namespace NationalReserve.Helpers
{
    public static class GlobalConstants
    {
        public const string UrlBase = "https://apinatreserve.azurewebsites.net/api";

        public const string ErrorMessage = "Произошла ошибка. Сообщение: ";
        public const string UnsetErrorMessage = "Произошла ошибка при отправке данных.";
        public const string LoginPasswordError = "Неверный логин или пароль";

        public const string SuccessCreateMessage = "Данные успешно добавлены!";
        public const string SuccessUpdateMessage = "Данные успешно изменены!";
        public const string SuccessDeleteMessage = "Данные успешно удалены!";

        public const string DeleteQuestionMessage = "Вы точно хотите удалить запись? Данные будут утеряны";

    }
}
