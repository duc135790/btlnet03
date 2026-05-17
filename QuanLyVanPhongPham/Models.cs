namespace QuanLyVanPhongPham
{
    public class DanhMuc
    {
        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }
        public DanhMuc(int ma, string ten)
        {
            this.MaDanhMuc = ma;
            this.TenDanhMuc = ten;
        }
        public override string ToString() => TenDanhMuc;
    }
    public class SanPham
    {
        public int MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int GiaBan { get; set; }
        public int SoLuong { get; set; }
        public int MaDanhMuc { get; set; }
        public string MoTa { get; set; }
        public string TrangThai { get; set; }

        public SanPham(int ma, string ten, int gia, int sl, int maDM, string moTa = "", string trangThai = "")
        {
            this.MaSanPham = ma;
            this.TenSanPham = ten;
            this.GiaBan = gia;
            this.SoLuong = sl;
            this.MaDanhMuc = maDM;
            this.MoTa = moTa;
            this.TrangThai = trangThai;
        }
        public override string ToString() => TenSanPham;
    }
}