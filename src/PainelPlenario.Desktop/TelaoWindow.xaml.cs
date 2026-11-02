using System;
using System.Windows;

namespace PainelPlenario.Desktop;

public partial class TelaoWindow : Window
{
    public TelaoWindow(IServiceProvider services)
    {
        // Compartilha a mesma caixa de serviços para acessar o mesmo PlenarioStateService
        Resources.Add("services", services);
        InitializeComponent();
    }
}