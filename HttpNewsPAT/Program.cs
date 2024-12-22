using HtmlAgilityPack;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HttpNewsPAT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener("log.txt"));
            SingIn("user", "user");
            Console.Read();
        }
        public static void SingIn(string Login, string Password)
        {
            string url = "http://127.0.0.1/ajax/login.php";
            WriteLog($"Выполняем запрос: {url}");
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
            WriteLog($"Статус выполннения: {response.StatusCode}");
            string responseFromServer = "Печенька: токен = " + response.Cookies[0].Value.ToString();
            Console.WriteLine(responseFromServer);
            string Content = GetContent(new Cookie("token", response.Cookies[0].Value.ToString(), "/", "127.0.0.1"));
            ParsingHtml(Content);
        }
        public static string GetContent(Cookie Token)
        {
            string url = "http://127.0.0.1/main";
            WriteLog($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(Token);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            WriteLog($"Статус выполннения: {response.StatusCode}");
            string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseFromServer;
        }
        public static void ParsingHtml(string htmlCode)
        {
            string content = "";
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);
            var Document = html.DocumentNode;
            IEnumerable DivsNews = Document.Descendants(0).Where(n => n.HasClass("news"));
            foreach (HtmlNode DivNews in DivsNews)
            {
                var src = DivNews.ChildNodes[1].GetAttributeValue("src", "none");
                var name = DivNews.ChildNodes[3].InnerText;
                var description = DivNews.ChildNodes[5].InnerText;
                content += name + "\n" + "Изображение: " + src + "\n" + "Описание: " + description + "\n";
            }
            Console.Write(content);
            WriteToFile(content);
        }
        public static void WriteToFile(string content)
        {
            StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "/parsedfile.txt");
            writer.Write(content);
            writer.Close();
        }
        public static void WriteLog(string debugContent)
        {
            Debug.WriteLine(debugContent);
            Debug.Flush();
        }
    }
}
