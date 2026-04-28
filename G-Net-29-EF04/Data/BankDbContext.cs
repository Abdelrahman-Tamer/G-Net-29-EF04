using G_Net_29_EF04.Models;
using Microsoft.EntityFrameworkCore;

namespace G_Net_29_EF04.Data
{
    public class BankDbContext : DbContext
    {
        private const string DefaultConnectionString =
            "Data Source=ABDO;Initial Catalog=NationalBankDB;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=G-Net-29-EF04";

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public BankDbContext()
        {
        }

        public BankDbContext(DbContextOptions<BankDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(GetConnectionString());
            }
        }

        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("BANK_DB_CONNECTION_STRING")
                ?? DefaultConnectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ─── Branch ───────────────────────────────────────────────
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Code).IsRequired().HasMaxLength(20);
                entity.HasIndex(b => b.Code).IsUnique();
                entity.Property(b => b.Name).IsRequired().HasMaxLength(100);
                entity.Property(b => b.Address).IsRequired().HasMaxLength(200);
                entity.Property(b => b.PhoneNumber).IsRequired().HasMaxLength(20);
            });

            // ─── Manager ──────────────────────────────────────────────
            modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.FullName).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Email).IsRequired().HasMaxLength(100);
                entity.Property(m => m.PhoneNumber).IsRequired().HasMaxLength(20);

                // 1-to-1: Branch has one Manager
                entity.HasOne(m => m.Branch)
                      .WithOne(b => b.Manager)
                      .HasForeignKey<Manager>(m => m.BranchId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── Customer ─────────────────────────────────────────────
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.FullName).IsRequired().HasMaxLength(100);
                entity.Property(c => c.NationalId).IsRequired().HasMaxLength(20);
                entity.HasIndex(c => c.NationalId).IsUnique();
                entity.Property(c => c.Email).IsRequired().HasMaxLength(100);
                entity.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);
                entity.Property(c => c.Address).IsRequired().HasMaxLength(200);
                entity.Property(c => c.CustomerType).HasConversion<string>();
            });

            // ─── Account ──────────────────────────────────────────────
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(a => a.AccountNumber);
                entity.Property(a => a.AccountNumber).HasMaxLength(20);
                entity.Property(a => a.AccountType).HasConversion<string>();
                entity.Property(a => a.CurrentBalance).HasColumnType("decimal(18,2)");

                // M-to-1: Many accounts belong to one branch
                entity.HasOne(a => a.Branch)
                      .WithMany(b => b.Accounts)
                      .HasForeignKey(a => a.BranchId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── CustomerAccount (join table M-to-M with payload) ─────
            modelBuilder.Entity<CustomerAccount>(entity =>
            {
                entity.HasKey(ca => new { ca.CustomerId, ca.AccountNumber });
                entity.Property(ca => ca.OwnershipType).HasConversion<string>();
                entity.Property(ca => ca.AccountStatus).HasConversion<string>();

                entity.HasOne(ca => ca.Customer)
                      .WithMany(c => c.CustomerAccounts)
                      .HasForeignKey(ca => ca.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ca => ca.Account)
                      .WithMany(a => a.CustomerAccounts)
                      .HasForeignKey(ca => ca.AccountNumber)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── Transaction ──────────────────────────────────────────
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(t => t.TransactionNumber);
                entity.Property(t => t.Amount).HasColumnType("decimal(18,2)");
                entity.Property(t => t.TransactionType).HasConversion<string>();
                entity.Property(t => t.Note).HasMaxLength(300);

                entity.HasOne(t => t.Account)
                      .WithMany(a => a.Transactions)
                      .HasForeignKey(t => t.AccountNumber)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── Seed Data ────────────────────────────────────────────
            modelBuilder.Entity<Branch>().HasData(
                new Branch { Id = 1, Code = "CAI-01", Name = "Cairo Main Branch",    Address = "10 Tahrir Square, Cairo",       PhoneNumber = "0221234567" },
                new Branch { Id = 2, Code = "ALX-01", Name = "Alexandria Branch",    Address = "5 Corniche Road, Alexandria",   PhoneNumber = "0331234567" },
                new Branch { Id = 3, Code = "GIZ-01", Name = "Giza Branch",          Address = "15 Haram Street, Giza",         PhoneNumber = "0381234567" }
            );

            modelBuilder.Entity<Manager>().HasData(
                new Manager { Id = 1, FullName = "Ahmed Hassan",   Email = "ahmed.hassan@bank.com",   PhoneNumber = "01001111111", HireDate = new DateTime(2018, 3, 15), BranchId = 1 },
                new Manager { Id = 2, FullName = "Sara Mohamed",   Email = "sara.mohamed@bank.com",   PhoneNumber = "01002222222", HireDate = new DateTime(2019, 7, 1),  BranchId = 2 },
                new Manager { Id = 3, FullName = "Omar Khaled",    Email = "omar.khaled@bank.com",    PhoneNumber = "01003333333", HireDate = new DateTime(2020, 1, 10), BranchId = 3 }
            );
        }
    }
}
