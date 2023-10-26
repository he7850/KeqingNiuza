using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Interactivity;
using HandyControl.Tools.Extension;
using KeqingNiuza.Core.CloudBackup;
using KeqingNiuza.Service;

namespace KeqingNiuza.View
{
    /// <summary>
    /// CloudLoginDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CloudLoginDialog : UserControl, IDialogResultable<CloudClient>
    {
        public CloudLoginDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public CloudClient Result { get; set; }
        public Action CloseAction { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ControlCommands.Close.Execute(null, this);

        }

    }
}
