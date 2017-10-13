using UnityEngine;


namespace Sigma88LoadingScreensPlugin
{
    public static class Version
    {
        public static string number
        {
            get
            {
                return "v" + LoadingScreenSettings.TheChosenOne?.assembly?.GetName()?.Version?.ToString()?.TrimEnd('0').TrimEnd('.');
            }
        }

        internal static void Print()
        {
            Debug.Log("[SigmaLog] Version Check:   Sigma LoadingScreens " + number);
        }
    }
}
