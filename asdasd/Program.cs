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
        Respondent = 1,
        Manager = 2,
        Admin = 3
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
        private int respondentID;
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





    class Program
    {
        static void Main(string[] args)
        {
            Application app = new Application();

        }
    }

}

