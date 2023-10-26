using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KeqingNiuza.Core.Wish;
using KeqingNiuza.ViewModel;

namespace KeqingNiuza.View
{
    /// <summary>
    /// WishItemDetailView.xaml 的交互逻辑
    /// </summary>
    public partial class WishItemDetailView : UserControl
    {

        public WishItemDetailView(List<ItemInfo> list)
        {
            InitializeComponent();
        }

        public WishItemDetailView(List<ItemInfo> list, ItemInfo selectedInfo)
        {
            InitializeComponent();
        }

    }
}
