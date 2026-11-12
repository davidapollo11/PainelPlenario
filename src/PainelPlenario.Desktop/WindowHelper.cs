using System.Windows;

namespace PainelPlenario.Desktop;

public static class WindowHelper
{
    private static TelaoWindow? _telaoWindow;

    public static void AbrirTelao(IServiceProvider services)
    {
        if (_telaoWindow == null || !_telaoWindow.IsLoaded)
        {
            _telaoWindow = new TelaoWindow(services);
            _telaoWindow.Show();
        }
        else
        {
            if (_telaoWindow.WindowState == WindowState.Minimized)
            {
                _telaoWindow.WindowState = WindowState.Normal;
            }
            _telaoWindow.Activate();
        }
    }
}