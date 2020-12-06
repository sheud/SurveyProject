using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace asdasd
{
    enum UserLevel { Admin = 1, Manager = 2, Respodent = 3}

    class Database
    {
        MySqlConnection connection;
        public Database()
        {
            OpenConnection();
        }
        public void OpenConnection()
        {
            connection = new MySqlConnection("server=127.0.0.1;user=root;database=testbase;port=3306;password=");
            connection.Open();
        }

        public void Login()
        {
            
        }

        public void AddUserKey(string key)
        {
            MySqlCommand addKeyCommand = connection.CreateCommand();
            addKeyCommand.CommandText = "INSERT INTO asd (avain, userlevel) VALUES ('" + key + "'" + ",'"+ (int)UserLevel.Respodent + "');";//  + "WHERE name=@name;";
            addKeyCommand.Parameters.AddWithValue("@avain", key);
            MySqlDataReader results = addKeyCommand.ExecuteReader();
            while (results.Read())
            {
                Console.WriteLine(results.GetString(0));
            }

            Console.ReadKey(true);
            CloseConnection();
        }

        //MySqlCommand addKeyCommand = connection.CreateCommand();
        //addKeyCommand.CommandText = "INSERT INTO asd (avain, userLevel) VALUES " + "('" + key + "'" + "'" +
        //(int) UserLevel.Respodent + "');";//  + "WHERE name=@name;";
        //addKeyCommand.Parameters.AddWithValue("@name", "B");
        //MySqlDataReader results = addKeyCommand.ExecuteReader();

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
            //currentUser = db.Login(key);
        }

        public void AddRegularKey()
        {
            string key; // ehkä kysytään databasesta
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

            Console.WriteLine("moikka 'Add'");
            var command = Console.ReadLine();
            while (true)
            {
                switch (command)
                {
                    case "Add":
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


