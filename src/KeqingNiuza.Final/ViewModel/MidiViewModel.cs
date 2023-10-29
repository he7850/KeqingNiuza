using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Interop;
using HandyControl.Controls;
using KeqingNiuza.Core.Midi;
using KeqingNiuza.Service;

namespace KeqingNiuza.ViewModel
{
    public class MidiViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<MidiFileInfo> _MidiFileInfoList;
        /// <summary>
        /// midi文件信息列表
        /// </summary>
        public ObservableCollection<MidiFileInfo> MidiFileInfoList
        {
            get { return _MidiFileInfoList; }
            set
            {
                _MidiFileInfoList = value;
                OnPropertyChanged();
            }
        }

        private MidiFileInfo _SelectedMidiFile;
        /// <summary>
        /// 已选中midi文件
        /// </summary>
        public MidiFileInfo SelectedMidiFile
        {
            get { return _SelectedMidiFile; }
            set
            {
                _SelectedMidiFile = value;
                OnPropertyChanged();
            }
        }

        private bool _IsAdmin;
        /// <summary>
        /// 是否已开启管理员权限
        /// </summary>
        public bool IsAdmin
        {
            get { return _IsAdmin; }
            set
            {
                _IsAdmin = value;
                OnPropertyChanged();
            }
        }

 
        private bool _CanPlay;
        /// <summary>
        /// 是否可以演奏
        /// </summary>
        public bool CanPlay
        {
            get { return _CanPlay; }
            set
            {
                _CanPlay = value;
                OnPropertyChanged();
            }
        }

        #region ControlProperties
        
        private string _StateText;
        /// <summary>
        /// midi演奏器状态文本
        /// </summary>
        public string StateText
        {
            get { return _StateText; }
            set
            {
                _StateText = value;
                OnPropertyChanged();
            }
        }

        
        private string _Button_Restart_Content;
        /// <summary>
        /// 重启按钮文本
        /// </summary>
        public string Button_Restart_Content
        {
            get { return _Button_Restart_Content; }
            set
            {
                _Button_Restart_Content = value;
                OnPropertyChanged();
            }
        }

        
        private string _TextBlock_Color;
        /// <summary>
        /// 重启按钮文本颜色
        /// </summary>
        public string TextBlock_Color
        {
            get { return _TextBlock_Color; }
            set
            {
                _TextBlock_Color = value;
                OnPropertyChanged();
            }
        }

        
        private string _Tooltip_Content;
        /// <summary>
        /// 建议文本
        /// </summary>
        public string Tooltip_Content
        {
            get { return _Tooltip_Content; }
            set
            {
                _Tooltip_Content = value;
                OnPropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// midi演奏器当前曲名
        /// </summary>
        public string Name => MidiPlayer.Name;

        /// <summary>
        /// 是否正在演奏
        /// </summary>
        public bool IsPlaying
        {
            get { return MidiPlayer?.IsPlaying ?? false; }
            set
            {
                MidiPlayer.IsPlaying = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否自动切换到原神窗口
        /// </summary>
        public bool AutoSwitchToGenshinWindow
        {
            get { return MidiPlayer.AutoSwitchToGenshinWindow; }
            set
            {
                MidiPlayer.AutoSwitchToGenshinWindow = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否在后台演奏
        /// </summary>
        public bool PlayBackground
        {
            get { return MidiPlayer.PlayBackground; }
            set
            {
                MidiPlayer.PlayBackground = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 演奏速度
        /// </summary>
        public double Speed
        {
            get { return MidiPlayer.Speed; }
            set
            {
                MidiPlayer.Speed = value;
                timer.Interval = 1000 / value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 升降调偏移数，X表示向上偏移X个半音
        /// </summary>
        public int NoteLevel
        {
            get { return MidiPlayer.NoteLevel; }
            set
            {
                MidiPlayer.NoteLevel = value;
                RefreshMidiFileInfoByNoteLevel(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// midi总时长
        /// </summary>
        public TimeSpan TotalTime => MidiPlayer.TotalTime;

        /// <summary>
        /// midi演奏当前时间
        /// </summary>
        public TimeSpan CurrentTime
        {
            get { return MidiPlayer.CurrentTime; }
            set
            {
                MidiPlayer.CurrentTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否已注册热键
        /// </summary>
        private bool hotkey;
        /// <summary>
        /// 当前程序窗口句柄
        /// </summary>
        private readonly IntPtr hWnd;
        /// <summary>
        /// 当前程序句柄源
        /// </summary>
        private readonly HwndSource hwndSource;
        /// <summary>
        /// midi演奏器实例
        /// </summary>
        private static MidiPlayer MidiPlayer;
        /// <summary>
        /// 计时器，用来更新演奏时间和演奏状态
        /// </summary>
        private Timer timer;

        public MidiViewModel()
        {
            // 记录 Resource/Midi 目录下的midi文件，按名字排序
            List<string> files;
            if (Directory.Exists("Resource\\Midi"))
            {
                files = Directory.GetFiles("Resource\\Midi").ToList();
            }
            else
            {
                throw new Exception("Resource\\Midi 文件夹内没有Midi文件");
            }
            if (files.Count == 0)
            {
                throw new Exception("Resource\\Midi 文件夹内没有Midi文件");
            }
            var infos = files.ConvertAll(x => new MidiFileInfo(x)).OrderBy(x => x.Name);
            MidiFileInfoList = new ObservableCollection<MidiFileInfo>(infos);
            // 初始化midi演奏器，寻找原神或WindsongLyre窗口
            MidiPlayer = new MidiPlayer(new List<string> { "YuanShen", "GenshinImpact", "WindsongLyre" });
            // 注册MidiPlayer回调
            MidiPlayer.Started += MidiPlayer_Started;
            MidiPlayer.Stopped += MidiPlayer_Stopped;
            MidiPlayer.Finished += MidiPlayer_Finished;
            // 设置默认选中为第一个midi文件
            SelectedMidiFile = MidiFileInfoList.First();
            ChangePlayFile(SelectedMidiFile, false);
            // 获取当前程序窗口句柄
            hWnd = Process.GetCurrentProcess().MainWindowHandle;
            // 注册热键
            hotkey = Util.RegisterHotKey(hWnd);
            // 添加热键回调
            hwndSource = HwndSource.FromHwnd(hWnd);
            hwndSource.AddHook(HwndHook);
            // 刷新midi演奏器状态
            RefreshState();
            // 启动计时器，间隔1000ms，注册回调
            timer = new Timer(1000);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        /// 移调后，刷新midi的所有track
        /// </summary>
        /// <param name="noteLevel"></param>
        public void RefreshMidiFileInfoByNoteLevel(int noteLevel)
        {
            foreach (var item in MidiFileInfoList)
            {
                item.RefreshTracksByNoteLevel(noteLevel);
            }
            // 通知选中midi和全局变化
            //OnPropertyChanged("SelectedMidiFile");
            //OnPropertyChanged();
            var info = SelectedMidiFile;
            SelectedMidiFile = null;
            SelectedMidiFile = info;
        }

        /// <summary>
        /// 计时器启动，通知演奏时间和演奏状态变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MidiPlayer_Started(object sender, EventArgs e)
        {
            timer.Start();
            OnPropertyChanged("IsPlaying");
            OnPropertyChanged("CurrentTime");
        }
        /// <summary>
        /// 计时器暂停，通知演奏时间和演奏状态变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MidiPlayer_Stopped(object sender, EventArgs e)
        {
            timer.Stop();
            OnPropertyChanged("IsPlaying");
            OnPropertyChanged("CurrentTime");
        }
        /// <summary>
        /// 计时器结束，通知演奏时间和演奏状态变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MidiPlayer_Finished(object sender, EventArgs e)
        {
            timer.Stop();
            OnPropertyChanged("IsPlaying");
            OnPropertyChanged("CurrentTime");
        }

        /// <summary>
        /// 计时器时间流逝，通知IsPlaying和CurrentTime属性变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged("IsPlaying");
            OnPropertyChanged("CurrentTime");
        }

        /// <summary>
        /// 更新midi演奏曲目，通知Name、IsPlaying等属性变化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="autoPlay"></param>
        public void ChangePlayFile(MidiFileInfo info, bool autoPlay = true)
        {
            MidiPlayer.ChangeFileInfo(info, autoPlay);
            OnPropertyChanged("Name");
            OnPropertyChanged("IsPlaying");
            OnPropertyChanged("AutoSwitchToGenshinWindow");
            OnPropertyChanged("PlayBackground");
            OnPropertyChanged("Speed");
            OnPropertyChanged("NoteLevel");
            OnPropertyChanged("TotalTime");
            OnPropertyChanged("CurrentTime");
        }

        /// <summary>
        /// midi音轨变更
        /// </summary>
        public void ChangeMidiTrack()
        {
            timer.Stop();
            MidiPlayer.Started -= MidiPlayer_Started;
            MidiPlayer.Stopped -= MidiPlayer_Stopped;
            MidiPlayer.Finished -= MidiPlayer_Finished;
            MidiPlayer?.ChangeFileInfo();
            MidiPlayer.Started += MidiPlayer_Started;
            MidiPlayer.Stopped += MidiPlayer_Stopped;
            MidiPlayer.Finished += MidiPlayer_Finished;
            timer.Start();
        }

        /// <summary>
        /// 检查热键、窗口、管理员等状态，若非正常则显示建议
        /// </summary>
        public void RefreshState()
        {
            IsAdmin = Util.IsAdmin();
            CanPlay = MidiPlayer.CanPlay;
            StateText = "正常";
            TextBlock_Color = "Black";
            Button_Restart_Content = null;
            Tooltip_Content = null;
            if (!hotkey)
            {
                StateText = "热键定义失败";
                TextBlock_Color = "Red";
                Button_Restart_Content = "重试";
                Tooltip_Content = null;
            }
            if (!CanPlay)
            {
                StateText = "没有找到原神或WindsongLyre的窗口";
                TextBlock_Color = "Red";
                Button_Restart_Content = "刷新";
                Tooltip_Content = "请打开游戏后点击刷新";
            }
            if (!IsAdmin)
            {
                StateText = "需要管理员权限";
                TextBlock_Color = "Red";
                Button_Restart_Content = "重启";
                Tooltip_Content = "软件会用管理员权限干什么？\n《原神》以管理员权限启动，软件需要管理员权限才能向游戏窗口发送键盘信息";
            }
        }

        /// <summary>
        /// 刷新管理员（需重启）、原神窗口关联、热键注册状态
        /// </summary>
        public void RestartOrRefresh()
        {
            if (!IsAdmin)
            {
                try
                {
                    Util.RestartAsAdmin();
                }
                catch (Exception ex)
                {
                    Log.OutputLog(LogType.Error, "RestartAsAdmin", ex);
                    Growl.Error("无法重启，请手动以管理员权限启动");
                }
            }
            if (!CanPlay)
            {
                MidiPlayer = new MidiPlayer();
                SelectedMidiFile = MidiFileInfoList.First();
                RefreshState();
            }
            if (!hotkey)
            {
                _ = Util.UnregisterHotKey(hWnd);
                hotkey = Util.RegisterHotKey(hWnd);
                RefreshState();
            }
        }

        /// <summary>
        /// 程序热键回调，关注以下事件：1000（暂停/播放），1001（上一首），1002（下一首）
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr HwndHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_KOTKEY = 0x0312;
            if (msg == WM_KOTKEY)
            {
                switch (wParam.ToInt32())
                {
                    // 播放/暂停
                    case 1000:
                        if (IsPlaying)
                        {
                            IsPlaying = false;
                        }
                        else
                        {
                            if (MidiPlayer?.MidiFileInfo == null)
                            {
                                ChangePlayFile(MidiFileInfoList.First());
                            }
                            else
                            {
                                IsPlaying = true;
                            }
                        }
                        handled = true;
                        break;
                    // 上一首
                    case 1001:
                        PlayLast();
                        handled = true;
                        break;
                    // 下一首
                    case 1002:
                        PlayNext();
                        handled = true;
                        break;
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 演奏midi列表上一曲
        /// </summary>
        public void PlayLast()
        {
            if (MidiPlayer.MidiFileInfo == null)
            {
                ChangePlayFile(MidiFileInfoList.Last());
            }
            else
            {
                var index = MidiFileInfoList.IndexOf(MidiPlayer.MidiFileInfo);
                if (index == 0)
                {
                    ChangePlayFile(MidiFileInfoList.Last());
                }
                else
                {
                    ChangePlayFile(MidiFileInfoList[index - 1]);
                }
            }
        }

        /// <summary>
        /// 演奏midi列表下一曲
        /// </summary>
        public void PlayNext()
        {
            if (MidiPlayer.MidiFileInfo == null)
            {
                ChangePlayFile(MidiFileInfoList.First());
            }
            else
            {
                var index = MidiFileInfoList.IndexOf(MidiPlayer.MidiFileInfo);
                if (index == MidiFileInfoList.Count - 1)
                {
                    ChangePlayFile(MidiFileInfoList.First());
                }
                else
                {
                    ChangePlayFile(MidiFileInfoList[index + 1]);
                }
            }
        }
    }
}
