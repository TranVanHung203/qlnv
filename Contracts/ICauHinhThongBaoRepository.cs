using Entities.Models;

namespace Contracts
{
    public interface ICauHinhThongBaoRepository
    {
        Task<CauHinhThongBao?> GetAsync();
        Task<List<CauHinhThongBao>> GetAllAsync();
        Task<CauHinhThongBao?> GetByIdAsync(int id);
        Task<CauHinhThongBao> CreateAsync(CauHinhThongBao cfg);
        Task UpdateAsync(CauHinhThongBao cfg);
        Task DeleteAsync(CauHinhThongBao entity);

    }
}
