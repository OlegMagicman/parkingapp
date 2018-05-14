using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParkingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Parking parking = new Parking();

            TimerCallback TimerDelegate1 = new TimerCallback(parking.TakeFineFromBalance);
            Timer fineTimer = new Timer(TimerDelegate1, parking, parking.Settings.timeout, parking.Settings.timeout);

            TimerCallback TimerDelegate2 = new TimerCallback(parking.LogLastMinuteMoney);
            Timer logTimer = new Timer(TimerDelegate2, parking, 60000, 60000);

            CarType CarType = new CarType();
            var exit = false;
            while (exit == false) {
                var id = 0;
                var sum = 0;
                var key = 0;
                key = ShowMenu();
                switch (key)
                {
                    case 1:
                        Console.WriteLine("Choose car type id from this list: ");
                        var inc = 0;
                        int carType;
                        foreach (var type in CarType.GetCarTypes())
                            Console.WriteLine("{0} - {1}", ++inc, type.Value);
                        if (int.TryParse(Console.ReadLine(), out carType))
                        {
                            parking.AddCar(carType);
                        }
                        else
                        {
                            Console.WriteLine("Wrong input data");
                        }
                        break;
                    case 2:
                        Console.WriteLine("Write car id to delete car or write any letter to turn to main menu: ");
                        if (int.TryParse(Console.ReadLine(), out id))
                        {
                            parking.RemoveCar(id);
                        }
                        else
                        {
                            Console.WriteLine("Wrong input data");
                        }
                        break;
                    case 3:
                        Console.WriteLine("Enter car id and how much you want to pay:");
                        if (int.TryParse(Console.ReadLine(), out id) && int.TryParse(Console.ReadLine(), out carType))
                        {
                            parking.RaiseCarBalance(id, sum);
                        }
                        else
                        {
                            Console.WriteLine("Wrong input data");
                        }
                        break;
                    case 4:
                        parking.GetFreePlacesCount();
                        break;
                    case 5:
                        parking.GetEarnedMoney();
                        break;
                    case 6:
                        parking.GetLastMinuteTransactions();
                        break;
                    case 7:
                        parking.ShowAllTransactions();
                        break;
                    case 8:
                        parking.GetCarsList();
                        break;
                    case 9:
                        Console.Clear();
                        break;
                    case 10:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Please, enter correct number");
                        System.Threading.Thread.Sleep(3000);
                        ShowMenu();
                        break;
                }
            }
        }

        static public int ShowMenu()
        {
            Console.WriteLine("Main menu:");
            Console.WriteLine("1. Add Car");
            Console.WriteLine("2. Remove Car");
            Console.WriteLine("3. Raise balance");
            Console.WriteLine("4. Count free places");
            Console.WriteLine("5. Get earned money");
            Console.WriteLine("6. Last minute transactions");
            Console.WriteLine("7. Show Transactions.log");
            Console.WriteLine("8. Get cars list");
            Console.WriteLine("9. Clear console");
            Console.WriteLine("10. Exit");
            Console.Write("Choose action: ");

            int key;

            if (int.TryParse(Console.ReadLine(), out key))
            {
                return Convert.ToInt32(key);
            }
            return 0;
        }
    }
}
