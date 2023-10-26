using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using HandyControl.Controls;
using HandyControl.Tools.Extension;

using KeqingNiuza.Service;
using KeqingNiuza.View;
using Microsoft.Win32;
using static KeqingNiuza.Service.Const;

namespace KeqingNiuza.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowViewModel()
        {
            _viewContentList = new List<object>();
            Directory.CreateDirectory($"{UserDataPath}");
            ViewContent = new WelcomeView();
            _viewContentList = new List<object>();
            _timer = new System.Timers.Timer(1000);
            _timer.AutoReset = false;
            _timer.Start();
        }

        private readonly List<object> _viewContentList;
        private readonly System.Timers.Timer _timer;

        #region ControlProperty
        private object _ViewContent;
        public object ViewContent
        {
            get { return _ViewContent; }
            set
            {
                _ViewContent = value;
                OnPropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// 更换页面内容
        /// </summary>
        /// <param name="className">页面类名</param>
        public void ChangeViewContent(string className)
        {
            var assembly = Assembly.GetAssembly(GetType());
            var type = assembly.GetType($"KeqingNiuza.View.{className}");
            if (ViewContent?.GetType().Name != type.Name)
            {
                if (_viewContentList.Any(x => x.GetType().Name == type.Name))
                {
                    ViewContent = _viewContentList.First(x => x.GetType().Name == type.Name);
                }
                else
                {
                    try
                    {
                        ViewContent = assembly.CreateInstance(type.FullName);
                        _viewContentList.Add(ViewContent);
                    }
                    catch (Exception ex)
                    {
                        ViewContent = new ErrorView(ex);
                        Log.OutputLog(LogType.Warning, "ChangeViewContent", ex);
                    }
                }
            }
        }


        /// <summary>
        /// 重新加载内容页面
        /// </summary>
        public void ReloadViewContent()
        {
            var type = ViewContent.GetType();
            try
            {
                ViewContent = new WelcomeView();
                _viewContentList.Clear();
                _viewContentList.Add(ViewContent);
            }
            catch (Exception ex)
            {
                ViewContent = new ErrorView(ex);
                Log.OutputLog(LogType.Warning, "ReloadViewContent", ex);
            }

        }
    }
}
