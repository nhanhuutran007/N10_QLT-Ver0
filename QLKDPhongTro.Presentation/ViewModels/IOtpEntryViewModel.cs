using System.Windows.Input;

namespace QLKDPhongTro.Presentation.ViewModels
{
    public interface IOtpEntryViewModel
    {
        string OtpCode { get; set; }
        ICommand VerifyOtpCommand { get; }
    }
}
