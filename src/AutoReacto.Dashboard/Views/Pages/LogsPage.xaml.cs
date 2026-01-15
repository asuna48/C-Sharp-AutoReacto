using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AutoReacto.Dashboard.Views.Pages;

public partial class LogsPage : Page
{
    private readonly StringBuilder _logBuilder = new();
    private const int MaxLogLines = 1000;
    private int _lineCount = 0;

    public LogsPage()
    {
        InitializeComponent();
        AddLog("[Dashboard] Log viewer initialized.");
    }

    public void AddLog(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        // Add timestamp if not present
        if (!message.StartsWith("[2"))
        {
            message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
        }

        _logBuilder.AppendLine(message);
        _lineCount++;

        // Trim old logs if too many
        if (_lineCount > MaxLogLines)
        {
            var lines = _logBuilder.ToString().Split(Environment.NewLine);
            var trimmedLines = lines.Skip(lines.Length - MaxLogLines);
            _logBuilder.Clear();
            _logBuilder.AppendLine(string.Join(Environment.NewLine, trimmedLines));
            _lineCount = MaxLogLines;
        }

        // Update UI
        Dispatcher.Invoke(() =>
        {
            LogTextBlock.Text = _logBuilder.ToString();
            LogScrollViewer.ScrollToEnd();
        });
    }

    private void ClearLogs_Click(object sender, RoutedEventArgs e)
    {
        _logBuilder.Clear();
        _lineCount = 0;
        LogTextBlock.Text = "";
        AddLog("[Dashboard] Logs cleared.");
    }
}
