using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace HttpNewsPAT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SingIn("user", "user");
            Console.Read();
        }
        public static void SingIn(string Login, string Password)
        {
            string url = "http://127.0.0.1/ajax/login.php";
            Debug.WriteLine($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = new CookieContainer();
            string postData = $"login={Login}&password={Password}";
            byte[] Data = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = Data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(Data, 0, Data.Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Debug.WriteLine($"Статус выполннения: {response.StatusCode}");
            string responseFromServer = "Печенька: токен = " + response.Cookies[0].Value.ToString();
            Console.WriteLine(responseFromServer);
        }

    }
}
