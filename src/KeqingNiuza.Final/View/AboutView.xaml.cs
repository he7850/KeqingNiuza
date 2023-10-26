using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using KeqingNiuza.Service;
using System.Drawing.Imaging;
using Mapster;

namespace KeqingNiuza.View
{
    /// <summary>
    /// AboutView.xaml 的交互逻辑
    /// </summary>
    public partial class AboutView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }





        public AboutView()
        {
            InitializeComponent();
            TextBlock_Version.Text = "版本：" + Service.Const.FileVersion;
        }


        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as Hyperlink;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri));
        }

        private unsafe Rectangle GetUntransparentRect(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int left = width, top = height, right = 0, bottom = 0;
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var ptr = (byte*)data.Scan0;
            var stride = data.Stride;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if ((*(ptr + 3) & 0xF0) != 0)
                    {
                        if (x < left)
                        {
                            left = x;
                        }
                        if (x > right)
                        {
                            right = x;
                        }
                        if (y < top)
                        {
                            top = y;
                        }
                        if (y > bottom)
                        {
                            bottom = y;
                        }
                    }
                    ptr += 4;
                }
                ptr += stride - width * 4;
            }
            bitmap.UnlockBits(data);
            return new Rectangle(left, top, right - left + 1, bottom - top + 1);
        }


    }
}
