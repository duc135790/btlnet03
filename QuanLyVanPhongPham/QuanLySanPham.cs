using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuanLyVanPhongPham
{
    public partial class QuanLySanPham : Form
    {
        private int _idSanPham = 0;
        private int _idDanhMuc = 0;
        private SanPhamManager spManager = new SanPhamManager();

        public QuanLySanPham()
        {
            InitializeComponent();
        }

        private void QuanLySanPham_Load(object sender, EventArgs e)
        {
            // Wire nút Làm mới (button2 chưa được wire trong designer)
            button2.Click += btnLamMoiSP_Click;

            LoadDanhSachSanPham();
            LoadComboDanhMuc();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = spManager.TimKiem(textBox1.Text.Trim(), "");
        }

        private void btnLamMoiSP_Click(object sender, EventArgs e) => LamMoi();

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dataGridView1.Rows.Count > 0 && dataGridView1.CurrentRow != null)
            {
                // Đọc MaSanPham từ DataBoundItem thay vì Cells[] để tránh lỗi tên column
                DataRowView drv = dataGridView1.CurrentRow.DataBoundItem as DataRowView;
                if (drv == null) return;

                _idSanPham = Convert.ToInt32(drv["MaSanPham"]);
                _idDanhMuc = Convert.ToInt32(drv["MaDanhMuc"] == DBNull.Value ? 0 : drv["MaDanhMuc"]);

                textBox2.Text = drv["MaSanPham"].ToString();
                textBox4.Text = drv["TenSanPham"].ToString();
                textBox3.Text = drv["GiaBan"].ToString();
                textBox5.Text = drv["SoLuong"].ToString();

                // Load combo và chọn đúng danh mục
                LoadComboDanhMuc();
            }
        }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            if (!KiemTraInput()) return;
            var dm = cboDanhMuc.SelectedItem as DanhMuc;
            if (dm == null) { MessageBox.Show("Vui lòng chọn danh mục!"); return; }
            spManager.Them(textBox4.Text.Trim(),
                           int.Parse(textBox3.Text),
                           int.Parse(textBox5.Text),
                           dm.MaDanhMuc);
            MessageBox.Show("Thêm sản phẩm thành công!");
            LamMoi();
        }

        private void btnSuaSP_Click(object sender, EventArgs e)
        {
            if (_idSanPham == 0) { MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!"); return; }
            if (!KiemTraInput()) return;
            var dm = cboDanhMuc.SelectedItem as DanhMuc;
            if (dm == null) { MessageBox.Show("Vui lòng chọn danh mục!"); return; }
            spManager.Sua(_idSanPham,
                          textBox4.Text.Trim(),
                          int.Parse(textBox3.Text),
                          int.Parse(textBox5.Text),
                          dm.MaDanhMuc);
            MessageBox.Show("Sửa sản phẩm thành công!");
            LamMoi();
        }

        private void btnXoaSP_Click(object sender, EventArgs e)
        {
            if (_idSanPham == 0) { MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!"); return; }
            if (MessageBox.Show("Bạn có chắc chắn xóa sản phẩm này không?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                spManager.Xoa(_idSanPham);
                MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoi();
            }
        }

        private void menuSanPham_Click(object sender, EventArgs e) { }
        private void menuDanhMuc_Click(object sender, EventArgs e) => AppContext.NavTo(this, new QuanLyDanhMuc());
        private void menuGiaoDich_Click(object sender, EventArgs e) => AppContext.NavTo(this, new GiaoDichBanHang());
        private void menuThongKe_Click(object sender, EventArgs e) => AppContext.NavTo(this, new ThongKe());

        private void LoadDanhSachSanPham()
        {
            // Cần kéo thêm MaDanhMuc để đọc được khi chọn hàng
            DataTable dt = DatabaseConnection.ExecuteQuery(
                "SELECT sp.MaSanPham, sp.MaDanhMuc, dm.TenDanhMuc, sp.TenSanPham, sp.GiaBan, sp.SoLuong " +
                "FROM SanPham sp " +
                "JOIN DanhMuc dm ON sp.MaDanhMuc = dm.MaDanhMuc " +
                "ORDER BY sp.MaSanPham DESC");
            dataGridView1.DataSource = dt;
        }

        private void LoadComboDanhMuc()
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.GetConnStr()))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT MaDanhMuc, TenDanhMuc FROM DanhMuc ORDER BY MaDanhMuc ASC", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<DanhMuc> list = new List<DanhMuc>();
                int selIdx = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int maDM = Convert.ToInt32(dt.Rows[i]["MaDanhMuc"]);
                    list.Add(new DanhMuc(maDM, dt.Rows[i]["TenDanhMuc"].ToString()));
                    if (_idDanhMuc != 0 && _idDanhMuc == maDM) selIdx = i;
                }
                cboDanhMuc.DataSource = list;
                if (list.Count > 0) cboDanhMuc.SelectedIndex = selIdx;
            }
        }

        private void LamMoi()
        {
            textBox1.Text = textBox2.Text = textBox3.Text =
            textBox4.Text = textBox5.Text = "";
            _idSanPham = 0;
            _idDanhMuc = 0;
            LoadDanhSachSanPham();
            LoadComboDanhMuc();
        }

        private bool KiemTraInput()
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            { MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (!int.TryParse(textBox3.Text, out int gia) || gia < 0)
            { MessageBox.Show("Giá bán phải là số nguyên dương!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (!int.TryParse(textBox5.Text, out int sl) || sl < 0)
            { MessageBox.Show("Số lượng phải là số nguyên dương!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            return true;
        }

        // Stubs
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void cboDanhMuc_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void groupBox2_Enter(object sender, EventArgs e) { }
        private void btnSuaSP_Click_1(object sender, EventArgs e) => btnSuaSP_Click(sender, e);
    }
}