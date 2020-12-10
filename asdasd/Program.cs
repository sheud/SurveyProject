using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.OpenSsl;


namespace asdasd
{
    enum UserLevel
    {
        Respodent = 1,
        Manager = 2,
        Admin = 3
    }

    enum QuestionType
    {
    }

    class Database
    {
        
        MySqlConnection connection;

        public Database()
        {
          
            Login();
        }

        public void OpenConnection()
        {
            connection =
                new MySqlConnection(
                    string.Format("server=127.0.0.1;user=root;database=group3survey;port=3306;password="));
            connection.Open();
        }

        public void Login()
        {
            OpenConnection();
        }

        public void AddUserKey(string key)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                string.Format("INSERT INTO avaimet (avainkoodi, status, level ) VALUES ('{0}', 'usable', '{1}');", key, (int)UserLevel.Respodent);
            command.ExecuteNonQuery();
        }

        public void AddManagerKey(string key)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("INSERT INTO manager_account (account_key, key_status ) VALUES ('{0}', 'usable');", key);
            command.ExecuteNonQuery();
        }

        public void CreateManager(string _managerkey)
        {
            string userName;
            string password;
            Console.WriteLine("Insert username:");
            userName = Console.ReadLine();
            Console.WriteLine("Insert password:");
            password = Console.ReadLine();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("UPDATE manager_account SET account_name = '{0}', account_password = '{1}' WHERE account_key='{2}';", userName, password, _managerkey);
            command.ExecuteNonQuery();
        }

        public Survey CreateSurvey(string _surveyName)
        {
            string surveyName = _surveyName;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("INSERT INTO survey (status, surveyName) VALUES ('draft','{0}');", surveyName);
            command.ExecuteNonQuery();


            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM survey WHERE surveyName='{0}';", surveyName);
            cmd.CommandType = CommandType.Text;
            MySqlDataReader myReader = cmd.ExecuteReader();
            int id = 0;
            bool notEoF;
            notEoF = myReader.Read();
            while (notEoF)
            {
            myReader.Read();
            id = (int)myReader["sID"];
            Console.Write(string.Format("ID: " + myReader["sID"].ToString()));
            Console.WriteLine(string.Format(" Survey name: " + surveyName.ToString()));
                notEoF = myReader.Read();
            }
            Survey survey = new Survey(surveyName, id);
            myReader.Close();
            return survey;
        }



        public void GetSurveys()
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM survey;");
            command.CommandType = CommandType.Text;
            MySqlDataReader myReader = command.ExecuteReader(); 
            
            bool notEoF;
            notEoF = myReader.Read();
            Console.WriteLine("\nSurveys in database:\n");
            while (notEoF)
            {
                Console.Write(string.Format("ID: " + myReader["sID"].ToString()));
                Console.WriteLine(string.Format(" Survey name: " + myReader["surveyName"].ToString() ));
                notEoF = myReader.Read();
            }

            Console.WriteLine("\n");
            myReader.Close();
        }

        public void CreateQuestion(string question)
        {
           
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public void EditSurvey()
        {

        }
    }

    class Question
    {

    }

    class Page
    {
        private List<Question> questions;
    }

    public class Survey
    {
        private string surveyName; 
        private int surveyID =0;
        private string status;
        private DateTime startDate;
        DateTime endDate;
        private int managerID;

        public string GetSurveyName() { return surveyName;}
        public int GetSurveyID() { return surveyID;}
        public int GetManagerID() { return managerID; }
        public string GetSurveyStatus() { return status; }
        public DateTime GetStartDate() { return startDate;}
        public DateTime GetEndDate() { return endDate; }
        


        private List<Page> pages;

        public Survey(string _name)
        {
            surveyName = _name;
        }
        public Survey(string _name, int _id)
        {
            surveyName = _name;
            surveyID = _id;
        }

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
        public void AddManagerKey()
        {
            string key;
            key = Console.ReadLine();
            if (key != "q")
            {
                db.AddManagerKey(key);
            }
        }

        public void CreateSurvey()
        {
            string surveyName;
            surveyName = Console.ReadLine();
            if (surveyName != "q")
            {
                survey = db.CreateSurvey(surveyName);
                Console.WriteLine(survey.GetSurveyID());
              
            }
        }
        public void CreateManager()
        {
            string key;
            key = Console.ReadLine();
            if (key != "q")
            {
                db.CreateManager(key);
            }
        }
        public void GetSurveys()
        {
            db.GetSurveys();
        }

        public void EditSurvey()
        {

        }
    }


    class UserInterface
    {
        private Application app;

        public void Run(Application _app)
        {
            int currentUserlevel = (int)UserLevel.Manager;
            app = _app;
            app.Login();



            //while (true)
            //{
            //    var com = Console.ReadLine();
            
            //    switch (com)
            //    {
            //        case "Admin Login":
            //            Console.WriteLine("Insert 'q' to go back");
            //            Console.WriteLine("Insert a new key:");
            //            app.AddRegularKey();
            //            Console.WriteLine("Choose the target survey: ");
            //            break;
            //        case "Create Manager Account:":
            //            Console.WriteLine("Insert 'q' to go back");
            //            Console.WriteLine("Insert survey name:");
            //            ManagerLogin();
            //            break;
            //        case "Manager Login":
            //            Console.WriteLine("Insert 'q' to go back");
            //            Console.WriteLine("Insert survey name:");
            //            ManagerLogin();
            //            break;
            //        case "Answer survey":
            //            Console.WriteLine("Insert 'q' to go back");
            //            Console.WriteLine("Insert survey name:");
            //            app.CreateSurvey();
            //            break;
            //    }
            //}

            while (true)
            {
                if (currentUserlevel == 2)
                {


                    Console.WriteLine("write 'Add key' to add a new key");
                    Console.WriteLine("write 'Create survey' to create a new survey");
                    Console.WriteLine("write 'Show surveys' to show surveys");
                    Console.WriteLine("write 'Add manager key' to add a new manager key");
                    Console.WriteLine("write 'Register manager' to register as a manager");

                    var command = Console.ReadLine();

                    switch (command)
                    {
                        case "Add key":
                            Console.WriteLine("Insert 'q' to go back");
                            Console.WriteLine("Insert a new key:");
                            app.AddRegularKey();
                            Console.WriteLine("Choose the target survey: ");
                            break;
                        case "Add manager key":
                            Console.WriteLine("Insert 'q' to go back");
                            Console.WriteLine("Insert a new key:");
                            app.AddManagerKey();
                            break;
                        case "Register manager":
                            Console.WriteLine("Insert 'q' to go back");
                            Console.WriteLine("Insert a valid key;");
                            app.CreateManager();
                            break;
                        case "Create survey":
                            Console.WriteLine("Insert 'q' to go back");
                            Console.WriteLine("Insert survey name:");
                            app.CreateSurvey();
                            break;
                        case "Edit survey":
                            Console.WriteLine("Insert 'q' to go back");
                            Console.WriteLine("Insert survey name:");
                            app.EditSurvey();
                            break;
                        case "Show surveys":
                            Console.WriteLine("Insert 'q' to go back");
                            app.GetSurveys();
                            break;
                    }
                }

            }
        }
            public void ManagerLogin(string _user, string _pw)
            {

                Console.WriteLine("write 'Add key' to add a new key");
                Console.WriteLine("write 'Create survey' to create a new survey");
                Console.WriteLine("write 'Show surveys' to show surveys");

                var command = Console.ReadLine();

                switch (command)
                {
                    case "Add key":
                        Console.WriteLine("Insert 'q' to go back");
                        Console.WriteLine("Insert a new key:");
                        app.AddRegularKey();
                        Console.WriteLine("Choose the target survey: ");
                        break;
                    case "Create survey":
                        Console.WriteLine("Insert 'q' to go back");
                        Console.WriteLine("Insert survey name:");
                        app.CreateSurvey();
                        break;
                    case "Edit survey":
                        Console.WriteLine("Insert 'q' to go back");
                        Console.WriteLine("Insert survey name:");
                        app.EditSurvey();
                        break;
                    case "Show surveys":
                        Console.WriteLine("Insert 'q' to go back");
                        app.GetSurveys();
                        break;
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

