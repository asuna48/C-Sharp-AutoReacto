using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutoReacto.Dashboard.Utils;

namespace AutoReacto.Dashboard.Controls;

public partial class EmojiPicker : UserControl
{
    public event EventHandler<string>? EmojiSelected;
    private string _currentCategory = "Frequent";

    public EmojiPicker()
    {
        InitializeComponent();
        CreateCategoryTabs();
        ShowCategory("Frequent");
    }

    private void CreateCategoryTabs()
    {
        // Add Frequent tab first
        var frequentTab = CreateCategoryTab("⭐", "Frequent");
        CategoryTabs.Children.Add(frequentTab);

        // Add category tabs
        foreach (var category in DiscordEmojis.Categories)
        {
            var emoji = category.Key.Split(' ')[0]; // Get emoji from category name
            var tab = CreateCategoryTab(emoji, category.Key);
            CategoryTabs.Children.Add(tab);
        }
    }

    private Button CreateCategoryTab(string emoji, string categoryKey)
    {
        var button = new Button
        {
            Content = emoji,
            FontSize = 18,
            Width = 36,
            Height = 32,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Cursor = System.Windows.Input.Cursors.Hand,
            Tag = categoryKey,
            Margin = new Thickness(2, 0, 2, 0)
        };

        button.Click += (s, e) =>
        {
            var btn = s as Button;
            if (btn?.Tag is string cat)
            {
                ShowCategory(cat);
            }
        };

        // Custom template for hover effect
        var template = new ControlTemplate(typeof(Button));
        var borderFactory = new FrameworkElementFactory(typeof(Border));
        borderFactory.Name = "border";
        borderFactory.SetValue(Border.BackgroundProperty, Brushes.Transparent);
        borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
        
        var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
        contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
        borderFactory.AppendChild(contentFactory);
        
        template.VisualTree = borderFactory;
        
        var trigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
        trigger.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(0x3F, 0x41, 0x47)), "border"));
        template.Triggers.Add(trigger);
        
        button.Template = template;

        return button;
    }

    private void ShowCategory(string categoryKey)
    {
        _currentCategory = categoryKey;
        
        string[] emojis;
        
        if (categoryKey == "Frequent")
        {
            emojis = DiscordEmojis.Frequent;
        }
        else if (DiscordEmojis.Categories.TryGetValue(categoryKey, out var categoryEmojis))
        {
            emojis = categoryEmojis;
        }
        else
        {
            emojis = DiscordEmojis.Frequent;
        }

        EmojiGrid.ItemsSource = emojis;
        
        // Update tab selection visual
        UpdateTabSelection(categoryKey);
    }

    private void UpdateTabSelection(string selectedCategory)
    {
        foreach (Button tab in CategoryTabs.Children)
        {
            if (tab.Tag is string cat)
            {
                var isSelected = cat == selectedCategory;
                tab.Background = isSelected 
                    ? new SolidColorBrush(Color.FromRgb(0x58, 0x65, 0xF2)) 
                    : Brushes.Transparent;
            }
        }
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = SearchBox.Text;
        SearchPlaceholder.Visibility = string.IsNullOrEmpty(searchText) ? Visibility.Visible : Visibility.Collapsed;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            ShowCategory(_currentCategory);
            return;
        }

        // Search all emojis - for now just show all
        var allEmojis = DiscordEmojis.GetAllEmojis().ToArray();
        EmojiGrid.ItemsSource = allEmojis;
    }

    private void Emoji_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Content is string emoji)
        {
            EmojiSelected?.Invoke(this, emoji);
        }
    }
}
