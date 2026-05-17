using System;
using System.Data;
using System.Windows.Forms;
namespace QuanLyVanPhongPham
{
    public partial class ThongKe : Form
    {
        private GiaoDichManager gdManager = new GiaoDichManager();
        public ThongKe()
        {
            InitializeComponent();
        }
        private void ThongKe_Load_1(object sender, EventArgs e)
        {
            dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePicker2.Value = DateTime.Now;

            btn_show.Click += btn_show_Click_1;
            LoadThongKe();
        }
        private void LoadThongKe()
        {
            DataTable dt = gdManager.ThongKeDoanhThu();
            HienThiDataTable(dt);
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dateTimePicker2.Value = DateTime.Now;
            LoadThongKe();
        }
        private void HienThiDataTable(DataTable dt)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = dt;
            if (dataGridView1.Columns["TenSanPham"] != null)
                dataGridView1.Columns["TenSanPham"].HeaderText = "Tên Sản Phẩm";
            if (dataGridView1.Columns["TongSoLuong"] != null)
                dataGridView1.Columns["TongSoLuong"].HeaderText = "Tổng Số Lượng";
            if (dataGridView1.Columns["TongDoanhThu"] != null)
                dataGridView1.Columns["TongDoanhThu"].HeaderText = "Tổng Doanh Thu (đ)";
        }
        private void menuSanPham_Click_1(object sender, EventArgs e) => AppContext.NavTo(this, new QuanLySanPham());
        private void menuSanPham_Click(object sender, EventArgs e) => AppContext.NavTo(this, new QuanLyDanhMuc());
        private void menuGiaoDich_Click(object sender, EventArgs e) => AppContext.NavTo(this, new GiaoDichBanHang());
        private void menuThongKe_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }

        

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btn_show_Click_1(object sender, EventArgs e)
        {
            DateTime tuNgay = dateTimePicker1.Value.Date;
            DateTime denNgay = dateTimePicker2.Value.Date;

            if (tuNgay > denNgay)
            {
                MessageBox.Show("Từ Ngày không được lớn hơn Đến Ngày!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataTable dt = DatabaseConnection.ExecuteQuery(
                "SELECT TenSanPham, " +
                "       SUM(SoLuongBan)       AS TongSoLuong, " +
                "       SUM(Gia * SoLuongBan) AS TongDoanhThu " +
                "FROM GiaoDich " +
                "WHERE NgayGiaoDich >= '" + tuNgay.ToString("yyyy-MM-dd") + "' " +
                "  AND NgayGiaoDich <  '" + denNgay.AddDays(1).ToString("yyyy-MM-dd") + "' " +
                "GROUP BY TenSanPham " +
                "ORDER BY TongDoanhThu DESC");

            HienThiDataTable(dt);
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}