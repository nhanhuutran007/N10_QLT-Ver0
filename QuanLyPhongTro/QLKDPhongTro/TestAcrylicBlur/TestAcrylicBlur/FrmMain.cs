using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static TestAcrylicBlur.EffectBlur;

namespace TestAcrylicBlur
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }


        #region Code to blur form

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private uint _blurOpacity;

        public double BlurOpacity
        {
            get { return _blurOpacity; }
            set { _blurOpacity = (uint)value; EnableBlur(); }
        }

        private uint _blurBackgroundColor = 0x990000; 

        internal void EnableBlur()
        {

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND;
            accent.GradientColor = (_blurOpacity << 24) | (_blurBackgroundColor & 0xFFFFFF);  
            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);
            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;
            SetWindowCompositionAttribute(this.Handle, ref data);
            Marshal.FreeHGlobal(accentPtr);
        }
        #endregion


        #region Blur form on form load
        private void Form1_Load(object sender, EventArgs e)
        {
            EnableBlur();
            this.BackColor = System.Drawing.ColorTranslator.FromHtml("#010000");
            var frm = new FrmLogin();
            frm.ShowDialog();
        }
        #endregion


      
    }
}
