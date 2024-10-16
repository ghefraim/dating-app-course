using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

    public DbSet<Photo> Photos { get; set; }

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

      modelBuilder.ApplyUtcDateTimeConverter();
    }
  }

  public static class UtcDateAnnotation
  {
    private const String IsUtcAnnotation = "IsUtc";
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
      new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
      new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

    public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, Boolean isUtc = true) =>
      builder.HasAnnotation(IsUtcAnnotation, isUtc);

    public static Boolean IsUtc(this IMutableProperty property) =>
      ((Boolean?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

    /// <summary>
    /// Make sure this is called after configuring all your entities.
    /// </summary>
    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
      foreach (var entityType in builder.Model.GetEntityTypes())
      {
        foreach (var property in entityType.GetProperties())
        {
          if (!property.IsUtc())
          {
            continue;
          }

          if (property.ClrType == typeof(DateTime))
          {
            property.SetValueConverter(UtcConverter);
          }

          if (property.ClrType == typeof(DateTime?))
          {
            property.SetValueConverter(UtcNullableConverter);
          }
        }
      }
    }
  }
}