using ATMApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMApp.UI
{
    public class AppScreen
    {
        internal const string cur = "R ";
        internal static void Welcome() 
        {
            //clears the console screen
            Console.Clear();
            //sets the title of the console window
            Console.Title = "My ATM App";

            //SETS the text color 
            Console.ForegroundColor = ConsoleColor.White;


            //set the welcome message 
            Console.WriteLine("\n\n-----------------------Welcome to my ATm App--------------------------\n\n");
            // prompt the user to insert atm card
            Console.WriteLine("Please insert your ATM Card");
            Console.WriteLine("Note: Actucal ATM machine will accept and validate a physical ATM card, reead the card number and validate it.");
            Utility.PressEnterToContinue();
            

        }
        internal static UserAccount UserLoginForm() 
        {
            UserAccount tempUserAccount = new UserAccount();
            tempUserAccount.CardNumber = Validator.Convert<long>("your Card Number.");
            tempUserAccount.CardPin = Convert.ToInt32(Utility.GetSecretInput("Enter your Card PIN "));
            return tempUserAccount;
        }

        internal static void LoginProgress()
        {
            Console.WriteLine("\nChecking card number and PIN...");
            Utility.PrintDotAnimation();
        }

        internal static void PrintLockScreen() 
        {
            Console.Clear();
            Utility.PrintMessage("Your account is locked. Please go to the nearest branch to unlcok your account. Thank you ",true);
            Utility.PressEnterToContinue();
            Environment.Exit(1);
        }

        internal static void WelcomeCustomer(string fullName)
        {
            Console.WriteLine($"Welcome back, {fullName}");
            Utility.PressEnterToContinue();
        }
        internal static void DisplayAppMenu() 
        {
            Console.Clear();
            Console.WriteLine("-----------My ATM App Menu---------------");
            Console.WriteLine(":                                       :");
            Console.WriteLine("1. Account Balance                      :");
            Console.WriteLine("2. Cash Deposit                         :");
            Console.WriteLine("3. Withdrawl                            :");
            Console.WriteLine("4. Transfer                             :");
            Console.WriteLine("5. Transactions                         :");
            Console.WriteLine("6. Logout                               :");
        }

        internal static void LogoutProgress() 
        {
            Console.WriteLine("Thank you for using My ATM App.");
            Utility.PrintDotAnimation();
            Console.Clear();
        }

        internal static int SelectAmount() 
        {
            Console.WriteLine("");
            Console.WriteLine(":1.{0}100   5.{0}5000", cur);
            Console.WriteLine(":2.{0}200   6.{0}10,000", cur);
            Console.WriteLine(":3.{0}500   7.{0}15,000", cur);
            Console.WriteLine(":4.{0}1000  8.{0}20,000", cur);
            Console.WriteLine(":0.Other");
            Console.WriteLine("");

            int selectedAmount = Validator.Convert<int>("option:");
            switch (selectedAmount)
            {
                case 1:
                    return 100;
                    break;
                case 2:
                    return 200;
                    break;
                case 3:
                    return 500;
                    break;
                case 4:
                    return 1000;
                    break;
                case 5:
                    return 50000;
                    break;
                case 6:
                    return 100000;
                    break;
                case 7:
                    return 15000;
                    break;
                case 8:
                    return 20000;
                    break;
                case 0:
                    return 0;
                    break;
                default:
                    Utility.PrintMessage("Invalid input. Try again.",false);
                   
                    return -1;
                    break;

            }
        }
        internal InternalTransfer InternalTransferForm() 
        {
            var internalTransfer = new InternalTransfer();
            internalTransfer.ReciepeintBankAccountNumber = Validator.Convert<long>("recipients account:");
            internalTransfer.TransferAmount = Validator.Convert<decimal>($"amount {cur}");
            internalTransfer.ReciepeintBankAccountName = Utility.GetUserInput("recipient's name");
            return internalTransfer;
        }
    }
}
