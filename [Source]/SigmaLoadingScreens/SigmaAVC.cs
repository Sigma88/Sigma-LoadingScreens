using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KSP_AVC;


namespace Sigma88LoadingScreensPlugin
{
    internal static class SigmaAVC
    {
        internal static bool skip = false;
        
        internal static void ADD()
        {
            if (AddonLibrary.Populated)
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
            }
        }
    }
}
