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
            this.panelMain = new Guna.UI2.WinForms.Guna2Panel();
            this.panelSidebar = new Guna.UI2.WinForms.Guna2Panel();
            this.panelContent = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnPhongTro = new Guna.UI2.WinForms.Guna2Button();
            this.btnKhachThue = new Guna.UI2.WinForms.Guna2Button();
            this.btnHopDong = new Guna.UI2.WinForms.Guna2Button();
            this.btnDienNuoc = new Guna.UI2.WinForms.Guna2Button();
            this.btnHoaDon = new Guna.UI2.WinForms.Guna2Button();
            this.btnThongKe = new Guna.UI2.WinForms.Guna2Button();
            this.btnDoiMatKhau = new Guna.UI2.WinForms.Guna2Button();
            this.btnDangXuat = new Guna.UI2.WinForms.Guna2Button();
            this.lblFeedback = new System.Windows.Forms.LinkLabel();
            this.picLogo = new System.Windows.Forms.PictureBox();
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
            this.panelButtons = new Guna.UI2.WinForms.Guna2Panel();
            this.btnXoa = new Guna.UI2.WinForms.Guna2Button();
            this.btnXuatExcel = new Guna.UI2.WinForms.Guna2Button();
            this.btnCapNhat = new Guna.UI2.WinForms.Guna2Button();
            this.btnThem = new Guna.UI2.WinForms.Guna2Button();
            this.panelSearch = new Guna.UI2.WinForms.Guna2Panel();
            this.txtTimKiem = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblTimKiem = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.dgvPhongTro = new Guna.UI2.WinForms.Guna2DataGridView();
            this.panelMain.SuspendLayout();
            this.panelSidebar.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.panelRoomInfo.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.panelSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhongTro)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelContent);
            this.panelMain.Controls.Add(this.panelSidebar);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1200, 800);
            this.panelMain.TabIndex = 0;
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
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
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(280, 800);
            this.panelSidebar.TabIndex = 0;
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelContent.Controls.Add(this.dgvPhongTro);
            this.panelContent.Controls.Add(this.panelSearch);
            this.panelContent.Controls.Add(this.panelButtons);
            this.panelContent.Controls.Add(this.panelRoomInfo);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(280, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(920, 800);
            this.panelContent.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 150);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(200, 30);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "QUẢN LÝ PHÒNG TRỌ";
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
            this.panelRoomInfo.Location = new System.Drawing.Point(20, 60);
            this.panelRoomInfo.Name = "panelRoomInfo";
            this.panelRoomInfo.Size = new System.Drawing.Size(600, 300);
            this.panelRoomInfo.TabIndex = 0;
            // 
            // lblMaPhong
            // 
            this.lblMaPhong.AutoSize = true;
            this.lblMaPhong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaPhong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblMaPhong.Location = new System.Drawing.Point(20, 30);
            this.lblMaPhong.Name = "lblMaPhong";
            this.lblMaPhong.Size = new System.Drawing.Size(80, 17);
            this.lblMaPhong.TabIndex = 0;
            this.lblMaPhong.Text = "Mã phòng trọ:";
            // 
            // txtMaPhong
            // 
            this.txtMaPhong.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMaPhong.DefaultText = "PT003";
            this.txtMaPhong.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtMaPhong.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtMaPhong.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMaPhong.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMaPhong.FillColor = System.Drawing.Color.White;
            this.txtMaPhong.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMaPhong.ForeColor = System.Drawing.Color.Black;
            this.txtMaPhong.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMaPhong.Location = new System.Drawing.Point(20, 50);
            this.txtMaPhong.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMaPhong.Name = "txtMaPhong";
            this.txtMaPhong.PasswordChar = '\0';
            this.txtMaPhong.PlaceholderForeColor = System.Drawing.Color.Gray;
            this.txtMaPhong.PlaceholderText = "";
            this.txtMaPhong.SelectedText = "";
            this.txtMaPhong.Size = new System.Drawing.Size(120, 30);
            this.txtMaPhong.TabIndex = 1;
            // 
            // lblToaNha
            // 
            this.lblToaNha.AutoSize = true;
            this.lblToaNha.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToaNha.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblToaNha.Location = new System.Drawing.Point(200, 30);
            this.lblToaNha.Name = "lblToaNha";
            this.lblToaNha.Size = new System.Drawing.Size(60, 17);
            this.lblToaNha.TabIndex = 2;
            this.lblToaNha.Text = "Tòa nhà:";
            // 
            // txtToaNha
            // 
            this.txtToaNha.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtToaNha.Location = new System.Drawing.Point(160, 50);
            this.txtToaNha.Name = "txtToaNha";
            this.txtToaNha.Size = new System.Drawing.Size(80, 23);
            this.txtToaNha.TabIndex = 3;
            this.txtToaNha.Text = "Tòa A";
            // 
            // lblTang
            // 
            this.lblTang.AutoSize = true;
            this.lblTang.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblTang.Location = new System.Drawing.Point(330, 30);
            this.lblTang.Name = "lblTang";
            this.lblTang.Size = new System.Drawing.Size(40, 17);
            this.lblTang.TabIndex = 4;
            this.lblTang.Text = "Tầng:";
            // 
            // txtTang
            // 
            this.txtTang.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTang.Location = new System.Drawing.Point(260, 50);
            this.txtTang.Name = "txtTang";
            this.txtTang.Size = new System.Drawing.Size(50, 23);
            this.txtTang.TabIndex = 5;
            this.txtTang.Text = "2";
            // 
            // lblSoPhong
            // 
            this.lblSoPhong.AutoSize = true;
            this.lblSoPhong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoPhong.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblSoPhong.Location = new System.Drawing.Point(400, 30);
            this.lblSoPhong.Name = "lblSoPhong";
            this.lblSoPhong.Size = new System.Drawing.Size(70, 17);
            this.lblSoPhong.TabIndex = 6;
            this.lblSoPhong.Text = "Số phòng:";
            // 
            // txtSoPhong
            // 
            this.txtSoPhong.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSoPhong.Location = new System.Drawing.Point(330, 50);
            this.txtSoPhong.Name = "txtSoPhong";
            this.txtSoPhong.Size = new System.Drawing.Size(80, 23);
            this.txtSoPhong.TabIndex = 7;
            this.txtSoPhong.Text = "201";
            // 
            // lblGiaThue
            // 
            this.lblGiaThue.AutoSize = true;
            this.lblGiaThue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGiaThue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblGiaThue.Location = new System.Drawing.Point(20, 90);
            this.lblGiaThue.Name = "lblGiaThue";
            this.lblGiaThue.Size = new System.Drawing.Size(60, 17);
            this.lblGiaThue.TabIndex = 8;
            this.lblGiaThue.Text = "Giá thuê:";
            // 
            // txtGiaThue
            // 
            this.txtGiaThue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGiaThue.Location = new System.Drawing.Point(20, 110);
            this.txtGiaThue.Name = "txtGiaThue";
            this.txtGiaThue.Size = new System.Drawing.Size(120, 23);
            this.txtGiaThue.TabIndex = 9;
            this.txtGiaThue.Text = "4.600.000";
            // 
            // lblTrangThai
            // 
            this.lblTrangThai.AutoSize = true;
            this.lblTrangThai.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrangThai.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblTrangThai.Location = new System.Drawing.Point(200, 90);
            this.lblTrangThai.Name = "lblTrangThai";
            this.lblTrangThai.Size = new System.Drawing.Size(100, 17);
            this.lblTrangThai.TabIndex = 10;
            this.lblTrangThai.Text = "Trạng thái phòng:";
            // 
            // txtTrangThai
            // 
            this.txtTrangThai.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTrangThai.Location = new System.Drawing.Point(160, 110);
            this.txtTrangThai.Name = "txtTrangThai";
            this.txtTrangThai.Size = new System.Drawing.Size(120, 23);
            this.txtTrangThai.TabIndex = 11;
            this.txtTrangThai.Text = "Đang cho thuê";
            // 
            // lblTenThanhVien
            // 
            this.lblTenThanhVien.AutoSize = true;
            this.lblTenThanhVien.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTenThanhVien.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblTenThanhVien.Location = new System.Drawing.Point(20, 150);
            this.lblTenThanhVien.Name = "lblTenThanhVien";
            this.lblTenThanhVien.Size = new System.Drawing.Size(130, 17);
            this.lblTenThanhVien.TabIndex = 12;
            this.lblTenThanhVien.Text = "Tên thành viên phòng:";
            // 
            // txtTenThanhVien
            // 
            this.txtTenThanhVien.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTenThanhVien.Location = new System.Drawing.Point(20, 170);
            this.txtTenThanhVien.Name = "txtTenThanhVien";
            this.txtTenThanhVien.Size = new System.Drawing.Size(200, 23);
            this.txtTenThanhVien.TabIndex = 13;
            this.txtTenThanhVien.Text = "Phạm Minh Việt";
            // 
            // lblMoTa
            // 
            this.lblMoTa.AutoSize = true;
            this.lblMoTa.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMoTa.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblMoTa.Location = new System.Drawing.Point(20, 210);
            this.lblMoTa.Name = "lblMoTa";
            this.lblMoTa.Size = new System.Drawing.Size(50, 17);
            this.lblMoTa.TabIndex = 14;
            this.lblMoTa.Text = "Mô tả:";
            // 
            // txtMoTa
            // 
            this.txtMoTa.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMoTa.Location = new System.Drawing.Point(20, 230);
            this.txtMoTa.Multiline = true;
            this.txtMoTa.Name = "txtMoTa";
            this.txtMoTa.Size = new System.Drawing.Size(460, 50);
            this.txtMoTa.TabIndex = 15;
            this.txtMoTa.Text = "Phòng có máy giặt, gần công viên";
            // 
            // panelButtons
            // 
            this.panelButtons.BackColor = System.Drawing.Color.White;
            this.panelButtons.Controls.Add(this.btnXoa);
            this.panelButtons.Controls.Add(this.btnXuatExcel);
            this.panelButtons.Controls.Add(this.btnCapNhat);
            this.panelButtons.Controls.Add(this.btnThem);
            this.panelButtons.Location = new System.Drawing.Point(640, 60);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(120, 300);
            this.panelButtons.TabIndex = 1;
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
            this.btnThem.Location = new System.Drawing.Point(10, 20);
            this.btnThem.Name = "btnThem";
            this.btnThem.Size = new System.Drawing.Size(100, 40);
            this.btnThem.TabIndex = 0;
            this.btnThem.Text = "Thêm Phòng Trọ";
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
            this.btnCapNhat.Location = new System.Drawing.Point(10, 80);
            this.btnCapNhat.Name = "btnCapNhat";
            this.btnCapNhat.Size = new System.Drawing.Size(100, 40);
            this.btnCapNhat.TabIndex = 1;
            this.btnCapNhat.Text = "Cập nhật Phòng Trọ";
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
            this.btnXuatExcel.Location = new System.Drawing.Point(10, 140);
            this.btnXuatExcel.Name = "btnXuatExcel";
            this.btnXuatExcel.Size = new System.Drawing.Size(100, 40);
            this.btnXuatExcel.TabIndex = 2;
            this.btnXuatExcel.Text = "Xuất Excel";
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
            this.btnXoa.Location = new System.Drawing.Point(10, 200);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.Size = new System.Drawing.Size(100, 40);
            this.btnXoa.TabIndex = 3;
            this.btnXoa.Text = "Xóa Phòng Trọ";
            // 
            // panelSearch
            // 
            this.panelSearch.BackColor = System.Drawing.Color.White;
            this.panelSearch.Controls.Add(this.txtTimKiem);
            this.panelSearch.Controls.Add(this.lblTimKiem);
            this.panelSearch.Location = new System.Drawing.Point(20, 380);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(740, 50);
            this.panelSearch.TabIndex = 2;
            // 
            // lblTimKiem
            // 
            this.lblTimKiem.AutoSize = true;
            this.lblTimKiem.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTimKiem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.lblTimKiem.Location = new System.Drawing.Point(20, 15);
            this.lblTimKiem.Name = "lblTimKiem";
            this.lblTimKiem.Size = new System.Drawing.Size(70, 17);
            this.lblTimKiem.TabIndex = 0;
            this.lblTimKiem.Text = "Tìm Kiếm:";
            // 
            // txtTimKiem
            // 
            this.txtTimKiem.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimKiem.Location = new System.Drawing.Point(100, 12);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.Size = new System.Drawing.Size(620, 23);
            this.txtTimKiem.TabIndex = 1;
            // 
            // dgvPhongTro
            // 
            this.dgvPhongTro.AllowUserToAddRows = false;
            this.dgvPhongTro.AllowUserToDeleteRows = false;
            this.dgvPhongTro.BackgroundColor = System.Drawing.Color.White;
            this.dgvPhongTro.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPhongTro.Location = new System.Drawing.Point(20, 450);
            this.dgvPhongTro.Name = "dgvPhongTro";
            this.dgvPhongTro.ReadOnly = true;
            this.dgvPhongTro.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPhongTro.Size = new System.Drawing.Size(880, 320);
            this.dgvPhongTro.TabIndex = 3;
            // 
            // btnPhongTro
            // 
            this.btnPhongTro.BorderRadius = 5;
            this.btnPhongTro.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPhongTro.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPhongTro.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPhongTro.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPhongTro.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.btnPhongTro.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPhongTro.ForeColor = System.Drawing.Color.White;
            this.btnPhongTro.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnPhongTro.Location = new System.Drawing.Point(20, 200);
            this.btnPhongTro.Name = "btnPhongTro";
            this.btnPhongTro.Size = new System.Drawing.Size(240, 50);
            this.btnPhongTro.TabIndex = 1;
            this.btnPhongTro.Text = "Phòng Trọ";
            // 
            // btnKhachThue
            // 
            this.btnKhachThue.BorderRadius = 5;
            this.btnKhachThue.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnKhachThue.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnKhachThue.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnKhachThue.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnKhachThue.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnKhachThue.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnKhachThue.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnKhachThue.ForeColor = System.Drawing.Color.White;
            this.btnKhachThue.Location = new System.Drawing.Point(20, 260);
            this.btnKhachThue.Name = "btnKhachThue";
            this.btnKhachThue.Size = new System.Drawing.Size(240, 50);
            this.btnKhachThue.TabIndex = 2;
            this.btnKhachThue.Text = "Khách Thuê";
            // 
            // btnHopDong
            // 
            this.btnHopDong.BorderRadius = 5;
            this.btnHopDong.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHopDong.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHopDong.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHopDong.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHopDong.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnHopDong.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnHopDong.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnHopDong.ForeColor = System.Drawing.Color.White;
            this.btnHopDong.Location = new System.Drawing.Point(20, 320);
            this.btnHopDong.Name = "btnHopDong";
            this.btnHopDong.Size = new System.Drawing.Size(240, 50);
            this.btnHopDong.TabIndex = 3;
            this.btnHopDong.Text = "Hợp Đồng";
            // 
            // btnDienNuoc
            // 
            this.btnDienNuoc.BorderRadius = 5;
            this.btnDienNuoc.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDienNuoc.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDienNuoc.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDienNuoc.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDienNuoc.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnDienNuoc.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnDienNuoc.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDienNuoc.ForeColor = System.Drawing.Color.White;
            this.btnDienNuoc.Location = new System.Drawing.Point(20, 380);
            this.btnDienNuoc.Name = "btnDienNuoc";
            this.btnDienNuoc.Size = new System.Drawing.Size(240, 50);
            this.btnDienNuoc.TabIndex = 4;
            this.btnDienNuoc.Text = "Điện Nước";
            // 
            // btnHoaDon
            // 
            this.btnHoaDon.BorderRadius = 5;
            this.btnHoaDon.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnHoaDon.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnHoaDon.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnHoaDon.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnHoaDon.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnHoaDon.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnHoaDon.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnHoaDon.ForeColor = System.Drawing.Color.White;
            this.btnHoaDon.Location = new System.Drawing.Point(20, 440);
            this.btnHoaDon.Name = "btnHoaDon";
            this.btnHoaDon.Size = new System.Drawing.Size(240, 50);
            this.btnHoaDon.TabIndex = 5;
            this.btnHoaDon.Text = "Hóa Đơn";
            // 
            // btnThongKe
            // 
            this.btnThongKe.BorderRadius = 5;
            this.btnThongKe.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnThongKe.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnThongKe.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnThongKe.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnThongKe.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnThongKe.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnThongKe.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnThongKe.ForeColor = System.Drawing.Color.White;
            this.btnThongKe.Location = new System.Drawing.Point(20, 500);
            this.btnThongKe.Name = "btnThongKe";
            this.btnThongKe.Size = new System.Drawing.Size(240, 50);
            this.btnThongKe.TabIndex = 6;
            this.btnThongKe.Text = "Thống Kê";
            // 
            // btnDoiMatKhau
            // 
            this.btnDoiMatKhau.BorderRadius = 5;
            this.btnDoiMatKhau.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDoiMatKhau.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDoiMatKhau.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDoiMatKhau.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDoiMatKhau.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnDoiMatKhau.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnDoiMatKhau.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDoiMatKhau.ForeColor = System.Drawing.Color.White;
            this.btnDoiMatKhau.Location = new System.Drawing.Point(20, 560);
            this.btnDoiMatKhau.Name = "btnDoiMatKhau";
            this.btnDoiMatKhau.Size = new System.Drawing.Size(240, 50);
            this.btnDoiMatKhau.TabIndex = 7;
            this.btnDoiMatKhau.Text = "Đổi Mật Khẩu";
            // 
            // btnDangXuat
            // 
            this.btnDangXuat.BorderRadius = 5;
            this.btnDangXuat.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDangXuat.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDangXuat.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDangXuat.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDangXuat.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(150)))), ((int)(((byte)(243)))));
            this.btnDangXuat.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnDangXuat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDangXuat.ForeColor = System.Drawing.Color.White;
            this.btnDangXuat.Location = new System.Drawing.Point(20, 620);
            this.btnDangXuat.Name = "btnDangXuat";
            this.btnDangXuat.Size = new System.Drawing.Size(240, 50);
            this.btnDangXuat.TabIndex = 8;
            this.btnDangXuat.Text = "Đăng xuất";
            // 
            // lblFeedback
            // 
            this.lblFeedback.AutoSize = true;
            this.lblFeedback.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFeedback.LinkColor = System.Drawing.Color.LightBlue;
            this.lblFeedback.Location = new System.Drawing.Point(20, 750);
            this.lblFeedback.Name = "lblFeedback";
            this.lblFeedback.Size = new System.Drawing.Size(120, 13);
            this.lblFeedback.TabIndex = 9;
            this.lblFeedback.TabStop = true;
            this.lblFeedback.Text = "Báo lỗi / Góp ý";
            // 
            // picLogo
            // 
            this.picLogo.BackColor = System.Drawing.Color.Transparent;
            this.picLogo.Image = global::QLKDPhongTro.Properties.Resources.business_illustration;
            this.picLogo.Location = new System.Drawing.Point(20, 20);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(240, 120);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 10;
            this.picLogo.TabStop = false;
            // 
            // frmFormApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.panelMain);
            this.Name = "frmFormApp";
            this.Text = "Phần Mềm Quản Lý Kinh Doanh Phòng Trọ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmFormApp_Load);
            this.panelMain.ResumeLayout(false);
            this.panelSidebar.ResumeLayout(false);
            this.panelSidebar.PerformLayout();
            this.panelContent.ResumeLayout(false);
            this.panelRoomInfo.ResumeLayout(false);
            this.panelRoomInfo.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPhongTro)).EndInit();
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