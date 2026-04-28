namespace G_Net_29_EF04.Models
{
    public class Manager
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }

        // FK
        public int BranchId { get; set; }

        // Navigation
        public Branch Branch { get; set; } = null!;
    }
}
