using System.Windows;

namespace AutoReacto.Dashboard;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Global exception handling
        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show($"Unhandled error:\n{args.Exception.Message}\n\n{args.Exception.StackTrace}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };
        
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"Fatal error:\n{ex.Message}\n\n{ex.StackTrace}", 
                    "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        };
        
        base.OnStartup(e);
    }
}
