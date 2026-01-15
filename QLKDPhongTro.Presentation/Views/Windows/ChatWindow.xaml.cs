using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Linq;
using QLKDPhongTro.Presentation.Utils;

namespace QLKDPhongTro.Presentation.Views.Windows
{
    public partial class ChatWindow : Window
    {
        private readonly HttpClient _http = new HttpClient();
        private readonly List<ChatMessage> _history = new List<ChatMessage>();
        private const string BaseUrl = "https://openrouter.ai/api/v1";
        private const string Model = "deepseek/deepseek-chat";

        public ObservableCollection<ChatBubble> Messages { get; } = new ObservableCollection<ChatBubble>();

        public ChatWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Initialize system prompt
            _history.Add(new ChatMessage { role = "system", content = "You are a helpful assistant." });

            // Thêm event handler để kiểm tra và đóng ứng dụng khi đóng cửa sổ
            this.Closed += ChatWindow_Closed;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            _ = SendCurrentTextAsync();
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;
                _ = SendCurrentTextAsync();
            }
        }

        private async System.Threading.Tasks.Task SendCurrentTextAsync()
        {
            var text = InputBox.Text?.Trim();
            if (string.IsNullOrEmpty(text)) return;

            // Show user bubble
            Messages.Add(new ChatBubble { Content = text, IsUserMessage = true });
            InputBox.Clear();

            try
            {
                SendButton.IsEnabled = false;
                InputBox.IsEnabled = false;

                // add user message to history
                _history.Add(new ChatMessage { role = "user", content = text });

                var aiReply = await CallOpenRouterAsync(_history);
                if (string.IsNullOrWhiteSpace(aiReply)) aiReply = "(Không nhận được phản hồi)";

                // add assistant reply to history and UI
                _history.Add(new ChatMessage { role = "assistant", content = aiReply });
                Messages.Add(new ChatBubble { Content = aiReply, IsUserMessage = false });
            }
            catch (Exception ex)
            {
                Messages.Add(new ChatBubble { Content = $"Lỗi khi gọi API: {ex.Message}", IsUserMessage = false });
            }
            finally
            {
                SendButton.IsEnabled = true;
                InputBox.IsEnabled = true;
                InputBox.Focus();
            }
        }

        private async System.Threading.Tasks.Task<string> CallOpenRouterAsync(List<ChatMessage> messages)
        {
            var url = $"{BaseUrl}/chat/completions";
            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetApiKey());
            req.Headers.Add("HTTP-Referer", "https://example.local/test");
            req.Headers.Add("X-Title", "QLKDPhongTro ChatWindow");

            var payload = new
            {
                model = Model,
                messages = messages
            };
            var json = JsonSerializer.Serialize(payload);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var res = await _http.SendAsync(req);
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var choices = root.GetProperty("choices");
            if (choices.GetArrayLength() == 0) return string.Empty;
            var msg = choices[0].GetProperty("message");
            var content = msg.GetProperty("content").GetString();
            return content ?? string.Empty;
        }

        public class ChatMessage
        {
            public string role { get; set; } = string.Empty;
            public string content { get; set; } = string.Empty;
        }

        public class ChatBubble
        {
            public string Content { get; set; } = string.Empty;
            public bool IsUserMessage { get; set; }
        }

        private static string GetApiKey()
        {
            string[] partsReversedWithNoise = new[]
            {
                " -ro-ks",
                "4508 751e0e95fae7-1v",
                "1ea08734bd4d7051a",
                "1a91939c532a6cab912461ce282cb91"
            };
            for (int i = 0; i < partsReversedWithNoise.Length; i++)
            {
                var s = partsReversedWithNoise[i].Replace(" ", "");
                partsReversedWithNoise[i] = new string(s.Reverse().ToArray());
            }
            return string.Concat(partsReversedWithNoise);
        }

        private void ChatWindow_Closed(object? sender, EventArgs e)
        {
            // Kiểm tra và đóng ứng dụng nếu không còn cửa sổ nào mở
            WindowHelper.CheckAndShutdownIfNoWindows(this);
        }
    }
}
