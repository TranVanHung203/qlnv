using Entities.DTOs;

namespace Service.Contracts
{
    public interface INhanVienService
    {
        Task<NhanVienDto> CreateAsync(CreateNhanVienDto dto);
        Task<NhanVienDto?> GetByIdAsync(int id);
        Task<PagedResult<NhanVienDto>> GetPagedAsync(int page, int pageSize, string? ten = null, string? sdt = null);
        Task<NhanVienDto> UpdateAsync(UpdateNhanVienDto dto);
        Task DeleteAsync(int id);
    }
}
