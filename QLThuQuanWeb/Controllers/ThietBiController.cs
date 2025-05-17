using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuQuanWeb.Data;
using QLThuQuanWeb.Models;

namespace QLThuQuanWeb.Controllers
{
    public class ThietBiController : Controller
    {
        private readonly QLThuQuanWebContext _context;

        public ThietBiController(QLThuQuanWebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var thietBis = await _context.ThietBis
                .Where(t => t.IsExist)
                .ToListAsync();
            return View(thietBis);
        }

        public IActionResult ThietBi()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var thietBi = await _context.ThietBis.FindAsync(id);
            if (thietBi == null)
            {
                return NotFound();
            }
            return View(thietBi);
        }

        [HttpPost]
        public async Task<IActionResult> DatMuon(int id, DateTime ngayMuon)
        {
            try
            {
                var thietBi = await _context.ThietBis.FindAsync(id);
                if (thietBi == null)
                {
                    TempData["Error"] = "Không tìm thấy thiết bị";
                    return RedirectToAction(nameof(Index));
                }

                if (!thietBi.IsExist)
                {
                    TempData["Error"] = "Thiết bị này hiện không khả dụng";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra trạng thái thiết bị
                if (thietBi.TrangThai == "Đang mượn")
                {
                    TempData["Error"] = "Thiết bị này hiện đang được mượn";
                    return RedirectToAction(nameof(Index));
                }

                // Validate ngày mượn
                if (ngayMuon.Date < DateTime.Now.Date)
                {
                    TempData["Error"] = "Ngày mượn không hợp lệ";
                    return RedirectToAction(nameof(Index));
                }

                var username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction("Login", "Auth");
                }

                var user = await _context.ThanhViens
                    .FirstOrDefaultAsync(u => u.Username == username);

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Tạo phiếu mượn
                    var phieuMuon = new PhieuMuon
                    {
                        IdThietBi = id,
                        IdThanhVien = user.Id,
                        NgayMuon = ngayMuon,
                        NgayTra = ngayMuon.AddDays(7),
                        Loai = "Mượn thiết bị",
                        TrangThai = "Đang xử lý",
                        IsExist = true
                    };

                    // Cập nhật trạng thái thiết bị
                    thietBi.TrangThai = "Đang mượn";
                    _context.Entry(thietBi).State = EntityState.Modified;

                    _context.PhieuMuons.Add(phieuMuon);
                    await _context.SaveChangesAsync();
                    
                    await transaction.CommitAsync();

                    TempData["Success"] = "Đặt mượn thiết bị thành công";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi đặt mượn thiết bị";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
