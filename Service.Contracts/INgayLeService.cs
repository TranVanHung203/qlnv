using Entities.DTOs;

namespace Service.Contracts
{
    public interface INgayLeService
    {
        Task<NgayLeDto> CreateAsync(CreateNgayLeDto dto);
        Task<NgayLeDto?> GetByIdAsync(int id);
        Task<PagedResult<NgayLeDto>> GetPagedAsync(int page, int pageSize, string? ten = null);
        Task<NgayLeDto> UpdateAsync(UpdateNgayLeDto dto);
        Task DeleteAsync(int id);
    }
}
