using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asdasd
{
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
                        case "4":
                            Console.WriteLine("Insert 'q' to go back");
                            app.GetAnswers();
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
                            Console.WriteLine("Insert 'q' to go back");
                            Console.WriteLine("Insert a survey key");
                            app.AnswerSurvey();
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

}
