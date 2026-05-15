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

        private void ThongKe_Load(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(new string[] { "Doanh thu theo sản phẩm", "Tất cả giao dịch" });
            comboBox2.SelectedIndex = 0;
            LoadThongKe();
        }

        private void LoadThongKe()
        {
            DataTable dt = gdManager.ThongKeDoanhThu();
            dataGridView1.DataSource = dt;

            long tongDT = 0, tongSL = 0, maxSL = 0;
            string sanPhamBanChay = "";

            foreach (DataRow row in dt.Rows)
            {
                long sl = Convert.ToInt64(row["TongSoLuong"]);
                long tien = Convert.ToInt64(row["TongDoanhThu"]);
                tongSL += sl;
                tongDT += tien;
                if (sl > maxSL) { maxSL = sl; sanPhamBanChay = row["TenSanPham"].ToString(); }
            }

            textBox3.Text = string.Format("{0:N0} đ", tongDT);
            textBox7.Text = tongSL.ToString();
            textBox4.Text = sanPhamBanChay;
            textBox5.Text = sanPhamBanChay;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tuNgay = textBox1.Text.Trim();
            string denNgay = textBox2.Text.Trim();
            if (string.IsNullOrWhiteSpace(tuNgay) && string.IsNullOrWhiteSpace(denNgay))
            { LoadThongKe(); return; }

            string sql = "select TenSanPham, SUM(SoLuongBan) as TongSoLuong, SUM(TongTien) as TongDoanhThu " +
                         "from GiaoDich where 1=1 ";
            if (!string.IsNullOrWhiteSpace(tuNgay))
                sql += "and NgayGiaoDich >= N'" + tuNgay + "' ";
            if (!string.IsNullOrWhiteSpace(denNgay))
                sql += "and NgayGiaoDich <= N'" + denNgay + " 23:59:59' ";
            sql += "group by TenSanPham order by TongDoanhThu desc";
            dataGridView1.DataSource = DatabaseConnection.ExecuteQuery(sql);
        }

        private void btnLamMoiSP_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            LoadThongKe();
        }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng này không áp dụng cho Thống Kê.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSuaSP_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng này không áp dụng cho Thống Kê.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnXoaSP_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng này không áp dụng cho Thống Kê.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ── Menu navigation (FIX: mở form mới trước, close sau) ───
        private void menuSanPham_Click(object sender, EventArgs e)
        {
            new QuanLySanPham().Show();
            this.Close();
        }

        private void menuDanhMuc_Click(object sender, EventArgs e)
        {
            new QuanLyDanhMuc().Show();
            this.Close();
        }

        private void menuGiaoDich_Click(object sender, EventArgs e)
        {
            new GiaoDichBanHang().Show();
            this.Close();
        }

        private void menuThongKe_Click(object sender, EventArgs e) { /* đã ở đây rồi */ }

        // ── Stub handlers ──────────────────────────────────────────
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void groupBox2_Enter(object sender, EventArgs e) { }
        private void textBox7_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}