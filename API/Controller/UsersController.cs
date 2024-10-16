using API.Controllers;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controller
{
  [Authorize]
  public class UsersController : BaseApiController
  {
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;

    private readonly IPhotoService _photoService;

    public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
    {
      _unitOfWork = unitOfWork;
      _mapper = mapper;
      _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
      var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
      userParams.CurrentUsername = User.GetUsername();

      if (string.IsNullOrEmpty(userParams.Gender))
      {
        userParams.Gender = gender == "male" ? "female" : "male";
      }

      var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

      Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

      return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      MemberDto user = await _unitOfWork.UserRepository.GetMemberAsync(username);
      if (user == null)
      {
        return NotFound();
      }

      return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
      AppUser user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

      _mapper.Map(memberUpdateDto, user);

      _unitOfWork.UserRepository.Update(user);

      if (await _unitOfWork.Complete())
      {
        return NoContent();
      }

      return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
      AppUser user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

      ImageUploadResult result = await _photoService.AddPhotoAsync(file);

      if (result.Error != null)
      {
        return BadRequest(result.Error.Message);
      }

      Photo photo = new Photo
      {
        Url = result.SecureUrl.AbsoluteUri,
        PublicId = result.PublicId
      };

      user.Photos.Add(photo);

      if (await _unitOfWork.Complete())
      {
        return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
      }

      return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
      AppUser user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

      Photo photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if (photo == null)
      {
        return NotFound();
      }

      if (photo.IsApproved == false)
      {
        return BadRequest("You cannot set a photo as main that is not approved");
      }

      if (photo.IsMain)
      {
        return BadRequest("This is already your main photo");
      }

      Photo currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

      if (currentMain != null)
      {
        currentMain.IsMain = false;
      }

      photo.IsMain = true;

      if (await _unitOfWork.Complete())
      {
        return NoContent();
      }

      return BadRequest("Failed to set main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
      AppUser user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

      Photo photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if (photo == null)
      {
        return NotFound();
      }

      if (photo.IsMain)
      {
        return BadRequest("You cannot delete your main photo");
      }

      if (photo.PublicId != null)
      {
        DeletionResult result = await _photoService.DeletePhotoAsync(photo.PublicId);

        if (result.Error != null)
        {
          return BadRequest(result.Error.Message);
        }
      }

      user.Photos.Remove(photo);

      if (await _unitOfWork.Complete())
      {
        return Ok();
      }

      return BadRequest("Failed to delete photo");
    }
  }
}