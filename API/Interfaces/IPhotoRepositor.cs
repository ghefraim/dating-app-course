using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
  public interface IPhotoRepository
  {
    Task<IEnumerable<PhotoDto>> GetUnapprovedPhotosAsync();

    void ApprovePhoto(Photo photo, bool isMain);

    void DeletePhoto(Photo photo);

    Task<Photo> GetPhotoByIdAsync(int photoId);
  }
}