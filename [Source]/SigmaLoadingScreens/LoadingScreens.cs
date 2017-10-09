using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;


namespace SigmaLoadingScreensPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class LoadingScreens : MonoBehaviour
    {
        static bool first = true;
        static bool skip = false;

        // Loading Screens
        public static bool removeStockScreens = false;
        public static List<string> externalMods = new List<string>();
        public static List<Object> newScreens = new List<Object>();

        // Loading Tips
        public static bool removeStockTips = false;
        public static List<string> externalTipFiles = new List<string>();
        public static List<string> newTips = new List<string>();

        void Awake()
        {
            AssemblyLoader.LoadedAssembly[] list = AssemblyLoader.loadedAssemblies.Where(a => a.name == "Sigma88LoadingScreens").ToArray();
            AssemblyLoader.LoadedAssembly TheChosenOne = list.FirstOrDefault(a => a.versionMinor == list.Select(i => i.versionMinor).Max());
            if (first && Assembly.GetExecutingAssembly() == TheChosenOne.assembly)
            {
                first = false;
                DontDestroyOnLoad(this);
            }
            else
                DestroyImmediate(this);
        }

        void Update()
        {
            if (!skip && LoadingScreen.Instance?.Screens?.Skip(1)?.FirstOrDefault() != null)
            {
                LoadScreens(AssemblyLoader.loadedAssemblies.Select(a => a.name).ToArray());
                LoadExternal(GameDatabase.Instance.GetConfigNodes("Sigma88LoadingScreens"));
                AddScreens(LoadingScreen.Instance?.Screens?.Skip(1)?.FirstOrDefault());
                skip = true;
            }

            if (HighLogic.LoadedScene == GameScenes.MAINMENU)
            {
                DestroyImmediate(this);
            }
        }

        void LoadScreens(string[] mods)
        {
            if (mods.Contains("GalacticNeighborhood"))
                LoadModScreens("GalacticNeighborhood/LoadingScreens/PluginData/");
            if (mods.Contains("SigmaBinary"))
                LoadModScreens("Sigma/Binary/LoadingScreens/PluginData/");
            if (mods.Contains("SigmaDimensions"))
                LoadModScreens("Sigma/Dimensions/LoadingScreens/PluginData/");
        }

        void LoadExternal(ConfigNode[] nodes)
        {
            for (int i = 0; i < nodes?.Length; i++)
            {
                // Loading Screens
                string[] folders = nodes[i].GetValues("folder");
                for (int j = 0; j < folders?.Length; j++)
                {
                    LoadModScreens(folders[j]);
                }
                if (bool.TryParse(nodes[i].GetValue("removeStockScreens"), out bool clear) && clear)
                {
                    removeStockScreens = true;
                }

                // Loading Tips
                string[] tipsFiles = nodes[i].GetValues("tipsFile");
                for (int j = 0; j < tipsFiles?.Length; j++)
                {
                    LoadModTips(tipsFiles[j]);
                }
                if (bool.TryParse(nodes[i].GetValue("removeStockTips"), out clear) && clear)
                {
                    removeStockTips = true;
                }
            }
        }

        void AddScreens(LoadingScreen.LoadingScreenState screen)
        {
            // Loading Screens
            List<Object> screens = screen?.screens?.ToList();
            if (screens == null)
                screens = new List<Object>();

            if (removeStockScreens)
                screens.Clear();

            screens.AddRange(newScreens);
            if (screens.Count == 0)
            {
                Texture2D black = new Texture2D(1, 1);
                black.SetPixel(1, 1, Color.black);
                black.Apply();
                screens.Add(black);
            }

            screen.screens = screens.ToArray();

            // Loading Tips
            List<string> tips = screen?.tips?.ToList();
            if (tips == null)
                tips = new List<string>();

            if (removeStockTips)
                tips.Clear();

            tips.AddRange(newTips);
            if (tips.Count == 0)
                tips.Add(" ");

            screen.tips = tips.ToArray();
        }

        void LoadModScreens(string mod)
        {
            string filePath = "GameData/" + mod + "LoadingScreen_";

            for (int i = 1; File.Exists(filePath + i + ".dds"); i++)
            {
                Texture2D tex = new Texture2D(2, 2);
                byte[] fileData = File.ReadAllBytes(filePath + i + ".dds");

                tex = LoadDDS(fileData);

                if (tex == null) continue;
                tex.name = filePath.Substring(9);

                newScreens.Add(tex);
            }
        }

        void LoadModTips(string path)
        {
            if (File.Exists("GameData/" + path))
            {
                newTips.AddRange(File.ReadAllLines("GameData/" + path).Where(s => !string.IsNullOrEmpty(s)));
            }
        }

        public static Texture2D LoadDDS(byte[] bytes)
        {
            if (bytes[4] != 124) return null; //this byte should be 124 for DDS image files

            int height = bytes[13] * 256 + bytes[12];
            int width = bytes[17] * 256 + bytes[16];

            int header = 128;
            byte[] data = new byte[bytes.Length - header];
            Buffer.BlockCopy(bytes, header, data, 0, bytes.Length - header);

            Texture2D texture = new Texture2D(width, height, TextureFormat.DXT5, false);
            texture.LoadRawTextureData(data);
            texture.Apply();

            return (texture);
        }
    }
}
