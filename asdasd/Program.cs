using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.OpenSsl;

namespace asdasd
{
    enum UserLevel { Admin = 1, Manager = 2, Respodent = 3}
    enum QuestionType { }

    class Database
    {

        MySqlConnection connection;
        public Database()
        {
            Login();
        }
        public void OpenConnection()
        {

            connection = new MySqlConnection(string.Format("server=127.0.0.1;user=root;database=group3survey;port=3306;password="));
            connection.Open();
        }

        public void Login()
        {
            OpenConnection();
        }

        public void AddUserKey(string key)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("INSERT INTO avaimet (avainkoodi, status ) VALUES ('{0}', 'usable');", key);
            command.ExecuteNonQuery();
        }

        public void AddManagerKey(string key)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("INSERT INTO avaimet (avainkoodi, status ) VALUES ('{0}', 'usable');", key);
            command.ExecuteNonQuery();
        }

        public void CreateSurvey(string surveyName)
        {
            Survey sur = new Survey();
        }

        public void CreateQuestion(string question)
        {

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
            if (key != "q")
            {
                db.AddUserKey(key);
            }
            
        }

        public void CreateSurvey()
        {
            string surveyName;
            surveyName = Console.ReadLine();
            if (surveyName != "q")
            {
                //db.CreateSurvey();
            }
        }
    }

    class UserInterface
    {
        private Application app;
        
        public void Run(Application _app)
        { 
            app = _app;
        app.Login();   
            while (true)
            {
                Console.WriteLine("write 'Add key' to add a new key");
                Console.WriteLine("write 'Create a survey' to create a new survey");

                var command = Console.ReadLine();

                switch (command)
                {
                    case "Add key":
                        Console.WriteLine("Insert 'q' to go back");
                        Console.WriteLine("Insert a new key:");
                        app.AddRegularKey();
                        break;
                    case "Create a survey":
                        Console.WriteLine("Insert 'q' to go back");
                        Console.WriteLine("Insert survey name:");
                        app.CreateSurvey();
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


