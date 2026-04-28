namespace G_Net_29_EF04.Models
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer,
        Payment
    }

    public class Transaction
    {
        public int TransactionNumber { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Note { get; set; } = string.Empty;

        // FK
        public string AccountNumber { get; set; } = string.Empty;

        // Navigation
        public Account Account { get; set; } = null!;
    }
}
