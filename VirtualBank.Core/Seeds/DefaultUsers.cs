using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VirtualBank.Core.Constants;
using VirtualBank.Core.Entities;

namespace VirtualBank.Core.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedBasicUser(UserManager<AppUser> userManager)
        {
            var defaultUser = new AppUser()
            {
                UserName = "basicuser@virtualbank.com",
                Email = "basicuser@virtualbank.com",
                EmailConfirmed = true
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "P@ssw0rd123");
                await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
            }
        }

        public static async Task SeedSuperAdminUser(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new AppUser()
            {
                UserName = "superadmin@virtualbank.com",
                Email = "superadmin@virtualbank.com",
                EmailConfirmed = true
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "P@ssw0rd123");
                await userManager.AddToRolesAsync(defaultUser, new List<string> { Roles.Basic.ToString(), Roles.SuperAdmin.ToString(), Roles.Admin.ToString() });
            }

            await roleManager.SeedClaimsForSuperAdminUser();
        }

        private static async Task SeedClaimsForSuperAdminUser(this RoleManager<IdentityRole> roleManager)
        {
            var superAdminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
            await roleManager.AddPermissionClaims(superAdminRole, ModuleTypes.BankAccount.ToString());
        }

        private static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GeneratePermisiionList(module);

            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(c => c.Type == RoleClaimTypes.Permission.ToString() && c.Value == permission))

                    await roleManager.AddClaimAsync(role, new Claim(RoleClaimTypes.Permission.ToString(), permission));
            }
        }
    }
}