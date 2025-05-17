using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuQuanWeb.Models
{
    [Table("thiet_bi")]
    public class ThietBi
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("name")]
        public string Name { get; set; }

        [Column("loai")]
        public string Loai { get; set; }

        [Column("trang_thai")]
        public string TrangThai { get; set; }

        [Column("is_exist")]
        public bool IsExist { get; set; }
    }
}