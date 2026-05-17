using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace QuanLyVanPhongPham
{
    public partial class GiaoDichBanHang : Form
    {
        private GiaoDichManager gdManager = new GiaoDichManager();
        private int _idGiaoDich = 0;

        public GiaoDichBanHang()
        {
            InitializeComponent();
        }
        private void GiaoDichBanHang_Load(object sender, EventArgs e)
        {
            cboDanhMuc.Items.Clear();
            cboDanhMuc.Items.AddRange(new string[] { "Tiền mặt", "Chuyển khoản" });
            cboDanhMuc.SelectedIndex = 0;
            dateTimePicker1.Value = DateTime.Now;
            label1.Text = "Khách Hàng Hoặc Mã Đơn Hàng";
            button2.Click += button2_Click_1;
            if (dataGridView1.Columns["Column6"] != null)
                dataGridView1.Columns["Column6"].DataPropertyName = "SoLuongBan";
            dataGridView1.CellClick += dataGridView1_CellClick;
            btnSuaSP.Click += btnSuaSP_Click;
            btnXoaSP.Click += btnXoaSP_Click;
            LoadDanhSachGiaoDich();
        }
        private void LoadDanhSachGiaoDich()
        {
            dataGridView1.DataSource = DatabaseConnection.ExecuteQuery(
                "SELECT TenSanPham, MaGiaoDich, KhachHang, NgayGiaoDich, PhuongThuc, Gia, SoLuongBan " +
                "FROM GiaoDich ORDER BY MaGiaoDich DESC");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string keyword = textBox1.Text.Trim();
            dataGridView1.DataSource = DatabaseConnection.ExecuteQuery(
                "SELECT TenSanPham, MaGiaoDich, KhachHang, NgayGiaoDich, PhuongThuc, Gia, SoLuongBan " +
                "FROM GiaoDich " +
                "WHERE KhachHang LIKE N'%" + keyword + "%' " +
                "   OR CAST(MaGiaoDich AS NVARCHAR) LIKE N'%" + keyword + "%' " +
                "ORDER BY MaGiaoDich DESC");
        }
        private void btnThemSP_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            { MessageBox.Show("Vui lòng nhập Tên Sản Phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox5.Focus(); return; }
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            { MessageBox.Show("Vui lòng nhập tên Khách Hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox4.Focus(); return; }
            if (!long.TryParse(textBox2.Text.Replace(",", "").Replace(".", ""), out long gia) || gia < 0)
            { MessageBox.Show("Giá phải là số hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox2.Focus(); return; }
            if (!int.TryParse(textBox3.Text, out int soLuong) || soLuong <= 0)
            { MessageBox.Show("Số lượng phải là số nguyên dương!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); textBox3.Focus(); return; }
            try
            {
                gdManager.Them(textBox5.Text.Trim(), textBox4.Text.Trim(),
                               cboDanhMuc.SelectedItem?.ToString() ?? "Tiền mặt",
                               dateTimePicker1.Value, gia, soLuong);
                MessageBox.Show("Tạo giao dịch thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dataGridView1.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            DataRowView drv = row.DataBoundItem as DataRowView;
            if (drv == null) return;
            _idGiaoDich = Convert.ToInt32(drv["MaGiaoDich"]);
            textBox6.Text = drv["MaGiaoDich"]?.ToString() ?? "";
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
        private void menuSanPham_Click(object sender, EventArgs e) => AppContext.NavTo(this, new QuanLySanPham());
        private void menuDanhMuc_Click(object sender, EventArgs e) => AppContext.NavTo(this, new QuanLyDanhMuc());
        private void menuGiaoDich_Click(object sender, EventArgs e) { }
        private void menuThongKe_Click(object sender, EventArgs e) => AppContext.NavTo(this, new ThongKe());
        private void LamMoi()
        {
            _idGiaoDich = 0;
            textBox1.Text = "";
            textBox6.Text = "";
            textBox5.Text = "";
            textBox4.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            cboDanhMuc.SelectedIndex = 0;
            dateTimePicker1.Value = DateTime.Now;
            LoadDanhSachGiaoDich();
        }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void cboDanhMuc_SelectedIndexChanged(object sender, EventArgs e) { }
        private void groupBox2_Enter(object sender, EventArgs e) { }

        private void button2_Click_1(object sender, EventArgs e) => LamMoi();

        private void btnSuaSP_Click(object sender, EventArgs e)
        {
            if (_idGiaoDich == 0) { MessageBox.Show("Vui lòng chọn giao dịch cần sửa!"); return; }
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            { MessageBox.Show("Vui lòng nhập Tên Sản Phẩm!"); return; }
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            { MessageBox.Show("Vui lòng nhập tên Khách Hàng!"); return; }
            if (!long.TryParse(textBox2.Text.Replace(",", "").Replace(".", ""), out long gia) || gia < 0)
            { MessageBox.Show("Giá phải là số hợp lệ!"); return; }
            if (!int.TryParse(textBox3.Text, out int soLuong) || soLuong <= 0)
            { MessageBox.Show("Số lượng phải là số nguyên dương!"); return; }
            try
            {
                gdManager.Sua(_idGiaoDich,
                              textBox5.Text.Trim(),
                              textBox4.Text.Trim(),
                              cboDanhMuc.SelectedItem?.ToString() ?? "Tiền mặt",
                              dateTimePicker1.Value,
                              gia, soLuong);
                MessageBox.Show("Sửa giao dịch thành công!");
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoaSP_Click(object sender, EventArgs e)
        {
            if (_idGiaoDich == 0) { MessageBox.Show("Vui lòng chọn giao dịch cần xóa!"); return; }
            if (MessageBox.Show("Xóa giao dịch này? (Tồn kho sẽ được hoàn lại)",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    gdManager.Xoa(_idGiaoDich);
                    MessageBox.Show("Xóa giao dịch thành công!");
                    LamMoi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }
    }
}