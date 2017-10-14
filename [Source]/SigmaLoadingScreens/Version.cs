using UnityEngine;


namespace Sigma88LoadingScreensPlugin
{
    public static class Version
    {
        public static System.Version number
        {
            get
            {
                return LoadingScreenSettings.TheChosenOne?.assembly?.GetName()?.Version;
            }
        }

        internal static void Print()
        {
            Debug.Log("[SigmaLog] Version Check:   Sigma LoadingScreens v" + number?.ToString()?.TrimEnd('0')?.TrimEnd('.'));
        }
    }
}
