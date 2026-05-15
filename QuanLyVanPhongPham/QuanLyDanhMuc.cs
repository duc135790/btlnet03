using System;
using System.Data;
using System.Windows.Forms;

namespace QuanLyVanPhongPham
{
    public partial class QuanLyDanhMuc : Form
    {
        private int _idDanhMuc = 0;
        private DanhMucManager dmManager = new DanhMucManager();

        public QuanLyDanhMuc()
        {
            InitializeComponent();
        }

        private void QuanLyDanhMuc_Load(object sender, EventArgs e)
        {
            LoadDanhSachDanhMuc();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0 && dataGridView1.CurrentRow != null)
            {
                _idDanhMuc = Convert.ToInt32(dataGridView1.CurrentRow.Cells["MaDanhMuc"].Value);
                textBox2.Text = dataGridView1.CurrentRow.Cells["MaDanhMuc"].Value.ToString();
                textBox4.Text = dataGridView1.CurrentRow.Cells["TenDanhMuc"].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string keyword = textBox1.Text.Trim();
            string sql = "select MaDanhMuc, TenDanhMuc from DanhMuc " +
                         "where TenDanhMuc like N'%" + keyword + "%' " +
                         "order by MaDanhMuc desc";
            dataGridView1.DataSource = DatabaseConnection.ExecuteQuery(sql);
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            { MessageBox.Show("Vui lòng nhập tên danh mục!"); return; }
            dmManager.Them(textBox4.Text.Trim());
            MessageBox.Show("Thêm danh mục thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoi();
        }

        private void btnSuaSP_Click(object sender, EventArgs e)
        {
            if (_idDanhMuc == 0) { MessageBox.Show("Vui lòng chọn danh mục cần sửa!"); return; }
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            { MessageBox.Show("Vui lòng nhập tên danh mục!"); return; }
            dmManager.Sua(_idDanhMuc, textBox4.Text.Trim());
            MessageBox.Show("Sửa danh mục thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoi();
        }

        private void btnXoaSP_Click(object sender, EventArgs e)
        {
            if (_idDanhMuc == 0) { MessageBox.Show("Vui lòng chọn danh mục cần xóa!"); return; }
            var cf = MessageBox.Show("Xóa danh mục này sẽ ảnh hưởng đến sản phẩm liên quan. Tiếp tục?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (cf == DialogResult.Yes)
            {
                dmManager.Xoa(_idDanhMuc);
                MessageBox.Show("Xóa danh mục thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoi();
            }
        }

        private void btnLamMoiSP_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        // ── Menu navigation (FIX: mở form mới trước, close sau) ───
        private void menuSanPham_Click(object sender, EventArgs e)
        {
            new QuanLySanPham().Show();
            this.Close();
        }

        private void menuGiaoDich_Click(object sender, EventArgs e)
        {
            new GiaoDichBanHang().Show();
            this.Close();
        }

        private void menuThongKe_Click(object sender, EventArgs e)
        {
            new ThongKe().Show();
            this.Close();
        }

        private void menuDanhMuc_Click(object sender, EventArgs e) { /* đã ở đây rồi */ }

        // ── Unused stubs ───────────────────────────────────────────
        private void label7_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void textBox7_TextChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void cboDanhMuc_SelectedIndexChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void groupBox3_Enter(object sender, EventArgs e) { }
        private void LoadDanhSachDanhMuc()
        {
            dataGridView1.DataSource = dmManager.GetDanhSach();
        }

        private void LamMoi()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox4.Text = "";
            textBox7.Text = "";
            _idDanhMuc = 0;
            LoadDanhSachDanhMuc();
        }
    }
}