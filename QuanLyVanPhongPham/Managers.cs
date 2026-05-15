using System.Data;
using System.Data.SqlClient;

namespace QuanLyVanPhongPham
{
    // ─────────────────────────────────────────────────────────────────────────
    // SanPhamManager — khớp schema: SanPham(MaSanPham, TenSanPham, MaDanhMuc, GiaBan, SoLuong)
    // ─────────────────────────────────────────────────────────────────────────
    public class SanPhamManager
    {
        public DataTable GetDanhSach()
        {
            return DatabaseConnection.ExecuteQuery(
                "SELECT sp.MaSanPham, dm.TenDanhMuc, sp.TenSanPham, sp.GiaBan, sp.SoLuong " +
                "FROM SanPham sp " +
                "JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc " +
                "ORDER BY sp.MaSanPham DESC");
        }

        public DataTable TimKiem(string tenSP, string tenDanhMuc)
        {
            string sql =
                "SELECT sp.MaSanPham, dm.TenDanhMuc, sp.TenSanPham, sp.GiaBan, sp.SoLuong " +
                "FROM SanPham sp " +
                "JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc " +
                "WHERE sp.TenSanPham LIKE N'%" + tenSP + "%' " +
                "  AND dm.TenDanhMuc LIKE N'%" + tenDanhMuc + "%' " +
                "ORDER BY sp.MaSanPham DESC";
            return DatabaseConnection.ExecuteQuery(sql);
        }

        public void Them(string tenSP, int giaBan, int soLuong, int maDanhMuc)
        {
            SqlCommand cmd = new SqlCommand(
                "INSERT INTO SanPham(TenSanPham, GiaBan, SoLuong, MaDanhMuc) " +
                "VALUES(@tensp, @gia, @soluong, @madm)");
            cmd.Parameters.AddWithValue("@tensp", tenSP);
            cmd.Parameters.AddWithValue("@gia", giaBan);
            cmd.Parameters.AddWithValue("@soluong", soLuong);
            cmd.Parameters.AddWithValue("@madm", maDanhMuc);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }

        public void Sua(int maSP, string tenSP, int giaBan, int soLuong, int maDanhMuc)
        {
            SqlCommand cmd = new SqlCommand(
                "UPDATE SanPham SET TenSanPham=@tensp, GiaBan=@gia, " +
                "SoLuong=@soluong, MaDanhMuc=@madm " +
                "WHERE MaSanPham=@id");
            cmd.Parameters.AddWithValue("@tensp", tenSP);
            cmd.Parameters.AddWithValue("@gia", giaBan);
            cmd.Parameters.AddWithValue("@soluong", soLuong);
            cmd.Parameters.AddWithValue("@madm", maDanhMuc);
            cmd.Parameters.AddWithValue("@id", maSP);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }

        public void Xoa(int maSP)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM SanPham WHERE MaSanPham=@id");
            cmd.Parameters.AddWithValue("@id", maSP);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // DanhMucManager — không đổi
    // ─────────────────────────────────────────────────────────────────────────
    public class DanhMucManager
    {
        public DataTable GetDanhSach()
        {
            return DatabaseConnection.ExecuteQuery(
                "SELECT MaDanhMuc, TenDanhMuc FROM DanhMuc ORDER BY MaDanhMuc DESC");
        }

        public void Them(string tenDM)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO DanhMuc(TenDanhMuc) VALUES(@ten)");
            cmd.Parameters.AddWithValue("@ten", tenDM);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }

        public void Sua(int maDM, string tenDM)
        {
            SqlCommand cmd = new SqlCommand(
                "UPDATE DanhMuc SET TenDanhMuc=@ten WHERE MaDanhMuc=@id");
            cmd.Parameters.AddWithValue("@ten", tenDM);
            cmd.Parameters.AddWithValue("@id", maDM);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }

        public void Xoa(int maDM)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM DanhMuc WHERE MaDanhMuc=@id");
            cmd.Parameters.AddWithValue("@id", maDM);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GiaoDichManager — khớp schema:
    //   GiaoDich(MaGiaoDich, NgayGiaoDich, TenSanPham, KhachHang,
    //            PhuongThuc, Gia, SoLuongBan)
    // ─────────────────────────────────────────────────────────────────────────
    public class GiaoDichManager
    {
        public DataTable GetDanhSach()
        {
            return DatabaseConnection.ExecuteQuery(
                "SELECT MaGiaoDich, TenSanPham, KhachHang, NgayGiaoDich, PhuongThuc, Gia, SoLuongBan " +
                "FROM GiaoDich " +
                "ORDER BY MaGiaoDich DESC");
        }

        public void Them(string tenSanPham, string khachHang, string phuongThuc,
                         System.DateTime ngayGiaoDich, long gia, int soLuongBan)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.GetConnStr()))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // 1. Kiểm tra tồn kho
                    SqlCommand cmdCheck = new SqlCommand(
                        "SELECT SoLuong FROM SanPham WHERE TenSanPham = @tensp", conn, tran);
                    cmdCheck.Parameters.AddWithValue("@tensp", tenSanPham);
                    object objSL = cmdCheck.ExecuteScalar();
                    if (objSL == null)
                        throw new System.Exception($"Không tìm thấy sản phẩm '{tenSanPham}'!");
                    int tonKho = (int)objSL;
                    if (tonKho < soLuongBan)
                        throw new System.Exception($"Tồn kho chỉ còn {tonKho}, không đủ để bán {soLuongBan}!");

                    // 2. Thêm giao dịch
                    SqlCommand cmdGD = new SqlCommand(
                        "INSERT INTO GiaoDich(TenSanPham, KhachHang, PhuongThuc, NgayGiaoDich, Gia, SoLuongBan) " +
                        "VALUES(@tensp, @khach, @phuongthuc, @ngay, @gia, @sl)",
                        conn, tran);
                    cmdGD.Parameters.AddWithValue("@tensp", tenSanPham);
                    cmdGD.Parameters.AddWithValue("@khach", khachHang);
                    cmdGD.Parameters.AddWithValue("@phuongthuc", phuongThuc);
                    cmdGD.Parameters.AddWithValue("@ngay", ngayGiaoDich);
                    cmdGD.Parameters.AddWithValue("@gia", gia);
                    cmdGD.Parameters.AddWithValue("@sl", soLuongBan);
                    cmdGD.ExecuteNonQuery();

                    // 3. Trừ tồn kho
                    SqlCommand cmdSP = new SqlCommand(
                        "UPDATE SanPham SET SoLuong = SoLuong - @sl WHERE TenSanPham = @tensp",
                        conn, tran);
                    cmdSP.Parameters.AddWithValue("@sl", soLuongBan);
                    cmdSP.Parameters.AddWithValue("@tensp", tenSanPham);
                    cmdSP.ExecuteNonQuery();

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Thống kê doanh thu theo sản phẩm.
        /// TongDoanhThu = SUM(Gia * SoLuongBan) vì bảng không có cột TongTien.
        /// </summary>
        public DataTable ThongKeDoanhThu()
        {
            return DatabaseConnection.ExecuteQuery(
                "SELECT TenSanPham, " +
                "       SUM(SoLuongBan)         AS TongSoLuong, " +
                "       SUM(Gia * SoLuongBan)   AS TongDoanhThu " +
                "FROM GiaoDich " +
                "GROUP BY TenSanPham " +
                "ORDER BY TongDoanhThu DESC");
        }
    }
}