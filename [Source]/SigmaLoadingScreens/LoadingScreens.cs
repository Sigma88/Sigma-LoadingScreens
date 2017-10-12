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
            if (mods.Contains("GalacticNeighborhood"))
            {
                LoadScreens("GalacticNeighborhood/LoadingScreens/PluginData/");
                LoadingScreenSettings.newTips.Add("Populating Star Systems...");
            }
            if (mods.Contains("SigmaBinary"))
            {
                LoadScreens("Sigma/Binary/LoadingScreens/PluginData/");
                LoadingScreenSettings.newTips.Add("Re-centering Barycenters...");
            }
            if (mods.Contains("SigmaDimensions"))
            {
                LoadScreens("Sigma/Dimensions/LoadingScreens/PluginData/");
                LoadingScreenSettings.newTips.Add("Scrambling Universal Constants...");
            }
        }

        internal static void LoadExternal(ConfigNode[] nodes)
        {
            for (int i = 0; i < nodes?.Length; i++)
            {
                // Loading Screens
                string[] folders = nodes[i].GetValues("folder");
                for (int j = 0; j < folders?.Length; j++)
                {
                    LoadScreens(folders[j]);
                }
                if (bool.TryParse(nodes[i].GetValue("removeStockScreens"), out bool clear) && clear)
                {
                    LoadingScreenSettings.removeStockScreens = true;
                }

                // Loading Tips
                string[] tipsFiles = nodes[i].GetValues("tipsFile");
                for (int j = 0; j < tipsFiles?.Length; j++)
                {
                    LoadTips(tipsFiles[j]);
                }
                if (bool.TryParse(nodes[i].GetValue("removeStockTips"), out clear) && clear)
                {
                    LoadingScreenSettings.removeStockTips = true;
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
            screens.Add(Utility.LoadDDS(Resources.SigmaLSLS_1));

            screen.screens = screens.ToArray();


            // Loading Tips
            List<string> tips = screen?.tips?.ToList();
            if (tips == null)
                tips = new List<string>();

            if (LoadingScreenSettings.removeStockTips)
                tips.Clear();

            tips.AddRange(LoadingScreenSettings.newTips);
            tips.Add("Hacking Loading Screen...");

            screen.tips = tips.ToArray();


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

        static void LoadScreens(string path)
        {
            var files = Directory.GetFiles("GameData/" + path)?.Where(f => Path.GetExtension(f) == ".dds")?.ToArray();

            for (int i = 0; i < files?.Length; i++)
            {
                string filePath = files[i];

                Texture2D tex = new Texture2D(2, 2);
                byte[] fileData = File.ReadAllBytes(filePath);

                tex = Utility.LoadDDS(fileData);

                if (tex == null) continue;
                tex.name = path + Path.GetFileNameWithoutExtension(filePath);

                LoadingScreenSettings.newScreens.Add(tex);
            }
        }

        static void LoadTips(string path)
        {
            if (File.Exists("GameData/" + path))
            {
                LoadingScreenSettings.newTips.AddRange(File.ReadAllLines("GameData/" + path).Where(s => !string.IsNullOrEmpty(s)));
            }
        }
    }
}
