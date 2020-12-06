using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.OpenSsl;

namespace asdasd
{
    enum UserLevel { Admin = 1, Manager = 2, Respodent = 3}

    class Database
    {
        string pw;
        private string user;

        MySqlConnection connection;
        public Database()
        {
        }
        public void OpenConnection(string _user, string _pw)
        {
            pw = _pw;
            user = _user;
            connection = new MySqlConnection(string.Format("server=127.0.0.1;user={0};database=testbase;port=3306;password={1}", _user, _pw));
            connection.Open();
        }

        public void Login()
        {
            Console.WriteLine("Login:");
            user = Console.ReadLine();
            Console.WriteLine("Password:");
            pw = Console.ReadLine();
            OpenConnection(user, pw);
        }

        public void AddUserKey(string key)
        {
            MySqlCommand addKeyCommand = connection.CreateCommand();
            addKeyCommand.CommandText = string.Format("INSERT INTO asd (avain, userlevel) VALUES ('{0}', '{1}');", key, (int)UserLevel.Respodent);
            //showKey.CommandText = string.Format("select * from asd where avain = '{0}';", key);
            //addKeyCommand.Parameters.AddWithValue("@avain", key);
            MySqlDataReader results = addKeyCommand.ExecuteReader();
            while (results.Read())
            {
                Console.WriteLine(results.GetString(0));
            }
            Console.ReadKey(true);
            CloseConnection();
        }

        public void CloseConnection()
        {
            connection.Close();
        }
    }
    class Question
    {

    }

    class Page
    {
        private List<Question> questions;
    }

    class Survey
    {
        private List<Page> pages;
    }

    class Application
    {
        Database db;
        UserInterface ui;
        private Survey survey;
        private UserLevel currentUser;

        public Application()
        {
            db = new Database();
            ui = new UserInterface();
            ui.Run(this);
        }

        public void Login()
        {
            db.Login();
        }

        public void AddRegularKey()
        {
            string key;
            key = Console.ReadLine();
            db.AddUserKey(key);
        }
    }

    class UserInterface
    {
        private Application app;
        
        public void Run(Application _app)
        {
            app = _app;
        app.Login();   
            Console.WriteLine("write 'Add' to add a new key");
            var command = Console.ReadLine();
            while (true)
            {
                switch (command)
                {
                    case "Add":
                        Console.WriteLine("Insert a new key:");
                        app.AddRegularKey();
                        break;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Application app = new Application();
           
        }
    }
}


