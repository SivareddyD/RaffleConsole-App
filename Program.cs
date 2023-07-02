using System;
using System.Collections;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.Azure.Services.AppAuthentication;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
       private static Dictionary<string, List<int[]>> _usersTicketsDictionary = new Dictionary<string, List<int[]>>();

       private static Dictionary<Dictionary<string, string>, List<int[]>> _matchedGroupsDictionary =
           new Dictionary<Dictionary<string, string>, List<int[]>>();
        static void Main(string[] args)
        {
            Int16 initialSeeding = 100;
            Console.WriteLine("\n\n\t------  Welcome to My Raffle App  -------\n\n\tStatus: Draw has not started\n\n\t[1] Start a New Draw\n\n\t[2] Buy Tickets\n\n\t[3] Run Raffle\n\n\tPlease Select option from above menu");

            Raffle(initialSeeding);

        }

        private static void Raffle(double potSize)
        {
            string option = Console.ReadLine();
            int result;
            if (!int.TryParse(option, out result) && (result <1 || result > 3))
            {
                Console.WriteLine("Please enter a valid option(1,2,3) from above menu");
               
                Raffle(potSize);
            }

            double potsizeBalance=0;
            switch (option)
            {
                case "1":
                    NewDraw(potSize);
                    break;
                case "2":
                    BuyTickets(potSize);
                    break;
                case "3":
                    potsizeBalance= Draw(potSize);
                    break;
            }
       
            Raffle(potsizeBalance);
        }
        private static void NewDraw(double potSize)
        {
            _matchedGroupsDictionary.Clear();
            _usersTicketsDictionary.Clear();
            Console.WriteLine("\n\tNew Raffle draw has been started. Initial pot size: $" + potSize + "\n\tPress any key to return to main menu");
            Console.Clear();
            //goto PrintMe;
            Console.Clear();
            Console.WriteLine("\n\t\t\t-------- Welcome to My Raffle App --------\n\n\t\tStatus: Draw is ongoing. Raffle pot size is $" + potSize + "\n\t\t[1] Start a New Draw\n\t\t[2] Buy Tickets\n\t\t[3] Run Raffle");
            Raffle(potSize);
        }

        private static void BuyTickets(double potSize)
        {
            Console.Write("Enter user name:");

            Entervalue:
                string userName = Console.ReadLine();

            if (!IsInputValid(userName))
            {
                Console.WriteLine("Please enter a valid input");
                goto Entervalue;
            }

            Console.Write("Enter number of Tickets:");

            NoofTickets:
            string numberofTickets = Console.ReadLine();
            int noOfTickets;
            if (int.TryParse(numberofTickets,out noOfTickets) && (noOfTickets < 1 || noOfTickets > 5))
            {
                Console.WriteLine("Please enter a valid number(1 to 5):");
                goto NoofTickets;
            }
            potSize = Convert.ToInt16(potSize + (noOfTickets * 5));
            if (noOfTickets > 5)
            {
                Console.WriteLine("Number of tickets should not be more than 5 per user!");
                goto NoofTickets;
            }

            List<int[]> ltrNumbersArrays = new List<int[]>();
            for (int i = 0; i < noOfTickets; i++)
            {
                ltrNumbersArrays.Add(GenerateRandomTicket());

            }
            _usersTicketsDictionary.Add(userName, ltrNumbersArrays);
           

            PrintUserTickets(potSize, userName, ltrNumbersArrays);

        }

        private static double Draw(double potSize)
        {
            int[] arrDrawTicket = GenerateRandomTicket();
            double potsizeBalance=0;
            if (_usersTicketsDictionary.Any())
            {
                foreach (var keyValuePair in _usersTicketsDictionary)
                {
                    int ticketSize = keyValuePair.Value.Count;
                    for (int i = 0; i <= ticketSize-1; i++)
                    {
                        int ticketsMatchedCounter = 0;
                        for (int j = 0; j <= keyValuePair.Value[i].Length-1;j++)
                        {
                            for (int k = 0; k <=arrDrawTicket.Length-1; k++)
                            {
                                if ((keyValuePair.Value[i])[j] == arrDrawTicket[k])
                                {
                                    ticketsMatchedCounter++;

                                }
                            }
                        }
                        List<int[]> matchedTickets = new List<int[]>();
                        Dictionary<string, string> userandGroup; 
                        switch (ticketsMatchedCounter)
                        {
                            case 2:
                                matchedTickets.Add(keyValuePair.Value[i]);
                                userandGroup = new Dictionary<string, string>
                                {
                                    {keyValuePair.Key, "Group2"}
                                };
                                _matchedGroupsDictionary.Add(userandGroup, matchedTickets);
                                break;
                            case 3:
                                matchedTickets.Add(keyValuePair.Value[i]);
                                userandGroup = new Dictionary<string, string>
                                {
                                    {keyValuePair.Key, "Group3"}
                                };
                                _matchedGroupsDictionary.Add(userandGroup, matchedTickets);
                                break;
                            case 4:
                                matchedTickets.Add(keyValuePair.Value[i]);
                                userandGroup = new Dictionary<string, string>
                                {
                                    {keyValuePair.Key, "Group4"}
                                };
                                _matchedGroupsDictionary.Add(userandGroup, matchedTickets);
                                break;
                            case 5:
                                matchedTickets.Add(keyValuePair.Value[i]);
                                userandGroup = new Dictionary<string, string>
                                {
                                    {keyValuePair.Key, "Group5"}
                                };
                                _matchedGroupsDictionary.Add(userandGroup, matchedTickets);
                                break;
                        }
                    }
                }
                potsizeBalance= PrintFinalDrawResults(_matchedGroupsDictionary, arrDrawTicket,potSize);
            }

            return potsizeBalance;
        }

        private static int[] GenerateRandomTicket()
        {
            int[] ltrNumbers = new int[5];
            int val;
            Random rnd;
            for (int j = 0; j < ltrNumbers.Length; j++)
            {
                rnd = new Random();
                val = rnd.Next(1, 15);
                while (IsRandNumDuplicate(val, ltrNumbers))
                {
                    val = rnd.Next(1, 15);
                }

                ltrNumbers[j] = val;
            }

            return ltrNumbers;
        }

        private static bool IsRandNumDuplicate(int tmp, int[] arr)
        {
            foreach (var item in arr)
            {
                if (item == tmp)
                {
                    return true;
                }
            }
            return false;
        }

        private static void PrintUserTickets(double potSize,string name, List<int[]> ltrArrays)
        {
            Console.WriteLine("Hi {0}, You have purchased {1} Tickets-", name, ltrArrays.Count);
            int i = 1;
            foreach (var ticket in ltrArrays)
            {
                // int[] arr = new int[5]{1,2,3,4,5};

                string ltrNumbers1 = string.Join(",", ticket);

                Console.WriteLine("Ticket{0}:{1}", i, ltrNumbers1);
                i++;
            }

            Console.WriteLine("Press any key to return to main menu");
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Welcome to My Raffle App\nStatus: Draw is ongoing. Raffle pot size is $" + potSize + "\n[1] Start a New Draw\n[2] Buy Tickets\n[3] Run Raffle");
            Raffle(potSize);
        }

        private static double PrintFinalDrawResults(Dictionary<Dictionary<string, string>, List<int[]>> winningGroups,int[] winningTicket,double potSize)
        {
            Console.WriteLine("Running Raffle..");
            string finaldrawTicket = string.Join(",", winningTicket);
            Console.WriteLine("Winning Ticket is:{0}", finaldrawTicket+"\n");
            double sumofWinningMoney = 0;
            StringBuilder stringBuilderGroup2 = new StringBuilder();
            StringBuilder stringBuilderGroup3 = new StringBuilder();
            StringBuilder stringBuilderGroup4 = new StringBuilder();
            StringBuilder stringBuilderGroup5 = new StringBuilder();
            foreach (var keyWinningGroup in winningGroups)
            {
                int winGroupCount = keyWinningGroup.Value.Count;
                double winningMoney;
                foreach (var keyValuePair in keyWinningGroup.Key)
                {
                    if (keyValuePair.Value == "Group2")
                    {
                         winningMoney = Convert.ToDouble(potSize) / 100 * 10;
                         sumofWinningMoney += winningMoney;
                         stringBuilderGroup2.Append(keyValuePair.Key + " with " + winGroupCount + " Tickets- $"+ winningMoney + "\n");
                    }
                    if (keyValuePair.Value == "Group3")
                    {
                         winningMoney = Convert.ToDouble(potSize) / 100 * 15;
                         sumofWinningMoney += winningMoney;
                         stringBuilderGroup3.Append(keyValuePair.Key + " with " + winGroupCount + " Tickets- $"+ winningMoney + "\n");
                    }
                    if (keyValuePair.Value == "Group4")
                    {
                         winningMoney = Convert.ToDouble(potSize) / 100 * 25;
                         sumofWinningMoney += winningMoney;
                         
                         stringBuilderGroup4.Append(keyValuePair.Key + " with " + winGroupCount + " Tickets- $"+ winningMoney + "\n");
                    }
                    if (keyValuePair.Value == "Group5")
                    {
                         winningMoney = Convert.ToDouble(potSize) / 100 * 50;
                         sumofWinningMoney += winningMoney;
                         stringBuilderGroup5.Append(keyValuePair.Key + " with " + winGroupCount + " Tickets- $"+ winningMoney+"\n");
                    }
                    
                }
               
            }
            Console.WriteLine("Group 2 Winners:");
            if (stringBuilderGroup2.Length > 0)
            {
                Console.WriteLine(stringBuilderGroup2);
            }
            else
            {
                Console.WriteLine("Nill");
            }
            Console.WriteLine("Group 3 Winners:");
            if (stringBuilderGroup3.Length > 0)
            {
                Console.WriteLine(stringBuilderGroup3);
            }
            else
            {
                Console.WriteLine("Nill");
            }
            Console.WriteLine("Group 4 Winners:");
            if (stringBuilderGroup4.Length > 0)
            {
                Console.WriteLine(stringBuilderGroup4);
            }
            else
            {
                Console.WriteLine("Nill");
            }
            Console.WriteLine("Group 5 Winners:");
            if (stringBuilderGroup5.Length > 0)
            {
                Console.WriteLine(stringBuilderGroup5);
            }
            else
            {
                Console.WriteLine("Nill");
            }

            stringBuilderGroup2.Clear();
            stringBuilderGroup3.Clear();
            stringBuilderGroup4.Clear();
            stringBuilderGroup5.Clear();
            Console.WriteLine("Press any key to return to Main menu");
            return (potSize - sumofWinningMoney);
            //Console.ReadLine();
        }

        private static bool IsOptionValid(string option)
        {
            if (string.IsNullOrWhiteSpace(option.Trim()))
            {
                return false;
            }
            else
            {
                int result;
                return int.TryParse(option, out result);
            }
        }

        private static bool IsInputValid(string input)
        {
            int result;
            if (string.IsNullOrWhiteSpace(input.Trim()) || int.TryParse(input,out result))
            {
                return false;
            }

            return true;
        }
    }
    
}
