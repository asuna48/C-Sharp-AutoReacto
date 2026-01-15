using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoReacto.Core.Models;
using AutoReacto.Dashboard.Controls;
using AutoReacto.Dashboard.Localization;
using AutoReacto.Dashboard.Models;
using AutoReacto.Dashboard.Utils;
using AutoReacto.Dashboard.Views.Pages;

namespace AutoReacto.Dashboard.Views;

public partial class MainWindow : Window
{
    private Process? _botProcess;
    private BotConfig _config = new();
    private EmojisConfig _emojisConfig = new();
    private string _configPath = string.Empty;
    private string _emojisPath = string.Empty;
    private GeneralSettingsPage? _generalPage;
    private ReactionRulesPage? _rulesPage;
    private LogsPage? _logsPage;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Preserve emoji characters as-is
    };

    public MainWindow()
    {
        try
        {
            InitializeComponent();
            
            // Enable dark title bar
            DarkModeHelper.EnableDarkMode(this);
            
            // Load logo
            LoadLogo();
            
            // Update language button text
            UpdateLanguageButton();
            
            // Find config path - ALWAYS use bot's config files, not dashboard's
            var baseDir = AppContext.BaseDirectory;
            string configDir;
            
            // Priority 1: Bot project source directory (development mode)
            var devPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "AutoReacto"));
            if (Directory.Exists(devPath) && File.Exists(Path.Combine(devPath, "config.json")))
            {
                configDir = devPath;
            }
            else
            {
                // Priority 2: Solution source directory
                var solutionPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "src", "AutoReacto"));
                if (Directory.Exists(solutionPath) && File.Exists(Path.Combine(solutionPath, "config.json")))
                {
                    configDir = solutionPath;
                }
                else
                {
                    // Priority 3: Same directory as dashboard exe (fallback)
                    configDir = baseDir;
                }
            }
            
            _configPath = Path.Combine(configDir, "config.json");
            _emojisPath = Path.Combine(configDir, "emojis.json");
            
            // Debug: Show which config is being used
            System.Diagnostics.Debug.WriteLine($"Using config path: {_configPath}");
            System.Diagnostics.Debug.WriteLine($"Using emojis path: {_emojisPath}");
            
            // Set emoji storage path based on config location
            EmojiStorage.SetBasePath(_configPath);

            LoadConfig();
            InitializePages();
            NavigateToPage("General");
            
            // Register toast notification for global access
            ToastNotification.Register(Toast);
            
            // Register confirm dialog for global access
            ConfirmDialog.Register(ConfirmDialogControl);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting application:\n{ex.Message}\n\n{ex.StackTrace}", 
                "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void InitializePages()
    {
        _generalPage = new GeneralSettingsPage(_config);
        _rulesPage = new ReactionRulesPage(_emojisConfig);
        _logsPage = new LogsPage();
    }

    private void LoadConfig()
    {
        try
        {
            // Show which config is being used (for debugging)
            this.Title = $"AutoReacto Dashboard - {Path.GetDirectoryName(_configPath)}";
            
            // Load config.json (bot settings)
            if (File.Exists(_configPath))
            {
                var json = File.ReadAllText(_configPath, System.Text.Encoding.UTF8);
                var config = JsonSerializer.Deserialize<BotConfig>(json, JsonOptions);
                if (config != null)
                {
                    _config = config;
                }
            }
            else
            {
                MessageBox.Show($"Config file not found at:\n{_configPath}\n\nA default config will be created.", 
                    "Config Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            // Load emojis.json (reaction rules)
            if (File.Exists(_emojisPath))
            {
                var json = File.ReadAllText(_emojisPath, System.Text.Encoding.UTF8);
                var emojisConfig = JsonSerializer.Deserialize<EmojisConfig>(json, JsonOptions);
                if (emojisConfig != null)
                {
                    _emojisConfig = emojisConfig;
                }
            }
            else
            {
                // Create default emojis.json
                _emojisConfig = new EmojisConfig
                {
                    ReactionRules = new List<ReactionRule>(),
                    CustomEmojis = new List<string>(),
                    FrequentEmojis = new List<string> { "👋", "😊", "❤️", "💕", "😂", "🎉" }
                };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading config: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadLogo()
    {
        try
        {
            var baseDir = AppContext.BaseDirectory;
            var logoPath = Path.Combine(baseDir, "logo.png");
            
            // Try development path
            if (!File.Exists(logoPath))
            {
                logoPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "Assets", "logo.png"));
            }
            
            // Try solution root
            if (!File.Exists(logoPath))
            {
                logoPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "logo.png"));
            }
            
            if (File.Exists(logoPath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(logoPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                LogoImage.Source = bitmap;
            }
        }
        catch
        {
            // Logo couldn't be loaded, that's okay
        }
    }

    private void LanguageButton_Click(object sender, RoutedEventArgs e)
    {
        LocalizationManager.Instance.ToggleLanguage();
        UpdateLanguageButton();
        
        // Refresh pages
        InitializePages();
        
        var tag = ((ListBoxItem)NavigationList.SelectedItem)?.Tag?.ToString();
        if (!string.IsNullOrEmpty(tag))
        {
            NavigateToPage(tag);
        }
    }

    private void UpdateLanguageButton()
    {
        var langName = LocalizationManager.Instance.Strings.LanguageName;
        LanguageButton.Content = $"🌐 {langName}";
    }

    private void SaveConfig_Click(object sender, RoutedEventArgs e)
    {
        SaveConfig();
    }

    public void SaveConfig()
    {
        try
        {
            // Get updated values from pages
            _generalPage?.UpdateConfig(_config);
            _rulesPage?.UpdateConfig(_emojisConfig);

            // Ensure directory exists
            var dir = Path.GetDirectoryName(_configPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            
            // Save config.json (bot settings: token, prefix, settings)
            var configJson = JsonSerializer.Serialize(_config, JsonOptions);
            File.WriteAllText(_configPath, configJson, new System.Text.UTF8Encoding(false));
            
            // Save emojis.json (reaction rules, custom emojis, frequent emojis)
            var emojisJson = JsonSerializer.Serialize(_emojisConfig, JsonOptions);
            File.WriteAllText(_emojisPath, emojisJson, new System.Text.UTF8Encoding(false));
            
            // Debug: confirm save location
            System.Diagnostics.Debug.WriteLine($"Config saved to: {_configPath}");
            System.Diagnostics.Debug.WriteLine($"Emojis saved to: {_emojisPath}");
            
            SaveStatusText.Text = "✓ Saved!";
            SaveStatusText.Foreground = (Brush)FindResource("DiscordGreenBrush");
            
            // Show toast notification
            var strings = LocalizationManager.Instance.Strings;
            Toast.Show(strings.SavedSuccessfully, strings.ConfigSavedMessage, ToastType.Success);
            
            // Clear message after 3 seconds
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (s, e) =>
            {
                SaveStatusText.Text = "";
                timer.Stop();
            };
            timer.Start();
        }
        catch (Exception ex)
        {
            SaveStatusText.Text = "✗ Error saving";
            SaveStatusText.Foreground = (Brush)FindResource("DiscordRedBrush");
            
            var strings = LocalizationManager.Instance.Strings;
            Toast.Show(strings.ErrorOccurred, ex.Message, ToastType.Error);
        }
    }

    private void NavigationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Skip if pages not initialized yet
        if (_generalPage == null) return;
        
        if (NavigationList.SelectedItem is ListBoxItem item && item.Tag is string tag)
        {
            NavigateToPage(tag);
        }
    }

    private void NavigateToPage(string pageName)
    {
        // Skip if pages not initialized yet
        if (_generalPage == null || _rulesPage == null || _logsPage == null) return;
        
        MainFrame.Content = pageName switch
        {
            "General" => _generalPage,
            "Rules" => _rulesPage,
            "Logs" => _logsPage,
            _ => _generalPage
        };
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        StartBot();
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        StopBot();
    }

    private void StartBot()
    {
        try
        {
            // First save current config
            SaveConfig();

            // Find the bot project path
            var baseDir = AppContext.BaseDirectory;
            var botProjectPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "AutoReacto"));
            
            if (!Directory.Exists(botProjectPath))
            {
                botProjectPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "AutoReacto"));
            }

            if (!Directory.Exists(botProjectPath))
            {
                MessageBox.Show("Could not find AutoReacto project directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _botProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run",
                    WorkingDirectory = botProjectPath,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            _botProcess.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Dispatcher.Invoke(() => _logsPage?.AddLog(e.Data));
                    
                    // Check for connection status
                    if (e.Data.Contains("Bot connected as"))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var match = System.Text.RegularExpressions.Regex.Match(e.Data, @"Bot connected as (.+)");
                            if (match.Success)
                            {
                                BotNameText.Text = match.Groups[1].Value;
                            }
                            UpdateStatus(true);
                        });
                    }
                }
            };

            _botProcess.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Dispatcher.Invoke(() => _logsPage?.AddLog($"[ERROR] {e.Data}"));
                }
            };

            _botProcess.Exited += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    UpdateStatus(false);
                    BotNameText.Text = "Bot Stopped";
                });
            };

            _botProcess.Start();
            _botProcess.BeginOutputReadLine();
            _botProcess.BeginErrorReadLine();

            StatusText.Text = "Starting...";
            StatusIndicator.Fill = (Brush)FindResource("DiscordYellowBrush");
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            _logsPage?.AddLog("[Dashboard] Starting bot...");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error starting bot: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateStatus(false);
        }
    }

    private void StopBot()
    {
        try
        {
            if (_botProcess != null && !_botProcess.HasExited)
            {
                _botProcess.Kill(true);
                _botProcess.Dispose();
                _botProcess = null;
                _logsPage?.AddLog("[Dashboard] Bot stopped.");
            }
            
            UpdateStatus(false);
            BotNameText.Text = "Bot Stopped";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error stopping bot: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateStatus(bool isOnline)
    {
        if (isOnline)
        {
            StatusIndicator.Fill = (Brush)FindResource("DiscordGreenBrush");
            StatusText.Text = "Online";
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }
        else
        {
            StatusIndicator.Fill = (Brush)FindResource("DiscordRedBrush");
            StatusText.Text = "Offline";
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        StopBot();
        base.OnClosed(e);
    }
}
