//____________Using Directives__________
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

//______________Namespace Declaration__________
namespace MiniBankSystemProjectOverview
{
    //___________Class Declaration________
    class Program
    {
        // _______Constants_________
        const double MinimumBalance = 100.0;
        const string AccountsFilePath = "accounts.txt";
        const string ReviewsFilePath = "reviews.txt";
        const string RequestsFilePath = "requests.txt";
        const string InAcceptRequestsFilePath = "InRequests.txt";
        const string AdminInformationFilePath = "Admin.txt";
        private static readonly char adminChoice;
        private static readonly bool inAdminMenu;

        // ____Global lists (parallel)______
        static List<int> accountNumbers = new List<int>();
        static List<string> accountNames = new List<string>();
        static List<double> balances = new List<double>();

        //______generate ID number for every account__________ 
        static int LastAccountNumber = 0;
        static int IndexID = 0;

        //______generate ID number for Admin account_________ 
        static int LastAdminAccountNumber = 0;

        //_____Admin Login information________ 
        static List<int> AdminAccountNumber = new List<int>();
        static List<string> AdminName = new List<string>();
        static List<string> AdminID = new List<string>();

        //_____Stacks & Queues_____________
        static Stack<string> reviewsStack = new Stack<string>();
        static Stack<string> UserReviewsStack = new Stack<string>();

        //static Queue<(string name, string nationalID)> createAccountRequests = new Queue<(string, string)>();
        static Queue<string> createAccountRequests = new Queue<string>(); // format: "Name|NationalID"
        static Queue<string> InAcceptcreateAccountRequests = new Queue<string>(); // format: "Name|NationalID"

        //_______Export All Account Info file path_________
        static string ExportFilePath = "ExportedAccounts.txt";

        //_______Account number generator_____
        static int lastAccountNumber;


        //________Account data in Lists_________
        static List<string> AccountUserNames = new List<string>();
        static List<string> AccountUserNationalID = new List<string>();
        static List<double> UserBalances = new List<double>();


        // ____________________________ Menu Functions _______________________
        static void Main(string[] args)
        {


            LoadAccountsInformationFromFile(); //This function makes the program retrieve all previous calculations from a text file and prepare them for operation while it runs.
            LoadReviews(); //Read reviews from the file and refill them in the stack.
            LoadRequests(); //To download or retrieve all requests from a source
            LoadAdminInformationFromFile(); //To download Admin information from the file
            LoadInAcceptRequests(); //To upload applications that have not yet been accepted (not accepted or pending).

            // _______main menu for system bank to store user choice in avriable _______
            bool running = true;
            char choice;
            do
            {
                while (running)
                {
                    Console.Clear();//____just to clear the screen____
                    Console.WriteLine("\n====== Welcome To Bank System ======");
                    Console.WriteLine("1. User Menu");
                    Console.WriteLine("2. Admin Menu");
                    Console.WriteLine("0. Exit the system");
                    Console.Write(" Please Select Your Option:\n ");
                    string mainChoice = Console.ReadLine();

                    // user switch method to select one of many code blocks to be executed.
                    switch (mainChoice)
                    {
                        case "1": UserMenu(); break;  // case to display user menu
                        case "2": AdminMenu(); break;   // case to display Admin menu
                        case "0":    // case to Exist from whole system
                            SaveAccountsInformationToFile();// This saves all account data to a text file in an organized and secure manner.
                            SaveReviews(); //All revisions in a stack are saved in a text file.
                            SaveRequestsToFaile(); //Save orders to a file
                            SaveAdminInformationToFile(); //Save manager/administrator information to a file.
                            SaveInRequestsToFaile(); //Save "Entry Requests" or "Internal Requests" to a file


                            running = false; //____Keep running  false to repeat the loop____   
                            break;
                        default: Console.WriteLine("Invalid choice , please try agine!"); break;
                    }
                }

                Console.WriteLine("Do You Want Another Option  ? y / n");
                choice = Console.ReadKey().KeyChar;

            } while (choice == 'y' || choice == 'Y');
            Console.WriteLine("Thank you for using Bank System!");
        }


        // ==========  Addition The  Function Types Of The  User Menu==========
        static void UserMenu()
        {
            bool inUserMenu = true;
            //const int MaxLoginAttempts = 5;
            while (inUserMenu)
            {
                Console.Clear();
                Console.WriteLine("\n------ User Menu ------");
                Console.WriteLine("1. Request Account Creation");
                Console.WriteLine("2. Login");
                Console.WriteLine("0. Return to Main Menu");
                Console.Write("Select Option: ");
                string userChoice = Console.ReadLine();
                Console.WriteLine();



                switch (userChoice)
                {
                    case "1": RequestAccountCreation(); break;
                    case "2":
                        IndexID = UserLoginWithID();
                        Console.ReadLine(); // Wait for user input before continuing
                        if (IndexID != -1)
                        {
                            UserMenuOperations(IndexID);
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check your National ID.");
                        }
                        break;
                    // case to exist from user menu and Return to Main Menu 
                    case "0": inUserMenu = false; break;
                    default: Console.WriteLine("Invalid choice , Please Try Agine! "); break;
                }
                Console.WriteLine("press any key");
                Console.ReadLine();
            }
        }

        // ==========  Addition The  Function Types Of The  Admin Menu==========

        static void AdminMenu()
        {
            bool inAdminMenu = true;
            // while loop to display the mnue ewhile the flag is true
            while (inAdminMenu)
            {
                Console.Clear();
                Console.WriteLine("\n------ Admin Menu ------");
                Console.WriteLine("1. create account");
                Console.WriteLine("2. Login");
                Console.WriteLine("0. Return to Main Menu");
                Console.Write("Select option: ");
                string adminChoice = Console.ReadLine();

                switch (adminChoice)
                {
                    // case to Request Account Creation
                    case "1":
                        AdminCreateAccount();
                        Console.ReadLine();
                        break;

                    case "2":
                        IndexID = AdminLoginWithID();
                        Console.ReadLine(); // Wait for user input before continuing
                        if (IndexID != -1)
                        {
                            AdminMenuOperations();
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check your National ID.");
                        }
                        break;

                    case "0": inAdminMenu = false; break; // this will Eixt the  loop and return
                    default:
                        Console.WriteLine("Invalid choice.");

                        Console.WriteLine("press any key");
                        Console.ReadLine();

                        break;
                }

            }
        }

        // ========== User features function ==========
        //_______________Request account creation (1) ______
        static void RequestAccountCreation()
        {
            string UserName = "";
            bool ValidName = true;
            string UserID = "";
            bool ValidID = true;
            bool IsSave = true;
            int tries = 0;

            //  ____Adding  Error Handling____
            try
            {
                do
                {
                    // ask user to enter his name
                    Console.WriteLine("Enter Your Name: ");
                    string name = Console.ReadLine();
                    // valid the name input 
                    ValidName = stringOnlyLetterValidation(name);
                    if (ValidName == true)
                    {
                        UserName = name;
                        IsSave = true;
                    }
                    else
                    {
                        IsSave = false;
                        tries++;
                    }
                } while (ValidName == false && tries < 3);

                tries = 0;
                do
                {

                    // ask user to enter his national ID 
                    Console.WriteLine("Enter your National ID: ");
                    string ID = Console.ReadLine();

                    // check if number already exist in the list of accounts
                    bool IDExist = CheckUserIDExist(ID);
                    if (IDExist)
                    {
                        Console.WriteLine("This National ID already exists. Please enter a different ID.");
                        IsSave = false;
                        tries++;
                    }
                    else
                    {


                        // valid the ID input
                        ValidID = IDValidation(ID);
                        // check if 

                        if (ValidID == true)
                        {
                            UserID = ID;
                            IsSave = true;
                        }
                        else
                        {
                            IsSave = false;
                            tries++;
                        }

                    }
                } while (IsSave == false && tries < 3);
                // check if user tries to enter valid ID more than 3 times
                if (tries == 3)
                {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                    Console.ReadLine();
                    return;
                }
                // reset tries to 0 for next validation
                tries = 0;


                if (IsSave == true)
                {
                    bool AlreadyExist = false;
                    // loop in queue to chech if request with same id already submit to Prevent Duplicate Account Requests 
                    for (int i = 0; i < AdminID.Count; i++)
                    {
                        //check if id in rqures queue id exist or not 
                        if (AdminID[i] == UserID)
                        {
                            // if yes put AlreadyRequested flag with true value
                            AlreadyExist = true;
                            break;
                        }

                    }
                    // based on AlreadyRequested flad we decided if we save user inputes of account information or not 
                    if (AlreadyExist)
                    {
                        Console.WriteLine("Admin with this ID number is already exist");
                    }
                    else
                    {
                        int NewAdminAccountNumber = LastAdminAccountNumber + 1;
                        AdminAccountNumber.Add(NewAdminAccountNumber);
                        AdminName.Add(UserName);
                        AdminID.Add(UserID);
                        Console.WriteLine("Request Account Creation successfully submitted.");
                        LastAdminAccountNumber = NewAdminAccountNumber;
                    }
                }


            }
            catch
            {
                // display message submit failed 
                Console.WriteLine("Admin Account Creation failed");
            }

        }