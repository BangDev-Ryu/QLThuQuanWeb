using Microsoft.AspNetCore.Mvc;
using QLThuQuanWeb.Models;
using QLThuQuanWeb.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QLThuQuanWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly QLThuQuanWebContext _context;

        public AuthController(QLThuQuanWebContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.ThanhViens
                .FirstOrDefault(u => u.Username == username && 
                                   u.Password == password && 
                                   u.IsExist == true);

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Fullname", user.Fullname);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(ThanhVien model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.ThanhViens
                    .FirstOrDefaultAsync(u => u.Username == model.Username);

                if (existingUser != null)
                {
                    ViewBag.Error = "Tên đăng nhập đã tồn tại";
                    return View(model);
                }

                model.TrangThai = "Hoạt động";
                model.IsExist = true;

                _context.ThanhViens.Add(model);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction(nameof(Login));
            }

            var user = await _context.ThanhViens
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ThanhVien model)
        {
            try 
            {
                if (!ModelState.IsValid)
                {
                    return View("Profile", model);
                }

                var username = HttpContext.Session.GetString("Username");
                if (string.IsNullOrEmpty(username))
                {
                    return RedirectToAction(nameof(Login));
                }

                // Tìm user hiện tại
                var user = await _context.ThanhViens
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsExist == true);

                if (user == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin
                user.Fullname = model.Fullname;
                user.Khoa = model.Khoa;
                user.Nganh = model.Nganh;

                // Đánh dấu entity đã thay đổi
                _context.Entry(user).State = EntityState.Modified;

                // Lưu vào database
                await _context.SaveChangesAsync();

                // Cập nhật session
                HttpContext.Session.SetString("Fullname", user.Fullname);

                TempData["Success"] = "Cập nhật thông tin thành công";
                return RedirectToAction(nameof(Profile));
            }
            catch (DbUpdateException ex)
            {
                // Log lỗi
                System.Diagnostics.Debug.WriteLine($"Error updating profile: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thông tin");
                return View("Profile", model);
            }
            catch (Exception ex)
            {
                // Log lỗi
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError("", "Có lỗi xảy ra, vui lòng thử lại sau");
                return View("Profile", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction(nameof(Login));
            }

            var user = await _context.ThanhViens
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound();
            }

            var phieuMuons = await _context.PhieuMuons
                .Where(p => p.IdThanhVien == user.Id)
                .Include(p => p.ThietBi)
                .OrderByDescending(p => p.NgayMuon)
                .ToListAsync();

            return View(phieuMuons);
        }
    }
}