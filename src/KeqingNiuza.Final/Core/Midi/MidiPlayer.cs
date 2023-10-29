using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using KeqingNiuza.Core.Native;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Melanchall.DryWetMidi.Interaction;

namespace KeqingNiuza.Core.Midi
{
    public class MidiPlayer
    {
        /// <summary>
        /// midi文件信息
        /// </summary>
        public MidiFileInfo MidiFileInfo { get; private set; }


        /// <summary>
        /// Midi文件名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 是否可以演奏
        /// </summary>
        public bool CanPlay { get; private set; }


        /// <summary>
        /// 是否演奏中
        /// </summary>
        public bool IsPlaying
        {
            get { return _playback.IsRunning; }
            set
            {
                if (_playback == null)
                {
                    return;
                }
                if (IsPlaying == value)
                {
                    return;
                }
                if (value)
                {
                    if (AutoSwitchToGenshinWindow)
                    {
                        User32.SwitchToThisWindow(_hWnd, true);
                        Thread.Sleep(100);
                    }
                    _playback.Start();
                }
                else
                {
                    _playback.Stop();
                }
            }
        }


        /// <summary>
        /// Midi文件总时间
        /// </summary>
        public TimeSpan TotalTime { get; private set; }


        /// <summary>
        /// 获取/设置Midi文件的演奏时间位置
        /// </summary>
        public TimeSpan CurrentTime
        {
            get { 
                return _playback.GetCurrentTime<MetricTimeSpan>(); 
            }
            set { 
                _playback.MoveToTime(new MetricTimeSpan(value)); 
            }
        }


        /// <summary>
        /// 是否后台播放
        /// </summary>
        public bool PlayBackground { get; set; }


        /// <summary>
        /// 是否自动跳转到游戏窗口
        /// </summary>
        public bool AutoSwitchToGenshinWindow { get; set; } = true;


        /// <summary>
        /// midi演奏速度，X表示X倍速
        /// </summary>
        public double Speed
        {
            get { return _playback.Speed; }
            set
            {
                //if (_playback == null || IsPlaying)
                if (_playback == null)
                {
                    return;
                }
                if (value > 0)
                {
                    //_playback.Stop();
                    _playback.Speed = value;
                    //_playback.Start();
                }
            }
        }

        /// <summary>
        /// 升降调偏移数，X表示向上偏移X个半音
        /// </summary>
        public int NoteLevel { get; set; }


        /// <summary>
        /// midi回放开始时执行的回调，在midi回放初始化时绑定
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// midi回放暂停时执行的回调，在midi回放初始化时绑定
        /// </summary>
        public event EventHandler Stopped;
        /// <summary>
        /// midi回放结束时执行的回调，在midi回放初始化时绑定
        /// </summary>
        public event EventHandler Finished;

        /// <summary>
        /// midi回放started事件转发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _playback_Started(object sender, EventArgs e)
        {
            Started?.Invoke(this, e);
        }
        /// <summary>
        /// midi回放stopped事件转发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _playback_Stopped(object sender, EventArgs e)
        {
            Stopped?.Invoke(this, e);
        }
        /// <summary>
        /// midi回放finished事件转发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _playback_Finished(object sender, EventArgs e)
        {
            Finished?.Invoke(this, e);
        }

        /// <summary>
        /// midi回放器
        /// </summary>
        private Playback _playback;
        /// <summary>
        /// 
        /// </summary>
        private readonly IntPtr _hWnd;

        /// <summary>
        /// 初始化MidiPlayer，并关联指定进程列表中第一个可找到的进程
        /// </summary>
        /// <param name="processNames">进程名列表</param>
        public MidiPlayer(List<string> processNames)
        {
            foreach (string processName in processNames)
            {
                var pros = Process.GetProcessesByName(processName);
                if (pros.Any())
                {
                    _hWnd = pros[0].MainWindowHandle;
                    CanPlay = true;
                    break;
                }
            }
        }

        /// <summary>
        /// 初始化MidiPlayer，并关联YuanShen/GenshinImpact/WindsongLyre中的一个进程
        /// </summary>
        public MidiPlayer()
        {
            // 搜索原神窗口，以确定是否可以演奏
            var pros = Process.GetProcessesByName("YuanShen").ToList();
            pros.AddRange(Process.GetProcessesByName("GenshinImpact"));
            pros.AddRange(Process.GetProcessesByName("WindsongLyre"));
            if (pros.Any())
            {
                _hWnd = pros[0].MainWindowHandle;
                CanPlay = true;
            }
        }

        ~MidiPlayer()
        {
            _playback?.Dispose();
        }


        /// <summary>
        /// 更新为指定midi文件并回到原有时间位置，根据autoPlay参数可自动演奏
        /// </summary>
        /// <param name="info">指定midi文件</param>
        /// <param name="autoPlay">是否自动演奏</param>
        public void ChangeFileInfo(MidiFileInfo info, bool autoPlay = true)
        {
            Name = info.Name;
            MidiFileInfo = info;
            ChangeFileInfo();
            if (autoPlay)
            {
                IsPlaying = true;
            }
        }

        /// <summary>
        /// 更新midi文件并回到原有时间位置，根据autoPlay参数可自动演奏
        /// </summary>
        /// <param name="autoPlay">是否自动演奏</param>
        public void ChangeFileInfo(bool autoPlay = true)
        {
            var time = CurrentTime;
            ChangeFileInfo();
            if (autoPlay)
            {
                IsPlaying = true;
            }
            CurrentTime = time;
        }

        /// <summary>
        /// 根据当前midi文件和选择的音轨，重新初始化回放
        /// </summary>
        private void ChangeFileInfo()
        {
            var speed = _playback?.Speed;
            // 释放当前重放资源
            _playback?.Dispose();
            // 清空并重新加入被选的音轨
            MidiFileInfo.MidiFile.Chunks.Clear();
            MidiFileInfo.MidiFile.Chunks.AddRange(MidiFileInfo.MidiTracks.Where(x => x.IsCheck).Select(x => x.Track));
            // 重新初始化重放，及midi总时长
            _playback = MidiFileInfo.MidiFile.GetPlayback();
            TotalTime = MidiFileInfo.MidiFile.GetDuration<MetricTimeSpan>();
            // 设置midi演奏速度
            _playback.Speed = speed ?? 1;
            // 设置midi中的停止符需停顿
            _playback.InterruptNotesOnStop = true;
            // 注册midi演奏相关回调
            _playback.EventPlayed += NoteEventPlayed;
            _playback.Started += _playback_Started;
            _playback.Stopped += _playback_Stopped;
            _playback.Finished += _playback_Finished;
        }

        /// <summary>
        /// Note事件演奏回调，向关联的窗口发送音符对应按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">NoteOnEvent类型事件，包含音符号码NoteNumber</param>
        private void NoteEventPlayed(object sender, MidiEventPlayedEventArgs e)
        {
            if (e.Event.EventType == MidiEventType.NoteOn)
            {
                var note = e.Event as NoteOnEvent;
                var num = note.NoteNumber + NoteLevel;
                while (true)
                {
                    if (num < 48)
                    {
                        num += 12;
                    }
                    if (num > 83)
                    {
                        num -= 12;
                    }
                    if (num >= 48 || num <= 83)
                    {
                        break;
                    }
                }
                if (Const.NoteToVisualKeyDictionary.ContainsKey(num))
                {
                    Util.Postkey(_hWnd, num, PlayBackground);
                }
                else if (Const.NoteToVisualKeyDictionary.ContainsKey(num + 1))
                {
                    Util.Postkey(_hWnd, num + 1, PlayBackground);
                }
            }
        }
    }
}
