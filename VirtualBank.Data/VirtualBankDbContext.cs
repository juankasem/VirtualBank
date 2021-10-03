using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Entities;

namespace VirtualBank.Data
{
    public class VirtualBankDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public VirtualBankDbContext(DbContextOptions<VirtualBankDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<CashTransaction> CashTransactions { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DebitCard> DebitCards { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<FastTransaction> FastTransactions { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<UtilityPayment> UtilityPayments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BankAccount>().HasIndex(c => c.AccountNo).IsUnique();
            builder.Entity<BankAccount>().HasIndex(c => c.IBAN).IsUnique();
            builder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
            builder.Entity<Customer>().HasIndex(c => c.IdentificationNo).IsUnique();
            builder.Entity<CreditCard>().HasIndex(c => c.CreditCardNo).IsUnique();
            builder.Entity<DebitCard>().HasIndex(c => c.DebitCardNo).IsUnique();
        }
    }
}
