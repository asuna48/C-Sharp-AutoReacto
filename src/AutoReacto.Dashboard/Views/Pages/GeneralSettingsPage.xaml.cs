using System.Windows;
using System.Windows.Controls;
using AutoReacto.Core.Models;

namespace AutoReacto.Dashboard.Views.Pages;

public partial class GeneralSettingsPage : Page
{
    private readonly BotConfig _config;
    private bool _tokenVisible = false;
    private string _actualToken = string.Empty;

    public GeneralSettingsPage(BotConfig config)
    {
        InitializeComponent();
        _config = config;
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Token (masked)
        _actualToken = _config.Token;
        TokenTextBox.Text = MaskToken(_actualToken);
        
        // Prefix
        PrefixTextBox.Text = _config.Prefix;
        
        // Settings
        IgnoreBotsCheckBox.IsChecked = !_config.Settings.IgnoreBots;
        IgnoreSelfCheckBox.IsChecked = !_config.Settings.IgnoreSelf;
        ReactionDelayTextBox.Text = _config.Settings.ReactionDelayMs.ToString();
        
        // Log Level
        foreach (ComboBoxItem item in LogLevelComboBox.Items)
        {
            if (item.Tag?.ToString() == _config.Settings.LogLevel)
            {
                LogLevelComboBox.SelectedItem = item;
                break;
            }
        }
    }

    private string MaskToken(string token)
    {
        if (string.IsNullOrEmpty(token) || token.Length < 10)
            return token;
        
        return token[..6] + new string('•', token.Length - 10) + token[^4..];
    }

    private void ShowTokenButton_Click(object sender, RoutedEventArgs e)
    {
        _tokenVisible = !_tokenVisible;
        
        if (_tokenVisible)
        {
            // First save any changes to actual token if user edited the masked version
            if (!TokenTextBox.Text.Contains('•'))
            {
                _actualToken = TokenTextBox.Text;
            }
            TokenTextBox.Text = _actualToken;
            ShowTokenButton.Content = "🙈";
        }
        else
        {
            // Save the visible token before masking
            _actualToken = TokenTextBox.Text;
            TokenTextBox.Text = MaskToken(_actualToken);
            ShowTokenButton.Content = "👁";
        }
    }

    public void UpdateConfig(BotConfig config)
    {
        // Token - use actual token if visible, or if user edited the textbox
        if (_tokenVisible || !TokenTextBox.Text.Contains('•'))
        {
            config.Token = TokenTextBox.Text;
        }
        else
        {
            config.Token = _actualToken;
        }
        
        config.Prefix = PrefixTextBox.Text;
        
        config.Settings.IgnoreBots = !(IgnoreBotsCheckBox.IsChecked ?? false);
        config.Settings.IgnoreSelf = !(IgnoreSelfCheckBox.IsChecked ?? false);
        
        if (int.TryParse(ReactionDelayTextBox.Text, out int delay))
        {
            config.Settings.ReactionDelayMs = delay;
        }
        
        if (LogLevelComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            config.Settings.LogLevel = selectedItem.Tag?.ToString() ?? "Information";
        }
    }
}
