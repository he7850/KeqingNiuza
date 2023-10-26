using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using HandyControl.Controls;
using KeqingNiuza.Core.CloudBackup;
using KeqingNiuza.Service;
using static KeqingNiuza.Service.Const;

namespace KeqingNiuza.View
{
    /// <summary>
    /// DialyCheckView.xaml 的交互逻辑
    /// </summary>
    public partial class DailyCheckView : UserControl
    {
        public DailyCheckView()
        {
            InitializeComponent();
            Loaded += DialyCheckView_Loaded;
        }


        private string cookies;


        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as Hyperlink;
            if (string.IsNullOrWhiteSpace(link.NavigateUri.OriginalString))
            {
                return;
            }
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
        }


        private void DialyCheckView_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($@"{UserDataPath}\DailyCheckCookies"))
            {
                try
                {
                    var bytes = File.ReadAllBytes($@"{UserDataPath}\DailyCheckCookies");
                    cookies = Endecryption.Decrypt(bytes);
                }
                catch (Exception ex)
                {

                    Growl.Warning(ex.Message);
                    Log.OutputLog(LogType.Warning, "LoadCookies", ex);
                }
            }
        }



    }
}
