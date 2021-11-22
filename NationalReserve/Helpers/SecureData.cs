using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace NationalReserve.Helpers
{
    public class SecureData
    {
        public static string Hash(string data)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bytedString = Encoding.UTF8.GetBytes(data);
                var buffer = md5.ComputeHash(bytedString);
                var sb = new StringBuilder();

                foreach (var t in buffer)
                {
                    sb.Append(t.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static bool IsHash(string hash)
        {
            return Regex.IsMatch(hash, "^[0-9a-fA-F]{32}$");
        }
    }
}
