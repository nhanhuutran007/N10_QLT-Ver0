using CommunityToolkit.Mvvm.ComponentModel;

namespace QLKDPhongTro.Presentation.ViewModels
{
    // Lớp này kế thừa từ ObservableObject của framework.
    // Nó đã triển khai sẵn INotifyPropertyChanged cho chúng ta.
    // Từ khóa 'partial' là cần thiết để Source Generators hoạt động.
    public abstract partial class ViewModelBase : ObservableObject
    {
    }
}