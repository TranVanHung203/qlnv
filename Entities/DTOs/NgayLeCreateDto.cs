using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs
{
    // DTO khi tạo mới Ngày Lễ
    public class CreateNgayLeDto
    {
        [Required(ErrorMessage = "Tên ngày lễ không được để trống")]
        [StringLength(100, ErrorMessage = "Tên ngày lễ tối đa 100 ký tự")]
        public string TenNgayLe { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime NgayBatDau { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        public DateTime NgayKetThuc { get; set; }
    }

    // DTO khi cập nhật Ngày Lễ
    public class UpdateNgayLeDto : CreateNgayLeDto
    {
        [Required(ErrorMessage = "Id không được để trống")]
        public int Id { get; set; }
    }

    // DTO trả về cho client
    public class NgayLeDto
    {
        public int Id { get; set; }
        public string TenNgayLe { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
    }
}
