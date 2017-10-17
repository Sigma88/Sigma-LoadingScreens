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
                Debug.Log("LoadBuiltIn", "GN detected");
                LoadingScreenSettings.newScreens.AddRange(LoadScreens("GalacticNeighborhood/LoadingScreens/PluginData/"));
                LoadingScreenSettings.newTips.Add("Populating Star Systems...");
            }
            if (mods.Contains("SigmaBinary"))
            {
                Debug.Log("LoadBuiltIn", "SB detected");
                LoadingScreenSettings.newScreens.AddRange(LoadScreens("Sigma/Binary/LoadingScreens/PluginData/"));
                LoadingScreenSettings.newTips.Add("Re-centering Barycenters...");
            }
            if (mods.Contains("SigmaDimensions"))
            {
                Debug.Log("LoadBuiltIn", "SD detected");
                LoadingScreenSettings.newScreens.AddRange(LoadScreens("Sigma/Dimensions/LoadingScreens/PluginData/"));
                LoadingScreenSettings.newTips.Add("Scrambling Universal Constants...");
            }
        }

        internal static void LoadExternal(UrlDir.UrlConfig[] config)
        {
            Debug.Log("LoadExternal", "Loading " + config?.Length + " external config nodes");

            for (int i = 0; i < config?.Length; i++)
            {
                Debug.Log("LoadExternal", "Loading node nr. " + i + ", url = " + config[i].parent.url);
                ConfigNode node = config[i].config;
                List<Object> screens = new List<Object>();
                List<string> tips = new List<string>();


                // Loading Screens
                string[] folders = node.GetValues("folder");
                for (int j = 0; j < folders?.Length; j++)
                {
                    screens.AddRange(LoadScreens(folders[j]));
                }
                if (bool.TryParse(node.GetValue("removeStockScreens"), out bool clear) && clear)
                {
                    LoadingScreenSettings.removeStockScreens = true;
                    Debug.Log("LoadExternal", "'removeStockScreens' set to 'true'");
                }
                LoadingScreenSettings.newScreens.AddRange(screens);


                // Loading Tips
                string[] tipsFiles = node.GetValues("tipsFile");
                for (int j = 0; j < tipsFiles?.Length; j++)
                {
                    tips.AddRange(LoadTips(tipsFiles[j]));
                }
                if (bool.TryParse(node.GetValue("removeStockTips"), out clear) && clear)
                {
                    LoadingScreenSettings.removeStockTips = true;
                    Debug.Log("LoadExternal", "'removeStockTips' set to 'true'");
                }


                // Theme
                if (bool.TryParse(node.GetValue("themedTips"), out bool themed) && themed && screens?.Count() > 0 && tips?.Count() > 0)
                {
                    Debug.Log("LoadExternal", "'themedTips' is 'true'");

                    if (LoadingScreenSettings.themes == null)
                        LoadingScreenSettings.themes = new List<KeyValuePair<Object[], string[]>>();
                    LoadingScreenSettings.themes.Add(new KeyValuePair<Object[], string[]>(screens.ToArray(), tips.ToArray()));
                    Debug.Log("LoadExternal", "Added " + screens.Count + " images and " + tips.Count + " tips to theme nr. " + LoadingScreenSettings.themes.Count);
                }
                else
                {
                    Debug.Log("LoadExternal", "'themedTips' is NOT 'true'");

                    if (LoadingScreenSettings.newScreens == null)
                        LoadingScreenSettings.newScreens = new List<Object>();
                    LoadingScreenSettings.newScreens.AddRange(screens);
                    Debug.Log("LoadExternal", "Added " + screens.Count + " images to newScreens");
                    Debug.Log("LoadExternal", "newScreens.Count = " + LoadingScreenSettings.newScreens.Count);

                    if (LoadingScreenSettings.newTips == null)
                        LoadingScreenSettings.newTips = new List<string>();
                    LoadingScreenSettings.newTips.AddRange(tips);
                    Debug.Log("LoadExternal", "Added " + tips.Count + " tips to newTips");
                    Debug.Log("LoadExternal", "newTips.Count = " + LoadingScreenSettings.newTips.Count);
                }


                // Logo
                if (node.HasValue("logoScreen"))
                {
                    string path = "GameData/" + node.GetValue("logoScreen");

                    if (File.Exists(path))
                        AddLogo(Utility.LoadDDS(File.ReadAllBytes(path)), node.GetValue("logoTip"));
                }
            }
        }

        internal static void AddScreens(LoadingScreen.LoadingScreenState screen)
        {
            Debug.Log("AddScreens", "Adding ");

            // Loading Screens
            List<Object> screens = screen?.screens?.ToList();
            if (screens == null)
                screens = new List<Object>();

            if (LoadingScreenSettings.removeStockScreens)
            {
                screens.Clear();
                Debug.Log("AddScreens", "Removed Stock Loading Screen Images");
            }

            screens.AddRange(LoadingScreenSettings.newScreens);
            screens.Add(Resources.SigmaLSLS_1);
            screens.Scramble();
            screen.screens = screens.ToArray();
            Debug.Log("AddScreens", "Final count of Loading Screen Images = " + screen.screens.Length);


            // Loading Tips
            if (LoadingScreenSettings.newTips == null)
                LoadingScreenSettings.newTips = new List<string>();

            if (LoadingScreenSettings.removeStockTips)
            {
                Debug.Log("AddScreens", "Removed Stock Loading Screen Images");
            }
            else if (screen.tips?.Length > 0)
            {
                LoadingScreenSettings.newTips.InsertRange(0, screen.tips);
            }

            LoadingScreenSettings.newTips.Add("Hacking Loading Screen...");

            screen.tips = LoadingScreenSettings.newTips.ToArray();
            Debug.Log("AddScreens", "Final count of Loading Screen Tips = " + screen.tips.Length);


            // Logo
            if (LoadingScreenSettings.logos?.Count > 0)
            {
                Debug.Log("AddScreens", "Found " + LoadingScreenSettings.logos?.Count + " new logo images");

                LoadingScreen.LoadingScreenState logo = new LoadingScreen.LoadingScreenState();
                Utility.Clone(LoadingScreen.Instance.Screens.FirstOrDefault(), logo);

                System.Random rnd = new System.Random();
                KeyValuePair<Texture2D, string> randomLogo = LoadingScreenSettings.logos[rnd.Next(LoadingScreenSettings.logos.Count)];

                logo.screens = new Object[] { randomLogo.Key };
                Debug.Log("AddScreens", "Chosen logoScreen = " + randomLogo.Key);

                logo.tips = new string[] { randomLogo.Value };
                Debug.Log("AddScreens", "Chosen logoTip = " + randomLogo.Value);

                List<LoadingScreen.LoadingScreenState> Screens = LoadingScreen.Instance.Screens;
                Screens.Insert(1, logo);
                LoadingScreen.Instance.Screens = Screens;
            }
        }

        static void AddLogo(Texture2D logoScreen, string logoTip)
        {
            if (logoScreen != null)
            {
                LoadingScreenSettings.logos.Add(new KeyValuePair<Texture2D, string>(logoScreen, string.IsNullOrEmpty(logoTip) ? "Loading..." : logoTip));
                Debug.Log("AddLogo", "Added to list new logo pair: image = " + logoScreen + ", tip = " + LoadingScreenSettings.logos.Last().Value);
            }
            else
                Debug.Log("AddLogo", "logo is null");
        }

        static Object[] LoadScreens(string path)
        {
            if (!path.EndsWith("/")) path += "/";
            Debug.Log("LoadScreens", "Loading images from path = " + path);

            if (!Directory.Exists("GameData/" + path))
            {
                Debug.Log("LoadScreens", "Path does not exist");
                return new Object[] { };
            }

            List<Object> list = new List<Object>();

            var files = Directory.GetFiles("GameData/" + path)?.Where(f => Path.GetExtension(f) == ".dds")?.ToArray();
            Debug.Log("LoadScreens", "Path contains " + files.Length + " .dds files");

            for (int i = 0; i < files?.Length; i++)
            {
                string filePath = files[i];

                Texture2D tex = new Texture2D(2, 2);
                byte[] fileData = File.ReadAllBytes(filePath);

                tex = Utility.LoadDDS(fileData);

                if (tex == null)
                {
                    Debug.Log("LoadScreens", "Loaded dds file is not a valid texture");
                    continue;
                }
                tex.name = path + Path.GetFileNameWithoutExtension(filePath);
                Debug.Log("LoadScreens", "Successfully loaded texture = " + tex);

                list.Add(tex);
            }
            Debug.Log("LoadScreens", "Successfully loaded " + list?.Count + " textures from folder " + path);

            return list?.Count > 0 ? list.ToArray() : new Object[] { };
        }

        static string[] LoadTips(string path)
        {
            Debug.Log("LoadTips", "Loading tips from file = " + path);

            if (!File.Exists(path))
            {
                Debug.Log("LoadTips", "Path does not exist"); return new string[] { };
            }

            string[] tips = null;
            try { tips = File.ReadAllLines("GameData/" + path).Where(s => !string.IsNullOrEmpty(s)).ToArray(); } catch { }
            Debug.Log("LoadTips", "Successfully loaded " + tips?.Length + " tips from file " + path);

            return tips?.Length > 0 ? tips : new string[] { };
        }
    }
}
