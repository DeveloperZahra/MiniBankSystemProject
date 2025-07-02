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
