using System.Resources;


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

        internal static byte[] SigmaLSLS_1
        {
            get
            {
                object obj = ResourceManager.GetObject("SigmaLSLS_1");
                return ((byte[])(obj));
            }
        }
    }
}
