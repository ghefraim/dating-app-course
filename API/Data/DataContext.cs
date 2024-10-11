using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class DataContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>,
    AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
  {
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserLike> Likes { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<Group> Groups { get; set; }

    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<AppUser>()
        .HasMany(user => user.UserRoles)
        .WithOne(userRole => userRole.User)
        .HasForeignKey(userRole => userRole.UserId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<AppRole>()
        .HasMany(role => role.UserRoles)
        .WithOne(userRole => userRole.Role)
        .HasForeignKey(userRole => userRole.RoleId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<UserLike>()
        .HasKey(key => new { key.SourceUserId, key.LikedUserId });

      modelBuilder.Entity<UserLike>()
        .HasOne(userLike => userLike.SourceUser)
        .WithMany(user => user.LikedUsers)
        .HasForeignKey(userLike => userLike.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<UserLike>()
        .HasOne(userLike => userLike.LikedUser)
        .WithMany(user => user.LikedByUsers)
        .HasForeignKey(userLike => userLike.LikedUserId)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<Message>()
        .HasOne(message => message.Sender)
        .WithMany(user => user.MessagesSent)
        .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<Message>()
        .HasOne(message => message.Recipient)
        .WithMany(user => user.MessagesReceived)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}