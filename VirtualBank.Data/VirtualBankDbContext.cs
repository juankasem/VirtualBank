using System;
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
    }
}
