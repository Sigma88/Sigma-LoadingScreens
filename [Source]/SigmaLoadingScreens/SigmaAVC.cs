using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using KSP_AVC;


namespace Sigma88LoadingScreensPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class SigmaAVC : MonoBehaviour
    {
        static bool first = true;
        static bool skip = false;

        void Awake()
        {
            if (first && AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "KSP-AVC") != null)
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
            if (!skip && AddonLibrary.Populated)
            {
                skip = true;
                try
                {
                    FieldInfo addons = typeof(AddonLibrary).GetFields(BindingFlags.NonPublic | BindingFlags.Static).Skip(1).FirstOrDefault();
                    List<Addon> list = (List<Addon>)addons.GetValue(null);
                    string path = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".sigma");
                    if (File.Exists(path))
                    {
                        Addon addon = new Addon(path);
                        list.Add(addon);
                        addons.SetValue(addons, list);
                    }
                }
                catch
                {
                }
                DestroyImmediate(this);
            }
        }
    }
}
