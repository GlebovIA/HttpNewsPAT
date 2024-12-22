using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpNewsPAT
{
    internal class Program
    {
        private static HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            Debug.Listeners.Add(new TextWriterTraceListener("log.txt"));
            await SignIn("user", "user");
            Console.Read();
        }

        public static async Task SignIn(string login, string password)
        {
            string url = "http://127.0.0.1/ajax/login.php";
            WriteLog($"Выполняем запрос: {url}");

            var postData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("login", login),
                new KeyValuePair<string, string>("password", password)
            });

            HttpResponseMessage response = await httpClient.PostAsync(url, postData);
            WriteLog($"Статус выполнения: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                string cookies = response.Headers.GetValues("Set-Cookie").FirstOrDefault();
                if (!string.IsNullOrEmpty(cookies))
                {
                    string token = cookies.Split(';')[0].Split('=')[1];
                    Console.WriteLine("Печенька: токен = " + token);

                    string content = await GetContentAsync(token);
                    ParsingHtml(content);
                }
            }
            else
            {
                Console.WriteLine($"Ошибка выполнения запроса: {response.StatusCode}");
            }
        }

        public static async Task<string> GetContentAsync(string token)
        {
            string url = "http://127.0.0.1/main";
            WriteLog($"Выполняем запрос: {url}");
            httpClient.DefaultRequestHeaders.Add("token", token);

            HttpResponseMessage response = await httpClient.GetAsync(url);
            WriteLog($"Статус выполнения: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine($"Ошибка выполнения запроса: {response.StatusCode}");
                return string.Empty;
            }
        }

        public static void ParsingHtml(string htmlCode)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlCode);
            HtmlNode document = html.DocumentNode;
            IEnumerable<HtmlNode> divsNews = document.Descendants().Where(n => n.HasClass("news"));

            string content = "";
            foreach (HtmlNode divNews in divsNews)
            {
                string src = divNews.ChildNodes[1].GetAttributeValue("src", "none");
                string name = divNews.ChildNodes[3].InnerText;
                string description = divNews.ChildNodes[5].InnerText;

                content += $"{name}\nИзображение: {src}\nОписание: {description}\n";
            }

            Console.Write(content);
            WriteToFile(content);
        }

        public static void WriteToFile(string content)
        {
            File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "parsedfile.txt"), content);
        }

        public static void WriteLog(string debugContent)
        {
            Debug.WriteLine(debugContent);
            Debug.Flush();
        }
    }
}