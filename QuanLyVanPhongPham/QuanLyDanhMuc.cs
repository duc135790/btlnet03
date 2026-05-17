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
            button1.Click += button1_Click;
            LoadDanhSachDanhMuc();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dataGridView1.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            DataRowView drv = row.DataBoundItem as DataRowView;
            if (drv == null) return;

            _idDanhMuc = Convert.ToInt32(drv["MaDanhMuc"]);
            textBox1.Text = drv["MaDanhMuc"].ToString();
            textBox4.Text = drv["TenDanhMuc"].ToString();
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1_CellClick(sender, e);
        }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            { MessageBox.Show("Vui lòng nhập tên danh mục!"); return; }
            dmManager.Them(textBox4.Text.Trim());
            MessageBox.Show("Thêm danh mục thành công!");
            LamMoi();
        }
        private void btnSuaSP_Click(object sender, EventArgs e)
        {
            if (_idDanhMuc == 0) { MessageBox.Show("Vui lòng chọn danh mục cần sửa!"); return; }
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            { MessageBox.Show("Vui lòng nhập tên danh mục!"); return; }
            dmManager.Sua(_idDanhMuc, textBox4.Text.Trim());
            MessageBox.Show("Sửa danh mục thành công!");
            LamMoi();
        }
        private void btnXoaSP_Click(object sender, EventArgs e)
        {
            if (_idDanhMuc == 0) { MessageBox.Show("Vui lòng chọn danh mục cần xóa!"); return; }
            if (MessageBox.Show("Xóa danh mục này sẽ ảnh hưởng đến sản phẩm liên quan. Tiếp tục?",
                "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                dmManager.Xoa(_idDanhMuc);
                MessageBox.Show("Xóa danh mục thành công!");
                LamMoi();
            }
        }
       
        private void menuSanPham_Click(object sender, EventArgs e) => AppContext.NavTo(this, new QuanLySanPham());
        private void menuDanhMuc_Click(object sender, EventArgs e) { }
        private void menuGiaoDich_Click(object sender, EventArgs e) => AppContext.NavTo(this, new GiaoDichBanHang());
        private void menuThongKe_Click(object sender, EventArgs e) => AppContext.NavTo(this, new ThongKe());
        private void LoadDanhSachDanhMuc()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = dmManager.GetDanhSach();
        }
        private void LamMoi()
        {
            textBox4.Text = "";
            textBox1.Text = "";
            _idDanhMuc = 0;
            LoadDanhSachDanhMuc();
        }

        private void button1_Click(object sender, EventArgs e) => LamMoi();
    }
}