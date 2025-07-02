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

        //________User Menu Operation__________
        public static void UserMenuOperations(int IndexID)
        {
            bool inUserMenu = true;
            // while loop to display the mnue ewhile the flag is true 
            while (inUserMenu)
            {
                Console.Clear();
                Console.WriteLine("\n------ User Menu Operation ------");
                Console.WriteLine("1. Deposit");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. View Balance");
                Console.WriteLine("4. Submit Review/Complaint");
                Console.WriteLine("5. Transfer Money");
                Console.WriteLine("6. Undo Last Complaint");
                Console.WriteLine("0. Return to Main Menu");
                Console.Write("Select Option: ");
                string userChoice = Console.ReadLine();
                Console.WriteLine();


                switch (userChoice)
                {
                    // case to Deposit
                    case "1":

                        Console.WriteLine("Proceeding to deposit...");
                        Deposit(IndexID); // If user exists, proceed with deposit
                        Console.ReadLine(); // Wait for user input before continuing
                        break;

                    // case to Withdraw
                    case "2":

                        Console.WriteLine("Proceeding to withdraw...");
                        withdraw(); // If user exists, proceed with withdraw
                        Console.ReadLine(); // Wait for user input before continuing
                        break;

                    // case to View Balance
                    case "3":

                        Console.WriteLine("Proceeding to Check Balance...");
                        CheckBalance(IndexID); // If user exists, proceed with chech balance
                        Console.ReadLine(); // Wait for user input before continuing
                        break;

                    // case to Submit Review/Complaint
                    case "4":
                        Console.WriteLine("Review submitte...");
                        SubmitReview();
                        Console.ReadLine();
                        break;

                    // Transfer Money
                    case "5":
                        // Ask user to enter the National ID of the account to transfer money to
                        int UserIndexID2 = UserLoginWithID();
                        if (UserIndexID2 != -1 && UserIndexID2 != IndexID) // when user login to its account by accountID number, this number save in value IndexID which decalre in "internal calss program" so when want to transer from its account to another account, no need to enter its accountIDNumber agine it save temberary in variable "IndexID"
                        {
                            Transfer(IndexID, UserIndexID2); // If user exists, proceed with transfer
                        }
                        else if (UserIndexID2 == IndexID)
                        {
                            Console.WriteLine("You cannot transfer money to your own account.");
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check the National ID of the recipient.");
                        }
                        Console.ReadLine(); // Wait for user input before continuing
                        break;

                    // Undo Last Complaint
                    case "6":
                        UndoLastComplaint();
                        Console.ReadLine(); // Wait for user input before continuing
                        break;

                    // case to exist from user menu and Return to Main Menu 
                    case "0": inUserMenu = false; break;
                    default: Console.WriteLine("Invalid choice , Please Try Agine! "); break;
                }
                Console.WriteLine("press any key");
                Console.ReadLine();
            }
        }

        //___________Deposit Function_________ 
        public static void Deposit(int IndexID)
        {
            int tries = 0;
            // Initialize a boolean flag to control the deposit loop.
            bool IsDeposit = false;
            // Initialize a variable to store the final parsed deposit amount.
            double FinalDepositAmount = 0.0;
            // Initialize an index to find the user's position in the account list.

            // Start a try block to catch potential runtime exceptions.
            try
            {
                // Repeat until a valid deposit is made.
                do
                {
                    //IndexID = LoginWithID();
                    Console.WriteLine("Enter the amount of money you want to deposit: ");
                    string DepositAmount = Console.ReadLine();
                    // Validate the entered amount using a custom method.
                    bool ValidDepositAmount = AmountValid(DepositAmount);
                    if (ValidDepositAmount == false)
                    {
                        // Display error if the input is not valid.
                        Console.WriteLine("Invalid input");
                        IsDeposit = false;
                        tries++;
                    }
                    // If input is valid, find the user index.
                    else
                    {
                        // convert string to double using TryParse
                        double.TryParse(DepositAmount, out FinalDepositAmount);

                        // Update the user's balance by adding the deposit amount.
                        UserBalances[IndexID] = UserBalances[IndexID] + FinalDepositAmount;
                        UserBalances[IndexID] = UserBalances[IndexID] + FinalDepositAmount;
                        PrintReceipt(transactionType: "Deposit", amount: FinalDepositAmount, balance: UserBalances[IndexID]);
                        // Set the flag to true to exit the loop.
                        IsDeposit = true;
                        // Exit the method (if inside a method).
                        return;

                    }
                    if (tries == 3)
                    {
                        Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                        return;
                    }
                } while (IsDeposit == false && tries < 3);
            }
            //Print any exception message that occurs during execution.
            catch (Exception e) { Console.WriteLine(e.Message); }

        }

        //__________________Withdraw Function_____________ 
        public static void withdraw()
        {
            int tries = 0;
            // Initialize a boolean flag to control the deposit loop.
            bool IsWithdraw = false;
            // Initialize a variable to store the final parsed deposit amount.
            double FinalwithdrawAmount = 0.0;
            // declare variable print balance after any process
            double BalanceAfterProcess = 0.0;
            // Start a try block to catch potential runtime exceptions.
            try
            {
                // Repeat until a valid deposit is made.
                do
                {

                    Console.WriteLine("Enter the amount of money you want to withdrw from your balance: ");
                    string WithdrawAmount = Console.ReadLine();
                    // Validate the entered amount using a custom method.
                    bool ValidWithAmount = AmountValid(WithdrawAmount);
                    if (ValidWithAmount == false)
                    {
                        // Display error if the input is not valid.
                        Console.WriteLine("Invalid input");
                        IsWithdraw = false;
                        tries++;
                    }
                    // If input is valid, find the user index.
                    else
                    {

                        // convert string to double using TryParse
                        double.TryParse(WithdrawAmount, out FinalwithdrawAmount);
                        // check if user balamce is less than or equal MinimumBalance
                        bool checkBalance = CheckBalanceAmount(FinalwithdrawAmount, IndexID);
                        if (checkBalance == true)
                        {
                            // Update the user's balance by adding the deposit amount.
                            UserBalances[IndexID] = UserBalances[IndexID] - FinalwithdrawAmount;
                            Console.WriteLine($"Successfully withdraw");
                            PrintReceipt(transactionType: "Withdraw", amount: FinalwithdrawAmount, balance: UserBalances[IndexID]);
                            // Set the flag to true to exit the loop.
                            IsWithdraw = true;
                        }
                        else
                        {
                            BalanceAfterProcess = UserBalances[IndexID] - FinalwithdrawAmount;
                            Console.WriteLine($"Can not withdraw {FinalwithdrawAmount} from your balance, becouse your balance after with draw is {BalanceAfterProcess} which less than 100.00");
                        }
                        return;

                    }
                    if (tries == 3)
                    {
                        Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                        return;
                    }
                } while (IsWithdraw == false && tries < 3);
            }
            //Print any exception message that occurs during execution.
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
        //__________Check Balance Function________
        public static void CheckBalance(int IndexID)
        {
            Console.WriteLine($"Your Current Balance is {UserBalances[IndexID]}");
        }

        //___________Submit Review Function__________ 
        public static void SubmitReview()
        {
            //error handling using try-catch
            try
            {
                // ask user to enter their review
                Console.WriteLine("Enter Your Review");
                string review = Console.ReadLine();
                //check valid the input review 
                bool ValidReview = StringlettersWithNumbers(review);
                if (ValidReview == true)
                {// push review in stack named "UserReviews"
                    UserReviewsStack.Push(review);
                    Console.WriteLine("Your Review successfully submited");
                }
                else
                {
                    Console.WriteLine("Unvalid input review");
                }

            }
            catch
            {
                //display massage if review 
                Console.WriteLine("Failed Submitted, try agine!");
            }
        }

        //___________login user____________ 
        public static int UserLoginWithID()
        {
            int tries = 0;
            int IndexId = -1;
            bool UserExist = false;
            string ID = "";
            do
            {
                // Prompt user to enter their National ID
                Console.WriteLine("Enter User National ID: ");
                ID = Console.ReadLine(); // Read user input from console                           
                // valid user exist
                UserExist = UserLogin(ID);
                if (UserExist == false)
                {
                    tries++;
                }
                if (tries == 3)
                {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid ID.");
                    break;
                }
            } while (UserExist == false && tries < 3);
            if (UserExist == true)
            {
                //lopp thriugh items in list
                for (int i = 0; i < AccountUserNationalID.Count; i++)
                {
                    //check if Input exist in the list 
                    if (AccountUserNationalID[i] == ID)
                    {
                        // Store the index of the user with the matching ID.
                        IndexId = i;
                    }
                }
            }

            return IndexId;
        }


        //_____________Process Next Acount Request(2)____________
        static void ProcessNextAccountRequest()
        {
            //check if the list of account requests is empty or not. If it is empty, it prints a message "There are no pending account requests", then exits the function directly using return.
            if (createAccountRequests.Count == 0)
            {
                Console.WriteLine("No pending account requests.");
                return;
            }

            //var (name, nationalID) = createAccountRequests.Dequeue();
            string request = createAccountRequests.Dequeue();//This request is a string that takes the first request in the createAccountRequests list, dequeues it, and stores it in the request variable.
                                                             //Here, the string is split into two parts using the | symbol as a separator.
            string[] parts = request.Split('|');
            // The resulting array will be parts, where parts[0] = the name and parts[1] = the national ID number.
            string name = parts[0];
            string nationalID = parts[1];

            int newAccountNumber = lastAccountNumber + 1;//This creates a new account number by adding 1 to the last registered account number.

            accountNumbers.Add(newAccountNumber);//to Adds new account to several lists
            accountNames.Add($"{name} ");
            balances.Add(0.0);

            lastAccountNumber = newAccountNumber;//The last account number (lastAccountNumber) will be the new account number that was created.

            Console.WriteLine($"Account created for {name} with Account Number: {newAccountNumber}");
        }

        /*
       //_________________Transfer money________
          • Let users transfer money between two account numbers. 
          • Check balance constraints on sender before transferring.
       */
        public static void Transfer(int UserIndexID, int UserIndexID2)
        {
            string TransferAmount = "";
            double FinalTransferAmount = 0.0;
            bool IsTransfer = false;
            int tries = 0;
            // Start a try block to catch potential runtime exceptions.
            int IndexId = -1;
            bool UserExist = false;
            try
            {
                // Repeat until a valid transfer is made.
                do
                {
                    // Ask user to enter the amount to transfer.
                    Console.WriteLine("Enter the amount of money you want to transfer: ");
                    TransferAmount = Console.ReadLine();
                    // Validate the entered amount using a custom method.
                    bool ValidTransferAmount = AmountValid(TransferAmount);
                    if (ValidTransferAmount == false)
                    {
                        // Display error if the input is not valid.
                        Console.WriteLine("Invalid input");
                        IsTransfer = false;
                        tries++;
                    }
                    else
                    {
                        // Convert string to double using TryParse
                        double.TryParse(TransferAmount, out FinalTransferAmount);
                        // Check if user balance is sufficient for the transfer.
                        bool checkBalance = CheckBalanceAmount(FinalTransferAmount, UserIndexID);
                        if (checkBalance == true)
                        {
                            // Update the sender's balance by subtracting the transfer amount.
                            UserBalances[UserIndexID] -= FinalTransferAmount;
                            // Update the receiver's balance by adding the transfer amount.
                            UserBalances[UserIndexID2] += FinalTransferAmount;
                            Console.WriteLine($"Successfully transferred {FinalTransferAmount} from Account {AccountNumbers[UserIndexID]} to Account {AccountNumbers[UserIndexID2]}");
                            Console.WriteLine($"Your Current Balance is {UserBalances[UserIndexID]}");
                            IsTransfer = true; // Set flag to true to exit loop.
                        }
                        else
                        {
                            Console.WriteLine($"Cannot transfer {FinalTransferAmount} from your balance, as it would result in a balance below 100.00.");
                        }
                        return; // Exit method after successful transfer.
                    }
                    if (tries == 3)
                    {
                        Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                        return;
                    }
                } while (IsTransfer == false && tries < 3);
            }
            catch (Exception e) { Console.WriteLine(e.Message); } // Print any exception message that occurs during execution.
        }

        //_________Undo Last Complaint Submitted________ 
        public static void UndoLastComplaint()
        {
            // Check if there are any reviews in the stack
            if (UserReviewsStack.Count > 0)
            {
                // Pop the last review from the stack
                string lastReview = UserReviewsStack.Pop();
                Console.WriteLine($"Last review '{lastReview}' has been undone.");
            }
            else
            {
                Console.WriteLine("No reviews to undo.");
            }
        }

        //_________Print Receipt After Deposit/Withdraw_________ 
        public static void PrintReceipt(string transactionType, double amount, double balance)
        {
            Console.WriteLine("\n--- Transaction Receipt ---");
            Console.WriteLine($"Transaction Type: {transactionType}");
            Console.WriteLine($"Amount: {amount}");
            Console.WriteLine($"New Balance: {balance}");
            Console.WriteLine("---------------------------\n");
        }

        // ===================== Admin Features Function ==========================
        // Admin create account 
        public static void AdminCreateAccount()
        {
            string UserName = "";
            bool ValidName = true;
            string UserID = "";
            bool ValidID = false;
            bool IsSave = true;
            int tries = 0;
            // Error handling 
            try
            {
                // Enter User Name Process
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
                    if (tries == 3)
                    {
                        Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                        return;
                    }
                } while (ValidName == false && tries < 3);

                tries = 0;
                do
                {
                    // ask user to enter his national ID 
                    Console.WriteLine("Enter your National ID: ");
                    string ID = Console.ReadLine();
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
                    if (tries == 3)
                    {
                        Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid value.");
                        return;
                    }
                } while (ValidID == false && tries < 3);
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
        //_____Admin Operation______ 
        public static void AdminMenuOperations()
        {
            bool InAdminMenu = true;
            // while loop to display the mnue ewhile the flag is true
            while (inAdminMenu)
            {
                Console.Clear();
                Console.WriteLine("\n------ Admin Menu ------");
                Console.WriteLine("1. Process Next Account Request");
                Console.WriteLine("2. View Submitted Reviews");
                Console.WriteLine("3. View All Accounts");
                Console.WriteLine("4. View Pending Account Requests");
                Console.WriteLine("5. Search User account by user National ID");
                Console.WriteLine("6. Show Total Bank Balance");
                Console.WriteLine("7. Delete Account");
                Console.WriteLine("8. Show Top 3 Richest Customers");
                Console.WriteLine("9. Export All Account Info to a New File (CSV or txt)");
                Console.WriteLine("0. Return to Main Menu");
                Console.Write("Select Option: ");
                string userChoice = Console.ReadLine();
                Console.WriteLine();

                // use switch to select one of many code blocks to be executed
                switch (adminChoice)
                {
                    // case to Process Next Account Request
                    case '1':
                        ProcessAccountRequest();
                        Console.ReadLine();
                        break;
                    // case to View Submitted Reviews
                    case '2':
                        ViewReviews();
                        Console.ReadLine();
                        break;
                    // case to View All Accounts
                    case '3':
                        ViewAllAccounts();
                        Console.ReadLine();
                        break;
                    // case to View Pending Account Requests
                    case '4':
                        ViewPendingRequests();
                        Console.ReadLine();
                        break;
                    // Search user by enter user National ID
                    case '5':
                        int UserIndexID = UserLoginWithID();
                        SearchUserByNationalID(UserIndexID);
                        Console.ReadLine();
                        break;
                    // Show Total Bank Balance
                    case '6':
                        ShowTotalBankBalance();
                        Console.ReadLine();
                        break;
                    // Delete Account
                    case '7':
                        int deleteIndexID = UserLoginWithID();
                        if (deleteIndexID != -1)
                        {
                            DeleteAccount(deleteIndexID);
                        }
                        else
                        {
                            Console.WriteLine("Login failed. Please check your National ID.");
                        }
                        Console.ReadLine();
                        break;
                    case '8':
                        ShowTop3RichestCustomers();
                        Console.ReadLine();
                        break;
                    // Export All Account Info to a New File (CSV or txt)
                    case '9':
                        ExportAccountsToFile(ExportFilePath);
                        Console.WriteLine($"All account information has been exported to {ExportFilePath}");
                        Console.ReadLine();
                        break;
                    // case to Return to Main Menu
                    case '0':
                        InAdminMenu = false; // this will exit the loop and return
                        break;
                    // default case to display message to the admin if selected the wronge number
                    default:
                        Console.WriteLine("Wronge choice number, Try Agine!");
                        Console.ReadKey();
                        break;

                }
            }

        }

        //________login Admin__________ 
        public static int AdminLoginWithID()
        {
            int tries = 0;
            int IndexId = -1;
            bool AdminExist = false;
            string ID = "";
            do
            {
                // Prompt user to enter their National ID
                Console.WriteLine("Enter You National ID: ");
                ID = Console.ReadLine(); // Read user input from console
                AdminExist = AdminLogin(ID);
                if (AdminExist == false)
                {
                    tries++;
                }
                if (tries == 3)
                {
                    Console.WriteLine("You have exceeded the number of times you are allowed to enter a valid ID.");
                    break;
                }
            } while (AdminExist == false && tries < 3);
            if (AdminExist == true)
            {
                //loop thriugh items in list
                for (int i = 0; i < AdminID.Count; i++)
                {
                    //check if Input exist in the list 
                    if (AdminID[i] == ID)
                    {
                        // Store the index of the user with the matching ID.
                        IndexId = i;
                    }
                }
            }

            return IndexId;
        }
        //_______View Pending Requests Function_______ 
        public static void ViewPendingRequests()
        {
            //check if tere is any pending requests
            if (createAccountRequests.Count() == 0)
            {
                Console.WriteLine("There is no pending request yet");
            }
            else
            {
                // display all pending request
                foreach (string request in createAccountRequests)
                {
                    Console.WriteLine(request);
                    Console.WriteLine("=======================");
                }
            }
        }
        //___________ View All Accounts Function______________ 
        public static void ViewAllAccounts()
        {

            Console.WriteLine($"Account ID {"|"} User Name {"|"} National ID {"|"} Balance Amount");
            //iteration for loop all index values in lists
            for (int i = 0; i < AccountUserNationalID.Count; i++)
            {
                //display list values for every index
                Console.WriteLine($"{accountNumbers[i]}\t{"|"}{AccountUserNames[i]}\t{"|"}{AccountUserNationalID[i]}\t{"|"}{UserBalances[i]}");

            }

        }

        //___________ View Reviews Function______________ 
        public static void ViewReviews()
        {
            //error handling using try-catch
            try
            {
                //iteration all users reviews in UserReviews stack 
                foreach (string Review in UserReviewsStack)
                {
                    Console.WriteLine(Review);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        //______Process Account Request Function____ 
        public static void ProcessAccountRequest()
        {
            // handling error using try-catch
            try
            {
                // check if there is request
                if (createAccountRequests.Count == 0)
                {
                    Console.WriteLine("No pending account requests.");
                    return;
                }
                // get last element (which it is first element enter) in the queue
                string request = createAccountRequests.Dequeue();
                // Split the request string using '|' to separate username and national ID
                string[] SplitRrquest = request.Split("|");
                // Extract and store the username from the request
                string UserName = SplitRrquest[0];
                Console.WriteLine($"User Name: {UserName} ");
                // Extract and store the national ID from the request
                string UserNationalID = SplitRrquest[1];
                Console.WriteLine($"User National ID: {UserNationalID} ");
                // Increment the last account ID number for the new account
                int NewAccountIDNumber = LastAccountNumber + 1;
                // Set initial account balance to 0
                double balance = MinimumBalance;
                Console.WriteLine("Do you want to accept account creation request (y/n) !");
                char choice = Console.ReadKey().KeyChar;
                if (choice == 'y' || choice == 'Y')
                {
                    // Add user name in the AccountUserNames list
                    AccountUserNames.Add(UserName);
                    // Add user national ID in the AccountUserNationalID list
                    AccountUserNationalID.Add(UserNationalID);
                    // Add user Account ID in the AccountIDNumbers list
                    accountNumbers.Add(NewAccountIDNumber);
                    // Add user initial balance in the Balances list
                    UserBalances.Add(balance);
                    // add user type 
                    Console.WriteLine($"Account created for {UserName} with Account Number: {NewAccountIDNumber}");
                    LastAccountNumber = NewAccountIDNumber;
                }
                else
                {
                    Console.WriteLine("Account Dose not accept!");
                    string InAcceptRequest = request;
                    InAcceptcreateAccountRequests.Enqueue(InAcceptRequest);
                }
            }
            catch
            {
                //display massage to the user if anyy error happened during running program 
                Console.WriteLine("Accept process fail, Try Agine!");
            }

        }
        //______Search by National ID or Name (Admin Tool)_____
        public static void SearchUserByNationalID(int UserIndexID)
        {

            //display user account number, name , balance with the enter national id 
            Console.WriteLine($"User Account Numbaer : {accountNumbers[UserIndexID]}");
            Console.WriteLine($"User Name : {AccountUserNames[UserIndexID]}");
            Console.WriteLine($"User Balance : {UserBalances[UserIndexID]}");


        }
        //_______Show Total Bank Balance______
        public static void ShowTotalBankBalance()
        {
            // Calculate the total balance by summing all user balances
            double totalBalance = UserBalances.Sum();
            // Display the total balance
            Console.WriteLine($"Total Bank Balance: {totalBalance}");
        }


        //_________Delete Account___________
        public static void DeleteAccount(int IndexID)
        {
            // Check if the user exists in the list
            if (IndexID >= 0 && IndexID < AccountUserNationalID.Count)
            {
                // Remove the user from all lists
                AccountUserNames.RemoveAt(IndexID);
                AccountUserNationalID.RemoveAt(IndexID);
                UserBalances.RemoveAt(IndexID);
                accountNumbers.RemoveAt(IndexID);
                Console.WriteLine("Account deleted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid user index. Account deletion failed.");
            }
        }

        //_____________Show Top 3 Richest Customers_________ 
        public static void ShowTop3RichestCustomers()
        {
            // Check if there are at least 3 users
            if (UserBalances.Count < 3)
            {
                Console.WriteLine("Not enough users to determine the top 3 richest customers.");
                return;
            }
            // Create a list of tuples containing user index and balance
            var userBalancesWithIndex = UserBalances.Select((balance, index) => (index, balance)).ToList();
            // Sort the list by balance in descending order
            var top3Richest = userBalancesWithIndex.OrderByDescending(x => x.balance).Take(3).ToList();
            // Display the top 3 richest customers
            Console.WriteLine("Top 3 Richest Customers:");
            foreach (var user in top3Richest)
            {
                Console.WriteLine($"Account Number: {accountNumbers[user.index]}, Name: {AccountUserNames[user.index]}, Balance: {user.balance}");
            }
        }

        /* 
      _________________Export All Account Info to a New File (CSV or txt)__________ 
         • Create a clean export with headers and all customer info.
     */

        public static void ExportAccountsToFile(string filePath)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write headers
                    writer.WriteLine("Account Number,User Name,National ID,Balance");
                    // Iterate through all accounts and write their information
                    for (int i = 0; i < AccountUserNationalID.Count; i++)
                    {
                        writer.WriteLine($"{accountNumbers[i]},{AccountUserNames[i]},{AccountUserNationalID[i]},{UserBalances[i]}");
                    }
                }
                Console.WriteLine($"Accounts exported successfully to {filePath}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error exporting accounts: {e.Message}");
            }
        }