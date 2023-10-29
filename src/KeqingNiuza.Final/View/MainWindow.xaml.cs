using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using KeqingNiuza.Service;
using KeqingNiuza.ViewModel;
using static KeqingNiuza.Service.Const;

namespace KeqingNiuza.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        public MainWindowViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
        }


        private void Window_Main_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowState = Properties.Settings.Default.IsWindowMaximized ? WindowState.Maximized : WindowState.Normal;
            }
            catch (Exception ex)
            {
                WindowState = WindowState.Normal;
                Log.OutputLog(LogType.Warning, "Window_Main_Loaded", ex);
            }
            // 如果软件窗口超出屏幕边界，则最大化
            if (Native.IsWindowBeyondBounds(ActualWidth, ActualHeight))
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Window_Main_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.IsWindowMaximized = WindowState == WindowState.Maximized;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Maxmize_Click(object sender, RoutedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
            }
        }

        private void RadioButton_SideMenu_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            var tag = radioButton.Tag as string;
            ViewModel.ChangeViewContent(tag);
        }
        private void RadioButton_Reload_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReloadViewContent();
        }

        private void Window_Main_StateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    BorderThickness = new Thickness(0);
                    break;
                case WindowState.Minimized:
                    BorderThickness = new Thickness(0);
                    break;
                case WindowState.Maximized:
                    BorderThickness = new Thickness(7);
                    break;
                default:
                    break;
            }
        }

    }
}
