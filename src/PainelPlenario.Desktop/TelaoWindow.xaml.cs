using System;
using System.Windows;
using System.Windows.Input;

namespace PainelPlenario.Desktop;

public partial class TelaoWindow : Window
{
    public TelaoWindow(IServiceProvider services)
    {
        // Compartilha a mesma caixa de serviços para acessar o mesmo PlenarioStateService
        Resources.Add("services", services);
        InitializeComponent();

        PreviewKeyDown += TelaoWindow_PreviewKeyDown;
    }

    private void TelaoWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F11)
        {
            if (WindowStyle == WindowStyle.None)
                SairTelaCheia();
            else
                EntrarTelaCheia();
        }
        else if (e.Key == Key.Escape && WindowStyle == WindowStyle.None)
        {
            SairTelaCheia();
        }
    }

    public void EntrarTelaCheia()
    {
        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.NoResize;
        WindowState = WindowState.Maximized;
    }

    public void SairTelaCheia()
    {
        WindowState = WindowState.Normal;
        ResizeMode = ResizeMode.CanResize;
        WindowStyle = WindowStyle.SingleBorderWindow;
    }
}