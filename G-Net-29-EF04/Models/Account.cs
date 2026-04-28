namespace G_Net_29_EF04.Models
{
    public enum AccountType
    {
        Savings,
        Current,
        Business
    }

    public class Account
    {
        public string AccountNumber { get; set; } = string.Empty;
        public AccountType AccountType { get; set; }
        public DateTime OpeningDate { get; set; }
        public decimal CurrentBalance { get; set; }

        // FK
        public int BranchId { get; set; }

        // Navigation
        public Branch Branch { get; set; } = null!;
        public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
