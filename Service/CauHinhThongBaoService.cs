using Contracts;
using Entities.DTOs;
using Entities.Models;
using Service.Contracts;
using System.Net;

namespace Service
{
    public class CauHinhThongBaoService : ICauHinhThongBaoService
    {
        private readonly ICauHinhThongBaoRepository _cfgRepo;
        private readonly INhanVienRepository _nvRepo;
        private readonly INgayLeRepository _ngayLeRepo;
        private readonly IEmailSenderService _emailSender;
        private readonly IEmailThongBaoRepository _emailThongBaoRepo;
        private readonly IThongBaoRepository _thongBaoRepo;

        public CauHinhThongBaoService(ICauHinhThongBaoRepository cfgRepo,
            INhanVienRepository nvRepo,
            INgayLeRepository ngayLeRepo,
            IEmailSenderService emailSender,
            IThongBaoRepository thongBaoRepo,
            IEmailThongBaoRepository emailThongBaoRepo)
        {
            _cfgRepo = cfgRepo;
            _nvRepo = nvRepo;
            _ngayLeRepo = ngayLeRepo;
            _emailSender = emailSender;
            _thongBaoRepo = thongBaoRepo;
            _emailThongBaoRepo = emailThongBaoRepo;
        }

        public async Task<List<CauHinhThongBaoDto>> GetAllConfigsAsync()
        {
            var list = await _cfgRepo.GetAllAsync();
            return list.Select(c => new CauHinhThongBaoDto
            {
                Id = c.Id,
                SoNgayThongBao = c.SoNgayThongBao,
                DanhSachNamThongBao = c.DanhSachNamThongBao,
                IsActive = c.IsActive
            }).ToList();
        }

        public async Task<CauHinhThongBaoDto?> GetConfigByIdAsync(int id)
        {
            var c = await _cfgRepo.GetByIdAsync(id);
            if (c == null) return null;
            return new CauHinhThongBaoDto { Id = c.Id, SoNgayThongBao = c.SoNgayThongBao, DanhSachNamThongBao = c.DanhSachNamThongBao, IsActive = c.IsActive };
        }

        public async Task<CauHinhThongBaoDto?> ActivateConfigAsync(int id)
        {
            var cfg = await _cfgRepo.GetByIdAsync(id);
            if (cfg == null) return null;

            cfg.IsActive = true;
            await _cfgRepo.UpdateAsync(cfg);

            return new CauHinhThongBaoDto { Id = cfg.Id, SoNgayThongBao = cfg.SoNgayThongBao, DanhSachNamThongBao = cfg.DanhSachNamThongBao, IsActive = cfg.IsActive };
        }

        public async Task<CauHinhThongBaoDto?> GetActiveOnlyAsync()
        {
            // Use repository to fetch only explicitly active record; do NOT fallback to newest
            var all = await _cfgRepo.GetAllAsync();
            var active = all.FirstOrDefault(c => c.IsActive);
            if (active == null) return null;
            return new CauHinhThongBaoDto { Id = active.Id, SoNgayThongBao = active.SoNgayThongBao, DanhSachNamThongBao = active.DanhSachNamThongBao, IsActive = active.IsActive };
        }

        public async Task<CauHinhThongBaoDto?> GetConfigAsync()
        {
            var cfg = await _cfgRepo.GetAsync();
            if (cfg == null) return null;
            return new CauHinhThongBaoDto { Id = cfg.Id, SoNgayThongBao = cfg.SoNgayThongBao };
        }

        public async Task<CauHinhThongBaoDto> CreateConfigAsync(CreateCauHinhThongBaoDto dto)
        {
            var cfg = new CauHinhThongBao
            {
                SoNgayThongBao = dto.SoNgayThongBao,
                DanhSachNamThongBao = dto.DanhSachNamThongBao
                ,
                IsActive = dto.IsActive
            };

            var created = await _cfgRepo.CreateAsync(cfg);
            return new CauHinhThongBaoDto { Id = created.Id, SoNgayThongBao = created.SoNgayThongBao, DanhSachNamThongBao = created.DanhSachNamThongBao, IsActive = created.IsActive };
        }

        public async Task<CauHinhThongBaoDto> UpdateConfigAsync(UpdateCauHinhThongBaoDto dto)
        {
            var cfg = await _cfgRepo.GetAsync();
            if (cfg == null)
            {
                cfg = new CauHinhThongBao { SoNgayThongBao = dto.SoNgayThongBao, DanhSachNamThongBao = dto.DanhSachNamThongBao, IsActive = dto.IsActive };
                var created = await _cfgRepo.CreateAsync(cfg);
                return new CauHinhThongBaoDto { Id = created.Id, SoNgayThongBao = created.SoNgayThongBao, DanhSachNamThongBao = created.DanhSachNamThongBao, IsActive = created.IsActive };
            }

            cfg.SoNgayThongBao = dto.SoNgayThongBao;
            cfg.DanhSachNamThongBao = dto.DanhSachNamThongBao;
            cfg.IsActive = dto.IsActive;
            await _cfgRepo.UpdateAsync(cfg);

            return new CauHinhThongBaoDto { Id = cfg.Id, SoNgayThongBao = cfg.SoNgayThongBao, DanhSachNamThongBao = cfg.DanhSachNamThongBao, IsActive = cfg.IsActive };
        }
        public async Task DeleteConfigAsync(int id)
        {
            var entity = await _cfgRepo.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException("Không tìm thấy cấu hình thông báo.");

            await _cfgRepo.DeleteAsync(entity);
        }

        public async Task<int> RunCheckAndSendAsync()
        {
            // Load config
            var cfg = await _cfgRepo.GetAsync();
            int soNgay = cfg?.SoNgayThongBao ?? 60;

            // Load employees
            var employees = await _nvRepo.GetAllAsync();

            // Load holidays
            var holidaysPaged = await _ngayLeRepo.GetPagedAsync(1, int.MaxValue);
            var holidays = holidaysPaged.Items.SelectMany(n =>
                Enumerable.Range(0, (int)(n.NgayKetThuc.Date - n.NgayBatDau.Date).TotalDays + 1)
                          .Select(i => n.NgayBatDau.Date.AddDays(i))).ToHashSet();

            var toNotify = new List<(int NhanVienId, string Email, string Reason)>();

            var utcNow = DateTime.UtcNow.Date;

            foreach (var nv in employees)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(nv.Email)) continue;

                    //// Avoid sending if already sent today
                    //var alreadySent = await _thongBaoRepo.ExistsForNhanVienOnDateAsync(nv.Id, utcNow);
                    //if (alreadySent) continue;

                    var join = nv.NgayVaoLam.Date;

                    // Anniversary years from config (e.g. "1,2,3")
                    var years = new List<int>();
                    if (!string.IsNullOrWhiteSpace(cfg?.DanhSachNamThongBao))
                    {
                        years = cfg.DanhSachNamThongBao.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => { if (int.TryParse(s.Trim(), out var v)) return v; return 0; })
                            .Where(v => v > 0).ToList();
                    }

                    // Check anniversary years
                    bool anniversaryHandled = false;
                    var reasons = new List<string>();

                    foreach (var y in years)
                    {
                        var anniversaryDate = join.AddYears(y);
                        if (utcNow >= anniversaryDate)
                        {
                            reasons.Add($"Kỷ niệm {y} năm làm việc");
                        }
                    }

                    foreach (var reason in reasons)
                    {
                        toNotify.Add((nv.Id, nv.Email, reason));
                        anniversaryHandled = true;
                    }


                    if (anniversaryHandled) continue;

                    // Probation logic: measure working days (exclude Saturdays and holidays)
                    var workingDays = CountWorkingDays(join, utcNow, holidays);
                    if (workingDays >= soNgay)
                    {
                        var reason = $"Đã đủ {soNgay} ngày làm việc (thử việc)";
                        // Tentatively add; final duplicate suppression will be done per recipient when sending
                        toNotify.Add((nv.Id, nv.Email, reason));
                    }
                }
                catch
                {
                    // swallow per-user errors to continue processing others
                }
            }

            // Send emails and log
            if (!toNotify.Any()) return 0;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<h3>Danh sách nhân viên cần thông báo</h3>");
            sb.AppendLine("<table border=1 cellpadding=5 cellspacing=0>");
            sb.AppendLine("<tr><th>Id</th><th>Họ tên</th><th>Email NV</th><th>Lý do</th></tr>");

            var nvMap = employees.ToDictionary(e => e.Id);
            foreach (var item in toNotify)
            {
                nvMap.TryGetValue(item.NhanVienId, out var emp);
                var name = emp?.Ten ?? "-";
                var emailNv = item.Email ?? "-";
                sb.AppendLine($"<tr><td>{item.NhanVienId}</td><td>{System.Net.WebUtility.HtmlEncode(name)}</td><td>{System.Net.WebUtility.HtmlEncode(emailNv)}</td><td>{System.Net.WebUtility.HtmlEncode(item.Reason)}</td></tr>");
            }

            sb.AppendLine("</table>");

            var htmlBody = sb.ToString();

            var cfgEmailsPaged = await _emailThongBaoRepo.GetPagedAsync(1, int.MaxValue);
            var recipients = cfgEmailsPaged.Items.Select(e => e.Email).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            if (!recipients.Any())
            {
                return 0;
            }

            byte[] excelBytes;
            using (var wb = new ClosedXML.Excel.XLWorkbook())
            {
                var ws = wb.Worksheets.Add("ThongBao");
                ws.Cell(1, 1).Value = "Id";
                ws.Cell(1, 2).Value = "Ten";
                ws.Cell(1, 3).Value = "EmailNV";
                ws.Cell(1, 4).Value = "LyDo";
                int r = 2;
                foreach (var item in toNotify)
                {
                    var emp = employees.FirstOrDefault(e => e.Id == item.NhanVienId);
                    ws.Cell(r, 1).Value = item.NhanVienId;
                    ws.Cell(r, 2).Value = emp?.Ten ?? "-";
                    ws.Cell(r, 3).Value = item.Email ?? "-";
                    ws.Cell(r, 4).Value = item.Reason;
                    r++;
                }

                ws.Columns().AdjustToContents();

                using (var ms = new System.IO.MemoryStream())
                {
                    wb.SaveAs(ms);
                    excelBytes = ms.ToArray();
                }
            }

            int sent = 0;
            var subject = "Báo cáo thông báo nhân sự";
            var attachments = new List<(string FileName, byte[] Content)> { ("ThongBao.xlsx", excelBytes) };

            foreach (var to in recipients)
            {
                try
                {
                    // For each recipient, ensure we don't duplicate sends for the same employee/reason/email
                    var itemsForThisRecipient = new List<(int NhanVienId, string Email, string Reason)>();
                    foreach (var item in toNotify)
                    {
                        var alreadySentForRecipient = await _thongBaoRepo.ExistsForNhanVienWithReasonAsync(item.NhanVienId, item.Reason, to);
                        if (!alreadySentForRecipient)
                        {
                            itemsForThisRecipient.Add(item);
                        }
                    }

                    if (!itemsForThisRecipient.Any()) continue;

                    // Build per-recipient HTML/body (we reuse same htmlBody/excel for simplicity)
                    await _emailSender.SendEmailAsync(to, subject, htmlBody, attachments);

                    foreach (var item in itemsForThisRecipient)
                    {
                        await _thongBaoRepo.CreateAsync(new ThongBao
                        {
                            NhanVienId = item.NhanVienId,
                            EmailNhan = to,
                            NgayGui = DateTime.UtcNow,
                            LyDo = item.Reason
                        });
                        sent++;
                    }
                }
                catch
                {
                    // continue to next recipient
                }
            }

            return sent;
        }

        private int CountWorkingDays(DateTime startDate, DateTime endDate, HashSet<DateTime> holidays)
        {
            if (endDate <= startDate) return 0; // chưa qua ngày nào thì = 0

            int count = 0;
            for (var d = startDate.Date.AddDays(1); d <= endDate.Date; d = d.AddDays(1))
            {
                if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday) continue;
                if (holidays.Contains(d.Date)) continue;
                count++;
            }

            return count;
        }

    }
}
