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
                Debug.Log("[SigmaLog] Version Check:   Sigma LoadingScreens v" + TheChosenOne.assembly.GetName().Version);
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

        void LoadModScreens(string mod)
        {
            string filePath = "GameData/" + mod + "LoadingScreens/LoadingScreen_";

            for (int i = 1; File.Exists(filePath + i + ".png"); i++)
            {
                Texture2D tex = new Texture2D(2, 2);
                tex.name = mod.Replace("/", "") + "_" + i;
                byte[] fileData = File.ReadAllBytes(filePath + i + ".png");

                tex.LoadImage(fileData);

                if (tex != null)
                    newScreens.Add(tex);
            }
        }

        void AddScreens(LoadingScreen.LoadingScreenState screen)
        {
            List<Object> screens = screen?.screens?.ToList();
            if (screens == null)
                screens = new List<Object>();
            screens.AddRange(newScreens);
            screen.screens = screens.ToArray();
        }
    }
}
