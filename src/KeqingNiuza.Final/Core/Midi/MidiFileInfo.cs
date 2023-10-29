using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Core;

namespace KeqingNiuza.Core.Midi
{
    public class MidiFileInfo
    {
        /// <summary>
        /// midi文件名（去拓展名）
        /// </summary>
        public string Name { get; set; }

        public FileInfo FileInfo { get; private set; }

        /// <summary>
        /// 原始 DryWetMidi.Core.MidiFile 类型
        /// </summary>
        public MidiFile MidiFile { get; private set; }

        /// <summary>
        /// midi文件的所有音轨
        /// </summary>
        public List<MidiTrack> MidiTracks { get; set; }

        /// <summary>
        /// midi文件的可演奏音轨（音符数 > 0）
        /// </summary>
        public List<MidiTrack> CanPlayTracks { get; set; }

        /// <summary>
        /// midi音符总数量
        /// </summary>
        public int NoteNumber { get; set; }
        /// <summary>
        /// midi可演奏音符总数量
        /// </summary>
        public int CanPlayNoteNumber { get; set; }
        /// <summary>
        /// midi可演奏音符比例
        /// </summary>
        public double CanPlayNoteRadio => (double)CanPlayNoteNumber / NoteNumber;

        /// <summary>
        /// midi所有音轨中的最高音符
        /// </summary>
        public int MaxNoteLevel { get; set; }
        /// <summary>
        /// midi所有音轨中的最低音符
        /// </summary>
        public int MinNoteLevel { get; set; }

        /// <summary>
        /// 给定midi文件路径，获取midi信息
        /// </summary>
        /// <param name="path">midi文件路径</param>
        public MidiFileInfo(string path)
        {
            // 读取midi文件，设置来自示例 https://github.com/stakira/OpenUtau/blob/master/OpenUtau.Core/Format/MidiWriter.cs
            MidiFile = MidiFile.Read(Path.GetFullPath(path), new ReadingSettings
            {
                InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.ReadValid,
                InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore,
                InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.SnapToLimits,
                MissedEndOfTrackPolicy = MissedEndOfTrackPolicy.Ignore,
                NoHeaderChunkPolicy = NoHeaderChunkPolicy.Ignore,
                NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore,
                UnexpectedTrackChunksCountPolicy = UnexpectedTrackChunksCountPolicy.Ignore,
                UnknownChannelEventPolicy = UnknownChannelEventPolicy.SkipStatusByteAndOneDataByte,
                UnknownChunkIdPolicy = UnknownChunkIdPolicy.ReadAsUnknownChunk,
                UnknownFileFormatPolicy = UnknownFileFormatPolicy.Ignore,
                TextEncoding = Encoding.UTF8, // 解析中文track名
            });
            Name = Path.GetFileNameWithoutExtension(path);
            // 获取所有音轨
            MidiTracks = MidiFile.GetTrackChunks().Select(x => new MidiTrack(x)).ToList();
            // 过滤可演奏音轨，并默认开启所有
            CanPlayTracks = MidiTracks.Where(x => x.CanBeChecked).ToList();
            CanPlayTracks.ForEach(x => x.IsCheck = true);
            // 统计音符总数
            NoteNumber = CanPlayTracks.Sum(x => x.NoteNumber);
            // 统计可演奏音符总数（非半音）
            CanPlayNoteNumber = CanPlayTracks.Sum(x => x.CanPlayNoteNumber);
            // 获取所有音轨里的最高、最低音符
            MaxNoteLevel = CanPlayTracks.Max(x => x.MaxNoteLevel);
            MinNoteLevel = CanPlayTracks.Min(x => x.MinNoteLevel);
        }

        /// <summary>
        /// 根据移调数，刷新所有音轨的可演奏音符统计
        /// </summary>
        /// <param name="noteLevel">升降调的平移数，0表示不移调</param>
        public void RefreshTracksByNoteLevel(int noteLevel)
        {
            CanPlayTracks.ForEach(x => x.RefreshByNoteLevel(noteLevel));
            NoteNumber = CanPlayTracks.Sum(x => x.NoteNumber);
            CanPlayNoteNumber = CanPlayTracks.Sum(x => x.CanPlayNoteNumber);
            MaxNoteLevel = CanPlayTracks.Max(x => x.MaxNoteLevel);
            MinNoteLevel = CanPlayTracks.Min(x => x.MinNoteLevel);
        }

    }
}
