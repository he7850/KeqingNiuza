using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KeqingNiuza.Core.Wish;
using KeqingNiuza.Model;
using KeqingNiuza.Service;
using KeqingNiuza.ViewModel;

namespace KeqingNiuza.View
{
    /// <summary>
    /// WishlogBackupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WishlogBackupWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WishlogBackupWindow()
        {
            InitializeComponent();
            UserDataList = MainWindowViewModel.GetUserDataList();
        }


        private List<UserData> _UserDataList;
        public List<UserData> UserDataList
        {
            get { return _UserDataList; }
            set
            {
                _UserDataList = value;
                OnPropertyChanged();
            }
        }


    }
}
