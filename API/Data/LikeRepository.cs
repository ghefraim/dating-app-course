using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class LikeRepository : ILikeRepository
  {
    private readonly DataContext _context;

    public LikeRepository(DataContext context)
    {
      _context = context;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
    {
      return await _context.Likes.FindAsync(sourceUserId, likedUserId);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
      return await _context.Users
        .Include(x => x.LikedUsers)
        .FirstOrDefaultAsync(x => x.Id == userId);
    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
    {
      var users = _context.Users.OrderBy(user => user.UserName).AsQueryable();

      var likes = _context.Likes.AsQueryable();

      if (likeParams.Predicate == "liked")
      {
        likes = likes.Where(like => like.SourceUserId == likeParams.UserId);
        users = likes.Select(like => like.LikedUser);
      }

      if (likeParams.Predicate == "likedBy")
      {
        likes = likes.Where(like => like.LikedUserId == likeParams.UserId);
        users = likes.Select(like => like.SourceUser);
      }

      var likedUsers = users.Select(user => new LikeDto
      {
        Id = user.Id,
        Username = user.UserName,
        KnownAs = user.KnownAs,
        Age = user.DateOfBirth.CalculateAge(),
        City = user.City,
        PhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsMain).Url
      });

      return await PagedList<LikeDto>.CreateAsync(likedUsers, likeParams.PageNumber, likeParams.PageSize);
    }
  }
}