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
        internal static AssemblyLoader.LoadedAssembly TheChosenOne = null;
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

        // Logo
        public static List<KeyValuePair<Texture2D, string>> logos = new List<KeyValuePair<Texture2D, string>>();

        // Themes
        static int? lastTheme = null;
        static Object lastScreen = null;
        public static List<KeyValuePair<Object[], string[]>> themes = new List<KeyValuePair<Object[], string[]>>();

        void Awake()
        {
            AssemblyLoader.LoadedAssembly[] list = AssemblyLoader.loadedAssemblies.Where(a => a.name == "Sigma88LoadingScreens").ToArray();
            TheChosenOne = list.FirstOrDefault(a => a.assembly.GetName().Version.Minor == list.Select(i => i.assembly.GetName().Version.Minor).Max());
            if (first && Assembly.GetExecutingAssembly() == TheChosenOne.assembly)
            {
                first = false;
                DontDestroyOnLoad(this);
            }
            else
            {
                DestroyImmediate(this);
            }
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

            if (themes?.Count() > 0 && LoadingScreen.Instance?.Screens?.Skip(logos?.Count > 0 ? 2 : 1)?.FirstOrDefault()?.activeScreen != null)
            {
                LoadingScreen.LoadingScreenState screen = LoadingScreen.Instance.Screens.Skip(logos?.Count > 0 ? 2 : 1).FirstOrDefault();

                if (lastScreen != screen.activeScreen)
                {
                    lastScreen = screen.activeScreen;

                    int? theme = null;
                    try { theme = themes.FindIndex(t => t.Key?.Contains(screen?.activeScreen) == true); } catch { }

                    if (lastTheme != theme)
                    {
                        lastTheme = theme;
                        screen.tips = theme == null ? newTips.ToArray() : themes[(int)theme].Value;
                        LoadingScreen.Instance.SetTip(screen);
                    }
                }
            }

            if (!SigmaAVC.skip)
                SigmaAVC.ADD();

            if (HighLogic.LoadedScene == GameScenes.MAINMENU)
            {
                DestroyImmediate(this);
            }
        }
    }
}
