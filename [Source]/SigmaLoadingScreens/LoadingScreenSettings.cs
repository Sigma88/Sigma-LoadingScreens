using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Sigma88LoadingScreensPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class LoadingScreenSettings : MonoBehaviour
    {
        static bool first = true;
        static bool skip = false;

        // Loading Screens
        public static bool removeStockScreens = false;
        public static Texture2D logoScreen = null;
        public static List<string> externalMods = new List<string>();
        public static List<Object> newScreens = new List<Object>();

        // Loading Tips
        public static bool removeStockTips = false;
        public static string logoTip = "";
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
                skip = true;
                LoadingScreens.LoadBuiltIn(AssemblyLoader.loadedAssemblies.Select(a => a.name).ToArray());
                LoadingScreens.LoadExternal(GameDatabase.Instance.GetConfigNodes("Sigma88LoadingScreens"));
                LoadingScreens.AddScreens(LoadingScreen.Instance?.Screens?.Skip(1)?.FirstOrDefault());
            }

            if (HighLogic.LoadedScene == GameScenes.MAINMENU)
            {
                DestroyImmediate(this);
            }
        }
    }
}
