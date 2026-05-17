using System.Data;
using System.Data.SqlClient;

namespace QuanLyVanPhongPham
{
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
                "SELECT sp.MaSanPham, sp.MaDanhMuc, dm.TenDanhMuc, sp.TenSanPham, sp.GiaBan, sp.SoLuong " +
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
                    SqlCommand cmdCheck = new SqlCommand(
                        "SELECT SoLuong FROM SanPham WHERE TenSanPham = @tensp", conn, tran);
                    cmdCheck.Parameters.AddWithValue("@tensp", tenSanPham);
                    object objSL = cmdCheck.ExecuteScalar();
                    if (objSL == null)
                        throw new System.Exception($"Không tìm thấy sản phẩm '{tenSanPham}'!");
                    int tonKho = (int)objSL;
                    if (tonKho < soLuongBan)
                        throw new System.Exception($"Tồn kho chỉ còn {tonKho}, không đủ để bán {soLuongBan}!");
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
        public void Sua(int maGiaoDich, string tenSanPham, string khachHang, string phuongThuc,
                        System.DateTime ngayGiaoDich, long gia, int soLuongBanMoi)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.GetConnStr()))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    SqlCommand cmdOld = new SqlCommand(
                        "SELECT TenSanPham, SoLuongBan FROM GiaoDich WHERE MaGiaoDich = @id",
                        conn, tran);
                    cmdOld.Parameters.AddWithValue("@id", maGiaoDich);
                    SqlDataReader rdr = cmdOld.ExecuteReader();
                    string tenSPCu = "";
                    int slCu = 0;
                    if (rdr.Read())
                    {
                        tenSPCu = rdr["TenSanPham"].ToString();
                        slCu = (int)rdr["SoLuongBan"];
                    }
                    rdr.Close();
                    if (!string.IsNullOrEmpty(tenSPCu))
                    {
                        SqlCommand cmdHoan = new SqlCommand(
                            "UPDATE SanPham SET SoLuong = SoLuong + @sl WHERE TenSanPham = @tensp",
                            conn, tran);
                        cmdHoan.Parameters.AddWithValue("@sl", slCu);
                        cmdHoan.Parameters.AddWithValue("@tensp", tenSPCu);
                        cmdHoan.ExecuteNonQuery();
                    }
                    SqlCommand cmdCheck = new SqlCommand(
                        "SELECT SoLuong FROM SanPham WHERE TenSanPham = @tensp", conn, tran);
                    cmdCheck.Parameters.AddWithValue("@tensp", tenSanPham);
                    object objSL = cmdCheck.ExecuteScalar();
                    if (objSL == null)
                        throw new System.Exception($"Không tìm thấy sản phẩm '{tenSanPham}'!");
                    int tonKho = (int)objSL;
                    if (tonKho < soLuongBanMoi)
                        throw new System.Exception($"Tồn kho chỉ còn {tonKho}, không đủ để bán {soLuongBanMoi}!");
                    SqlCommand cmdUpd = new SqlCommand(
                        "UPDATE GiaoDich SET TenSanPham=@tensp, KhachHang=@khach, PhuongThuc=@pt, " +
                        "NgayGiaoDich=@ngay, Gia=@gia, SoLuongBan=@sl " +
                        "WHERE MaGiaoDich=@id",
                        conn, tran);
                    cmdUpd.Parameters.AddWithValue("@tensp", tenSanPham);
                    cmdUpd.Parameters.AddWithValue("@khach", khachHang);
                    cmdUpd.Parameters.AddWithValue("@pt", phuongThuc);
                    cmdUpd.Parameters.AddWithValue("@ngay", ngayGiaoDich);
                    cmdUpd.Parameters.AddWithValue("@gia", gia);
                    cmdUpd.Parameters.AddWithValue("@sl", soLuongBanMoi);
                    cmdUpd.Parameters.AddWithValue("@id", maGiaoDich);
                    cmdUpd.ExecuteNonQuery();
                    SqlCommand cmdTru = new SqlCommand(
                        "UPDATE SanPham SET SoLuong = SoLuong - @sl WHERE TenSanPham = @tensp",
                        conn, tran);
                    cmdTru.Parameters.AddWithValue("@sl", soLuongBanMoi);
                    cmdTru.Parameters.AddWithValue("@tensp", tenSanPham);
                    cmdTru.ExecuteNonQuery();
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void Xoa(int maGiaoDich)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.GetConnStr()))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    SqlCommand cmdGet = new SqlCommand(
                        "SELECT TenSanPham, SoLuongBan FROM GiaoDich WHERE MaGiaoDich = @id",
                        conn, tran);
                    cmdGet.Parameters.AddWithValue("@id", maGiaoDich);
                    SqlDataReader rdr = cmdGet.ExecuteReader();
                    string tenSP = "";
                    int sl = 0;
                    if (rdr.Read())
                    {
                        tenSP = rdr["TenSanPham"].ToString();
                        sl = (int)rdr["SoLuongBan"];
                    }
                    rdr.Close();
                    SqlCommand cmdDel = new SqlCommand(
                        "DELETE FROM GiaoDich WHERE MaGiaoDich = @id", conn, tran);
                    cmdDel.Parameters.AddWithValue("@id", maGiaoDich);
                    cmdDel.ExecuteNonQuery();
                    if (!string.IsNullOrEmpty(tenSP))
                    {
                        SqlCommand cmdHoan = new SqlCommand(
                            "UPDATE SanPham SET SoLuong = SoLuong + @sl WHERE TenSanPham = @tensp",
                            conn, tran);
                        cmdHoan.Parameters.AddWithValue("@sl", sl);
                        cmdHoan.Parameters.AddWithValue("@tensp", tenSP);
                        cmdHoan.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
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