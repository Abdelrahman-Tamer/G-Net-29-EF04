using G_Net_29_EF04.Data;
using G_Net_29_EF04.Models;
using Microsoft.EntityFrameworkCore;

namespace G_Net_29_EF04
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var ctx = new BankDbContext())
            {
                ctx.Database.Migrate();
            }

            bool running = true;
            while (running)
            {
                ClearConsole();
                PrintMenu();

                string? input = Console.ReadLine();
                if (!int.TryParse(input, out int choice))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input. Please enter a number.");
                    Console.ResetColor();
                    Pause();
                    continue;
                }

                switch (choice)
                {
                    case 1: AddCustomer();               break;
                    case 2: OpenAccount();               break;
                    case 3: UpdateAccountStatus();       break;
                    case 4: RemoveAccountFromCustomer(); break;
                    case 5: ListCustomers();             break;
                    case 0: running = false;             break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Try again.");
                        Console.ResetColor();
                        Pause();
                        break;
                }
            }
            Console.WriteLine("Goodbye!");
        }

        static void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("========================================");
            Console.WriteLine("        National Bank - Management      ");
            Console.WriteLine("========================================");
            Console.ResetColor();
            Console.WriteLine("  1) Add a new Customer");
            Console.WriteLine("  2) Open a new Account for a Customer");
            Console.WriteLine("  3) Update Account Status (Active / Closed)");
            Console.WriteLine("  4) Remove an Account from a Customer");
            Console.WriteLine("  5) List all Customers (with accounts)");
            Console.WriteLine("  0) Exit");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("----------------------------------------");
            Console.ResetColor();
            Console.Write("  Enter choice: ");
        }

        static void AddCustomer()
        {
            Console.WriteLine("\n--- Add New Customer ---");
            string fullName   = ReadString("Full Name      ");
            string nationalId = ReadString("National ID    ");
            DateTime dob      = ReadDate("Date of Birth  (yyyy-MM-dd)");
            string email      = ReadString("Email          ");
            string phone      = ReadString("Phone          ");
            string address    = ReadString("Address        ");

            Console.WriteLine("Customer Type:");
            Console.WriteLine("     1) Individual");
            Console.WriteLine("     2) Business");
            int typeChoice = ReadMenuChoice("  Choice", 1, 2);
            CustomerType ctype = typeChoice == 1 ? CustomerType.Individual : CustomerType.Business;

            using var db = new BankDbContext();
            if (db.Customers.Any(c => c.NationalId == nationalId))
            {
                PrintError($"National ID '{nationalId}' is already registered.");
                Pause(); return;
            }

            var customer = new Customer
            {
                FullName     = fullName,
                NationalId   = nationalId,
                DateOfBirth  = dob,
                Email        = email,
                PhoneNumber  = phone,
                Address      = address,
                CustomerType = ctype
            };
            db.Customers.Add(customer);
            db.SaveChanges();
            PrintSuccess($"Customer created successfully. CustomerId = {customer.Id}");
            Pause();
        }

        static void OpenAccount()
        {
            Console.WriteLine("\n--- Open New Account ---");
            string accountNumber = ReadString("Account Number ");
            Console.WriteLine("Account Type:");
            Console.WriteLine("     1) Savings");
            Console.WriteLine("     2) Current");
            Console.WriteLine("     3) Business");
            int atChoice = ReadMenuChoice("  Choice", 1, 3);
            AccountType accountType = atChoice switch { 1 => AccountType.Savings, 2 => AccountType.Current, _ => AccountType.Business };

            string branchCode = ReadString("Branch Code    ");
            int customerId    = ReadInt("Customer Id    ");
            Console.WriteLine("Ownership Role:");
            Console.WriteLine("     1) Primary");
            Console.WriteLine("     2) CoHolder");
            int ownerChoice = ReadMenuChoice("  Choice", 1, 2);
            OwnershipType ownershipType = ownerChoice == 1 ? OwnershipType.Primary : OwnershipType.CoHolder;

            using var db = new BankDbContext();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Validating branch '{branchCode}' and customer #{customerId}...");
            Console.ResetColor();

            var branch = db.Branches.FirstOrDefault(b => b.Code == branchCode);
            if (branch == null) { PrintError($"Branch '{branchCode}' not found."); Pause(); return; }

            var customer = db.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null) { PrintError($"Customer #{customerId} not found."); Pause(); return; }

            if (db.Accounts.Any(a => a.AccountNumber == accountNumber))
            { PrintError($"Account '{accountNumber}' already exists."); Pause(); return; }

            var account = new Account
            {
                AccountNumber  = accountNumber,
                AccountType    = accountType,
                OpeningDate    = DateTime.Today,
                CurrentBalance = 0,
                BranchId       = branch.Id
            };
            db.Accounts.Add(account);
            db.SaveChanges();

            db.CustomerAccounts.Add(new CustomerAccount
            {
                CustomerId         = customerId,
                AccountNumber      = accountNumber,
                OwnershipType      = ownershipType,
                OwnershipStartDate = DateTime.Today,
                AccountStatus      = AccountStatus.Active
            });
            db.SaveChanges();
            PrintSuccess($"Account '{accountNumber}' created and linked to customer {customerId} as {ownershipType} owner.");
            Pause();
        }

        static void UpdateAccountStatus()
        {
            Console.WriteLine("\n--- Update Account Status ---");
            string accountNumber = ReadString("Account Number ");
            int customerId       = ReadInt("Customer Id    ");

            using var db = new BankDbContext();
            var ca = db.CustomerAccounts.FirstOrDefault(x => x.AccountNumber == accountNumber && x.CustomerId == customerId);
            if (ca == null) { PrintError("No matching account/customer link found."); Pause(); return; }

            Console.WriteLine("New Status:");
            Console.WriteLine("     1) Active");
            Console.WriteLine("     2) Closed");
            int choice = ReadMenuChoice("  Choice", 1, 2);
            ca.AccountStatus = choice == 1 ? AccountStatus.Active : AccountStatus.Closed;
            db.SaveChanges();
            PrintSuccess($"Status updated to {ca.AccountStatus}.");
            Pause();
        }

        static void RemoveAccountFromCustomer()
        {
            Console.WriteLine("\n--- Remove Account From Customer ---");
            string accountNumber = ReadString("Account Number ");
            int customerId       = ReadInt("Customer Id    ");

            using var db = new BankDbContext();
            var ca = db.CustomerAccounts.FirstOrDefault(x => x.AccountNumber == accountNumber && x.CustomerId == customerId);
            if (ca == null) { PrintError("No matching account/customer link found."); Pause(); return; }

            db.CustomerAccounts.Remove(ca);
            db.SaveChanges();
            PrintSuccess("  Ownership link deleted.");

            bool hasOtherOwners = db.CustomerAccounts.Any(x => x.AccountNumber == accountNumber);
            if (!hasOtherOwners)
            {
                var account = db.Accounts.Find(accountNumber);
                if (account != null) { db.Accounts.Remove(account); db.SaveChanges(); }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"       That was the last owner - account '{accountNumber}' was also removed.");
                Console.ResetColor();
            }
            Pause();
        }

        static void ListCustomers()
        {
            Console.WriteLine("\n--- All Customers ---\n");
            using var db = new BankDbContext();
            var customers = db.Customers
                              .Include(c => c.CustomerAccounts)
                                  .ThenInclude(ca => ca.Account)
                                      .ThenInclude(a => a.Branch)
                              .OrderBy(c => c.Id).ToList();

            foreach (var c in customers)
            {
                Console.WriteLine($"  #{c.Id} {c.FullName} ({c.CustomerType})");
                if (!c.CustomerAccounts.Any())
                    Console.WriteLine("        (no accounts)");
                else
                    foreach (var ca in c.CustomerAccounts)
                    {
                        var a = ca.Account;
                        Console.WriteLine(
                            $"        {a.AccountNumber,-12} {a.AccountType,-10} " +
                            $"Balance: {a.CurrentBalance,12:F2}  " +
                            $"{ca.OwnershipType,-10} {ca.AccountStatus,-8} @ {a.Branch?.Name}");
                    }
            }
            Pause();
        }

        // ── Helpers ──────────────────────────────────────────────────
        static void ClearConsole()
        {
            if (!Console.IsOutputRedirected)
            {
                Console.Clear();
            }
        }

        static void Pause()
        {
            Console.WriteLine("\nPress any key to return to the menu...");

            if (!Console.IsInputRedirected)
            {
                Console.ReadKey(true);
            }
        }
        static void PrintSuccess(string msg) { Console.ForegroundColor = ConsoleColor.Green;  Console.WriteLine(msg); Console.ResetColor(); }
        static void PrintError(string msg)   { Console.ForegroundColor = ConsoleColor.Red;    Console.WriteLine($"ERROR: {msg}"); Console.ResetColor(); }

        static string ReadString(string label)
        {
            while (true) { Console.Write($"{label} : "); string? v = Console.ReadLine()?.Trim(); if (!string.IsNullOrWhiteSpace(v)) return v; PrintError("Value cannot be empty."); }
        }
        static int ReadInt(string label)
        {
            while (true) { Console.Write($"{label} : "); if (int.TryParse(Console.ReadLine(), out int r)) return r; PrintError("Please enter a valid number."); }
        }
        static DateTime ReadDate(string label)
        {
            while (true)
            {
                Console.Write($"{label} : ");
                if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime dt)) return dt;
                PrintError("Invalid date. Use format yyyy-MM-dd (e.g. 2000-01-25).");
            }
        }
        static int ReadMenuChoice(string label, int min, int max)
        {
            while (true) { Console.Write($"{label}: "); if (int.TryParse(Console.ReadLine(), out int v) && v >= min && v <= max) return v; PrintError($"Please enter a number between {min} and {max}."); }
        }
    }
}
