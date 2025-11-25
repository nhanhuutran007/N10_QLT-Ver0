using System.Windows;
using System.Linq;
using System.Diagnostics;
using System.Windows.Threading;

namespace QLKDPhongTro.Presentation.Utils
{
    public static class WindowHelper
    {
        /// <summary>
        /// Kiểm tra và đóng ứng dụng nếu không còn cửa sổ nào mở (trừ cửa sổ đang đóng)
        /// </summary>
        /// <param name="closingWindow">Cửa sổ đang được đóng</param>
        public static void CheckAndShutdownIfNoWindows(Window closingWindow)
        {
            // Sử dụng Dispatcher để đợi một chút, đảm bảo cửa sổ đã được remove khỏi danh sách
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new System.Action(() =>
            {
                try
                {
                    // Lấy tất cả các cửa sổ (kể cả cửa sổ ẩn)
                    var allWindows = Application.Current.Windows.Cast<Window>().ToList();
                    
                    // Đếm số cửa sổ còn lại (trừ cửa sổ đang đóng)
                    var remainingWindows = allWindows
                        .Where(w => w != closingWindow)
                        .ToList();

                    // Debug: In ra thông tin các cửa sổ còn lại
                    Debug.WriteLine($"=== Window Check ===");
                    Debug.WriteLine($"Closing window: {closingWindow?.GetType().Name}");
                    Debug.WriteLine($"Total windows: {allWindows.Count}");
                    Debug.WriteLine($"Remaining windows: {remainingWindows.Count}");
                    
                    foreach (var window in remainingWindows)
                    {
                        Debug.WriteLine($"  - {window.GetType().Name}: Visible={window.IsVisible}, State={window.WindowState}, Owner={window.Owner?.GetType().Name ?? "null"}");
                    }

                    // Kiểm tra: Nếu không còn cửa sổ nào (kể cả cửa sổ ẩn), đóng ứng dụng
                    // Hoặc nếu chỉ còn các cửa sổ không visible (đã bị đóng nhưng chưa dispose)
                    var visibleWindows = remainingWindows
                        .Where(w => w.IsVisible && w.WindowState != WindowState.Minimized)
                        .ToList();

                    Debug.WriteLine($"Visible windows: {visibleWindows.Count}");

                    // Nếu không còn cửa sổ visible nào, đóng ứng dụng
                    if (visibleWindows.Count == 0)
                    {
                        Debug.WriteLine("No visible windows remaining. Shutting down application...");
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        Debug.WriteLine("Still have visible windows. Not shutting down.");
                    }
                }
                catch (System.Exception ex)
                {
                    // Nếu có lỗi, vẫn cố gắng đóng ứng dụng
                    Debug.WriteLine($"Error in CheckAndShutdownIfNoWindows: {ex.Message}");
                    Application.Current.Shutdown();
                }
            }));
        }
    }
}

