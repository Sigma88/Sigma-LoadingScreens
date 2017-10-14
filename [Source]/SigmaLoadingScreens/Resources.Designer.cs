using System.Resources;
using UnityEngine;


namespace Sigma88LoadingScreensPlugin
{
    internal class Resources
    {
        private static ResourceManager resourceMan;

        internal Resources()
        {
        }

        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    ResourceManager temp = new ResourceManager("Sigma88LoadingScreensPlugin.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        internal static Texture2D SigmaLSLS_1
        {
            get
            {
                Texture2D tex = Utility.LoadDDS((byte[])(ResourceManager.GetObject("SigmaLSLS_1")));
                tex.name = "SigmaLSLS_1";
                return tex;
            }
        }
    }
}
