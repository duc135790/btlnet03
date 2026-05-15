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
        private SanPhamManager spManager = new SanPhamManager();

        public GiaoDichBanHang()
        {
            InitializeComponent();
        }

        private void GiaoDichBanHang_Load(object sender, EventArgs e)
        {
            LoadDanhSachGiaoDich();
            LoadComboSanPham();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[] { "Hoàn thành", "Đang xử lý", "Đã hủy" });
            comboBox1.SelectedIndex = 0;
            cboDanhMuc.Items.Clear();
            cboDanhMuc.Items.AddRange(new string[] { "Tiền mặt", "Chuyển khoản", "Thẻ tín dụng" });
            cboDanhMuc.SelectedIndex = 0;
        }

        private void LoadDanhSachGiaoDich()
        {
            dataGridView1.DataSource = gdManager.GetDanhSach();
        }

        private void LoadComboSanPham()
        {
            using (SqlConnection conn = new SqlConnection(DatabaseConnection.GetConnStr()))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "select MaSanPham, TenSanPham, GiaBan from SanPham order by TenSanPham", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<SanPham> list = new List<SanPham>();
                foreach (DataRow row in dt.Rows)
                    list.Add(new SanPham(
                        Convert.ToInt32(row["MaSanPham"]),
                        row["TenSanPham"].ToString(),
                        Convert.ToInt32(row["GiaBan"]), 0, 0));
                this.Tag = list;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string keyword = textBox1.Text.Trim();
            string sql = "select MaGiaoDich, NgayGiaoDich, NguoiThucHien, TenSanPham, SoLuongBan, TongTien " +
                         "from GiaoDich where CAST(MaGiaoDich as nvarchar) like N'%" + keyword +
                         "%' or NguoiThucHien like N'%" + keyword + "%' " +
                         "order by MaGiaoDich desc";
            dataGridView1.DataSource = DatabaseConnection.ExecuteQuery(sql);
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            { MessageBox.Show("Vui lòng nhập tên Khách Hàng!"); return; }

            using (var dlg = new GiaoDichInputDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    gdManager.Them(textBox4.Text.Trim(),
                                   dlg.TenSanPham, dlg.SoLuong, dlg.TongTien);
                    MessageBox.Show("Tạo giao dịch thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LamMoi();
                }
            }
        }

        private void btnSuaSP_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng sửa giao dịch không được hỗ trợ.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnXoaSP_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng xóa giao dịch không được hỗ trợ.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void menuDanhMuc_Click(object sender, EventArgs e)
        {
            new QuanLyDanhMuc().Show();
            this.Close();
        }

        private void menuThongKe_Click(object sender, EventArgs e)
        {
            new ThongKe().Show();
            this.Close();
        }

        private void menuGiaoDich_Click(object sender, EventArgs e) { /* đã ở đây rồi */ }

        // ── Unused stubs ───────────────────────────────────────────
        private void label7_Click(object sender, EventArgs e) { }
        private void textBox7_TextChanged(object sender, EventArgs e) { }
        private void groupBox2_Enter(object sender, EventArgs e) { }
        private void cboDanhMuc_SelectedIndexChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        // ── Helpers ────────────────────────────────────────────────
        private void LamMoi()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox4.Text = "";
            textBox7.Text = "";
            LoadDanhSachGiaoDich();
        }
    }

    // ── Dialog nhập nhanh thông tin giao dịch ─────────────────────
    public class GiaoDichInputDialog : Form
    {
        public string TenSanPham { get; private set; }
        public int SoLuong { get; private set; }
        public int TongTien { get; private set; }

        private ComboBox cboSP = new ComboBox();
        private TextBox txtSL = new TextBox();
        private Label lblTong = new Label();
        private Button btnOK = new Button();
        private Button btnCancel = new Button();
        private List<SanPham> _listSP = new List<SanPham>();

        public GiaoDichInputDialog()
        {
            this.Text = "Chọn Sản Phẩm & Số Lượng";
            this.Width = 400; this.Height = 220;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false; this.MinimizeBox = false;

            var lblSP = new Label { Text = "Sản phẩm:", Left = 20, Top = 20, Width = 100 };
            cboSP.Left = 130; cboSP.Top = 16; cboSP.Width = 220;
            cboSP.DropDownStyle = ComboBoxStyle.DropDownList;

            var lblSL = new Label { Text = "Số lượng bán:", Left = 20, Top = 60, Width = 100 };
            txtSL.Left = 130; txtSL.Top = 56; txtSL.Width = 100;

            lblTong.Left = 20; lblTong.Top = 100; lblTong.Width = 330;
            lblTong.Text = "Tổng tiền: 0 đ";

            btnOK.Text = "Xác nhận"; btnOK.Left = 130; btnOK.Top = 140;
            btnOK.BackColor = System.Drawing.Color.LightGreen;
            btnOK.Click += BtnOK_Click;

            btnCancel.Text = "Hủy"; btnCancel.Left = 240; btnCancel.Top = 140;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { lblSP, cboSP, lblSL, txtSL, lblTong, btnOK, btnCancel });

            cboSP.SelectedIndexChanged += (s, e) => TinhTong();
            txtSL.TextChanged += (s, e) => TinhTong();

            LoadSanPham();
        }

        private void LoadSanPham()
        {
            using (var conn = new SqlConnection(DatabaseConnection.GetConnStr()))
            {
                var da = new SqlDataAdapter(
                    "select MaSanPham, TenSanPham, GiaBan, SoLuong from SanPham order by TenSanPham", conn);
                var dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                    _listSP.Add(new SanPham(
                        Convert.ToInt32(row["MaSanPham"]),
                        row["TenSanPham"].ToString(),
                        Convert.ToInt32(row["GiaBan"]),
                        Convert.ToInt32(row["SoLuong"]), 0));
                cboSP.DataSource = _listSP;
            }
        }

        private void TinhTong()
        {
            var sp = cboSP.SelectedItem as SanPham;
            if (sp == null) return;
            if (int.TryParse(txtSL.Text, out int sl))
                lblTong.Text = "Tổng tiền: " + string.Format("{0:N0}", (long)sp.GiaBan * sl) + " đ";
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            var sp = cboSP.SelectedItem as SanPham;
            if (sp == null) { MessageBox.Show("Vui lòng chọn sản phẩm!"); return; }
            if (!int.TryParse(txtSL.Text, out int sl) || sl <= 0)
            { MessageBox.Show("Số lượng phải là số nguyên dương!"); return; }
            if (sl > sp.SoLuong)
            { MessageBox.Show($"Số lượng tồn kho chỉ còn {sp.SoLuong}!"); return; }
            TenSanPham = sp.TenSanPham;
            SoLuong = sl;
            TongTien = sp.GiaBan * sl;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}