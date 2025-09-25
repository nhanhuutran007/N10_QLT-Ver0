using Guna.UI2.WinForms;

namespace QLKDPhongTro.Views
{
    partial class frmFormApp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelMain = new Guna.UI2.WinForms.Guna2Panel();
            this.panelContent = new Guna.UI2.WinForms.Guna2Panel();
            this.dgvPhongTro = new Guna.UI2.WinForms.Guna2DataGridView();
            this.panelSearch = new Guna.UI2.WinForms.Guna2Panel();
            this.txtTimKiem = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblTimKiem = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.panelButtons = new Guna.UI2.WinForms.Guna2Panel();
            this.btnXoa = new Guna.UI2.WinForms.Guna2Button();
            this.btnXuatExcel = new Guna.UI2.WinForms.Guna2Button();
            this.btnCapNhat = new Guna.UI2.WinForms.Guna2Button();
            this.btnThem = new Guna.UI2.WinForms.Guna2Button();
            this.panelRoomInfo = new Guna.UI2.WinForms.Guna2Panel();
            this.txtMoTa = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblMoTa = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtTenThanhVien = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblTenThanhVien = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtTrangThai = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblTrangThai = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtGiaThue = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblGiaThue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtSoPhong = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblSoPhong = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtTang = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblTang = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtToaNha = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblToaNha = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtMaPhong = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblMaPhong = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.panelSidebar = new Guna.UI2.WinForms.Guna2Panel();
            this.lblFeedback = new System.Windows.Forms.LinkLabel();
            this.btnDangXuat = new Guna.UI2.WinForms.Guna2Button();
            this.btnDoiMatKhau = new Guna.UI2.WinForms.Guna2Button();
            this.btnThongKe = new Guna.UI2.WinForms.Guna2Button();
            this.btnHoaDon = new Guna.UI2.WinForms.Guna2Button();
            this.btnDienNuoc = new Guna.UI2.WinForms.Guna2Button();
            this.btnHopDong = new Guna.UI2.WinForms.Guna2Button();
            this.btnKhachThue = new Guna.UI2.WinForms.Guna2Button();
            this.btnPhongTro = new Guna.UI2.WinForms.Guna2Button();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.panelMain.SuspendLayout();
            this.panelContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhongTro)).BeginInit();
            this.panelSearch.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelRoomInfo.SuspendLayout();
            this.panelSidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelContent);
            this.panelMain.Controls.Add(this.panelSidebar);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1800, 1170);
            this.panelMain.TabIndex = 0;
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelContent.Controls.Add(this.dgvPhongTro);
            this.panelContent.Controls.Add(this.panelSearch);
            this.panelContent.Controls.Add(this.panelButtons);
            this.panelContent.Controls.Add(this.panelRoomInfo);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(369, 0);
            this.panelContent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1431, 1170);
            this.panelContent.TabIndex = 1;
            // 
            // dgvPhongTro
            // 
            this.dgvPhongTro.AllowUserToAddRows = false;
            this.dgvPhongTro.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvPhongTro.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPhongTro.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvPhongTro.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvPhongTro.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvPhongTro.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvPhongTro.Location = new System.Drawing.Point(8, 664);
            this.dgvPhongTro.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvPhongTro.Name = "dgvPhongTro";
            this.dgvPhongTro.ReadOnly = true;
            this.dgvPhongTro.RowHeadersVisible = false;
            this.dgvPhongTro.RowHeadersWidth = 62;
            this.dgvPhongTro.Size = new System.Drawing.Size(1410, 492);
            this.dgvPhongTro.TabIndex = 3;
            this.dgvPhongTro.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvPhongTro.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvPhongTro.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvPhongTro.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvPhongTro.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvPhongTro.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvPhongTro.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvPhongTro.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvPhongTro.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvPhongTro.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvPhongTro.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvPhongTro.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPhongTro.ThemeStyle.HeaderStyle.Height = 4;
            this.dgvPhongTro.ThemeStyle.ReadOnly = true;
            this.dgvPhongTro.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvPhongTro.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvPhongTro.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvPhongTro.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvPhongTro.ThemeStyle.RowsStyle.Height = 22;
            this.dgvPhongTro.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvPhongTro.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.Color.White;
            this.panelSearch.Controls.Add(this.txtTimKiem);
            this.panelSearch.Controls.Add(this.lblTimKiem);
            this.panelSearch.Location = new System.Drawing.Point(30, 564);
            this.panelSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(1320, 77);
            this.panelSearch.TabIndex = 2;
            // 
            // txtTimKiem
            // 
            this.txtTimKiem.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTimKiem.DefaultText = "";
            this.txtTimKiem.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimKiem.Location = new System.Drawing.Point(150, 23);
            this.txtTimKiem.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.PlaceholderText = "";
            this.txtTimKiem.SelectedText = "";
            this.txtTimKiem.Size = new System.Drawing.Size(444, 30);
            this.txtTimKiem.TabIndex = 1;
            // 
            // lblTimKiem
            // 
            this.lblTimKiem.BackColor = System.Drawing.Color.Transparent;
            this.lblTimKiem.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimKiem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblTimKiem.Location = new System.Drawing.Point(30, 23);
            this.lblTimKiem.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblTimKiem.Name = "lblTimKiem";
            this.lblTimKiem.Size = new System.Drawing.Size(92, 27);
            this.lblTimKiem.TabIndex = 0;
            this.lblTimKiem.Text = "Tìm Kiếm:";
            // 
            // panelButtons
            // 
            this.panelButtons.BackColor = System.Drawing.Color.White;
            this.panelButtons.Controls.Add(this.btnXoa);
            this.panelButtons.Controls.Add(this.btnXuatExcel);
            this.panelButtons.Controls.Add(this.btnCapNhat);
            this.panelButtons.Controls.Add(this.btnThem);
            this.panelButtons.Location = new System.Drawing.Point(1170, 92);
            this.panelButtons.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(248, 462);
            this.panelButtons.TabIndex = 1;
            // 
            // btnXoa
            // 
            this.btnXoa.BorderRadius = 5;
            this.btnXoa.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnXoa.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnXoa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXoa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnXoa.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnXoa.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXoa.ForeColor = System.Drawing.Color.White;
            this.btnXoa.Location = new System.Drawing.Point(15, 332);
            this.btnXoa.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.Size = new System.Drawing.Size(219, 62);
            this.btnXoa.TabIndex = 3;
            this.btnXoa.Text = "Xóa Phòng Trọ";
            // 
            // btnXuatExcel
            // 
            this.btnXuatExcel.BorderRadius = 5;
            this.btnXuatExcel.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnXuatExcel.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnXuatExcel.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXuatExcel.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnXuatExcel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnXuatExcel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnXuatExcel.ForeColor = System.Drawing.Color.White;
            this.btnXuatExcel.Location = new System.Drawing.Point(15, 235);
            this.btnXuatExcel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnXuatExcel.Name = "btnXuatExcel";
            this.btnXuatExcel.Size = new System.Drawing.Size(219, 62);
            this.btnXuatExcel.TabIndex = 2;
            this.btnXuatExcel.Text = "Xuất Excel";
            // 
            // btnCapNhat
            // 
            this.btnCapNhat.BorderRadius = 5;
            this.btnCapNhat.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCapNhat.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCapNhat.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCapNhat.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCapNhat.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnCapNhat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCapNhat.ForeColor = System.Drawing.Color.White;
            this.btnCapNhat.Location = new System.Drawing.Point(15, 138);
            this.btnCapNhat.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCapNhat.Name = "btnCapNhat";
            this.btnCapNhat.Size = new System.Drawing.Size(219, 62);
            this.btnCapNhat.TabIndex = 1;
            this.btnCapNhat.Text = "Cập nhật Phòng Trọ";
            // 
            // btnThem
            // 
            this.btnThem.BorderRadius = 5;
            this.btnThem.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnThem.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnThem.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnThem.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnThem.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnThem.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnThem.ForeColor = System.Drawing.Color.White;
            this.btnThem.Location = new System.Drawing.Point(15, 46);
            this.btnThem.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnThem.Name = "btnThem";
            this.btnThem.Size = new System.Drawing.Size(219, 62);
            this.btnThem.TabIndex = 0;
            this.btnThem.Text = "Thêm Phòng Trọ";
            // 
            // panelRoomInfo
            // 
            this.panelRoomInfo.BackColor = System.Drawing.Color.White;
            this.panelRoomInfo.Controls.Add(this.txtMoTa);
            this.panelRoomInfo.Controls.Add(this.lblMoTa);
            this.panelRoomInfo.Controls.Add(this.txtTenThanhVien);
            this.panelRoomInfo.Controls.Add(this.lblTenThanhVien);
            this.panelRoomInfo.Controls.Add(this.txtTrangThai);
            this.panelRoomInfo.Controls.Add(this.lblTrangThai);
            this.panelRoomInfo.Controls.Add(this.txtGiaThue);
            this.panelRoomInfo.Controls.Add(this.lblGiaThue);
            this.panelRoomInfo.Controls.Add(this.txtSoPhong);
            this.panelRoomInfo.Controls.Add(this.lblSoPhong);
            this.panelRoomInfo.Controls.Add(this.txtTang);
            this.panelRoomInfo.Controls.Add(this.lblTang);
            this.panelRoomInfo.Controls.Add(this.txtToaNha);
            this.panelRoomInfo.Controls.Add(this.lblToaNha);
            this.panelRoomInfo.Controls.Add(this.txtMaPhong);
            this.panelRoomInfo.Controls.Add(this.lblMaPhong);
            this.panelRoomInfo.Location = new System.Drawing.Point(30, 92);
            this.panelRoomInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelRoomInfo.Name = "panelRoomInfo";
            this.panelRoomInfo.Size = new System.Drawing.Size(1132, 462);
            this.panelRoomInfo.TabIndex = 0;
            // 
            // txtMoTa
            // 
            this.txtMoTa.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMoTa.DefaultText = "Phòng có máy giặt, gần công viên";
            this.txtMoTa.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMoTa.Location = new System.Drawing.Point(30, 354);
            this.txtMoTa.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.txtMoTa.Multiline = true;
            this.txtMoTa.Name = "txtMoTa";
            this.txtMoTa.PlaceholderText = "";
            this.txtMoTa.SelectedText = "";
            this.txtMoTa.Size = new System.Drawing.Size(1050, 77);
            this.txtMoTa.TabIndex = 15;
            // 
            // lblMoTa
            // 
            this.lblMoTa.BackColor = System.Drawing.Color.Transparent;
            this.lblMoTa.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMoTa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblMoTa.Location = new System.Drawing.Point(30, 323);
            this.lblMoTa.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblMoTa.Name = "lblMoTa";
            this.lblMoTa.Size = new System.Drawing.Size(58, 27);
            this.lblMoTa.TabIndex = 14;
            this.lblMoTa.Text = "Mô tả:";
            // 
            // txtTenThanhVien
            // 
            this.txtTenThanhVien.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTenThanhVien.DefaultText = "Phạm Minh Việt";
            this.txtTenThanhVien.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTenThanhVien.Location = new System.Drawing.Point(30, 262);
            this.txtTenThanhVien.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.txtTenThanhVien.Name = "txtTenThanhVien";
            this.txtTenThanhVien.PlaceholderText = "";
            this.txtTenThanhVien.SelectedText = "";
            this.txtTenThanhVien.Size = new System.Drawing.Size(300, 35);
            this.txtTenThanhVien.TabIndex = 13;
            // 
            // lblTenThanhVien
            // 
            this.lblTenThanhVien.BackColor = System.Drawing.Color.Transparent;
            this.lblTenThanhVien.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenThanhVien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblTenThanhVien.Location = new System.Drawing.Point(30, 231);
            this.lblTenThanhVien.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblTenThanhVien.Name = "lblTenThanhVien";
            this.lblTenThanhVien.Size = new System.Drawing.Size(199, 27);
            this.lblTenThanhVien.TabIndex = 12;
            this.lblTenThanhVien.Text = "Tên thành viên phòng:";
            // 
            // txtTrangThai
            // 
            this.txtTrangThai.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTrangThai.DefaultText = "Đang cho thuê";
            this.txtTrangThai.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTrangThai.Location = new System.Drawing.Point(469, 130);
            this.txtTrangThai.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.txtTrangThai.Name = "txtTrangThai";
            this.txtTrangThai.PlaceholderText = "";
            this.txtTrangThai.SelectedText = "";
            this.txtTrangThai.Size = new System.Drawing.Size(180, 35);
            this.txtTrangThai.TabIndex = 11;
            // 
            // lblTrangThai
            // 
            this.lblTrangThai.BackColor = System.Drawing.Color.Transparent;
            this.lblTrangThai.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrangThai.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblTrangThai.Location = new System.Drawing.Point(300, 138);
            this.lblTrangThai.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblTrangThai.Name = "lblTrangThai";
            this.lblTrangThai.Size = new System.Drawing.Size(157, 27);
            this.lblTrangThai.TabIndex = 10;
            this.lblTrangThai.Text = "Trạng thái phòng:";
            // 
            // txtGiaThue
            // 
            this.txtGiaThue.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtGiaThue.DefaultText = "4.600.000";
            this.txtGiaThue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGiaThue.Location = new System.Drawing.Point(30, 169);
            this.txtGiaThue.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.txtGiaThue.Name = "txtGiaThue";
            this.txtGiaThue.PlaceholderText = "";
            this.txtGiaThue.SelectedText = "";
            this.txtGiaThue.Size = new System.Drawing.Size(180, 35);
            this.txtGiaThue.TabIndex = 9;
            // 
            // lblGiaThue
            // 
            this.lblGiaThue.BackColor = System.Drawing.Color.Transparent;
            this.lblGiaThue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGiaThue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblGiaThue.Location = new System.Drawing.Point(30, 138);
            this.lblGiaThue.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblGiaThue.Name = "lblGiaThue";
            this.lblGiaThue.Size = new System.Drawing.Size(82, 27);
            this.lblGiaThue.TabIndex = 8;
            this.lblGiaThue.Text = "Giá thuê:";
            // 
            // txtSoPhong
            // 
            this.txtSoPhong.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSoPhong.DefaultText = "201";
            this.txtSoPhong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSoPhong.Location = new System.Drawing.Point(706, 46);
            this.txtSoPhong.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.txtSoPhong.Name = "txtSoPhong";
            this.txtSoPhong.PlaceholderText = "";
            this.txtSoPhong.SelectedText = "";
            this.txtSoPhong.Size = new System.Drawing.Size(120, 35);
            this.txtSoPhong.TabIndex = 7;
            // 
            // lblSoPhong
            // 
            this.lblSoPhong.BackColor = System.Drawing.Color.Transparent;
            this.lblSoPhong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoPhong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblSoPhong.Location = new System.Drawing.Point(600, 46);
            this.lblSoPhong.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblSoPhong.Name = "lblSoPhong";
            this.lblSoPhong.Size = new System.Drawing.Size(94, 27);
            this.lblSoPhong.TabIndex = 6;
            this.lblSoPhong.Text = "Số phòng:";
            // 
            // txtTang
            // 
            this.txtTang.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtTang.DefaultText = "2";
            this.txtTang.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTang.Location = new System.Drawing.Point(390, 77);
            this.txtTang.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.txtTang.Name = "txtTang";
            this.txtTang.PlaceholderText = "";
            this.txtTang.SelectedText = "";
            this.txtTang.Size = new System.Drawing.Size(75, 35);
            this.txtTang.TabIndex = 5;
            // 
            // lblTang
            // 
            this.lblTang.BackColor = System.Drawing.Color.Transparent;
            this.lblTang.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblTang.Location = new System.Drawing.Point(495, 46);
            this.lblTang.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblTang.Name = "lblTang";
            this.lblTang.Size = new System.Drawing.Size(55, 27);
            this.lblTang.TabIndex = 4;
            this.lblTang.Text = "Tầng:";
            // 
            // txtToaNha
            // 
            this.txtToaNha.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtToaNha.DefaultText = "Tòa A";
            this.txtToaNha.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtToaNha.Location = new System.Drawing.Point(240, 77);
            this.txtToaNha.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.txtToaNha.Name = "txtToaNha";
            this.txtToaNha.PlaceholderText = "";
            this.txtToaNha.SelectedText = "";
            this.txtToaNha.Size = new System.Drawing.Size(120, 35);
            this.txtToaNha.TabIndex = 3;
            // 
            // lblToaNha
            // 
            this.lblToaNha.BackColor = System.Drawing.Color.Transparent;
            this.lblToaNha.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToaNha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblToaNha.Location = new System.Drawing.Point(300, 46);
            this.lblToaNha.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblToaNha.Name = "lblToaNha";
            this.lblToaNha.Size = new System.Drawing.Size(82, 27);
            this.lblToaNha.TabIndex = 2;
            this.lblToaNha.Text = "Tòa nhà:";
            // 
            // txtMaPhong
            // 
            this.txtMaPhong.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMaPhong.DefaultText = "PT003";
            this.txtMaPhong.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtMaPhong.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtMaPhong.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMaPhong.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMaPhong.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMaPhong.ForeColor = System.Drawing.Color.Black;
            this.txtMaPhong.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMaPhong.Location = new System.Drawing.Point(30, 77);
            this.txtMaPhong.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.txtMaPhong.Name = "txtMaPhong";
            this.txtMaPhong.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtMaPhong.PlaceholderText = "";
            this.txtMaPhong.SelectedText = "";
            this.txtMaPhong.Size = new System.Drawing.Size(180, 46);
            this.txtMaPhong.TabIndex = 1;
            // 
            // lblMaPhong
            // 
            this.lblMaPhong.BackColor = System.Drawing.Color.Transparent;
            this.lblMaPhong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaPhong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblMaPhong.Location = new System.Drawing.Point(30, 46);
            this.lblMaPhong.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblMaPhong.Name = "lblMaPhong";
            this.lblMaPhong.Size = new System.Drawing.Size(124, 27);
            this.lblMaPhong.TabIndex = 0;
            this.lblMaPhong.Text = "Mã phòng trọ:";
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.SystemColors.HighlightText;
            this.panelSidebar.Controls.Add(this.lblFeedback);
            this.panelSidebar.Controls.Add(this.btnDangXuat);
            this.panelSidebar.Controls.Add(this.btnDoiMatKhau);
            this.panelSidebar.Controls.Add(this.btnThongKe);
            this.panelSidebar.Controls.Add(this.btnHoaDon);
            this.panelSidebar.Controls.Add(this.btnDienNuoc);
            this.panelSidebar.Controls.Add(this.btnHopDong);
            this.panelSidebar.Controls.Add(this.btnKhachThue);
            this.panelSidebar.Controls.Add(this.btnPhongTro);
            this.panelSidebar.Controls.Add(this.picLogo);
            this.panelSidebar.Controls.Add(this.lblTitle);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(369, 1170);
            this.panelSidebar.TabIndex = 0;
            this.panelSidebar.Paint += new System.Windows.Forms.PaintEventHandler(this.panelSidebar_Paint);
            // 
            // lblFeedback
            // 
            this.lblFeedback.AutoSize = true;
            this.lblFeedback.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblFeedback.LinkColor = System.Drawing.Color.LightBlue;
            this.lblFeedback.Location = new System.Drawing.Point(97, 1108);
            this.lblFeedback.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFeedback.Name = "lblFeedback";
            this.lblFeedback.Size = new System.Drawing.Size(143, 28);
            this.lblFeedback.TabIndex = 9;
            this.lblFeedback.TabStop = true;
            this.lblFeedback.Text = "Báo lỗi / Góp ý";
            // 
            // btnDangXuat
            // 
            this.btnDangXuat.BorderRadius = 5;
            this.btnDangXuat.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDangXuat.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDangXuat.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDangXuat.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDangXuat.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnDangXuat.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnDangXuat.ForeColor = System.Drawing.Color.White;
            this.btnDangXuat.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnDangXuat.Location = new System.Drawing.Point(34, 919);
            this.btnDangXuat.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDangXuat.Name = "btnDangXuat";
            this.btnDangXuat.Size = new System.Drawing.Size(282, 61);
            this.btnDangXuat.TabIndex = 8;
            this.btnDangXuat.Text = "ĐĂNG XUẤT";
            // 
            // btnDoiMatKhau
            // 
            this.btnDoiMatKhau.BorderRadius = 5;
            this.btnDoiMatKhau.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDoiMatKhau.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDoiMatKhau.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDoiMatKhau.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDoiMatKhau.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnDoiMatKhau.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnDoiMatKhau.ForeColor = System.Drawing.Color.White;
            this.btnDoiMatKhau.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnDoiMatKhau.Location = new System.Drawing.Point(34, 827);
            this.btnDoiMatKhau.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDoiMatKhau.Name = "btnDoiMatKhau";
            this.btnDoiMatKhau.Size = new System.Drawing.Size(282, 61);
            this.btnDoiMatKhau.TabIndex = 7;
            this.btnDoiMatKhau.Text = "ĐỔI MẬT KHẨU";
            // 
            // btnThongKe
            // 
            this.btnThongKe.BorderRadius = 5;
            this.btnThongKe.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnThongKe.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnThongKe.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnThongKe.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnThongKe.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnThongKe.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnThongKe.ForeColor = System.Drawing.Color.White;
            this.btnThongKe.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnThongKe.Location = new System.Drawing.Point(34, 734);
            this.btnThongKe.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnThongKe.Name = "btnThongKe";
            this.btnThongKe.Size = new System.Drawing.Size(282, 61);
            this.btnThongKe.TabIndex = 6;
            this.btnThongKe.Text = "THỐNG KÊ";
            this.btnThongKe.Click += new System.EventHandler(this.btnThongKe_Click);
            // 
            // btnHoaDon
            // 
            this.btnHoaDon.BorderRadius = 5;
            this.btnHoaDon.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHoaDon.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHoaDon.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHoaDon.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHoaDon.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnHoaDon.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnHoaDon.ForeColor = System.Drawing.Color.White;
            this.btnHoaDon.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnHoaDon.Location = new System.Drawing.Point(34, 642);
            this.btnHoaDon.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnHoaDon.Name = "btnHoaDon";
            this.btnHoaDon.Size = new System.Drawing.Size(282, 61);
            this.btnHoaDon.TabIndex = 5;
            this.btnHoaDon.Text = "HÓA ĐƠN";
            this.btnHoaDon.Click += new System.EventHandler(this.btnHoaDon_Click);
            // 
            // btnDienNuoc
            // 
            this.btnDienNuoc.BorderRadius = 5;
            this.btnDienNuoc.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDienNuoc.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDienNuoc.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDienNuoc.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDienNuoc.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnDienNuoc.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnDienNuoc.ForeColor = System.Drawing.Color.White;
            this.btnDienNuoc.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnDienNuoc.Location = new System.Drawing.Point(34, 550);
            this.btnDienNuoc.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDienNuoc.Name = "btnDienNuoc";
            this.btnDienNuoc.Size = new System.Drawing.Size(282, 61);
            this.btnDienNuoc.TabIndex = 4;
            this.btnDienNuoc.Text = "ĐIỆN NƯỚC";
            this.btnDienNuoc.Click += new System.EventHandler(this.btnDienNuoc_Click);
            // 
            // btnHopDong
            // 
            this.btnHopDong.BorderRadius = 5;
            this.btnHopDong.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHopDong.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHopDong.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHopDong.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHopDong.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnHopDong.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnHopDong.ForeColor = System.Drawing.Color.White;
            this.btnHopDong.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnHopDong.Location = new System.Drawing.Point(34, 457);
            this.btnHopDong.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnHopDong.Name = "btnHopDong";
            this.btnHopDong.Size = new System.Drawing.Size(282, 61);
            this.btnHopDong.TabIndex = 3;
            this.btnHopDong.Text = "HỢP ĐỒNG";
            this.btnHopDong.Click += new System.EventHandler(this.btnHopDong_Click);
            // 
            // btnKhachThue
            // 
            this.btnKhachThue.BorderRadius = 5;
            this.btnKhachThue.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnKhachThue.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnKhachThue.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnKhachThue.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnKhachThue.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnKhachThue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnKhachThue.ForeColor = System.Drawing.Color.White;
            this.btnKhachThue.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnKhachThue.Location = new System.Drawing.Point(34, 364);
            this.btnKhachThue.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnKhachThue.Name = "btnKhachThue";
            this.btnKhachThue.Size = new System.Drawing.Size(282, 61);
            this.btnKhachThue.TabIndex = 2;
            this.btnKhachThue.Text = "KHÁCH THUÊ";
            this.btnKhachThue.Click += new System.EventHandler(this.btnKhachThue_Click);
            // 
            // btnPhongTro
            // 
            this.btnPhongTro.BorderRadius = 5;
            this.btnPhongTro.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPhongTro.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPhongTro.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPhongTro.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPhongTro.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.btnPhongTro.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnPhongTro.ForeColor = System.Drawing.Color.White;
            this.btnPhongTro.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnPhongTro.Location = new System.Drawing.Point(34, 267);
            this.btnPhongTro.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnPhongTro.Name = "btnPhongTro";
            this.btnPhongTro.Size = new System.Drawing.Size(282, 61);
            this.btnPhongTro.TabIndex = 1;
            this.btnPhongTro.Text = "PHÒNG TRỌ";
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.Image = global::QLKDPhongTro.Properties.Resources.business_illustration;
            this.picLogo.Location = new System.Drawing.Point(30, 31);
            this.picLogo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(300, 154);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 10;
            this.picLogo.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Black;
            this.lblTitle.Location = new System.Drawing.Point(11, 210);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(341, 47);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "QUẢN LÝ PHÒNG TRỌ";
            // 
            // frmFormApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1800, 1170);
            this.Controls.Add(this.panelMain);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmFormApp";
            this.Text = "Phần Mềm Quản Lý Kinh Doanh Phòng Trọ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmFormApp_Load);
            this.panelMain.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhongTro)).EndInit();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelRoomInfo.ResumeLayout(false);
            this.panelRoomInfo.PerformLayout();
            this.panelSidebar.ResumeLayout(false);
            this.panelSidebar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel panelMain;
        private Guna.UI2.WinForms.Guna2Panel panelSidebar;
        private Guna.UI2.WinForms.Guna2Panel panelContent;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2Panel panelRoomInfo;
        private Guna.UI2.WinForms.Guna2TextBox txtMoTa;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblMoTa;
        private Guna.UI2.WinForms.Guna2TextBox txtTenThanhVien;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTenThanhVien;
        private Guna.UI2.WinForms.Guna2TextBox txtTrangThai;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTrangThai;
        private Guna.UI2.WinForms.Guna2TextBox txtGiaThue;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblGiaThue;
        private Guna.UI2.WinForms.Guna2TextBox txtSoPhong;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSoPhong;
        private Guna.UI2.WinForms.Guna2TextBox txtTang;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTang;
        private Guna.UI2.WinForms.Guna2TextBox txtToaNha;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblToaNha;
        private Guna.UI2.WinForms.Guna2TextBox txtMaPhong;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblMaPhong;
        private Guna.UI2.WinForms.Guna2Panel panelButtons;
        private Guna.UI2.WinForms.Guna2Button btnXoa;
        private Guna.UI2.WinForms.Guna2Button btnXuatExcel;
        private Guna.UI2.WinForms.Guna2Button btnCapNhat;
        private Guna.UI2.WinForms.Guna2Button btnThem;
        private Guna.UI2.WinForms.Guna2Panel panelSearch;
        private Guna.UI2.WinForms.Guna2TextBox txtTimKiem;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTimKiem;
        private Guna.UI2.WinForms.Guna2DataGridView dgvPhongTro;
        private Guna.UI2.WinForms.Guna2Button btnPhongTro;
        private Guna.UI2.WinForms.Guna2Button btnKhachThue;
        private Guna.UI2.WinForms.Guna2Button btnHopDong;
        private Guna.UI2.WinForms.Guna2Button btnDienNuoc;
        private Guna.UI2.WinForms.Guna2Button btnHoaDon;
        private Guna.UI2.WinForms.Guna2Button btnThongKe;
        private Guna.UI2.WinForms.Guna2Button btnDoiMatKhau;
        private Guna.UI2.WinForms.Guna2Button btnDangXuat;
        private System.Windows.Forms.LinkLabel lblFeedback;
        private System.Windows.Forms.PictureBox picLogo;
    }
}