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
    private IUnitOfWork _unitOfWork;

    public LikesController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
      var sourceUserId = User.GetUserId();
      var likedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

      if (likedUser == null)
      {
        return NotFound();
      }

      var likedUserId = likedUser.Id;
      var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);

      if (sourceUser.UserName == username)
      {
        return BadRequest("You cannot like yourself");
      }

      var userLike = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUserId);

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

      if (await _unitOfWork.Complete())
      {
        return Ok();
      }

      return BadRequest("Failed to like user");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikeParams likeParams)
    {
      likeParams.UserId = User.GetUserId();
      var users = await _unitOfWork.LikesRepository.GetUserLikes(likeParams);

      Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

      return Ok(users);
    }
  }
}