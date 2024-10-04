using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;

namespace API.Data
{
  public class Seed
  {
    public static void SeedUsers(DataContext context)
    {
      if (context.Users.Any())
      { return; }

      var userData = File.ReadAllText("Data/UserSeedData.json");
      var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

      foreach (var user in users)
      {
        using var hmac = new HMACSHA512();

        user.UserName = user.UserName.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("asdasd"));
        user.PasswordSalt = hmac.Key;

        context.Users.Add(user);
      }

      context.SaveChangesAsync();
    }
  }
}