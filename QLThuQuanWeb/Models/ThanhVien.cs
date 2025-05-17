using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuQuanWeb.Models
{
    [Table("thanh_vien")]
    public class ThanhVien
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("fullname")]
        public string Fullname { get; set; }

        [Column("khoa")]
        public string Khoa { get; set; }

        [Column("nganh")]
        public string Nganh { get; set; }

        [Column("trang_thai")]
        public string TrangThai { get; set; }

        [Column("is_exist")]
        public bool IsExist { get; set; }
    }
}