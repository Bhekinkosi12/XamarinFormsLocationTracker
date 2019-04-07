using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings.Abstractions;

namespace LocationTracker.Helpers
{
    public class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string StartNum = "StartNum_key";
        private static readonly string StartDefault = string.Empty;

        #endregion


        public static string StartSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(StartNum, StartDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(StartNum, value);
            }
        }

        private const string EndNum = "EndNum_key";
        private static readonly string EndDefault = string.Empty;

        public static string EndSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(EndNum, EndDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(EndNum, value);
            }
        }

        private const string Group = "Group_key";
        private static readonly string GroupDefault = string.Empty;

        public static string GroupSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(Group, GroupDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(Group, value);
            }
        }

        private const string Radius = "Radius_key";
        private static readonly string RadiusDefault = "20";

        public static string RadiusSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(Radius, RadiusDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(Radius, value);
            }
        }
        private const string Premium = "Premium_key";
        private static readonly string PremiumDefault = "false";

        public static string PremiumSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(Premium, PremiumDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(Premium, value);
            }
        }

        private const string Donation = "Donation_key";
        private static readonly string DonationDefault = "false";

        public static string DonationSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(Donation, DonationDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(Donation, value);
            }
        }
    }
}
