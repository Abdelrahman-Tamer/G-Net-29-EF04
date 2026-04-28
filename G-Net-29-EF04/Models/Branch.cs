namespace G_Net_29_EF04.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Navigation Properties
        public Manager? Manager { get; set; }
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
