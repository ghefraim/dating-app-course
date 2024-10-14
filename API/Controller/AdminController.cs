using API.Controllers;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controller
{
  public class AdminController : BaseApiController
  {
    private readonly UserManager<AppUser> _userManager;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IPhotoService _photoService;

    public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IPhotoService photoService)
    {
      _userManager = userManager;
      _unitOfWork = unitOfWork;
      _photoService = photoService;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
      var users = await _userManager.Users
        .Include(r => r.UserRoles)
        .ThenInclude(r => r.Role)
        .OrderBy(u => u.UserName)
        .Select(u => new
        {
          u.Id,
          Username = u.UserName,
          Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
        })
        .ToListAsync();

      return Ok(users);
    }

    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
    {
      var selectedRoles = roles.Split(",").ToArray();

      var user = await _userManager.FindByNameAsync(username);

      if (user == null)
      {
        return NotFound("Could not find user");
      }

      var userRoles = await _userManager.GetRolesAsync(user);

      var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

      if (!result.Succeeded)
      {
        return BadRequest("Failed to add to roles");
      }

      result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

      if (!result.Succeeded)
      {
        return BadRequest("Failed to remove from roles");
      }

      return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForModeration()
    {
      return Ok(await _unitOfWork.PhotoRepository.GetUnapprovedPhotosAsync());
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPut("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
      var photo = await _unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);

      if (photo == null)
      {
        return NotFound();
      }

      var user = await _unitOfWork.UserRepository.GetUserByIdAsync(photo.AppUserId);
      var isMain = user.Photos.All(p => !p.IsMain);
      _unitOfWork.PhotoRepository.ApprovePhoto(photo, isMain);

      if (await _unitOfWork.Complete())
      {
        return NoContent();
      }

      return BadRequest("Problem approving the photo");
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpDelete("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectAndDeletePhoto(int photoId)
    {
      var photo = await _unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);

      if (photo == null)
      {
        return NotFound();
      }

      var deletePhotoResult = await _photoService.DeletePhotoAsync(photo.PublicId);

      if (deletePhotoResult.Error != null)
      {
        return BadRequest(deletePhotoResult.Error.Message);
      }

      _unitOfWork.PhotoRepository.DeletePhoto(photo);

      if (await _unitOfWork.Complete())
      {
        return NoContent();
      }

      return BadRequest("Problem rejecting the photo");
    }
  }
}