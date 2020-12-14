using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlX.XDevAPI.Relational;

namespace asdasd
{
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
        public void GetAnswers()
        {
            GetSurveys();
            string userKey;
            Console.WriteLine("Choose survey to check: ");
            int surveyId = Convert.ToInt32(Console.ReadLine());
            if (userLevel == 2 && managerId != 0)
            {
                GetQuestions(surveyId);
                db.GetUserKeyFromSurvey(surveyId);
                Console.WriteLine("choose user key: ");
                userKey = Console.ReadLine();
                db.GetAnswers(surveyId, userKey);
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
                    "[1] Create Question\n[2] Edit Question\n[3] Show Survey\n[4] Switch survey\n[5] Set survey status\n[6] Set End date \n[7] Exit");
                command = Console.ReadLine();
                switch (command)
                {
                    case "1":
                        CreateQuestion(survey);
                        break;
                    case "3":
                        GetQuestions(survey);
                        break;
                    case "4":
                        GetSurveys();
                        EditSurvey();
                        break;
                    case "5":
                        SetSurveyStatus(survey);
                        break;
                    case "6":
                        SetSurveyEndDate(survey);
                        break;
                    case "7":
                        break;

                }
            }
        }
    }

}
