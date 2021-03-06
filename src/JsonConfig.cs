﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace WinDynamicDesktop
{
    public class AppConfig
    {
        public string location { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public bool darkMode { get; set; }
        public bool hideTrayIcon { get; set; }
        public bool disableAutoUpdate { get; set; }
        public string lastUpdateCheck { get; set; }
        public bool changeSystemTheme { get; set; }
        public string themeName { get; set; }
        public bool useWindowsLocation { get; set; }
        //public bool changeLockScreen { get; set; }

        // Hack to make the object like a Python dictionary from
        // https://stackoverflow.com/a/24919811/5504760
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }

    public class ThemeConfig
    {
        public string themeName { get; set; }
        public string imagesZipUri { get; set; }
        public string imageFilename { get; set; }
        public int[] dayImageList { get; set; }
        public int[] nightImageList { get; set; }
    }

    class JsonConfig
    {
        private static string lastJson;

        public static AppConfig settings = new AppConfig();
        public static bool firstRun = !File.Exists("settings.conf");

        public static void LoadConfig()
        {
            if (!firstRun)
            {
                lastJson = File.ReadAllText("settings.conf");
                settings = JsonConvert.DeserializeObject<AppConfig>(lastJson);

                // TEMPORARY HACK TO FIX BUG INTRODUCED IN LAST VERSION
                if (settings.themeName == "")
                {
                    settings.themeName = "Mojave_Desert";
                }
            }
        }

        public static void UpdateSetting<T>(string name, T value, bool saveConfig = true)
        {
            settings[name] = value;

            if (saveConfig)
            {
                SaveConfig();
            }
        }

        private static async void SaveConfig()
        {
            string newJson = JsonConvert.SerializeObject(settings);

            if (newJson != lastJson)
            {
                await Task.Run(() => File.WriteAllText("settings.conf", newJson));
                lastJson = newJson;
            }
        }
        
        public static ThemeConfig LoadTheme(string name)
        {
            string themeJson;

            if (name == "Mojave_Desert")
            {
                themeJson = Encoding.UTF8.GetString(Properties.Resources.jsonMojaveDesert);
            }
            else if (name == "Solar_Gradients")
            {
                themeJson = Encoding.UTF8.GetString(Properties.Resources.jsonSolarGradients);
            }
            else
            {
                themeJson = File.ReadAllText(Path.Combine("themes", name + ".json"));
            }

            ThemeConfig theme = JsonConvert.DeserializeObject<ThemeConfig>(themeJson);
            theme.themeName = name;

            return theme;
        }
    }
}
