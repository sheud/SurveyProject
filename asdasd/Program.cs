using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Authentication;
using Org.BouncyCastle.OpenSsl;


namespace asdasd
{
    public enum UserLevel
    {
        None = 0,
        Respodent = 1,
        Manager = 2,
        Admin = 3
    }

    public class Database
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

        public void AddUserKey(string key, int _surveyId)
        {
            int surveyId = _surveyId;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                string.Format("INSERT INTO avaimet (avainkoodi, status, level, surveyID) VALUES ('{0}', 'usable', '{1}', '{2}');", key, (int)UserLevel.Respodent, surveyId);
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

        public (int, int) ManagerLogin(string _userName, string _password)
        {
            string userName = _userName;
            string password = _password;

            string checkName = "";
            string checkPassword = "";

            int userlevel;
            int managerId = 0;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM manager_account WHERE account_name = '{0}' AND account_password = '{1}';", userName, password);
            command.CommandType = CommandType.Text;
            MySqlDataReader myReader = command.ExecuteReader();

            bool notEoF;
            notEoF = myReader.Read();
            while (notEoF)
            {
                notEoF = myReader.Read();
                checkName = myReader["account_name"].ToString();
                checkPassword = myReader["account_password"].ToString();
                managerId = (int)myReader["mID"];
                notEoF = myReader.Read();
            }
            myReader.Close();
            if (checkName == "admin" && checkPassword == "admin")
            {
                Console.WriteLine("Login successful");
                userlevel = (int)UserLevel.Admin;
                return (managerId, userlevel);
            }
            else if (checkName == userName && checkPassword == password)
            {
                Console.WriteLine("Login successful");
                userlevel = (int)UserLevel.Manager;
                return (managerId, userlevel);
            }
            else
            {
                Console.WriteLine("invalid username or password");
            }

            return (0, 0);

        }

        public void SetSurveyStatus(int _surveyId)
        {
            List<string> statusList = new List<string>() {"draft", "ongoing", "closed"};
            Console.WriteLine(string.Format("Set survey status:\n[1]{0}\n[2]{1}\n[3]{2}", statusList[0],statusList[1],statusList[2]));
            int choice = Convert.ToInt32(Console.ReadLine());
            choice = choice - 1;
            
            
            int surveyId = _surveyId;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("UPDATE survey SET status = '{0}' WHERE sID = '{1}'; ", statusList[choice], surveyId);
            command.ExecuteNonQuery();
        }
        public void SetSurveyEndDate(int _surveyId)
        {
            DateTime closeDateTime = new DateTime();
            int year = 2021;
            int month = 12;
            int day = 1;
            int hour = 0;
            int minutes = 0;
            bool isTrue = true;
            while (isTrue == true)
            {
                Console.WriteLine("Insert closing year: ");
                year = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Insert closing month: ");
                month = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Insert closing day: ");
                day = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Insert closing Hour: ");
                hour = Convert.ToInt32(Console.ReadLine());
                closeDateTime = new DateTime(year, month, day, hour, minutes, 00);
                Console.WriteLine(closeDateTime.ToString("dd-MM-yyyy HH:mm:ss"));
                Console.WriteLine("Is this date ok? [y/n]");
                string answer = Console.ReadLine();
                if (answer == "y")
                {
                    isTrue = false;
                }

            }

            var SqlFormatTime = closeDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime.Compare(DateTime.Now, closeDateTime);

            int surveyId = _surveyId;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("UPDATE survey SET expireDate = '{0}' WHERE sID = '{1}'; ", SqlFormatTime, surveyId);
            command.ExecuteNonQuery();
        }
        public (string, int) CreateSurvey(string _surveyName, int _managerId)
        {
            string surveyName = _surveyName;
            int managerId = _managerId;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("INSERT INTO survey (status, surveyName, mID) VALUES ('draft','{0}', '{1}');", surveyName, managerId);
            command.ExecuteNonQuery();


            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM survey WHERE surveyName='{0}';", surveyName);
            cmd.CommandType = CommandType.Text;
            MySqlDataReader myReader = cmd.ExecuteReader();
            int sid = 0;
            bool notEoF;
            notEoF = myReader.Read();
            while (notEoF)
            {
                myReader.Read();
                sid = (int)myReader["sID"];
                surveyName = myReader["surveyName"].ToString();
                Console.Write(string.Format("ID: " + myReader["sID"].ToString()));
                Console.WriteLine(string.Format(" Survey name: " + surveyName.ToString()));
                notEoF = myReader.Read();
            }
            myReader.Close();
            return (surveyName, sid);
        }

        public void CreateQuestion(int _survid)
        {
            Console.WriteLine("[1] Text question\n[2] multiple choice\n[3] Radiobutton\n");
            int qType = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Insert Question:");
            string question = Console.ReadLine();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("INSERT INTO question (sID, qType, question) Values ('{0}', {1}, '{2}');", _survid, qType, question);
            command.ExecuteNonQuery();
        }


        public void GetSurveys(int _managerId)
        {
            int managerId = _managerId;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM survey WHERE mID='{0}';", managerId);
            command.CommandType = CommandType.Text;
            MySqlDataReader myReader = command.ExecuteReader();

            bool notEoF;
            notEoF = myReader.Read();
            Console.WriteLine("\nSurveys in database:\n");
            while (notEoF)
            {
                Console.Write(string.Format("ID: " + myReader["sID"].ToString()));
                Console.WriteLine(string.Format(" Survey name: " + myReader["surveyName"].ToString() + " status: " +myReader["status"] + " Close date: " + myReader["expireDate"]));
                notEoF = myReader.Read();
            }

            Console.WriteLine("\n");
            myReader.Close();
        }
        public void GetQuestions(int _surveyId)
        {
            int questionNumber = 1;
            int surveyId = _surveyId;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM question WHERE sID='{0}';", surveyId);
            command.CommandType = CommandType.Text;
            MySqlDataReader myReader = command.ExecuteReader();

            bool notEoF;
            notEoF = myReader.Read();
            Console.WriteLine("\nSurvey Questions:\n");
            while (notEoF)
            {
                Console.Write(string.Format("{0}: ", questionNumber));
                Console.WriteLine(string.Format(myReader["question"].ToString()));
                notEoF = myReader.Read();
                questionNumber++;
            }

            Console.WriteLine("\n");
            myReader.Close();
        }

        public void AnswerSurvey(string _userKey)
        {
            List<string> lista = new List<string>();
            int surveyId;
            string userKey = _userKey;
            int qType;
            int questionId;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM question WHERE userKey='{0}';", userKey);
            command.CommandType = CommandType.Text;
            MySqlDataReader myReader = command.ExecuteReader();
            bool notEoF;
            notEoF = myReader.Read();
            Console.WriteLine("\nSurvey Questions:\n");
            while (notEoF)
            {
                Console.Write(string.Format("questions:\n " + myReader["question"].ToString()));
                qType = (int)myReader["qType"];
                surveyId = (int)myReader["sID"];
                questionId = (int)myReader["qID"];
                string answer = Console.ReadLine();
                lista.Add(string.Format("INSERT INTO Answers(surveyID, qType, answer, userKey, qID) Values('{0}', '{1}', '{2}', '{3}', '{4}');", surveyId, qType, answer, userKey, questionId));
                Console.WriteLine("Answer:");
                Console.ReadLine();
                notEoF = myReader.Read();
            }

            Console.WriteLine("\n");
            myReader.Close();
            foreach (string c in lista)
            {
                command.CommandText = c;
                command.ExecuteNonQuery();
            }
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

    public class Manager
    {
        public int managerId;
        public Manager(int _mId)
        {
            managerId = _mId;
        }


    }
    public class Survey
    {
        private string surveyName;
        private int surveyID = 0;
        private string status;
        private DateTime startDate;
        DateTime endDate;
        private int managerID;
        private int respodentID;
        public string GetSurveyName() { return surveyName; }
        public int GetSurveyID() { return surveyID; }
        public int GetManagerID() { return managerID; }

        public DateTime GetStartDate() { return startDate; }
        public DateTime GetEndDate() { return endDate; }

        public int SurveyId
        {
            get => surveyID;
            set => surveyID = value;
        }
        public string SurveyName
        {
            get => surveyName;
            set => surveyName = value;
        }
        private List<Page> pages;

        public Survey(string _name)
        {
            surveyName = _name;
        }
        public Survey(string _name, int _id, int _managerId)
        {
            surveyName = _name;
            surveyID = _id;
            managerID = _managerId;
        }
    }

    public class Application
    {

        Database db;
        UserInterface ui;
        private Survey survey;
        private static int userLevel = 0;
        private static int managerId = 0;

        public static int UserLevel
        {
            get => userLevel;
            set => userLevel = value;
        }
        public Application()
        {
            db = new Database();
            ui = new UserInterface();
            ui.Run(this);
        }

        public int GetUserLevel()
        {
            return userLevel;
        }
        public void Login()
        {
            db.Login();
        }

        public void AddRegularKey()
        {
            string key;
            key = Console.ReadLine();
            GetSurveys();
            Console.WriteLine("Choose the survey id:");
            int surveyId = Convert.ToInt32(Console.ReadLine());
            if (key != "q")
            {
                db.AddUserKey(key, surveyId);
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

            if (userLevel == 2 && managerId != 0)
            {
                Console.WriteLine("Insert survey name:");
                surveyName = Console.ReadLine();
                if (surveyName != "q")
                {
                    var (result1, result2) = db.CreateSurvey(surveyName, managerId);
                    survey = new Survey(result1, result2, managerId);


                    Console.WriteLine(survey.GetSurveyID());
                    Console.WriteLine(survey.GetSurveyName());
                    Console.WriteLine(survey.GetManagerID());
                }

            }
            else
            {
                Console.WriteLine("Please login as a manager.");
            }
        }

        public void CreateQuestion(int _survey)
        {
            int surveyId = _survey;
            if (userLevel == 2 && managerId != 0)
            {
                db.CreateQuestion(surveyId);
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

        public void ManagerLogin()
        {

            Console.WriteLine("Insert username:");
            string userName = Console.ReadLine();
            Console.WriteLine("Insert password:");
            string password = Console.ReadLine();
            if (userName != "q" || password != "q")
            {
                var (managerId, userLevel) = db.ManagerLogin(userName, password);
                Application.managerId = managerId;
                Application.userLevel = userLevel;


            }

        }
        public void GetSurveys()
        {
            if (userLevel == 2 && managerId != 0)
            {
                int managerId = Application.managerId;
                db.GetSurveys(managerId);
            }
        }

        public void AnswerSurvey()
        {
            string userKey;
            userKey = Console.ReadLine();
            db.AnswerSurvey(userKey);
        }

        public void GetQuestions(int _survey)
        {
            int surveyId = _survey;
            if (userLevel == 2 && managerId != 0)
            {
                db.GetQuestions(surveyId);
            }
        }
        public void SetSurveyStatus(int _survey)
        {
            int surveyId = _survey;
            if (userLevel == 2 && managerId != 0)
            {
                db.SetSurveyStatus(surveyId);
            }
        }
        public void SetSurveyEndDate(int _survey)
        {
            int surveyId = _survey;
            if (userLevel == 2 && managerId != 0)
            {
                db.SetSurveyEndDate(surveyId);
            }
        }
        public void EditSurvey()
        {
            var command = "";
            GetSurveys();
            Console.WriteLine("Select survey to edit:");
            int survey = Convert.ToInt32(Console.ReadLine());
            while (command != "7")
            {
                Console.WriteLine(
                    "[1] Create Question\n[2] Edit Question\n[3] Show Survey\n[4] Switch survey\n[5] Set survey status\n[6] Set End date [7] Exit");
                command = Console.ReadLine();
                switch (command)
                {
                    case "1":
                        CreateQuestion(survey);
                        break;
                    case "3":
                        GetQuestions(survey);
                        break;
                    case "5":
                        SetSurveyStatus(survey);
                        break;
                    case "6":
                        SetSurveyEndDate(survey);
                        break;

                }
            }
        }
    }



    class UserInterface
    {
        private Application app;

        public void Run(Application _app)
        {
            app = _app;
            int currentUserlevel = app.GetUserLevel();

            app.Login();
            List<string> opts = new List<string>() { "[1] Login\n[2] Create account\n[3] Answer a survey",
                "[1] Create manager key\n[2] Show unused user keys\n[3] Exit",
                "[1] Create survey\n[2] Edit survey\n[3] Delete survey\n[4] See results\n[5] My surveys\n[6] Create survey key\n[7] Exit"
            };


            while (true)
            {
                currentUserlevel = app.GetUserLevel();
                if (currentUserlevel == 2)
                {
                    Console.WriteLine(opts[2]);

                    var command = Console.ReadLine();

                    switch (command)
                    {
                        case "6":
                            Console.WriteLine("Insert 'q' to go back");
                            Console.WriteLine("Insert a new key:");
                            app.AddRegularKey();
                            Console.WriteLine("Choose the target survey: ");
                            break;
                        case "1":
                            Console.WriteLine("Insert 'q' to go back");
                            app.CreateSurvey();
                            break;
                        case "2":
                            Console.WriteLine("Insert 'q' to go back");
                            app.EditSurvey();
                            break;
                        case "5":
                            Console.WriteLine("Insert 'q' to go back");
                            app.GetSurveys();
                            break;
                        case "7":
                            currentUserlevel = 0;
                            break;

                    }
                }
                else if (currentUserlevel == 0)
                {
                    Console.WriteLine(opts[0]);
                    var command = Console.ReadLine();
                    switch (command)
                    {
                        case "2":
                            Console.WriteLine("Insert 'q' to go back");
                            Console.WriteLine("Insert a valid key;");
                            app.CreateManager();
                            break;

                        case "1":
                            Console.WriteLine("Insert 'q' to go back");
                            app.ManagerLogin();
                            break;
                        case "3":
                            Console.WriteLine("Insert a survey key");
                            app.AnswerSurvey();
                            Console.WriteLine("Insert 'q' to go back");
                            break;

                    }

                }
                else if (currentUserlevel == 3)
                {
                    Console.WriteLine(opts[1]);
                    var command = Console.ReadLine();
                    switch (command)
                    {
                        case "1":
                            Console.WriteLine("Insert 'q' to go back");
                            Console.WriteLine("Insert a valid key;");
                            app.AddManagerKey();
                            break;

                        case "2":
                            Console.WriteLine("Insert 'q' to go back");
                            //app.ManagerLogin();
                            break;
                        case "3":
                            currentUserlevel = 0;
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

