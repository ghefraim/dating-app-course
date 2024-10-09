using API.Controllers;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller
{
  public class LikesController : BaseApiController
  {
    private readonly IUserRepository _userRepository;
    private readonly ILikeRepository _likeRepository;

    public LikesController(IUserRepository userRepository, ILikeRepository likeRepository)
    {
      _likeRepository = likeRepository;
      _userRepository = userRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
      var sourceUserId = User.GetUserId();
      var likedUser = await _userRepository.GetUserByUsernameAsync(username);

      if (likedUser == null)
      {
        return NotFound();
      }

      var likedUserId = likedUser.Id;
      var sourceUser = await _likeRepository.GetUserWithLikes(sourceUserId);

      if (sourceUser.UserName == username)
      {
        return BadRequest("You cannot like yourself");
      }

      var userLike = await _likeRepository.GetUserLike(sourceUserId, likedUserId);

      if (userLike != null)
      {
        return BadRequest("You already like this user");
      }

      userLike = new UserLike
      {
        SourceUserId = sourceUserId,
        LikedUserId = likedUserId
      };

      sourceUser.LikedUsers.Add(userLike);

      if (await _userRepository.SaveAllAsync())
      {
        return Ok();
      }

      return BadRequest("Failed to like user");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikeParams likeParams)
    {
      likeParams.UserId = User.GetUserId();
      var users = await _likeRepository.GetUserLikes(likeParams);

      Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

      return Ok(users);
    }
  }
}