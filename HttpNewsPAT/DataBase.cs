using MySqlConnector;
using System;
using System.Diagnostics;

namespace HttpNewsPAT
{
    public class DataBase
    {
        public static void AddNew(string name, string description, string image)
        {
            try
            {
                MySqlConnection connection = new MySqlConnection("database=news;server=127.0.0.1;port=3306;user=root;pwd=;");
                MySqlCommand command = new MySqlCommand($"INSERT INTO `news`(`img`, `name`, `description`) VALUES ('{image}','{name}','{description}')", connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                Write("Успешное добавление новости");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Write("Ошибка выполнения запроса: " + ex.Message);
            }
        }
        public static void Write(string debugContent)
        {
            Console.Write(debugContent);
            Debug.WriteLine(debugContent);
            Debug.Flush();
        }
    }
}
