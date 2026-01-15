using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AutoReacto.Dashboard.Controls;

public enum ToastType
{
    Success,
    Error,
    Warning,
    Info
}

public partial class ToastNotification : UserControl
{
    private System.Timers.Timer? _autoCloseTimer;

    public ToastNotification()
    {
        InitializeComponent();
        Visibility = Visibility.Collapsed;
    }

    public void Show(string title, string message, ToastType type, int autoCloseDuration = 3000)
    {
        Dispatcher.Invoke(() =>
        {
            TitleText.Text = title;
            MessageText.Text = message;

            // Set colors and icon based on type
            switch (type)
            {
                case ToastType.Success:
                    IconBorder.Background = new SolidColorBrush(Color.FromRgb(35, 134, 54));
                    IconText.Text = "✓";
                    IconText.Foreground = Brushes.White;
                    ToastBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(35, 134, 54));
                    break;

                case ToastType.Error:
                    IconBorder.Background = new SolidColorBrush(Color.FromRgb(218, 55, 60));
                    IconText.Text = "✕";
                    IconText.Foreground = Brushes.White;
                    ToastBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(218, 55, 60));
                    break;

                case ToastType.Warning:
                    IconBorder.Background = new SolidColorBrush(Color.FromRgb(250, 168, 26));
                    IconText.Text = "!";
                    IconText.Foreground = Brushes.Black;
                    ToastBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(250, 168, 26));
                    break;

                case ToastType.Info:
                    IconBorder.Background = new SolidColorBrush(Color.FromRgb(88, 101, 242));
                    IconText.Text = "i";
                    IconText.Foreground = Brushes.White;
                    ToastBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(88, 101, 242));
                    break;
            }

            Visibility = Visibility.Visible;

            // Play show animation
            var showAnim = (Storyboard)FindResource("ShowAnimation");
            showAnim.Begin(this);

            // Auto close
            if (autoCloseDuration > 0)
            {
                _autoCloseTimer?.Stop();
                _autoCloseTimer = new System.Timers.Timer(autoCloseDuration);
                _autoCloseTimer.Elapsed += (s, e) =>
                {
                    _autoCloseTimer?.Stop();
                    Dispatcher.Invoke(Hide);
                };
                _autoCloseTimer.Start();
            }
        });
    }

    public void Hide()
    {
        _autoCloseTimer?.Stop();

        var hideAnim = (Storyboard)FindResource("HideAnimation");
        hideAnim.Completed += (s, e) => Visibility = Visibility.Collapsed;
        hideAnim.Begin(this);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    // Static helper for easy access
    public static ToastNotification? Instance { get; private set; }

    public static void Register(ToastNotification instance)
    {
        Instance = instance;
    }

    public static void ShowToast(string title, string message, ToastType type, int duration = 3000)
    {
        Instance?.Show(title, message, type, duration);
    }

    public static void Success(string title, string message = "") =>
        ShowToast(title, message, ToastType.Success);

    public static void Error(string title, string message = "") =>
        ShowToast(title, message, ToastType.Error);

    public static void Warning(string title, string message = "") =>
        ShowToast(title, message, ToastType.Warning);

    public static void Info(string title, string message = "") =>
        ShowToast(title, message, ToastType.Info);
}
