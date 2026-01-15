# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.2.0] - 2026-01-15

### Added
- **WPF Dashboard Application** - Modern, Discord-themed GUI for managing the bot
- Multi-language support (Turkish/English) with runtime switching
- Visual emoji picker with categorized emojis (Smileys, Animals, Food, Activities, etc.)
- Real-time log viewer with filtering capabilities
- Toast notification system for user feedback
- Confirm dialog component for destructive actions
- General settings page with all bot configuration options
- Reaction rules management page with CRUD operations
- Bot status display with Start/Stop controls
- Dark mode Discord theme throughout the application

### Technical
- CommunityToolkit.MVVM for MVVM architecture
- Emoji.Wpf for native emoji rendering
- Costura.Fody for single-file deployment
- Localization system with resource dictionaries
- Custom WPF controls (EmojiPicker, ToastNotification, ConfirmDialog)

## [0.1.0] - 2024-01-15

### Added
- Initial release
- Discord.Net integration with bot connection
- JSON-based configuration system
- Word-based auto reaction triggers
- Multiple match modes: Contains, Exact, StartsWith, EndsWith, Regex
- Channel-based filtering
- User-based targeting and exclusions
- Comprehensive logging with Serilog (console + file)
- Rate limiting protection with configurable delays
- Modular architecture with Dependency Injection
- Hot-reload configuration support

### Technical
- .NET 8.0 target framework
- Discord.Net 3.13.0
- Serilog for structured logging
- Microsoft.Extensions.Hosting for service management
