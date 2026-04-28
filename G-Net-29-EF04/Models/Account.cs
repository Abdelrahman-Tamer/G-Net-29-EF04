
namespace BankProject.Models;
public class Account
{
    public int AccountNumber { get; set; }
    public string AccountType { get; set; }
    public DateTime OpeningDate { get; set; }
    public decimal CurrentBalance { get; set; }
    public string BranchCode { get; set; }
    public List<CustomerAccount> CustomerAccounts { get; set; } = new();
}
