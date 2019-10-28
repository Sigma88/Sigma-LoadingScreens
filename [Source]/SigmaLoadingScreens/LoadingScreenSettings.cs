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

        // Settings
        static UrlDir.UrlConfig[] SettingsNodes;

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
            TheChosenOne = list.FirstOrDefault(a => a.assembly.GetName().Version == list.Select(i => i.assembly.GetName().Version).Max());
            if (first && Assembly.GetExecutingAssembly() == TheChosenOne.assembly)
            {
                Version.Print();
                first = false;
                DontDestroyOnLoad(this);
            }
            else
            {
                DestroyImmediate(this);
            }
        }

        void Start()
        {
            SettingsNodes = GameDatabase.Instance.GetConfigs("Sigma88LoadingScreens");

            for (int i = 0; i < SettingsNodes?.Length; i++)
            {
                bool.TryParse(SettingsNodes[i].config.GetValue("debug"), out Debug.debug);
                if (Debug.debug) return;
            }
        }

        void Update()
        {
            if (!skip && LoadingScreen.Instance?.Screens?.LastOrDefault() != null)
            {
                skip = true;
                Debug.Log("Settings", "Loaded assembly location = " + Assembly.GetExecutingAssembly().Location);
                //Debug.Log("Settings", "Checking for BuiltIn mods...");
                //LoadingScreens.LoadBuiltIn(AssemblyLoader.loadedAssemblies.Select(a => a.name).ToArray());
                Debug.Log("Settings", "Checking for External mods...");
                LoadingScreens.LoadExternal(SettingsNodes);
                Debug.Log("Settings", "Applying Settings to LoadingScreen");
                LoadingScreens.AddScreens(LoadingScreen.Instance?.Screens?.LastOrDefault());

                for (int i = 0; i < 3; i++)
                {
                    if (LoadingScreen.Instance?.Screens?.ElementAt(i) != null)
                    {
                        LoadingScreen.Instance.Screens.ElementAt(i).displayTime = 0.8f;
                    }
                }
            }

            if (themes?.Count() > 0 && LoadingScreen.Instance?.Screens?.ElementAt(logos?.Count > 0 ? LoadingScreen.Instance.Screens.Count - 1 : LoadingScreen.Instance.Screens.Count - 2)?.activeScreen != null)
            {
                LoadingScreen.LoadingScreenState screen = LoadingScreen.Instance.Screens.ElementAt(logos?.Count > 0 ? LoadingScreen.Instance.Screens.Count - 1 : LoadingScreen.Instance.Screens.Count - 2);

                if (lastScreen != screen.activeScreen)
                {
                    Debug.Log("Settings", "Loading screen image has changed");

                    Debug.Log("Settings", "previous image = " + lastScreen.name);
                    lastScreen = screen.activeScreen;
                    Debug.Log("Settings", "current image = " + lastScreen.name);

                    int? theme = null;
                    try { theme = themes.FindIndex(t => t.Key?.Contains(screen?.activeScreen) == true); } catch { }

                    if (lastTheme != theme)
                    {
                        Debug.Log("Settings", "Loading screen theme has changed");

                        Debug.Log("Settings", "previous theme = " + lastTheme ?? "null");
                        lastTheme = theme;
                        Debug.Log("Settings", "current theme = " + lastTheme ?? "null");

                        Debug.Log("Settings", "previous tip count = " + screen?.tips?.Length);
                        screen.tips = theme == null ? newTips.ToArray() : themes[(int)theme].Value;
                        Debug.Log("Settings", "current tip count = " + screen?.tips?.Length);

                        LoadingScreen.Instance.SetTip(screen);
                    }
                }
            }

            if (HighLogic.LoadedScene == GameScenes.MAINMENU)
            {
                Debug.Log("Settings", "MainMenu is here. ByeBye!");
                DestroyImmediate(this);
            }
        }
    }
}
