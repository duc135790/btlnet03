using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuanLyVanPhongPham
{
    public partial class GiaoDichBanHang : Form
    {
        private GiaoDichManager gdManager = new GiaoDichManager();

        public GiaoDichBanHang()
        {
            InitializeComponent();
        }

        private void GiaoDichBanHang_Load(object sender, EventArgs e)
        {
            // cboDanhMuc = Phương Thức thanh toán
            cboDanhMuc.Items.Clear();
            cboDanhMuc.Items.AddRange(new string[] { "Tiền mặt", "Chuyển khoản", "Thẻ tín dụng" });
            cboDanhMuc.SelectedIndex = 0;

            // dateTimePicker1 = Ngày Giao Dịch, mặc định hôm nay
            dateTimePicker1.Value = DateTime.Now;

            // Đổi label tìm kiếm cho rõ hơn
            label1.Text = "Tìm theo Khách Hàng / Sản Phẩm:";

            // Wire nút Làm mới (button2 chưa được wire trong designer)
            button2.Click += button2_Click;

            // Fix Column6: DataPropertyName phải là SoLuongBan (không phải SoLuong)
            if (dataGridView1.Columns["Column6"] != null)
                dataGridView1.Columns["Column6"].DataPropertyName = "SoLuongBan";

            // Wire sự kiện click trên grid để điền form
            dataGridView1.CellClick += dataGridView1_CellClick;

            LoadDanhSachGiaoDich();
        }

        private void LoadDanhSachGiaoDich()
        {
            dataGridView1.DataSource = gdManager.GetDanhSach();
        }

        // ── Nút Tìm (button1) — tìm theo Khách Hàng và Tên Sản Phẩm ─────────
        private void button1_Click(object sender, EventArgs e)
        {
            string keyword = textBox1.Text.Trim();
            dataGridView1.DataSource = DatabaseConnection.ExecuteQuery(
                "SELECT MaGiaoDich, TenSanPham, KhachHang, NgayGiaoDich, PhuongThuc, Gia, SoLuongBan " +
                "FROM GiaoDich " +
                "WHERE KhachHang   LIKE N'%" + keyword + "%' " +
                "   OR TenSanPham  LIKE N'%" + keyword + "%' " +
                "   OR CAST(MaGiaoDich AS NVARCHAR) LIKE N'%" + keyword + "%' " +
                "ORDER BY MaGiaoDich DESC");
        }

        // ── Nút Làm mới (button2) ─────────────────────────────────────────────
        private void button2_Click(object sender, EventArgs e) => LamMoi();

        // ── Nút Bán (btnThemSP) ───────────────────────────────────────────────
        private void btnThemSP_Click_1(object sender, EventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên Sản Phẩm!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Vui lòng nhập tên Khách Hàng!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Focus();
                return;
            }
            if (!long.TryParse(textBox2.Text.Replace(",", "").Replace(".", ""), out long gia) || gia < 0)
            {
                MessageBox.Show("Giá phải là số hợp lệ!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }
            if (!int.TryParse(textBox3.Text, out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên dương!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }

            string tenSP = textBox5.Text.Trim();
            string khachHang = textBox4.Text.Trim();
            string phuongThuc = cboDanhMuc.SelectedItem?.ToString() ?? "Tiền mặt";
            DateTime ngayGD = dateTimePicker1.Value;

            try
            {
                gdManager.Them(tenSP, khachHang, phuongThuc, ngayGD, gia, soLuong);
                MessageBox.Show("Tạo giao dịch thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo giao dịch: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Khi click hàng trên DataGridView — đọc từ DataBoundItem ──────────
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dataGridView1.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            // Đọc qua DataBoundItem để tránh lỗi tên column
            DataRowView drv = row.DataBoundItem as DataRowView;
            if (drv != null)
            {
                textBox5.Text = drv["TenSanPham"]?.ToString() ?? "";
                textBox4.Text = drv["KhachHang"]?.ToString() ?? "";
                textBox2.Text = drv["Gia"]?.ToString() ?? "";
                textBox3.Text = drv["SoLuongBan"]?.ToString() ?? "";

                object pt = drv["PhuongThuc"];
                if (pt != null && pt != DBNull.Value)
                {
                    int idx = cboDanhMuc.FindStringExact(pt.ToString());
                    if (idx >= 0) cboDanhMuc.SelectedIndex = idx;
                }

                object ngay = drv["NgayGiaoDich"];
                if (ngay is DateTime dt)
                    dateTimePicker1.Value = dt;
            }
        }

        // ── Menu navigation ───────────────────────────────────────────────────
        private void menuSanPham_Click(object sender, EventArgs e) => AppContext.NavTo(this, new QuanLySanPham());
        private void menuDanhMuc_Click(object sender, EventArgs e) => AppContext.NavTo(this, new QuanLyDanhMuc());
        private void menuGiaoDich_Click(object sender, EventArgs e) { /* đang ở đây */ }
        private void menuThongKe_Click(object sender, EventArgs e) => AppContext.NavTo(this, new ThongKe());

        private void LamMoi()
        {
            textBox1.Text = "";   // ô tìm kiếm
            textBox5.Text = "";   // Tên Sản Phẩm
            textBox4.Text = "";   // Khách Hàng
            textBox2.Text = "";   // Giá
            textBox3.Text = "";   // Số Lượng
            cboDanhMuc.SelectedIndex = 0;
            dateTimePicker1.Value = DateTime.Now;
            LoadDanhSachGiaoDich();
        }

        // Stubs
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void cboDanhMuc_SelectedIndexChanged(object sender, EventArgs e) { }
        private void groupBox2_Enter(object sender, EventArgs e) { }
    }
}