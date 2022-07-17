using ATMApp.Domain.Entities;
using ATMApp.Domain.Enums;
using ATMApp.UI;
using ConsoleTables;
using System;

namespace ATMApp.App
{
   public class ATMApp : Domain.Interfaces.IUserLogin, IuserAccountActions, Domain.Interfaces.ITransaction
    {
        //creating new fields:
        private List<UserAccount> userAccountList;
        private UserAccount userAccountAccount;
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const decimal minimumKeptAmount = 500;
        private readonly AppScreen screen;

        public ATMApp() 
        {
            screen = new AppScreen();   
        }

        public void Run() 
        {
            AppScreen.Welcome();
            CheckUserCardNumAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            while (true) 
            { 
                AppScreen.DisplayAppMenu();
            
                ProcessMenuOption();
            }

        }
           
        public void InitializeData() 
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount { Id = 1,FullName = "Sino Max", AccountNumber = 123456,CardNumber=321321, CardPin =123123,AccountBalance = 50000.00m,IsLocked=false },
                 new UserAccount { Id = 2,FullName = "Amber Heard", AccountNumber = 456789,CardNumber=654654, CardPin =456456,AccountBalance = 40000.00m,IsLocked=false },
                  new UserAccount { Id = 3,FullName = "Johnny Depp", AccountNumber = 123555,CardNumber=987987, CardPin =789789,AccountBalance = 50000.00m,IsLocked=true },
            };
            _listOfTransactions = new List<Transaction>();
        }
        public void CheckUserCardNumAndPassword() 
        {
            bool isCorrectLogin = false;
            //NewMethod();
            while (isCorrectLogin == false)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProgress();
                foreach (UserAccount account in userAccountList) 
                {
                    selectedAccount = account;
                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        selectedAccount.TotalLogin++;

                        if (inputAccount.CardPin.Equals(selectedAccount.CardPin)) 
                        {
                            selectedAccount = account;

                            if (selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                            {
                                //print a lock message
                                AppScreen.PrintLockScreen();

                            }
                            else 
                            {
                                selectedAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }
                    if (isCorrectLogin == false)
                    {
                        Utility.PrintMessage("\n Invalid card number or PIN.", false);
                        selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                        if (selectedAccount.IsLocked)
                        {
                            AppScreen.PrintLockScreen();
                        }
                    }
                    Console.Clear();
                }
            }
            
            
            
           
        }
        private void ProcessMenuOption() 
        {
            switch (Validator.Convert<int>("an option: ")) 
            {
                case(int)AppMenu.CheckBalance:
                    CheckBalance();
                    break;
                case(int)AppMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)AppMenu.MakeWithdrawl:
                    MakeWithDrawl();
                    break;
                case (int)AppMenu.InternalTransfer:
                    var internalTransfer = screen.InternalTransferForm();
                    ProcessInternalTransfer(internalTransfer);
                    break;
                case (int)AppMenu.ViewTransaction :
                    ViewTransaction();
                    break;
                case (int)AppMenu.Logout:
                    AppScreen.LogoutProgress();
                    Utility.PrintMessage("You have successfully logged out. Please collect your ATM card");
                    Run();
                    break ;
                deafult:
                    Utility.PrintMessage("Invalid Option.", false);
                    break;
            }
            
        }
        public void CheckBalance()
        {
            Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}");
        }
        public void PlaceDeposit()
        {
            Console.WriteLine("\nOnly multiples of 50 and 100 Rand allowed ");
            var transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");

            //simulate counting 
            Console.WriteLine("\nChecking and Counting bank notes.");
            Utility.PrintDotAnimation();
            Console.WriteLine("");

            //some guard clause

            if (transaction_amt <= 0) 
            {
                Utility.PrintMessage("Amount needs to be greater than zero.  Try agian ", false);
                return;
            }
            if (transaction_amt % 50 != 0) 
            {
                Utility.PrintMessage($"Enter a deposit amount in multiples of 50 or 100. Try again.",false);
                return;
            }

            if (PreviewBankNotesCount(transaction_amt) == false) 
            {
                Utility.PrintMessage($"you have cancelled your action.", false);
                return;
            }

            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transaction_amt, "");

            //update account balance
            selectedAccount.AccountBalance += transaction_amt;

            //print success message
            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was successful.",true);


        }


        public void MakeWithDrawl()
        {
            var transaction_amt = 0;
            int selectedAmount = AppScreen.SelectAmount();
            if (selectedAmount == -1)
            {
                MakeWithDrawl();
                return;
            }
            else if (selectedAmount != 0)
            {
                transaction_amt = selectedAmount;
            }
            else 
            {
                transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");
            }

            //input validation
            if (transaction_amt <= 0) 
            {
                Utility.PrintMessage("Amount needs to be greater than zero. Try again", false);
                return;
            }
            if(transaction_amt % 50 != 0) 
            {
                Utility.PrintMessage("You can only withdraw amount in multiples of 50 or 100 rand. Try again",false);
                return ;
            }
            //bussiness logic validations
            if (transaction_amt > selectedAccount.AccountBalance) 
            {
                Utility.PrintMessage($"Withdrawal failed. Your balance is too low to withdraw"+
                    $"{Utility.FormatAmount(transaction_amt)}", false);
                return ;
            }

            if((selectedAccount.AccountBalance - transaction_amt)< minimumKeptAmount)
            {
                Utility.PrintMessage($"Withdrawal failed. Your account needs to have" +
                    $"minimum{Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }

            //Bind withdrawal details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Withdrawl, -transaction_amt, "");
            //update account balance
            selectedAccount.AccountBalance -= transaction_amt;
            //print message success
            Utility.PrintMessage($"You have successfully withdrawn" +
                $"{Utility.FormatAmount(transaction_amt)}.", true);
        }


        private bool PreviewBankNotesCount(int amount) 
        {
            int HundredNotesCount = amount / 100;
            int fivtyNotesCount = (amount % 1000) / 50;

            Console.WriteLine("\nSummary");
            Console.WriteLine("---------");
            Console.WriteLine($"{AppScreen.cur}100 x {HundredNotesCount} = {100 * HundredNotesCount}");
            Console.WriteLine($"{AppScreen.cur}50 x {fivtyNotesCount} = {50 * fivtyNotesCount}");
            Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");

            int opt = Validator.Convert<int>("1 to confirm");
            return opt.Equals(1);

        }

        public void InsertTransaction(long _UserBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc)
        {
            //create a new transaction object
            var transaction = new Transaction()
            {
                TransactionId = Utility.GetTransactionId(),
                UserBankAccountID = _UserBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _tranType,
                TransactionAmount = _tranAmount,
                Description = _desc
            };

            //add transaction object to the list
            _listOfTransactions.Add(transaction);
        }
        

        public void ViewTransaction()
        {
            var filteredTransactionList = _listOfTransactions.Where(t => t.UserBankAccountID == selectedAccount.Id).ToList();
            //check if theres a transaction
            if (filteredTransactionList.Count <= 0) 
            {
                Utility.PrintMessage("You have no transaction yet.", true);

            }
            else 
            {
                var table = new ConsoleTable("Id","Transaction Date", "Type","Descriptions","Amount" + AppScreen.cur);
                foreach (var tran in filteredTransactionList) 
                {
                    table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Description,tran.TransactionAmount);

                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)", true);
            }
        }
        private void ProcessInternalTransfer(InternalTransfer internalTransfer) 
        {
            if (internalTransfer.TransferAmount <= 0) 
            {
                Utility.PrintMessage("Amount needs to be more than zero. Try again.", false);
                return;
            }
            //check senders account balance 
            if (internalTransfer.TransferAmount > selectedAccount.AccountBalance) 
            {
                Utility.PrintMessage($"Transfer failed. You dont have enough balance" +
                    $"to transfer{Utility.FormatAmount(internalTransfer.TransferAmount)}", false);
                return;
            }
            //check the minimum kept amount
            if ((selectedAccount.AccountBalance - internalTransfer.TransferAmount) <minimumKeptAmount )
            {
                Utility.PrintMessage($"Transfer failed. Your account needs to have a minimum" +
                    $"{Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }

            //check recievers account number is valid
            var selectedBankAccountReciever = (from userAcc in userAccountList
                                               where userAcc.AccountNumber == internalTransfer.ReciepeintBankAccountNumber
                                               select userAcc).FirstOrDefault();
            if (selectedBankAccountReciever == null) 
            {
                Utility.PrintMessage("Transfer failed. Reciever bank account number is invalid.", false);
                return ;
            }
            //check receivers name 
            if (selectedBankAccountReciever.FullName != internalTransfer.ReciepeintBankAccountName) 
            {
                Utility.PrintMessage("Transfer Falied. Recipients bank account name does not match.",false);
                return ;
            }
            //add transaction to transaction record -sender
            InsertTransaction(selectedAccount.Id, TransactionType.Transfer, internalTransfer.TransferAmount, "Transfered"+
                $"to {selectedBankAccountReciever.AccountNumber} ({selectedBankAccountReciever.FullName})");
            //update sender acount balance
            selectedAccount.AccountBalance -= internalTransfer.TransferAmount;

            //add transaction record - reciever
            InsertTransaction(selectedBankAccountReciever.Id, TransactionType.Transfer, internalTransfer.TransferAmount, "Transfered from " +
                $"{selectedAccount.AccountNumber}({selectedAccount.FullName})");
            //update recievers account balance
            selectedBankAccountReciever.AccountBalance +=internalTransfer.TransferAmount;
            //print success message
            Utility.PrintMessage($"You have successfully transfered" +
                $"{Utility.FormatAmount(internalTransfer.TransferAmount)} to " +
                $"{internalTransfer.ReciepeintBankAccountName}", true);
        }
    }
}