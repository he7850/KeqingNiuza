using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Core;

namespace KeqingNiuza.Core.Midi
{
    public class MidiTrack
    {
        /// <summary>
        /// 原始音轨，类型 DryWetMidi.Core.TrackChunk
        /// </summary>
        public TrackChunk Track { get; set; }

        /// <summary>
        /// 音轨名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 音轨是否被勾选演奏
        /// </summary>
        public bool IsCheck { get; set; }

        /// <summary>
        /// 音轨可勾选（音符数大于0）
        /// </summary>
        public bool CanBeChecked => NoteNumber > 0;

        /// <summary>
        /// 音轨音符数量
        /// </summary>
        public int NoteNumber { get; set; }

        /// <summary>
        /// 音轨可演奏音符数量
        /// </summary>
        public int CanPlayNoteNumber { get; set; }

        /// <summary>
        /// 音轨可演奏音符比例
        /// </summary>
        public double CanPlayNoteRadio => (double)CanPlayNoteNumber / NoteNumber;

        /// <summary>
        /// 音轨最高音符
        /// </summary>
        public int MaxNoteLevel { get; set; }
        /// <summary>
        /// 音轨最低音符
        /// </summary>
        public int MinNoteLevel { get; set; }

        /// <summary>
        /// 初始化音轨：记录原始音轨，获取音轨名称，计算音符总数，按不移调统计音符数
        /// </summary>
        /// <param name="track">原始音轨</param>
        public MidiTrack(TrackChunk track)
        {
            Track = track;
            Name = track.Events.OfType<SequenceTrackNameEvent>().FirstOrDefault()?.Text;
            NoteNumber = track.Events.Count(x => x.EventType == MidiEventType.NoteOn);
            RefreshByNoteLevel(0);
        }

        /// <summary>
        /// 根据移调数，刷新可演奏音符统计
        /// </summary>
        /// <param name="noteLevel">升降调的平移数，0表示不移调</param>
        public void RefreshByNoteLevel(int noteLevel)
        {
            CanPlayNoteNumber = Track.Events.Where(x => x.EventType == MidiEventType.NoteOn).Count(x => Const.NoteToCharDictionary.ContainsKey((x as NoteOnEvent).NoteNumber + noteLevel));
            if (CanBeChecked)
            {
                MaxNoteLevel = Track.Events.Where(x => x.EventType == MidiEventType.NoteOn).Max(x => (x as NoteOnEvent).NoteNumber + noteLevel);
                MinNoteLevel = Track.Events.Where(x => x.EventType == MidiEventType.NoteOn).Min(x => (x as NoteOnEvent).NoteNumber + noteLevel);
            }
        }
    }
}
