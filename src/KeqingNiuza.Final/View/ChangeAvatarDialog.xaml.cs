using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HandyControl.Interactivity;
using HandyControl.Tools.Extension;

namespace KeqingNiuza.View
{
    /// <summary>
    /// ChangeAvatarDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ChangeAvatarDialog : UserControl, IDialogResultable<string>
    {
        public ChangeAvatarDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Result { get; set; }
        public Action CloseAction { get; set; }

    }
}
