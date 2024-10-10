using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class Seed
  {
    public static async void SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
      if (await userManager.Users.AnyAsync())
      {
        return;
      }

      var userData = File.ReadAllText("Data/UserSeedData.json");
      var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

      if (users == null)
      {
        return;
      }

      var roles = new List<AppRole>
      {
        new AppRole { Name = "Member" },
        new AppRole { Name = "Admin" },
        new AppRole { Name = "Moderator" }
      };

      foreach (var role in roles)
      {
        await roleManager.CreateAsync(role);
      }

      foreach (var user in users)
      {
        user.UserName = user.UserName.ToLower();
        await userManager.CreateAsync(user, "asdasd");
        await userManager.AddToRoleAsync(user, "Member");
      }

      var admin = new AppUser
      {
        UserName = "admin",
        KnownAs = "Admin"
      };

      await userManager.CreateAsync(admin, "asdasd");
      await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
    }
  }
}