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
                    string.Format("server=127.0.0.1;user=SurveyUser;database=group3survey;port=3306;password=survey3"));
            connection.Open();
        }

        public void Login()
        {
            OpenConnection();
        }

        public void AddUserKey(string key, int _surveyId)
        {
            // lisää avaimet tableen uuden käyttäjän:
            int surveyId = _surveyId;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                string.Format("INSERT INTO avaimet (avainkoodi, status, level, surveyID) VALUES ('{0}', 'usable', '{1}', '{2}');", key, (int)UserLevel.Respondent, surveyId);
            command.ExecuteNonQuery();

            // päivittää Userkeyn myös survey tableen:
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = string.Format("UPDATE survey SET surveyKey = '{0}' WHERE sID = '{1}';", key, surveyId);
            cmd.ExecuteNonQuery();


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
            List<string> statusList = new List<string>() { "draft", "ongoing", "closed" };
            Console.WriteLine(string.Format("Set survey status:\n[1]{0}\n[2]{1}\n[3]{2}", statusList[0], statusList[1], statusList[2]));
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
            Console.WriteLine("Add key for the survey:");
            string surveyKey = Console.ReadLine();
            AddUserKey(surveyKey, sid);
            return (surveyName, sid);
        }

        public string GetUserKeyFromSurvey(int _surveyId)
        {
            string userKey = "";
            int surveyId = _surveyId;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM survey WHERE sID='{0}';", surveyId);
            command.CommandType = CommandType.Text;
            MySqlDataReader myReader = command.ExecuteReader();

            bool notEoF;
            notEoF = myReader.Read();
            while (notEoF)
            {
                userKey = myReader["surveyKey"].ToString();
                notEoF = myReader.Read();
            }

            myReader.Close();
            return userKey;
        }

        public void CreateQuestion(int _survid)
        {
            int maxLength;
            string userKey = GetUserKeyFromSurvey(_survid);
            Console.WriteLine("[1] Text question\n[2] multiple choice\n[3] Radiobutton\n");
            int qType = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Insert Question:");
            string question = Console.ReadLine();
            Console.WriteLine("Insert max length for the answer (1-1000 characters)");
            maxLength = Convert.ToInt32(Console.ReadLine());
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("INSERT INTO question (sID, qType, question, userKey, maxLength) Values ('{0}', '{1}', '{2}', '{3}', {4});", _survid, qType, question, userKey, maxLength);
            command.ExecuteNonQuery();
            // testataan jos luodaan vastaus samalla kun tehdään kysymys
            //command.CommandText = string.Format("INSERT INTO answer (surveyID, qType, question, userKey) Values ('{0}', '{1}', '{2}', '{3}');", _survid, qType, question, userKey);
            //command.ExecuteNonQuery();
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
                Console.Write(string.Format("\n\nID: " + myReader["sID"].ToString()));
                Console.WriteLine(string.Format("\nSurvey name: " + myReader["surveyName"].ToString() + "\nstatus: " + myReader["status"] + "\nClose date: " + myReader["expireDate"]) + "\nRespondent keys: " + myReader["surveyKey"].ToString());
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

        public void GetAnswers(int _surveyId, string _userKey)
        {
            string userKey = _userKey;
            int questionNumber = 1;
            int surveyId = _surveyId;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM answers WHERE surveyID='{0}' AND userKey='{1}';", surveyId, userKey);
            command.CommandType = CommandType.Text;
            MySqlDataReader myReader = command.ExecuteReader();

            bool notEoF;
            notEoF = myReader.Read();
            Console.WriteLine("\nSurvey Answers:\n");
            while (notEoF)
            {
                Console.Write(string.Format("{0}: ", questionNumber));
                Console.WriteLine(string.Format(myReader["answer"].ToString()));
                notEoF = myReader.Read();
                questionNumber++;
            }

            Console.WriteLine("\n");
            myReader.Close();
        }

        public void AnswerSurvey(string _userKey)
        {
            Console.Clear();
            List<string> lista = new List<string>();
            int surveyId;
            string userKey = _userKey;
            int qType;
            int questionId;
            int maxLength;
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = string.Format("SELECT * FROM question WHERE userKey='{0}';", userKey);
            command.CommandType = CommandType.Text;
            MySqlDataReader myReader = command.ExecuteReader();
            bool notEoF;
            notEoF = myReader.Read();

            while (notEoF)
            {
                
                Console.Write(string.Format("question: " + myReader["question"].ToString()) + "\n");
                qType = (int)myReader["qType"];
                surveyId = (int)myReader["sID"];
                questionId = (int)myReader["qID"];
                maxLength = (int)myReader["maxLength"];
                
                string answer = Console.ReadLine();

                if (answer.Length <= maxLength)
                {
                    lista.Add(string.Format("INSERT INTO Answers(surveyID, qType, answer, userKey, qID) Values('{0}', '{1}', '{2}', '{3}', '{4}');", surveyId, qType, answer, userKey, questionId));

                    notEoF = myReader.Read();
                }

                else
                {
                    Console.WriteLine("Your answer is too long");
                    Console.WriteLine("Max length for this question is: " + maxLength + " characters");
                }

                
               
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

}
