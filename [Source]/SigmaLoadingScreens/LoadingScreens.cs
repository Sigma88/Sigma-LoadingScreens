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
        static List<Object> newScreens = new List<Object>();
        static bool first = true;
        static bool skip = false;

        void Awake()
        {
            AssemblyLoader.LoadedAssembly[] list = AssemblyLoader.loadedAssemblies.Where(a => a.name == "Sigma88LoadingScreens").ToArray();
            AssemblyLoader.LoadedAssembly TheChosenOne = list.FirstOrDefault(a => a.versionMinor == list.Select(i => i.versionMinor).Max());
            if (first && Assembly.GetExecutingAssembly() == TheChosenOne.assembly)
            {
                Debug.Log(("[SigmaLog] Version Check:   Sigma LoadingScreens v" + TheChosenOne.assembly.GetName().Version).TrimEnd('0').TrimEnd('.'));
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
                LoadModScreens("GalacticNeighborhood/");
            if (mods.Contains("SigmaBinary"))
                LoadModScreens("Sigma/Binary/");
            if (mods.Contains("SigmaDimensions"))
                LoadModScreens("Sigma/Dimensions/");

            if (newScreens.Count > 0)
                AddScreens(LoadingScreen.Instance?.Screens?.Skip(1)?.FirstOrDefault());
        }

        void AddScreens(LoadingScreen.LoadingScreenState screen)
        {
            List<Object> screens = screen?.screens?.ToList();
            if (screens == null)
                screens = new List<Object>();
            screens.AddRange(newScreens);
            screen.screens = screens.ToArray();
        }

        void LoadModScreens(string mod)
        {
            string filePath = "GameData/" + mod + "LoadingScreens/PluginData/LoadingScreen_";

            for (int i = 1; File.Exists(filePath + i + ".dds"); i++)
            {
                Texture2D tex = new Texture2D(2, 2);
                byte[] fileData = File.ReadAllBytes(filePath + i + ".dds");

                tex = LoadDDS(fileData);

                if (tex == null) continue;
                tex.name = mod.Replace("/", "") + "_" + i;

                newScreens.Add(tex);
                Debug.Log("[Sigma88Log] LoadingScreens: Added texture " + tex.name);
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
