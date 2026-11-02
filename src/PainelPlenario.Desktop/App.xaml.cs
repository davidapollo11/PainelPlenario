using System;
using System.Windows;

namespace PainelPlenario.Desktop;

public partial class App : Application
{
    public App()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            MessageBox.Show(e.ExceptionObject?.ToString(), "Erro Não Tratado");
        };

        DispatcherUnhandledException += (s, e) =>
        {
            MessageBox.Show(e.Exception.ToString(), "Erro Dispatcher");
        };
    }
}