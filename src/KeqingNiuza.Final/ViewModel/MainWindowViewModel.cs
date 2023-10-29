using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using HandyControl.Controls;
using HandyControl.Tools.Extension;

using KeqingNiuza.Service;
using KeqingNiuza.View;
using KeqingNiuza.Core.Midi;
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
            // 启动时创建用户数据目录
            Directory.CreateDirectory($"{UserDataPath}");
            // 启动时默认使用欢迎页
            ViewContent = new WelcomeView();
            // 初始化右侧页面实例列表
            _viewContentList = new List<object>();
        }

        // 右侧页面实例列表
        private readonly List<object> _viewContentList;

        #region ControlProperty
        // 右侧页面
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
        /// 更换右侧页面内容
        /// </summary>
        /// <param name="className">页面类名</param>
        public void ChangeViewContent(string className)
        {
            var assembly = Assembly.GetAssembly(GetType());
            var type = assembly.GetType($"KeqingNiuza.View.{className}");
            // 如果当前view不是选中的view（按类名匹配是否一样），则切换view
            if (ViewContent?.GetType().Name != type.Name)
            {
                // 如果view已在实例列表里，则直接获取，否则创建一个，并添加到实例列表里
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
        /// 重新加载右侧页面，并打开欢迎页
        /// </summary>
        public void ReloadViewContent()
        {
            try
            {
                // 停止midi演奏，注销热键
                var type = Assembly.GetAssembly(GetType()).GetType($"KeqingNiuza.View.MidiView");
                foreach (var view in _viewContentList)
                {
                    if (view?.GetType().Name == type.Name)
                    {
                        (view as MidiView).ViewModel.IsPlaying = false;
                    }
                }
                _ = Util.UnregisterHotKey(Process.GetCurrentProcess().MainWindowHandle);
                // 重新初始化view列表
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
