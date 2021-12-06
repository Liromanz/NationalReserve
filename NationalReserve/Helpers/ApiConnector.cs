using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NationalReserve.Helpers
{
    public class ApiConnector : DataSender
    {
        public static async Task<ObservableCollection<T>> GetAll<T>(string tableName)
        {
            string response = await GetRequest(tableName);
            if (!response.Contains(GlobalConstants.ErrorMessage))
                return (ObservableCollection<T>)JsonConvert.DeserializeObject(response, typeof(ObservableCollection<T>));
            return null;
        }

        public static async Task<T> GetOne<T>(string tableName, int id)
        {
            string response = await GetRequest(tableName+ "/" +id);
            if (!response.Contains(GlobalConstants.ErrorMessage))
                return (T)JsonConvert.DeserializeObject(response, typeof(T));
            return default;
        }

        public static async Task<string> AddData<T>(string tableName, object modelValue)
        {
            var json = JsonConvert.SerializeObject(modelValue);
            string response = await PostRequest(tableName, json);
            return response;
        }

        public static async Task<string> UpdateData(string tableName, object modelValue, int? modelId)
        {
            if (modelId == null) return GlobalConstants.UnsetErrorMessage;

            var json = JsonConvert.SerializeObject(modelValue);
            string response = await PutRequest(tableName, json, modelId.Value);
            return response == HttpStatusCode.NoContent.ToString() ? GlobalConstants.SuccessUpdateMessage : response;
        }

        public static async Task<string> DeleteData(string tableName, int? modelId)
        {
            if (modelId == null) return GlobalConstants.UnsetErrorMessage;

            string response = await DeleteRequest(tableName, modelId.Value);
            return response == HttpStatusCode.NoContent.ToString() ? GlobalConstants.SuccessDeleteMessage : response;
        }
    }
}
