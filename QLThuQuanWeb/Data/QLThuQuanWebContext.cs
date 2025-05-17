using Microsoft.EntityFrameworkCore;
using QLThuQuanWeb.Models;

namespace QLThuQuanWeb.Data
{
    public class QLThuQuanWebContext : DbContext
    {
        public QLThuQuanWebContext(DbContextOptions<QLThuQuanWebContext> options)
            : base(options)
        {
        }

        public DbSet<ThanhVien> ThanhViens { get; set; }
        public DbSet<ThietBi> ThietBis { get; set; }
        public DbSet<PhieuMuon> PhieuMuons { get; set; }
    }
}