﻿
using KeqingNiuza.Service;
using KeqingNiuza.View;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace KeqingNiuza
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Environment.CurrentDirectory = AppContext.BaseDirectory;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            if (e.Args.Length != 0)
            {
                Shutdown();
            }
            else
            {
                MainWindow = new MainWindow();
                MainWindow.Show();
            }

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            try
            {
                Log.OutputLog(LogType.Fault, "Crash", ex);
            }
            catch { }
            var msg = $"发生了未处理的错误";
            MessageBox.Show(msg, "KeqingNiuza");
        }
    }
}
