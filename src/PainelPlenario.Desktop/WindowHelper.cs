using System.Windows;

namespace PainelPlenario.Desktop;

public static class WindowHelper
{
    private static TelaoWindow? _telaoWindow;

    public static void AbrirTelao(IServiceProvider services)
    {
        var telas = System.Windows.Forms.Screen.AllScreens;
        var telaAlvo = telas.FirstOrDefault(s => !s.Primary)
                       ?? System.Windows.Forms.Screen.PrimaryScreen;

        if (_telaoWindow == null || !_telaoWindow.IsLoaded)
        {
            _telaoWindow = new TelaoWindow(services);
            _telaoWindow.Closed += (_, _) => _telaoWindow = null;

            if (telaAlvo != null)
            {
                // Posição inicial aproximada antes de criar a janela
                _telaoWindow.WindowState = WindowState.Normal;
                _telaoWindow.Left = telaAlvo.Bounds.Left;
                _telaoWindow.Top = telaAlvo.Bounds.Top;
            }

            _telaoWindow.Show();

            if (telaAlvo != null)
            {
                PosicionarETelaCheia(_telaoWindow, telaAlvo);
            }
            else
            {
                _telaoWindow.EntrarTelaCheia();
            }
        }
        else
        {
            if (_telaoWindow.WindowState == WindowState.Minimized)
            {
                _telaoWindow.WindowState = WindowState.Normal;
            }

            if (telaAlvo != null)
            {
                PosicionarETelaCheia(_telaoWindow, telaAlvo);
            }

            _telaoWindow.Activate();
        }
    }

    private static void PosicionarETelaCheia(TelaoWindow janela, System.Windows.Forms.Screen tela)
    {
        var area = tela.Bounds;

        // Converte pixels físicos da tela para DIPs do WPF (evita erro quando há zoom de DPI diferente entre monitores)
        var source = PresentationSource.FromVisual(janela);
        var transform = source?.CompositionTarget?.TransformFromDevice;

        if (transform.HasValue)
        {
            var topoEsquerdo = transform.Value.Transform(new System.Windows.Point(area.Left, area.Top));
            var tamanho = transform.Value.Transform(new System.Windows.Point(area.Width, area.Height));

            janela.WindowState = WindowState.Normal;
            janela.Left = topoEsquerdo.X;
            janela.Top = topoEsquerdo.Y;
            janela.Width = tamanho.X;
            janela.Height = tamanho.Y;
        }
        else
        {
            janela.WindowState = WindowState.Normal;
            janela.Left = area.Left;
            janela.Top = area.Top;
            janela.Width = area.Width;
            janela.Height = area.Height;
        }

        janela.EntrarTelaCheia();
    }
}