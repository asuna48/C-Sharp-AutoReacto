using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AutoReacto.Dashboard.Utils;

/// <summary>
/// Helper for enabling dark mode title bar on Windows 10/11
/// </summary>
public static class DarkModeHelper
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    /// <summary>
    /// Enables dark mode for the window title bar
    /// </summary>
    public static void EnableDarkMode(Window window)
    {
        try
        {
            var hwnd = new WindowInteropHelper(window).EnsureHandle();
            
            int darkMode = 1;
            
            // Try newer attribute first (Windows 10 20H1+)
            if (DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int)) != 0)
            {
                // Fall back to older attribute
                DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, ref darkMode, sizeof(int));
            }
        }
        catch
        {
            // Silently fail on unsupported systems
        }
    }
}
