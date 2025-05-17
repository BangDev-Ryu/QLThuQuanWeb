using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLThuQuanWeb.Models
{
    [Table("phieu_muon")]
    public class PhieuMuon
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("id_thiet_bi")]
        public int IdThietBi { get; set; }

        [ForeignKey("IdThietBi")]
        public ThietBi ThietBi { get; set; }

        [Column("id_thanh_vien")]
        public int IdThanhVien { get; set; }

        [Column("ngay_muon")]
        public DateTime NgayMuon { get; set; }

        [Column("ngay_han_tra")]
        public DateTime NgayTra { get; set; }

        [Column("loai")]
        public string Loai { get; set; }

        [Column("trang_thai")]
        public string TrangThai { get; set; }

        [Column("is_exist")]
        public bool IsExist { get; set; }

    }
}