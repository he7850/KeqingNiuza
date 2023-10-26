using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using KeqingNiuza.Core.Wish;
using KeqingNiuza.Model;
using KeqingNiuza.View;

namespace KeqingNiuza.ViewModel
{
    public class WishSummaryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WishSummaryViewModel(UserData userData)
        {
        }

        public WishSummaryViewModel()
        {
        }

        public UserData UserData { get; set; }
        public static List<WishData> WishDataList;

        public bool IsCorrectOrder
        {
            get { return Properties.Settings.Default.IsCorrectOrder; }
            set
            {
                if (IsCorrectOrder != value)
                {
                    if (WishSummary.CharacterStatistics != null)
                    {
                        WishSummary.CharacterStatistics.Star5List = WishSummary.CharacterStatistics.Star5List.Reverse<StarDetail>().ToList();
                        WishSummary.CharacterStatistics.Star4List = WishSummary.CharacterStatistics.Star4List.Reverse<StarDetail>().ToList();
                    }
                    if (WishSummary.WeaponStatistics != null)
                    {
                        WishSummary.WeaponStatistics.Star5List = WishSummary.WeaponStatistics.Star5List.Reverse<StarDetail>().ToList();
                        WishSummary.WeaponStatistics.Star4List = WishSummary.WeaponStatistics.Star4List.Reverse<StarDetail>().ToList();
                    }
                    if (WishSummary.PermanentStatistics != null)
                    {
                        WishSummary.PermanentStatistics.Star5List = WishSummary.PermanentStatistics.Star5List.Reverse<StarDetail>().ToList();
                        WishSummary.PermanentStatistics.Star4List = WishSummary.PermanentStatistics.Star4List.Reverse<StarDetail>().ToList();
                    }
                    Properties.Settings.Default.IsCorrectOrder = value;
                    OnPropertyChanged();
                }
            }
        }



        public bool IgnoreFirstStar5Character
        {
            get { return UserData.IgnoreFirstStar5Character; }
            set
            {
                UserData.IgnoreFirstStar5Character = value;
                WishSummary.CharacterStatistics = WishSummary.GetStatistics(WishDataList.Where(x => x.WishType == WishType.CharacterEvent || x.WishType == WishType.CharacterEvent_2).ToList(), value);
                OnPropertyChanged();
            }
        }


        public bool IgnoreFirstStar5Weapon
        {
            get { return UserData.IgnoreFirstStar5Weapon; }
            set
            {
                UserData.IgnoreFirstStar5Weapon = value;
                WishSummary.WeaponStatistics = WishSummary.GetStatistics(WishDataList.Where(x => x.WishType == WishType.WeaponEvent).ToList(), value);
                OnPropertyChanged();
            }
        }


        public bool IgnoreFirstStar5Permanent
        {
            get { return UserData.IgnoreFirstStar5Permanent; }
            set
            {
                UserData.IgnoreFirstStar5Permanent = value;
                WishSummary.PermanentStatistics = WishSummary.GetStatistics(WishDataList.Where(x => x.WishType == WishType.Permanent).ToList(), value);
                OnPropertyChanged();
            }
        }




        private WishSummary _WishSummary;
        public WishSummary WishSummary
        {
            get { return _WishSummary; }
            set
            {
                _WishSummary = value;
                OnPropertyChanged();
            }
        }


        private List<ItemInfo> _CharacterInfoList;
        public List<ItemInfo> CharacterInfoList
        {
            get { return _CharacterInfoList; }
            set
            {
                _CharacterInfoList = value;
                OnPropertyChanged();
            }
        }


        private List<ItemInfo> _WeaponInfoList;
        public List<ItemInfo> WeaponInfoList
        {
            get { return _WeaponInfoList; }
            set
            {
                _WeaponInfoList = value;
                OnPropertyChanged();
            }
        }

        private object _DetailContent;
        public object DetailContent
        {
            get { return _DetailContent; }
            set
            {
                _DetailContent = value;
                OnPropertyChanged();
            }
        }




        public void CharacterOrder(string order)
        {
            switch (order)
            {
                case "最近获得":
                    CharacterInfoList = WishSummary.CharacterInfoList.OrderByDescending(x => x.LastGetTime).ThenByDescending(x => x.Count).ToList();
                    break;
                case "数量":
                    CharacterInfoList = WishSummary.CharacterInfoList.OrderByDescending(x => x.Count).ThenByDescending(x => x.Rank).ThenByDescending(x => x.LastGetTime).ToList();
                    break;
                case "星级":
                    CharacterInfoList = WishSummary.CharacterInfoList.OrderByDescending(x => x.Rank).ThenByDescending(x => x.Count).ThenByDescending(x => x.LastGetTime).ToList();
                    break;
            }
        }

        public void WeaponOrder(string order)
        {
            switch (order)
            {
                case "最近获得":
                    WeaponInfoList = WishSummary.WeaponInfoList.OrderByDescending(x => x.LastGetTime).ThenByDescending(x => x.Count).ToList();
                    break;
                case "数量":
                    WeaponInfoList = WishSummary.WeaponInfoList.OrderByDescending(x => x.Count).ThenByDescending(x => x.Rank).ThenByDescending(x => x.LastGetTime).ToList();
                    break;
                case "星级":
                    WeaponInfoList = WishSummary.WeaponInfoList.OrderByDescending(x => x.Rank).ThenByDescending(x => x.Count).ThenByDescending(x => x.LastGetTime).ToList();
                    break;
            }
        }

    }
}
