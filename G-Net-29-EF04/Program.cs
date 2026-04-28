
using BankProject.Data;
using BankProject.Models;

class Program
{
    static void Main()
    {
        using var db = new BankContext();

        while (true)
        {
            ClearScreen();
            Console.WriteLine("=== National Bank Management ===");
            Console.WriteLine("1) Add Customer");
            Console.WriteLine("2) Open Account");
            Console.WriteLine("3) Update Account Status");
            Console.WriteLine("4) Remove Account");
            Console.WriteLine("5) List Customers");
            Console.WriteLine("0) Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AddCustomer(db); break;
                case "2": OpenAccount(db); break;
                case "3": UpdateStatus(db); break;
                case "4": RemoveAccount(db); break;
                case "5": ListCustomers(db); break;
                case "0": return;
            }

            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
        }
    }

    static void AddCustomer(BankContext db)
    {
        Console.Write("Name: ");
        var name = Console.ReadLine();

        var c = new Customer
        {
            FullName = name,
            NationalId = Guid.NewGuid().ToString(),
            DateOfBirth = DateTime.Now,
            Email = "test@mail.com",
            PhoneNumber = "010",
            Address = "Cairo",
            CustomerType = "Individual"
        };

        db.Customers.Add(c);
        db.SaveChanges();
        Console.WriteLine("Customer added!");
    }

    static void OpenAccount(BankContext db)
    {
        Console.Write("Account Number: ");
        int accNum = int.Parse(Console.ReadLine());

        Console.Write("Customer Id: ");
        int custId = int.Parse(Console.ReadLine());

        var customer = db.Customers.Find(custId);
        if (customer == null)
        {
            Console.WriteLine("Customer not found!");
            return;
        }

        var acc = new Account
        {
            AccountNumber = accNum,
            AccountType = "Savings",
            OpeningDate = DateTime.Now,
            CurrentBalance = 0,
            BranchCode = "CAI-01"
        };

        db.Accounts.Add(acc);

        db.CustomerAccounts.Add(new CustomerAccount
        {
            CustomerId = custId,
            AccountNumber = accNum,
            OwnershipType = "Primary",
            OwnershipStartDate = DateTime.Now,
            AccountStatus = "Active"
        });

        db.SaveChanges();
        Console.WriteLine("Account created!");
    }

    static void UpdateStatus(BankContext db)
    {
        Console.Write("Account Number: ");
        int acc = int.Parse(Console.ReadLine());

        Console.Write("Customer Id: ");
        int cust = int.Parse(Console.ReadLine());

        var link = db.CustomerAccounts.Find(cust, acc);
        if (link == null)
        {
            Console.WriteLine("Not found");
            return;
        }

        link.AccountStatus = link.AccountStatus == "Active" ? "Closed" : "Active";
        db.SaveChanges();

        Console.WriteLine("Updated!");
    }

    static void RemoveAccount(BankContext db)
    {
        Console.Write("Account Number: ");
        int acc = int.Parse(Console.ReadLine());

        Console.Write("Customer Id: ");
        int cust = int.Parse(Console.ReadLine());

        var link = db.CustomerAccounts.Find(cust, acc);
        if (link == null) return;

        db.CustomerAccounts.Remove(link);
        db.SaveChanges();

        Console.WriteLine("Removed!");
    }

    static void ListCustomers(BankContext db)
    {
        var list = db.Customers.ToList();
        foreach (var c in list)
            Console.WriteLine($"{c.Id} - {c.FullName}");
    }

    static void ClearScreen()
    {
        if (!Console.IsOutputRedirected)
            Console.Clear();
    }
}
