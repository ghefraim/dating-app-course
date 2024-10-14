using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
  public class PhotoRepository : IPhotoRepository
  {
    private readonly DataContext _context;

    private readonly IMapper _mapper;

    public PhotoRepository(DataContext context, IMapper mapper)
    {
      _context = context;
      _mapper = mapper;
    }

    public void ApprovePhoto(Photo photo, bool isMain)
    {
      photo.IsApproved = true;
      if (isMain)
      {
        photo.IsMain = true;
      }
    }

    public void DeletePhoto(Photo photo)
    {
      _context.Photos.Remove(photo);
    }

    public async Task<Photo> GetPhotoByIdAsync(int photoId)
    {
      return await _context.Photos.FindAsync(photoId);
    }

    public async Task<IEnumerable<PhotoDto>> GetUnapprovedPhotosAsync()
    {
      return await _context.Photos
        .IgnoreQueryFilters()
        .Where(p => !p.IsApproved)
        .ProjectTo<PhotoDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
    }
  }
}