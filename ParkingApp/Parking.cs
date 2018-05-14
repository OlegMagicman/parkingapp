using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingApp
{
    public class Parking
    {
        public List<Car> CarList { get; private set; }
        public int LastAddedCarId { get; private set; }
        public List<Transaction> TransactionList { get; private set; }
        public int EarnedMoney { get; private set; }
        public int LastMinuteMoney { get; private set; }
        public Settings Settings { get; }
        public CarType CarType { get; }

        public Parking()
        {
            CarList = new List<Car>();
            LastAddedCarId = 0;
            TransactionList = new List<Transaction>();
            EarnedMoney = 0;
            LastMinuteMoney = 0;
            Settings = Settings.Instance;
            CarType = new CarType();
        }

        public void AddCar(int type)
        {
            try
            {
                if (Settings.totalSpace - CarList.Count() <= 0)
                {
                    Console.WriteLine("No enough free space");
                    return;
                }
                LastAddedCarId++;
                var car = new Car(LastAddedCarId, type);
                CarList.Add(car);
                Console.WriteLine("Car was added");
            } catch (FormatException)
            {
                Console.WriteLine("Wrong input data");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Wrong input data");
            }
        }

        public void RemoveCar(int id)
        {
            try
            {
                var car = CarList.Single(c => c.Id == id);

                if (car != null)
                {
                    if (car.Balance < 0)
                    {
                        Console.WriteLine("You need to raise car balanse up to {0} ", car.Balance * (-1));
                    }
                    else
                    {
                        CarList.Remove(car);
                        Console.WriteLine("Car was successfully removed!");
                    }
                }
                else
                {
                    Console.WriteLine("Car wasn't found!");
                }
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Car ID has not defined");
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Choose correct ID");
            }
        }

        public void GetCarsList()
        {
            Console.WriteLine("Car list:");
            foreach (var c in CarList)
            {
                Console.WriteLine("{0}. Car type: {1}, Balance: {2}", c.Id, c.CarType, c.Balance);
            }
        }

        public Car GetCar(int id)
        {
            return CarList.SingleOrDefault(c => c.Id == id);
        }

        public Car RaiseCarBalance(int id, int sum)
        {
            try
            {
                var car = CarList.SingleOrDefault(c => c.Id == id);
                car.ChangeBalance(sum, true);
                Console.WriteLine("Balance for car with id {0} was raised by {1}", id, sum);
                TransactionList.Add(new Transaction(id, sum));
                return car;
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("Something went wrong");
            }
            return null;
        }

        public void TakeFineFromBalance(object StateObj)
        {
            Parking State = (Parking)StateObj;
            var sum = 0;
            int price = 0;
            foreach (var car in State.CarList)
            {
                price = Settings.prices[car.CarType];
                sum = (car.Balance < 0) ? price * Settings.fine : price;

                car.ChangeBalance(sum, false);
                State.TransactionList.Add(new Transaction(car.Id, sum * (-1)));
                State.EarnedMoney += sum;
                State.LastMinuteMoney += sum;
            }
        }

        public void GetFreePlacesCount()
        {
            Console.WriteLine("Free places: {0}", Settings.totalSpace - CarList.Count());
        }

        public void UsingPlacesCount()
        {
            Console.WriteLine("Using places: {0}", CarList.Count());
        }

        public void GetEarnedMoney()
        {
            Console.WriteLine("Earned money: {0}", EarnedMoney);
        }

        public void GetLastMinuteTransactions()
        {
            Console.WriteLine("Transactions from last minute:");
            List<Transaction> list = (from transaction in TransactionList
                    where transaction.DateTime.AddMinutes(1).Minute >= DateTime.Now.Minute
                    select transaction)
                    .ToList();
            foreach (var item in list)
            {
                Console.WriteLine("Datetime: {0}, Car id: {1}, Cash:{2}", item.DateTime, item.CarId, item.Cash);
            }
        }

        public void GetLastMinuteTransactions(int id)
        {
            Console.WriteLine("Transactions from last minute for car with id: {0}", id);
            List<Transaction> list = (from transaction in TransactionList
                    where transaction.DateTime.AddMinutes(1).Minute >= DateTime.Now.Minute
                    where transaction.CarId == id
                    select transaction)
                    .ToList();
            foreach (var item in list)
            {
                Console.WriteLine("Datetime: {0}, Car id: {1}, Cash:{2}", item.DateTime, item.CarId, item.Cash);
            }
        }

        public void LogLastMinuteMoney(object StateObj)
        {
            try
            {
                Parking State = (Parking)StateObj;
                string log = DateTime.Now.ToString("MM.dd.yyyy HH:mm ") + State.LastMinuteMoney;
                using (StreamWriter sw = new StreamWriter(Settings.filePath, true, Encoding.Default))
                {
                    sw.WriteLine(log);
                }
                State.LastMinuteMoney = 0;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found!");
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void ShowAllTransactions()
        {
            try
            {
                Console.WriteLine("Transaction list:");
                using (StreamReader sw = new StreamReader(Settings.filePath, Encoding.Default))
                {
                    string line;
                    while ((line = sw.ReadLine()) != null)
                    {
                        string[] token = new string[3];
                        token = line.Split(' ');
                        Console.WriteLine("Datetime: {0}, Car id: {1}, Cash:{2}", token[0], token[1], token[2]);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found!");
            }
            catch (FileLoadException)
            {
                Console.WriteLine("File not found!");
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
