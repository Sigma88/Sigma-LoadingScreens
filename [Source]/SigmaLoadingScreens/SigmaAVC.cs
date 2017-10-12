using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using KSP_AVC;


namespace Sigma88LoadingScreensPlugin
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class SigmaAVC : MonoBehaviour
    {
        static bool skip = false;

        void Awake()
        {
            if (AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.name == "KSP-AVC") != null)
            {
                DontDestroyOnLoad(this);
            }
            else
            {
                DestroyImmediate(this);
            }
        }

        void Update()
        {
            if (AddonLibrary.Populated && !skip)
            {
                skip = true;
                try
                {
                    FieldInfo addons = typeof(AddonLibrary).GetFields(BindingFlags.NonPublic | BindingFlags.Static).Skip(1).FirstOrDefault();
                    List<Addon> list = (List<Addon>)addons.GetValue(null);
                    string path = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".sigma");
                    Addon addon = new Addon(path);
                    list.Add(addon);
                    addons.SetValue(addons, list);
                }
                catch
                {
                }
                DestroyImmediate(this);
            }
        }
    }
}
