using System.Security.Claims;
using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controller
{
  [Authorize]
  public class UsersController : BaseApiController
  {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
      _userRepository = userRepository;
      _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
      IEnumerable<MemberDto> users = await _userRepository.GetMembersAsync();

      return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      MemberDto user = await _userRepository.GetMemberAsync(username);
      if (user == null)
      {
        return NotFound();
      }

      return Ok(user);
    }

    // [HttpPost]
    // public async Task<ActionResult<AppUser>> PostUser(AppUser user)
    // {
    //   _userRepository.Users.Add(user);
    //   await _userRepository.SaveChangesAsync();
    //   return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    // }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
      string? username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      AppUser user = await _userRepository.GetUserByUsernameAsync(username);

      _mapper.Map(memberUpdateDto, user);

      _userRepository.Update(user);

      if (await _userRepository.SaveAllAsync())
      {
        return NoContent();
      }

      return BadRequest("Failed to update user");
    }

    // [HttpDelete("{id}")]
    // public async Task<ActionResult<AppUser>> DeleteUser(int id)
    // {
    //   var user = await _userRepository.Users.FindAsync(id);
    //   if (user == null)
    //   {
    //     return NotFound();
    //   }

    //   _userRepository.Users.Remove(user);
    //   await _userRepository.SaveChangesAsync();

    //   return Ok(user);
    // }
  }
}