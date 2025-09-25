namespace Entities.Models
{
    public class ThongBao
    {
        public int Id { get; set; }
        public int NhanVienId { get; set; }
        public string EmailNhan { get; set; }
        public DateTime NgayGui { get; set; } = DateTime.UtcNow;
        public string? LyDo { get; set; }

        // navigation
        public NhanVien NhanVien { get; set; }
    }
}
