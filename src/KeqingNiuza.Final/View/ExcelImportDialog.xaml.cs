using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControl.Interactivity;
using HandyControl.Tools.Extension;
using KeqingNiuza.Core.Wish;
using KeqingNiuza.Model;
using KeqingNiuza.Service;
using UserControl = System.Windows.Controls.UserControl;

namespace KeqingNiuza.View
{
    /// <summary>
    /// ExcelImportView.xaml 的交互逻辑
    /// </summary>
    public partial class ExcelImportDialog : UserControl, INotifyPropertyChanged, IDialogResultable<bool>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool Result { get; set; }
        public Action CloseAction { get; set; }

        public ExcelImportDialog()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            ControlCommands.Close.Execute(null, this);
        }

    }
}
