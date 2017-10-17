namespace Sigma88LoadingScreensPlugin
{
    internal static class Debug
    {
        internal static bool debug = true;
        static string Tag = "[SigmaLog LS]";

        internal static void Log(string message)
        {
            if (debug)
            {
                UnityEngine.Debug.Log(Tag + ": " + message);
            }
        }

        internal static void Log(string Method, string message)
        {
            if (debug)
            {
                UnityEngine.Debug.Log(Tag + " " + Method + ": " + message);
            }
        }
    }
}
