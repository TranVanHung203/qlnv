using System;

namespace Entities.DTOs
{
    public class ThongBaoDto
    {
        public int Id { get; set; }
        public int NhanVienId { get; set; }
        public string EmailNhan { get; set; }
        public DateTime NgayGui { get; set; }
        // ISO 8601 UTC string representation for client-side timezone conversion (e.g. Angular)
        public string NgayGuiIso => NgayGui.ToUniversalTime().ToString("o");
        public string? LyDo { get; set; }

        // optional nav
        public string? TenNhanVien { get; set; }
    }

    public class ThongBaoFilterDto
    {
        public int? NhanVienId { get; set; }
        public string? EmailNhan { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
