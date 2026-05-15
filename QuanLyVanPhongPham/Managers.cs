using System.Data;
using System.Data.SqlClient;
namespace QuanLyVanPhongPham
{
    public class SanPhamManager
    {
        public DataTable GetDanhSach()
        {
            return DatabaseConnection.ExecuteQuery(
                "select sp.MaSanPham, dm.TenDanhMuc, sp.TenSanPham, sp.GiaBan, sp.SoLuong " +
                "from SanPham sp join DanhMuc dm on sp.MaDanhMuc = dm.MaDanhMuc " +
                "order by sp.MaSanPham desc");
        }
        public DataTable TimKiem(string tenSP, string tenDanhMuc)
        {
            string sql =
                "select sp.MaSanPham, dm.TenDanhMuc, sp.TenSanPham, sp.GiaBan, sp.SoLuong " +
                "from SanPham sp join DanhMuc dm on sp.MaDanhMuc = dm.MaDanhMuc " +
                "where sp.TenSanPham like N'%" + tenSP + "%' " +
                "and dm.TenDanhMuc like N'%" + tenDanhMuc + "%' " +
                "order by sp.MaSanPham desc";
            return DatabaseConnection.ExecuteQuery(sql);
        }
        public void Them(string tenSP, int giaBan, int soLuong, int maDanhMuc)
        {
            SqlCommand cmd = new SqlCommand(
                "insert into SanPham(TenSanPham, GiaBan, SoLuong, MaDanhMuc) " +
                "values(@tensp, @gia, @soluong, @madm)");
            cmd.Parameters.AddWithValue("@tensp", tenSP);
            cmd.Parameters.AddWithValue("@gia", giaBan);
            cmd.Parameters.AddWithValue("@soluong", soLuong);
            cmd.Parameters.AddWithValue("@madm", maDanhMuc);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }
        public void Sua(int maSP, string tenSP, int giaBan, int soLuong, int maDanhMuc)
        {
            SqlCommand cmd = new SqlCommand(
                "update SanPham set TenSanPham=@tensp, GiaBan=@gia, " +
                "SoLuong=@soluong, MaDanhMuc=@madm where MaSanPham=@id");
            cmd.Parameters.AddWithValue("@tensp", tenSP);
            cmd.Parameters.AddWithValue("@gia", giaBan);
            cmd.Parameters.AddWithValue("@soluong", soLuong);
            cmd.Parameters.AddWithValue("@madm", maDanhMuc);
            cmd.Parameters.AddWithValue("@id", maSP);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }
        public void Xoa(int maSP)
        {
            SqlCommand cmd = new SqlCommand(
                "delete from SanPham where MaSanPham=@id");
            cmd.Parameters.AddWithValue("@id", maSP);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }
    }
    public class DanhMucManager
    {
        public DataTable GetDanhSach()
        {
            return DatabaseConnection.ExecuteQuery(
                "select MaDanhMuc, TenDanhMuc from DanhMuc order by MaDanhMuc desc");
        }
        public void Them(string tenDM)
        {
            SqlCommand cmd = new SqlCommand(
                "insert into DanhMuc(TenDanhMuc) values(@ten)");
            cmd.Parameters.AddWithValue("@ten", tenDM);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }
        public void Sua(int maDM, string tenDM)
        {
            SqlCommand cmd = new SqlCommand(
                "update DanhMuc set TenDanhMuc=@ten where MaDanhMuc=@id");
            cmd.Parameters.AddWithValue("@ten", tenDM);
            cmd.Parameters.AddWithValue("@id", maDM);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }
        public void Xoa(int maDM)
        {
            SqlCommand cmd = new SqlCommand(
                "delete from DanhMuc where MaDanhMuc=@id");
            cmd.Parameters.AddWithValue("@id", maDM);
            DatabaseConnection.ExecuteNonQuery(cmd);
        }
    }
    public class GiaoDichManager
    {
        public DataTable GetDanhSach()
        {
            return DatabaseConnection.ExecuteQuery(
                "select MaGiaoDich, NgayGiaoDich, NguoiThucHien, TenSanPham, SoLuongBan, TongTien " +
                "from GiaoDich order by MaGiaoDich desc");
        }
        public void Them(string nguoiThucHien, string tenSP, int soLuongBan, int tongTien)
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.GetConnStr()))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                try
                {
                    SqlCommand cmdGD = new SqlCommand(
                        "insert into GiaoDich(NguoiThucHien, TenSanPham, SoLuongBan, TongTien) " +
                        "values(@nguoi, @tensp, @sl, @tien)", conn, tran);
                    cmdGD.Parameters.AddWithValue("@nguoi", nguoiThucHien);
                    cmdGD.Parameters.AddWithValue("@tensp", tenSP);
                    cmdGD.Parameters.AddWithValue("@sl", soLuongBan);
                    cmdGD.Parameters.AddWithValue("@tien", tongTien);
                    cmdGD.ExecuteNonQuery();
                    SqlCommand cmdSP = new SqlCommand(
                        "update SanPham set SoLuong = SoLuong - @sl " +
                        "where TenSanPham = @tensp", conn, tran);
                    cmdSP.Parameters.AddWithValue("@sl", soLuongBan);
                    cmdSP.Parameters.AddWithValue("@tensp", tenSP);
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
        public DataTable ThongKeDoanhThu()
        {
            return DatabaseConnection.ExecuteQuery(
                "select TenSanPham, SUM(SoLuongBan) as TongSoLuong, SUM(TongTien) as TongDoanhThu " +
                "from GiaoDich group by TenSanPham order by TongDoanhThu desc");
        }
    }
}