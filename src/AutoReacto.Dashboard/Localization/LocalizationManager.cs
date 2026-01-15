using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace AutoReacto.Dashboard.Localization;

public class LocalizationManager : INotifyPropertyChanged
{
    private static LocalizationManager? _instance;
    public static LocalizationManager Instance => _instance ??= new LocalizationManager();

    private string _currentLanguage = "en";
    
    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Strings));
            }
        }
    }

    public LocalizedStrings Strings => CurrentLanguage == "tr" ? Turkish : English;

    public static LocalizedStrings English { get; } = new()
    {
        // Main Window
        AppTitle = "AutoReacto",
        DashboardVersion = "Dashboard v0.1.0",
        Offline = "Offline",
        Online = "Online",
        NotConnected = "Not Connected",
        Start = "▶ Start",
        Stop = "■ Stop",
        Menu = "MENU",
        GeneralSettings = "⚙️  General Settings",
        ReactionRules = "📜  Reaction Rules",
        Logs = "📊  Logs",
        SaveChanges = "💾 Save Changes",
        
        // General Settings Page
        BotConfiguration = "🤖 Bot Configuration",
        BotToken = "BOT TOKEN",
        BotTokenPlaceholder = "Enter your Discord bot token",
        CommandPrefix = "COMMAND PREFIX",
        GlobalSettings = "⚙️ Global Settings",
        ReactToSelf = "React to bot's own messages",
        ReactToBots = "React to other bots' messages",
        LogLevel = "LOG LEVEL",
        ReactionDelay = "REACTION DELAY (MS)",
        
        // Reaction Rules Page
        ReactionRulesTitle = "📜 Reaction Rules",
        AddNewRule = "+ Add New Rule",
        SelectRuleToEdit = "Select a rule from the list to edit",
        RuleConfiguration = "🎯 Rule Configuration",
        RuleName = "RULE NAME",
        Enabled = "Enabled",
        TriggerWords = "TRIGGER WORDS",
        TriggerWordsDesc = "Words or phrases that trigger reactions",
        NoTriggerWordsSelected = "No trigger words - add words below",
        AddTriggerWordPlaceholder = "Type a word and press Enter...",
        MatchMode = "MATCH MODE",
        Options = "OPTIONS",
        CaseSensitive = "Case Sensitive",
        Emojis = "EMOJIS",
        EmojisDesc = "Click to add Discord emojis or type custom emoji IDs",
        NoEmojisSelected = "No emojis selected - click the button below to add",
        AddEmoji = "🎉 Add Emoji",
        CustomEmoji = "CUSTOM EMOJI (Discord format: <:name:id>)",
        Add = "Add",
        FiltersOptional = "FILTERS (Optional)",
        ChannelIds = "CHANNEL IDS",
        LeaveEmptyForAllChannels = "Leave empty for all channels",
        UserIdsTarget = "USER IDS (Target)",
        LeaveEmptyForAllUsers = "Leave empty for all users",
        IgnoreUserIds = "IGNORE USER IDS",
        UsersToIgnore = "Users to ignore",
        DeleteRule = "🗑️ Delete Rule",
        ApplyChanges = "✓ Apply Changes",
        
        // Match Modes
        Contains = "Contains",
        ExactMatch = "Exact Match",
        StartsWith = "Starts With",
        EndsWith = "Ends With",
        Regex = "Regex",
        
        // Logs Page
        BotLogs = "📊 Bot Logs",
        RealTimeLogs = "Real-time logs from the bot",
        ClearLogs = "🗑️ Clear Logs",
        
        // Common
        Debug = "Debug",
        Information = "Information",
        Warning = "Warning",
        Error = "Error",
        
        // Language
        Language = "LANGUAGE",
        LanguageName = "English",
        
        // Toast Notifications
        SavedSuccessfully = "Saved Successfully",
        ConfigSavedMessage = "Your configuration has been saved.",
        ErrorOccurred = "Error Occurred",
        RuleApplied = "Rule Applied",
        RuleAppliedMessage = "Rule changes have been applied.",
        RuleDeleted = "Rule Deleted",
        RuleDeletedMessage = "The rule has been deleted.",
        EmojiAdded = "Emoji Added",
        
        // Confirm Dialog
        DeleteRuleTitle = "Delete Rule?",
        DeleteRuleMessage = "Are you sure you want to delete '{0}'? This action cannot be undone.",
        Delete = "Delete",
        Cancel = "Cancel",
        
        // Placeholders
        TriggerWordsPlaceholder = "hello\ngoodbye\nwelcome",
        TriggerWordsHint = "⚠️ Each word on a separate line. Spaces matter! 'hello world' ≠ 'helloworld'",
        CustomEmojiPlaceholder = "<:emoji_name:123456789012345678>",
        CustomEmojiHint = "Format: <:name:id> or <a:name:id> for animated. Get ID by typing \\:emoji: in Discord"
    };

    public static LocalizedStrings Turkish { get; } = new()
    {
        // Main Window
        AppTitle = "AutoReacto",
        DashboardVersion = "Dashboard v0.1.0",
        Offline = "Çevrimdışı",
        Online = "Çevrimiçi",
        NotConnected = "Bağlı Değil",
        Start = "▶ Başlat",
        Stop = "■ Durdur",
        Menu = "MENÜ",
        GeneralSettings = "⚙️  Genel Ayarlar",
        ReactionRules = "📜  Tepki Kuralları",
        Logs = "📊  Günlükler",
        SaveChanges = "💾 Değişiklikleri Kaydet",
        
        // General Settings Page
        BotConfiguration = "🤖 Bot Yapılandırması",
        BotToken = "BOT TOKEN",
        BotTokenPlaceholder = "Discord bot tokeninizi girin",
        CommandPrefix = "KOMUT ÖN EKİ",
        GlobalSettings = "⚙️ Genel Ayarlar",
        ReactToSelf = "Botun kendi mesajlarına tepki ver",
        ReactToBots = "Diğer botların mesajlarına tepki ver",
        LogLevel = "GÜNLÜK SEVİYESİ",
        ReactionDelay = "TEPKİ GECİKMESİ (MS)",
        
        // Reaction Rules Page
        ReactionRulesTitle = "📜 Tepki Kuralları",
        AddNewRule = "+ Yeni Kural Ekle",
        SelectRuleToEdit = "Düzenlemek için listeden bir kural seçin",
        RuleConfiguration = "🎯 Kural Yapılandırması",
        RuleName = "KURAL ADI",
        Enabled = "Aktif",
        TriggerWords = "TETİKLEYİCİ KELİMELER",
        TriggerWordsDesc = "Tepkileri tetikleyen kelimeler veya ifadeler",
        NoTriggerWordsSelected = "Tetikleyici kelime yok - aşağıdan ekleyin",
        AddTriggerWordPlaceholder = "Bir kelime yazın ve Enter'a basın...",
        MatchMode = "EŞLEŞTİRME MODU",
        Options = "SEÇENEKLER",
        CaseSensitive = "Büyük/Küçük Harf Duyarlı",
        Emojis = "EMOJİLER",
        EmojisDesc = "Discord emojileri eklemek için tıklayın veya özel emoji ID'leri yazın",
        NoEmojisSelected = "Emoji seçilmedi - eklemek için aşağıdaki butona tıklayın",
        AddEmoji = "🎉 Emoji Ekle",
        CustomEmoji = "ÖZEL EMOJİ (Discord formatı: <:ad:id>)",
        Add = "Ekle",
        FiltersOptional = "FİLTRELER (İsteğe Bağlı)",
        ChannelIds = "KANAL ID'LERİ",
        LeaveEmptyForAllChannels = "Tüm kanallar için boş bırakın",
        UserIdsTarget = "KULLANICI ID'LERİ (Hedef)",
        LeaveEmptyForAllUsers = "Tüm kullanıcılar için boş bırakın",
        IgnoreUserIds = "YOKSAYILACAK KULLANICI ID'LERİ",
        UsersToIgnore = "Yoksayılacak kullanıcılar",
        DeleteRule = "🗑️ Kuralı Sil",
        ApplyChanges = "✓ Değişiklikleri Uygula",
        
        // Match Modes
        Contains = "İçerir",
        ExactMatch = "Tam Eşleşme",
        StartsWith = "İle Başlar",
        EndsWith = "İle Biter",
        Regex = "Regex",
        
        // Logs Page
        BotLogs = "📊 Bot Günlükleri",
        RealTimeLogs = "Bottan gelen gerçek zamanlı günlükler",
        ClearLogs = "🗑️ Günlükleri Temizle",
        
        // Common
        Debug = "Hata Ayıklama",
        Information = "Bilgi",
        Warning = "Uyarı",
        Error = "Hata",
        
        // Language
        Language = "DİL",
        LanguageName = "Türkçe",
        
        // Toast Notifications
        SavedSuccessfully = "Başarıyla Kaydedildi",
        ConfigSavedMessage = "Yapılandırmanız kaydedildi.",
        ErrorOccurred = "Hata Oluştu",
        RuleApplied = "Kural Uygulandı",
        RuleAppliedMessage = "Kural değişiklikleri uygulandı.",
        RuleDeleted = "Kural Silindi",
        RuleDeletedMessage = "Kural silindi.",
        EmojiAdded = "Emoji Eklendi",
        
        // Confirm Dialog
        DeleteRuleTitle = "Kuralı Sil?",
        DeleteRuleMessage = "'{0}' kuralını silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.",
        Delete = "Sil",
        Cancel = "İptal",
        
        // Placeholders
        TriggerWordsPlaceholder = "merhaba\ngüle güle\nhoş geldin",
        TriggerWordsHint = "⚠️ Her kelime ayrı satırda. Boşluklar önemli! 'merhaba dünya' ≠ 'merhabadünya'",
        CustomEmojiPlaceholder = "<:emoji_adi:123456789012345678>",
        CustomEmojiHint = "Format: <:ad:id> veya animasyonlu için <a:ad:id>. ID için Discord'da \\:emoji: yazın"
    };

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ToggleLanguage()
    {
        CurrentLanguage = CurrentLanguage == "en" ? "tr" : "en";
    }
}

public class LocalizedStrings
{
    // Main Window
    public string AppTitle { get; set; } = "";
    public string DashboardVersion { get; set; } = "";
    public string Offline { get; set; } = "";
    public string Online { get; set; } = "";
    public string NotConnected { get; set; } = "";
    public string Start { get; set; } = "";
    public string Stop { get; set; } = "";
    public string Menu { get; set; } = "";
    public string GeneralSettings { get; set; } = "";
    public string ReactionRules { get; set; } = "";
    public string Logs { get; set; } = "";
    public string SaveChanges { get; set; } = "";
    
    // General Settings Page
    public string BotConfiguration { get; set; } = "";
    public string BotToken { get; set; } = "";
    public string BotTokenPlaceholder { get; set; } = "";
    public string CommandPrefix { get; set; } = "";
    public string GlobalSettings { get; set; } = "";
    public string ReactToSelf { get; set; } = "";
    public string ReactToBots { get; set; } = "";
    public string LogLevel { get; set; } = "";
    public string ReactionDelay { get; set; } = "";
    
    // Reaction Rules Page
    public string ReactionRulesTitle { get; set; } = "";
    public string AddNewRule { get; set; } = "";
    public string SelectRuleToEdit { get; set; } = "";
    public string RuleConfiguration { get; set; } = "";
    public string RuleName { get; set; } = "";
    public string Enabled { get; set; } = "";
    public string TriggerWords { get; set; } = "";
    public string TriggerWordsDesc { get; set; } = "";
    public string NoTriggerWordsSelected { get; set; } = "";
    public string AddTriggerWordPlaceholder { get; set; } = "";
    public string MatchMode { get; set; } = "";
    public string Options { get; set; } = "";
    public string CaseSensitive { get; set; } = "";
    public string Emojis { get; set; } = "";
    public string EmojisDesc { get; set; } = "";
    public string NoEmojisSelected { get; set; } = "";
    public string AddEmoji { get; set; } = "";
    public string CustomEmoji { get; set; } = "";
    public string Add { get; set; } = "";
    public string FiltersOptional { get; set; } = "";
    public string ChannelIds { get; set; } = "";
    public string LeaveEmptyForAllChannels { get; set; } = "";
    public string UserIdsTarget { get; set; } = "";
    public string LeaveEmptyForAllUsers { get; set; } = "";
    public string IgnoreUserIds { get; set; } = "";
    public string UsersToIgnore { get; set; } = "";
    public string DeleteRule { get; set; } = "";
    public string ApplyChanges { get; set; } = "";
    
    // Match Modes
    public string Contains { get; set; } = "";
    public string ExactMatch { get; set; } = "";
    public string StartsWith { get; set; } = "";
    public string EndsWith { get; set; } = "";
    public string Regex { get; set; } = "";
    
    // Logs Page
    public string BotLogs { get; set; } = "";
    public string RealTimeLogs { get; set; } = "";
    public string ClearLogs { get; set; } = "";
    
    // Common
    public string Debug { get; set; } = "";
    public string Information { get; set; } = "";
    public string Warning { get; set; } = "";
    public string Error { get; set; } = "";
    
    // Language
    public string Language { get; set; } = "";
    public string LanguageName { get; set; } = "";
    
    // Toast Notifications
    public string SavedSuccessfully { get; set; } = "";
    public string ConfigSavedMessage { get; set; } = "";
    public string ErrorOccurred { get; set; } = "";
    public string RuleApplied { get; set; } = "";
    public string RuleAppliedMessage { get; set; } = "";
    public string RuleDeleted { get; set; } = "";
    public string RuleDeletedMessage { get; set; } = "";
    public string EmojiAdded { get; set; } = "";
    
    // Confirm Dialog
    public string DeleteRuleTitle { get; set; } = "";
    public string DeleteRuleMessage { get; set; } = "";
    public string Delete { get; set; } = "";
    public string Cancel { get; set; } = "";
    
    // Placeholders
    public string TriggerWordsPlaceholder { get; set; } = "";
    public string TriggerWordsHint { get; set; } = "";
    public string CustomEmojiPlaceholder { get; set; } = "";
    public string CustomEmojiHint { get; set; } = "";
}
