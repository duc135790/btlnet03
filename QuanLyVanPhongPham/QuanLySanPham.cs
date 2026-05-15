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
        private DanhMucManager dmManager = new DanhMucManager();

        public QuanLySanPham()
        {
            InitializeComponent();
        }

        private void QuanLySanPham_Load(object sender, EventArgs e)
        {
            LoadDanhSachSanPham();
            LoadComboDanhMuc();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            string tenSP = textBox1.Text.Trim();
            dataGridView1.DataSource = spManager.TimKiem(tenSP, "");
        }

        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void cboDanhMuc_SelectedIndexChanged(object sender, EventArgs e) { }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0 && dataGridView1.CurrentRow != null)
            {
                _idSanPham = Convert.ToInt32(dataGridView1.CurrentRow.Cells["MaSanPham"].Value);
                using (SqlConnection conn = new SqlConnection(DatabaseConnection.GetConnStr()))
                {
                    SqlDataAdapter da = new SqlDataAdapter(
                        "select MaSanPham, MaDanhMuc, TenSanPham, GiaBan, SoLuong " +
                        "from SanPham where MaSanPham=" + _idSanPham, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        textBox2.Text = dt.Rows[0]["MaSanPham"].ToString();
                        textBox4.Text = dt.Rows[0]["TenSanPham"].ToString();
                        textBox3.Text = dt.Rows[0]["GiaBan"].ToString();
                        textBox5.Text = dt.Rows[0]["SoLuong"].ToString();
                        _idDanhMuc = Convert.ToInt32(dt.Rows[0]["MaDanhMuc"]);
                        LoadComboDanhMuc();
                    }
                }
            }
        }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            if (!KiemTraInput()) return;
            var dm = cboDanhMuc.SelectedItem as DanhMuc;
            spManager.Them(textBox4.Text.Trim(),
                           int.Parse(textBox3.Text),
                           int.Parse(textBox5.Text),
                           dm.MaDanhMuc);
            MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoi();
        }

        private void btnSuaSP_Click(object sender, EventArgs e)
        {
            if (_idSanPham == 0) { MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!"); return; }
            if (!KiemTraInput()) return;
            var dm = cboDanhMuc.SelectedItem as DanhMuc;
            spManager.Sua(_idSanPham, textBox4.Text.Trim(),
                          int.Parse(textBox3.Text),
                          int.Parse(textBox5.Text),
                          dm.MaDanhMuc);
            MessageBox.Show("Sửa sản phẩm thành công!", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LamMoi();
        }

        private void btnXoaSP_Click(object sender, EventArgs e)
        {
            if (_idSanPham == 0) { MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!"); return; }
            var cf = MessageBox.Show("Bạn có chắc chắn xóa sản phẩm này không?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (cf == DialogResult.Yes)
            {
                spManager.Xoa(_idSanPham);
                MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LamMoi();
            }
        }

        private void btnLamMoiSP_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        // ── Menu navigation (FIX: mở form mới trước, close sau) ───
        private void menuSanPham_Click(object sender, EventArgs e) { /* đã ở đây rồi */ }

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

        private void menuThongKe_Click(object sender, EventArgs e)
        {
            new ThongKe().Show();
            this.Close();
        }

        // ── Helpers ────────────────────────────────────────────────
        private void LoadDanhSachSanPham()
        {
            dataGridView1.DataSource = spManager.GetDanhSach();
        }

        private void LoadComboDanhMuc()
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.GetConnStr()))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "select MaDanhMuc, TenDanhMuc from DanhMuc order by MaDanhMuc desc", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<DanhMuc> list = new List<DanhMuc>();
                int selIdx = 0, i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new DanhMuc(Convert.ToInt32(row["MaDanhMuc"]),
                                         row["TenDanhMuc"].ToString()));
                    if (_idDanhMuc == Convert.ToInt32(row["MaDanhMuc"])) selIdx = i;
                    else i++;
                }
                cboDanhMuc.DataSource = list;
                cboDanhMuc.SelectedIndex = selIdx;
            }
        }

        private void LamMoi()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox7.Text = "";
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

        private void btnSuaSP_Click_1(object sender, EventArgs e) => btnSuaSP_Click(sender, e);
        private void groupBox2_Enter(object sender, EventArgs e) { }
    }
}