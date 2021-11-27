using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NationalReserve.Helpers
{
    public class DataSender
    {

        public static async Task<string> GetRequest(string tableName)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync($"{GlobalConstants.UrlBase}/{tableName}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                return GlobalConstants.ErrorMessage + e.Message;
            }
        }

        public static async Task<string> PostRequest(string tableName, string jsonToSend)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpContent content = new StringContent(jsonToSend, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"{GlobalConstants.UrlBase}/{tableName}", content);
                response.EnsureSuccessStatusCode();
                return GlobalConstants.SuccessCreateMessage;
            }
            catch (Exception e)
            {
                return GlobalConstants.ErrorMessage + e.Message;
            }
        }

        public static async Task<string> PutRequest(string tableName, string jsonToSend, int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpContent content = new StringContent(jsonToSend, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"{GlobalConstants.UrlBase}/{tableName}/{id}", content);
                return response.StatusCode.ToString();
            }
            catch (Exception e)
            {
                return GlobalConstants.ErrorMessage + e.Message;
            }
        }
        public static async Task<string> DeleteRequest(string tableName, int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.DeleteAsync($"{GlobalConstants.UrlBase}/{tableName}/{id}");
                return response.StatusCode.ToString();
            }
            catch (Exception e)
            {
                return GlobalConstants.ErrorMessage + e.Message;

            }
        }
    }
}
