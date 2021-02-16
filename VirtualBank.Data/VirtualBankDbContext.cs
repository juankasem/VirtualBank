﻿using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.Models;

namespace VirtualBank.Data
{
    public class VirtualBankDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public VirtualBankDbContext(DbContextOptions<VirtualBankDbContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Branch>  Branches { get; set; }
        public DbSet<CashTransaction> CashTransactions { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }


    }
}
