namespace Entities.Models
{
    public class CauHinhThongBao
    {
        public int Id { get; set; }
        // probation days default (e.g., 60)
        public int SoNgayThongBao { get; set; } = 60;

        // comma separated list of anniversary years to notify, e.g. "1,2,3"
        public string? DanhSachNamThongBao { get; set; }
        
        // mark this configuration as the active one used by the system
        public bool IsActive { get; set; } = false;
    }
}
