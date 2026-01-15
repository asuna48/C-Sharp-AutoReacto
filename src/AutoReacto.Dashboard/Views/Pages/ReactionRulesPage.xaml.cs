using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutoReacto.Core.Models;
using AutoReacto.Dashboard.Controls;
using AutoReacto.Dashboard.Localization;
using AutoReacto.Dashboard.Models;
using AutoReacto.Dashboard.Utils;

namespace AutoReacto.Dashboard.Views.Pages;

public partial class ReactionRulesPage : Page
{
    private EmojisConfig _emojisConfig;
    private RuleViewModel? _selectedRule;
    private ObservableCollection<string> _selectedEmojis = new();
    private ObservableCollection<string> _selectedTriggerWords = new();
    private string _currentCategory = "😀 Yüzler";
    private Dictionary<string, string[]> _emojiCategories = DiscordEmojis.Categories;
    private EmojiStorage _emojiStorage;

    public ReactionRulesPage(EmojisConfig emojisConfig)
    {
        InitializeComponent();
        _emojisConfig = emojisConfig;
        _emojiStorage = EmojiStorage.Load();
        SelectedEmojisPanel.ItemsSource = _selectedEmojis;
        SelectedTriggerWordsPanel.ItemsSource = _selectedTriggerWords;
        InitializeEmojiPicker();
        LoadRules();
        
        // Setup placeholder visibility handlers
        TriggerWordInputTextBox.TextChanged += TriggerWordInputTextBox_TextChanged;
        CustomEmojiTextBox.TextChanged += CustomEmojiTextBox_TextChanged;
    }
    
    private void TriggerWordInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TriggerWordInputPlaceholder.Visibility = string.IsNullOrEmpty(TriggerWordInputTextBox.Text) 
            ? Visibility.Visible 
            : Visibility.Collapsed;
    }
    
    private void CustomEmojiTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        CustomEmojiPlaceholder.Visibility = string.IsNullOrEmpty(CustomEmojiTextBox.Text) 
            ? Visibility.Visible 
            : Visibility.Collapsed;
    }
    
    private void AddTriggerWord_Click(object sender, RoutedEventArgs e)
    {
        AddTriggerWordFromInput();
    }
    
    private void TriggerWordInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter)
        {
            AddTriggerWordFromInput();
            e.Handled = true;
        }
    }
    
    private void AddTriggerWordFromInput()
    {
        var word = TriggerWordInputTextBox.Text?.Trim();
        if (!string.IsNullOrEmpty(word) && !_selectedTriggerWords.Contains(word))
        {
            _selectedTriggerWords.Add(word);
            UpdateNoTriggerWordsVisibility();
            TriggerWordInputTextBox.Text = "";
        }
    }
    
    private void RemoveTriggerWord_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string word)
        {
            _selectedTriggerWords.Remove(word);
            UpdateNoTriggerWordsVisibility();
        }
    }
    
    private void UpdateNoTriggerWordsVisibility()
    {
        NoTriggerWordsText.Visibility = _selectedTriggerWords.Count == 0 
            ? Visibility.Visible 
            : Visibility.Collapsed;
    }

    private void InitializeEmojiPicker()
    {
        CategoryTabsPanel.Children.Clear();
        foreach (var category in _emojiCategories.Keys)
        {
            var emojiText = new Emoji.Wpf.TextBlock
            {
                Text = GetCategoryIcon(category),
                FontSize = 20,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };
            
            var border = new Border
            {
                Background = Brushes.Transparent,
                CornerRadius = new CornerRadius(6),
                Width = 40,
                Height = 36,
                Cursor = System.Windows.Input.Cursors.Hand,
                ToolTip = category.Length > 2 ? category.Substring(2).Trim() : category,
                Tag = category,
                Child = emojiText
            };
            
            border.MouseLeftButtonUp += CategoryTab_Click;
            border.MouseEnter += (s, e) => ((Border)s).Background = new SolidColorBrush(Color.FromRgb(0x3D, 0x3F, 0x45));
            border.MouseLeave += (s, e) => ((Border)s).Background = Brushes.Transparent;
            
            CategoryTabsPanel.Children.Add(border);
        }
        
        LoadEmojiCategory(_currentCategory);
    }

    private string GetCategoryIcon(string category)
    {
        // Category already contains emoji icon in the name
        if (category.Length > 0)
        {
            return category.Substring(0, Math.Min(2, category.Length));
        }
        return "📁";
    }

    private void CategoryButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string category)
        {
            _currentCategory = category;
            LoadEmojiCategory(category);
        }
    }

    private void CategoryTab_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Border border && border.Tag is string category)
        {
            _currentCategory = category;
            LoadEmojiCategory(category);
        }
    }

    private void LoadEmojiCategory(string category)
    {
        if (_emojiCategories.TryGetValue(category, out var emojis))
        {
            EmojiGrid.ItemsSource = emojis;
        }
    }

    private void EmojiSearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = EmojiSearchBox.Text?.ToLower() ?? "";
        
        if (string.IsNullOrWhiteSpace(searchText))
        {
            LoadEmojiCategory(_currentCategory);
            return;
        }
        
        // Search all categories
        var results = _emojiCategories.Values
            .SelectMany(emojis => emojis)
            .Distinct()
            .ToList();
        
        EmojiGrid.ItemsSource = results;
    }

    private void EmojiButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Content is string emoji)
        {
            AddEmojiToSelection(emoji);
        }
    }

    private void EmojiItem_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Border border && border.Tag is string emoji)
        {
            AddEmojiToSelection(emoji);
        }
    }

    private void AddEmojiButton_Click(object sender, RoutedEventArgs e)
    {
        EmojiPickerPopup.IsOpen = !EmojiPickerPopup.IsOpen;
    }

    private void AddCustomEmoji_Click(object sender, RoutedEventArgs e)
    {
        var customEmoji = CustomEmojiTextBox.Text?.Trim();
        if (!string.IsNullOrEmpty(customEmoji))
        {
            AddEmojiToSelection(customEmoji);
            
            // Track as custom emoji
            _emojiStorage.AddCustomEmoji(customEmoji);
            _emojiStorage.Save();
            
            CustomEmojiTextBox.Text = "";
        }
    }

    private void AddEmojiToSelection(string emoji)
    {
        if (!string.IsNullOrWhiteSpace(emoji) && !_selectedEmojis.Contains(emoji))
        {
            _selectedEmojis.Add(emoji);
            UpdateNoEmojisVisibility();
            
            // Track usage for frequently used list
            _emojiStorage.TrackEmojiUsage(emoji);
            
            // Close popup after selection
            EmojiPickerPopup.IsOpen = false;
        }
    }

    private void RemoveEmoji_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string emoji)
        {
            _selectedEmojis.Remove(emoji);
            UpdateNoEmojisVisibility();
        }
    }

    private void UpdateNoEmojisVisibility()
    {
        NoEmojisText.Visibility = _selectedEmojis.Count == 0 
            ? Visibility.Visible 
            : Visibility.Collapsed;
    }

    private void LoadRules()
    {
        var ruleViewModels = _emojisConfig.ReactionRules.Select(r => new RuleViewModel(r)).ToList();
        RulesListBox.ItemsSource = ruleViewModels;
    }

    private void RulesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RulesListBox.SelectedItem is RuleViewModel rule)
        {
            _selectedRule = rule;
            LoadRuleEditor(rule.Rule);
            RuleEditorPanel.Visibility = Visibility.Visible;
            NoSelectionPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void LoadRuleEditor(ReactionRule rule)
    {
        RuleNameTextBox.Text = rule.Name;
        RuleEnabledCheckBox.IsChecked = rule.Enabled;
        
        // Load trigger words into the collection
        _selectedTriggerWords.Clear();
        foreach (var word in rule.TriggerWords)
        {
            _selectedTriggerWords.Add(word);
        }
        UpdateNoTriggerWordsVisibility();
        
        // Load emojis into the collection
        _selectedEmojis.Clear();
        foreach (var emoji in rule.Emojis)
        {
            _selectedEmojis.Add(emoji);
        }
        UpdateNoEmojisVisibility();
        
        CaseSensitiveCheckBox.IsChecked = rule.CaseSensitive;
        
        // Match Mode
        foreach (ComboBoxItem item in MatchModeComboBox.Items)
        {
            if (item.Tag?.ToString() == rule.MatchMode.ToString())
            {
                MatchModeComboBox.SelectedItem = item;
                break;
            }
        }
        
        // IDs
        ChannelIdsTextBox.Text = string.Join(Environment.NewLine, rule.ChannelIds);
        UserIdsTextBox.Text = string.Join(Environment.NewLine, rule.UserIds);
        IgnoreUserIdsTextBox.Text = string.Join(Environment.NewLine, rule.IgnoreUserIds);
    }

    private void AddRule_Click(object sender, RoutedEventArgs e)
    {
        var newRule = new ReactionRule
        {
            Id = Guid.NewGuid().ToString(),
            Name = $"New Rule {_emojisConfig.ReactionRules.Count + 1}",
            Enabled = true,
            TriggerWords = new List<string> { "example" },
            Emojis = new List<string> { "👍" },
            MatchMode = MatchMode.Contains
        };
        
        _emojisConfig.ReactionRules.Add(newRule);
        LoadRules();
        
        // Select the new rule
        RulesListBox.SelectedIndex = RulesListBox.Items.Count - 1;
    }

    private async void DeleteRule_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRule == null) return;
        
        var strings = LocalizationManager.Instance.Strings;
        
        var result = await ConfirmDialog.ShowAsync(
            strings.DeleteRuleTitle,
            string.Format(strings.DeleteRuleMessage, _selectedRule.Name),
            strings.Delete,
            strings.Cancel,
            ConfirmDialogType.Danger);
        
        if (result)
        {
            _emojisConfig.ReactionRules.Remove(_selectedRule.Rule);
            LoadRules();
            
            RuleEditorPanel.Visibility = Visibility.Collapsed;
            NoSelectionPanel.Visibility = Visibility.Visible;
            _selectedRule = null;
            
            // Show toast notification
            ToastNotification.ShowToast(strings.RuleDeleted, strings.RuleDeletedMessage, ToastType.Info);
        }
    }

    private void ApplyRuleChanges_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRule == null) return;
        
        SaveCurrentRuleToModel();
        LoadRules();
        
        // Re-select the rule
        var index = _emojisConfig.ReactionRules.IndexOf(_selectedRule.Rule);
        if (index >= 0)
        {
            RulesListBox.SelectedIndex = index;
        }
        
        // Show toast notification
        var strings = LocalizationManager.Instance.Strings;
        ToastNotification.ShowToast(strings.RuleApplied, strings.RuleAppliedMessage, ToastType.Success);
    }

    private void SaveCurrentRuleToModel()
    {
        if (_selectedRule == null) return;
        
        var rule = _selectedRule.Rule;
        
        rule.Name = RuleNameTextBox.Text;
        rule.Enabled = RuleEnabledCheckBox.IsChecked ?? true;
        rule.CaseSensitive = CaseSensitiveCheckBox.IsChecked ?? false;
        
        // Trigger words - from the collection
        rule.TriggerWords = _selectedTriggerWords.ToList();
        
        // Emojis - from the collection
        rule.Emojis = _selectedEmojis.ToList();
        
        // Also save to separate emoji storage
        _emojiStorage.SetEmojisForRule(rule.Id, rule.Emojis);
        _emojiStorage.Save();
        
        // Match Mode
        if (MatchModeComboBox.SelectedItem is ComboBoxItem selectedMode)
        {
            if (Enum.TryParse<MatchMode>(selectedMode.Tag?.ToString(), out var mode))
            {
                rule.MatchMode = mode;
            }
        }
        
        // Channel IDs
        rule.ChannelIds = ParseUlongList(ChannelIdsTextBox.Text);
        
        // User IDs
        rule.UserIds = ParseUlongList(UserIdsTextBox.Text);
        
        // Ignore User IDs
        rule.IgnoreUserIds = ParseUlongList(IgnoreUserIdsTextBox.Text);
    }

    private List<ulong> ParseUlongList(string text)
    {
        return text
            .Split(new[] { Environment.NewLine, "\n", "," }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => ulong.TryParse(s, out _))
            .Select(s => ulong.Parse(s))
            .ToList();
    }

    public void UpdateConfig(EmojisConfig emojisConfig)
    {
        // Save current rule if one is selected
        SaveCurrentRuleToModel();
        
        // The config is already updated since we're modifying the same reference
        emojisConfig.ReactionRules = _emojisConfig.ReactionRules;
        emojisConfig.CustomEmojis = _emojisConfig.CustomEmojis;
        emojisConfig.FrequentEmojis = _emojisConfig.FrequentEmojis;
    }
}

public class RuleViewModel
{
    public ReactionRule Rule { get; }
    
    public string Name => Rule.Name;
    public string TriggerWordsPreview => string.Join(", ", Rule.TriggerWords.Take(3)) + 
                                         (Rule.TriggerWords.Count > 3 ? "..." : "");
    public Brush StatusColor => Rule.Enabled 
        ? new SolidColorBrush(Color.FromRgb(87, 242, 135))   // Green
        : new SolidColorBrush(Color.FromRgb(148, 155, 164)); // Gray

    public RuleViewModel(ReactionRule rule)
    {
        Rule = rule;
    }
}
