using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Sigma88LoadingScreensPlugin
{
    internal static class LoadingScreens
    {
        internal static void LoadBuiltIn(string[] mods)
        {
            if (LoadingScreenSettings.newScreens == null)
                LoadingScreenSettings.newScreens = new List<Object>();
            if (LoadingScreenSettings.newTips == null)
                LoadingScreenSettings.newTips = new List<string>();

            if (mods.Contains("GalacticNeighborhood"))
            {
                LoadingScreenSettings.newScreens.AddRange(LoadScreens("GalacticNeighborhood/LoadingScreens/PluginData/"));
                LoadingScreenSettings.newTips.Add("Populating Star Systems...");
            }
            if (mods.Contains("SigmaBinary"))
            {
                LoadingScreenSettings.newScreens.AddRange(LoadScreens("Sigma/Binary/LoadingScreens/PluginData/"));
                LoadingScreenSettings.newTips.Add("Re-centering Barycenters...");
            }
            if (mods.Contains("SigmaDimensions"))
            {
                LoadingScreenSettings.newScreens.AddRange(LoadScreens("Sigma/Dimensions/LoadingScreens/PluginData/"));
                LoadingScreenSettings.newTips.Add("Scrambling Universal Constants...");
            }
        }

        internal static void LoadExternal(ConfigNode[] nodes)
        {
            for (int i = 0; i < nodes?.Length; i++)
            {
                List<Object> screens = new List<Object>();
                List<string> tips = new List<string>();


                // Loading Screens
                string[] folders = nodes[i].GetValues("folder");
                for (int j = 0; j < folders?.Length; j++)
                {
                    screens.AddRange(LoadScreens(folders[j]));
                }
                if (bool.TryParse(nodes[i].GetValue("removeStockScreens"), out bool clear) && clear)
                {
                    LoadingScreenSettings.removeStockScreens = true;
                }
                LoadingScreenSettings.newScreens.AddRange(screens);


                // Loading Tips
                string[] tipsFiles = nodes[i].GetValues("tipsFile");
                for (int j = 0; j < tipsFiles?.Length; j++)
                {
                    tips.AddRange(LoadTips(tipsFiles[j]));
                }
                if (bool.TryParse(nodes[i].GetValue("removeStockTips"), out clear) && clear)
                {
                    LoadingScreenSettings.removeStockTips = true;
                }
                

                // Theme
                if (bool.TryParse(nodes[i].GetValue("themedTips"), out bool themed) && themed && screens?.Count() > 0 && tips?.Count() > 0)
                {
                    if (LoadingScreenSettings.themes == null)
                        LoadingScreenSettings.themes = new List<KeyValuePair<Object[], string[]>>();
                    LoadingScreenSettings.themes.Add(new KeyValuePair<Object[], string[]>(screens.ToArray(), tips.ToArray()));
                }
                else
                {
                    if (LoadingScreenSettings.newScreens == null)
                        LoadingScreenSettings.newScreens = new List<Object>();
                    LoadingScreenSettings.newScreens.AddRange(screens);

                    if (LoadingScreenSettings.newTips == null)
                        LoadingScreenSettings.newTips = new List<string>();
                    LoadingScreenSettings.newTips.AddRange(tips);
                }


                // Logo
                if (nodes[i].HasValue("logoScreen"))
                {
                    string path = "GameData/" + nodes[i].GetValue("logoScreen");

                    if (File.Exists(path))
                        AddLogo(Utility.LoadDDS(File.ReadAllBytes(path)), nodes[i].GetValue("logoTip"));
                }
            }
        }

        internal static void AddScreens(LoadingScreen.LoadingScreenState screen)
        {
            // Loading Screens
            List<Object> screens = screen?.screens?.ToList();
            if (screens == null)
                screens = new List<Object>();

            if (LoadingScreenSettings.removeStockScreens)
                screens.Clear();

            screens.AddRange(LoadingScreenSettings.newScreens);
            screens.Add(Resources.SigmaLSLS_1);
            screens.Scramble();
            screen.screens = screens.ToArray();


            // Loading Tips
            if (LoadingScreenSettings.newTips == null)
                LoadingScreenSettings.newTips = new List<string>();

            if (!LoadingScreenSettings.removeStockTips && screen.tips?.Length > 0)
                LoadingScreenSettings.newTips.InsertRange(0, screen.tips);

            LoadingScreenSettings.newTips.Add("Hacking Loading Screen...");

            screen.tips = LoadingScreenSettings.newTips.ToArray();


            // Logo
            if (LoadingScreenSettings.logos?.Count > 0)
            {
                LoadingScreen.LoadingScreenState logo = new LoadingScreen.LoadingScreenState();
                Utility.Clone(LoadingScreen.Instance.Screens.FirstOrDefault(), logo);

                System.Random rnd = new System.Random();
                KeyValuePair<Texture2D, string> randomLogo = LoadingScreenSettings.logos[rnd.Next(LoadingScreenSettings.logos.Count)];

                logo.screens = new Object[] { randomLogo.Key };
                logo.tips = new string[] { randomLogo.Value };

                List<LoadingScreen.LoadingScreenState> Screens = LoadingScreen.Instance.Screens;
                Screens.Insert(1, logo);
                LoadingScreen.Instance.Screens = Screens;
            }
        }

        static void AddLogo(Texture2D logoScreen, string logoTip)
        {
            if (logoScreen != null)
                LoadingScreenSettings.logos.Add(new KeyValuePair<Texture2D, string>(logoScreen, string.IsNullOrEmpty(logoTip) ? "Loading..." : logoTip));
        }

        static Object[] LoadScreens(string path)
        {
            List<Object> list = new List<Object>();

            var files = Directory.GetFiles("GameData/" + path)?.Where(f => Path.GetExtension(f) == ".dds")?.ToArray();

            for (int i = 0; i < files?.Length; i++)
            {
                string filePath = files[i];

                Texture2D tex = new Texture2D(2, 2);
                byte[] fileData = File.ReadAllBytes(filePath);

                tex = Utility.LoadDDS(fileData);

                if (tex == null) continue;
                tex.name = path + Path.GetFileNameWithoutExtension(filePath);

                list.Add(tex);
            }

            return list?.Count > 0 ? list.ToArray() : new Object[] { };
        }

        static string[] LoadTips(string path)
        {
            string[] tips = null;
            try { tips = File.ReadAllLines("GameData/" + path).Where(s => !string.IsNullOrEmpty(s)).ToArray(); } catch { }
            return tips?.Length > 0 ? tips : new string[] { };
        }
    }
}
