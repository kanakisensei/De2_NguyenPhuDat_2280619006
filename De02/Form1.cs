using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace De02
{

    public partial class frmSanpham : Form
    {
        string connectionString = @"Data Source=.;Initial Catalog=QLSanpham;Integrated Security=True";

        private bool isAdding = false; // Đang thêm mới
        private bool isEditing = false; // Đang sửa
        public frmSanpham()
        {
            InitializeComponent();
        }

        private void dgvDanhmucsanpham_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }



        private void LoadDanhMucSanPham()
        {
            // Xóa dữ liệu cũ trong ListView
            lvSanpham.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Câu lệnh SQL để lấy tất cả dữ liệu từ bảng Sanpham
                    string query = "SELECT MaSP, TenSP, Ngaynhap, LoaiSP FROM Sanpham";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Duyệt qua từng dòng dữ liệu trả về
                    while (reader.Read())
                    {
                        // Tạo một ListViewItem mới
                        ListViewItem item = new ListViewItem(reader["MaSP"].ToString());
                        item.SubItems.Add(reader["TenSP"].ToString());
                        item.SubItems.Add(Convert.ToDateTime(reader["Ngaynhap"]).ToString("dd/MM/yyyy"));
                        item.SubItems.Add(reader["LoaiSP"].ToString());

                        // Thêm dòng vào ListView
                        lvSanpham.Items.Add(item);



                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void frmSanpham_Load(object sender, EventArgs e)
        { }
        private void ResetButtons()
        {
            // Đặt lại trạng thái của các nút
            btnThem.Enabled = true;
            btnSua.Enabled = true;
            btnXoa.Enabled = true;
            btnLuu.Enabled = false;   // Vô hiệu hóa nút Lưu khi không cần thiết
            btnKLuu.Enabled = false;  // Vô hiệu hóa nút KLưu khi không cần thiết

            // Đảm bảo ListView có thể chọn lại các dòng
            lvSanpham.Enabled = true;

            // Đặt lại giá trị của các điều khiển nhập liệu
            txtMaSP.Clear();
            txtTenSP.Clear();
            cboLoaiSP.SelectedIndex = -1;  // Bỏ chọn loại sản phẩm
            dtNgaynhap.Value = DateTime.Now;  // Đặt lại ngày nhập là ngày hiện tại
        }





        private void btnThem_Click(object sender, EventArgs e)
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            dtNgaynhap.Value = DateTime.Now;
            cboLoaiSP.SelectedIndex = -1;

            txtMaSP.Focus();
            isAdding = true;
            isEditing = false;

            // Vô hiệu hóa ListView và các nút không cần thiết
            lvSanpham.Enabled = false;
            btnSua.Enabled = false;
            btnXoa.Enabled = false;
            btnLuu.Enabled = true;
            btnKLuu.Enabled = true;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (lvSanpham.SelectedItems.Count > 0)
            {
                isEditing = true;
                isAdding = false;

                // Lấy dữ liệu dòng được chọn đưa lên các TextBox
                ListViewItem selectedItem = lvSanpham.SelectedItems[0];
                txtMaSP.Text = selectedItem.SubItems[0].Text;
                txtTenSP.Text = selectedItem.SubItems[1].Text;
                dtNgaynhap.Value = Convert.ToDateTime(selectedItem.SubItems[2].Text);
                cboLoaiSP.Text = selectedItem.SubItems[3].Text;

                txtMaSP.Focus();

                // Vô hiệu hóa ListView và các nút không cần thiết
                lvSanpham.Enabled = false;
                btnThem.Enabled = false;
                btnXoa.Enabled = false;
                btnLuu.Enabled = true;
                btnKLuu.Enabled = true;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (lvSanpham.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Lấy mã sản phẩm của dòng được chọn
                    string maSP = lvSanpham.SelectedItems[0].SubItems[0].Text;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            string query = "DELETE FROM Sanpham WHERE MaSP = @MaSP";
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@MaSP", maSP);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Xóa thành công!", "Thông báo");
                            LoadDanhMucSanPham(); // Reload dữ liệu
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn dòng cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd;

                    if (isAdding) // Thêm mới
                    {
                        string query = "INSERT INTO Sanpham (MaSP, TenSP, Ngaynhap, LoaiSP) VALUES (@MaSP, @TenSP, @Ngaynhap, @LoaiSP)";
                        cmd = new SqlCommand(query, conn);
                    }
                    else if (isEditing) // Sửa
                    {
                        string query = "UPDATE Sanpham SET TenSP = @TenSP, Ngaynhap = @Ngaynhap, LoaiSP = @LoaiSP WHERE MaSP = @MaSP";
                        cmd = new SqlCommand(query, conn);
                    }
                    else return;

                    // Thêm các tham số
                    cmd.Parameters.AddWithValue("@MaSP", txtMaSP.Text);
                    cmd.Parameters.AddWithValue("@TenSP", txtTenSP.Text);
                    cmd.Parameters.AddWithValue("@Ngaynhap", dtNgaynhap.Value);
                    cmd.Parameters.AddWithValue("@LoaiSP", cboLoaiSP.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Lưu thành công!", "Thông báo");

                    // Reset trạng thái và reload dữ liệu
                    LoadDanhMucSanPham();
                    lvSanpham.Enabled = true;
                    ResetButtons();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void AddSanPham(string maSP, string tenSP, DateTime ngayNhap, string loaiSP)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO Sanpham (MaSP, TenSP, Ngaynhap, LoaiSP) VALUES (@MaSP, @TenSP, @Ngaynhap, @LoaiSP)";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@MaSP", maSP);
                    cmd.Parameters.AddWithValue("@TenSP", tenSP);
                    cmd.Parameters.AddWithValue("@Ngaynhap", ngayNhap);
                    cmd.Parameters.AddWithValue("@LoaiSP", loaiSP);

                    cmd.ExecuteNonQuery(); // Thực thi câu lệnh
                    MessageBox.Show("Thêm sản phẩm thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void UpdateSanPham(string maSP, string tenSP, DateTime ngayNhap, string loaiSP)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE Sanpham SET TenSP = @TenSP, Ngaynhap = @Ngaynhap, LoaiSP = @LoaiSP WHERE MaSP = @MaSP";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@MaSP", maSP);
                    cmd.Parameters.AddWithValue("@TenSP", tenSP);
                    cmd.Parameters.AddWithValue("@Ngaynhap", ngayNhap);
                    cmd.Parameters.AddWithValue("@LoaiSP", loaiSP);

                    cmd.ExecuteNonQuery(); // Thực thi câu lệnh
                    MessageBox.Show("Cập nhật sản phẩm thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string keyword = txtTim.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                // Nếu không nhập từ khóa, tải lại danh sách đầy đủ
                LoadDanhMucSanPham();
                return;
            }

            // Xóa dữ liệu cũ trong ListView
            lvSanpham.Items.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Câu lệnh SQL tìm kiếm sản phẩm theo từ khóa
                    string query = "SELECT MaSP, TenSP, Ngaynhap, LoaiSP FROM Sanpham " +
                                   "WHERE MaSP LIKE @Keyword OR TenSP LIKE @Keyword";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                    SqlDataReader reader = cmd.ExecuteReader();

                    // Duyệt qua từng dòng dữ liệu trả về
                    while (reader.Read())
                    {
                        // Tạo một ListViewItem mới
                        ListViewItem item = new ListViewItem(reader["MaSP"].ToString());
                        item.SubItems.Add(reader["TenSP"].ToString());
                        item.SubItems.Add(Convert.ToDateTime(reader["Ngaynhap"]).ToString("dd/MM/yyyy"));
                        item.SubItems.Add(reader["LoaiSP"].ToString());

                        // Thêm dòng vào ListView
                        lvSanpham.Items.Add(item);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cboLoaiSP_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

