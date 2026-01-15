using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AutoReacto.Dashboard.Controls;

public enum ConfirmDialogType
{
    Warning,
    Danger,
    Info,
    Success
}

public partial class ConfirmDialog : UserControl
{
    private TaskCompletionSource<bool>? _tcs;
    private static ConfirmDialog? _instance;

    public ConfirmDialog()
    {
        InitializeComponent();
    }

    public static void Register(ConfirmDialog instance)
    {
        _instance = instance;
    }

    public static async Task<bool> ShowAsync(
        string title,
        string message,
        string confirmText = "Confirm",
        string cancelText = "Cancel",
        ConfirmDialogType type = ConfirmDialogType.Warning)
    {
        if (_instance == null)
            return false;

        return await _instance.ShowDialogAsync(title, message, confirmText, cancelText, type);
    }

    public async Task<bool> ShowDialogAsync(
        string title,
        string message,
        string confirmText = "Confirm",
        string cancelText = "Cancel",
        ConfirmDialogType type = ConfirmDialogType.Warning)
    {
        _tcs = new TaskCompletionSource<bool>();

        // Set content
        TitleText.Text = title;
        MessageText.Text = message;
        ConfirmButton.Content = confirmText;
        CancelButton.Content = cancelText;

        // Set style based on type
        ApplyStyle(type);

        // Show with animation
        Visibility = Visibility.Visible;
        await AnimateIn();

        var result = await _tcs.Task;

        // Hide with animation
        await AnimateOut();
        Visibility = Visibility.Collapsed;

        return result;
    }

    private void ApplyStyle(ConfirmDialogType type)
    {
        var (icon, bgColor) = type switch
        {
            ConfirmDialogType.Danger => ("🗑️", "#ED4245"),
            ConfirmDialogType.Warning => ("⚠️", "#FEE75C"),
            ConfirmDialogType.Info => ("ℹ️", "#5865F2"),
            ConfirmDialogType.Success => ("✅", "#57F287"),
            _ => ("⚠️", "#FEE75C")
        };

        IconEmoji.Text = icon;
        IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(bgColor)!);
        
        if (type == ConfirmDialogType.Danger)
        {
            ConfirmButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ED4245")!);
        }
        else if (type == ConfirmDialogType.Success)
        {
            ConfirmButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#57F287")!);
            ConfirmButton.Foreground = new SolidColorBrush(Colors.Black);
        }
        else
        {
            ConfirmButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5865F2")!);
        }
    }

    private async Task AnimateIn()
    {
        var transform = DialogBox.RenderTransform as ScaleTransform;
        if (transform == null)
        {
            transform = new ScaleTransform(0.8, 0.8);
            DialogBox.RenderTransform = transform;
            DialogBox.RenderTransformOrigin = new Point(0.5, 0.5);
        }

        transform.ScaleX = 0.8;
        transform.ScaleY = 0.8;
        DialogBox.Opacity = 0;

        var scaleXAnim = new DoubleAnimation(0.8, 1.0, TimeSpan.FromMilliseconds(200))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        var scaleYAnim = new DoubleAnimation(0.8, 1.0, TimeSpan.FromMilliseconds(200))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        var fadeAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));

        transform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
        transform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
        DialogBox.BeginAnimation(OpacityProperty, fadeAnim);

        await Task.Delay(200);
    }

    private async Task AnimateOut()
    {
        var transform = DialogBox.RenderTransform as ScaleTransform ?? new ScaleTransform(1, 1);

        var scaleXAnim = new DoubleAnimation(1.0, 0.8, TimeSpan.FromMilliseconds(150))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };
        var scaleYAnim = new DoubleAnimation(1.0, 0.8, TimeSpan.FromMilliseconds(150))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };
        var fadeAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150));

        transform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnim);
        transform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnim);
        DialogBox.BeginAnimation(OpacityProperty, fadeAnim);

        await Task.Delay(150);
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        _tcs?.TrySetResult(true);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _tcs?.TrySetResult(false);
    }

    private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        // Click outside to cancel
        _tcs?.TrySetResult(false);
    }
}
