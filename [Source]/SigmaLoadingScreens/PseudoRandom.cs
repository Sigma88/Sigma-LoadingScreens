using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Sigma88LoadingScreensPlugin
{
    static class PseudoRandom
    {
        internal static int state = 0;

        internal static List<Object>[] states =
        {
            new List<Object>(),
            new List<Object>(),
            new List<Object>()
        };

        internal static void Choose(Object obj)
        {
            if (states[state].Contains(obj))
            {
                states[state].Remove(obj);
                states[(state + 2) % 3].Add(obj);

                if (states[state].Count == 0)
                {
                    state = (state + 1) % 3;
                }
            }
        }

        internal static void Add(Object[] objects)
        {
            if (objects?.Length > 0)
            {
                if (objects?.Length == 1)
                {
                    states[0] = objects.ToList();
                    states[1] = objects.ToList();
                }
                else
                {
                    states[0] = objects.Take(objects.Length / 2).ToList();
                    states[1] = objects.Skip(objects.Length / 2).ToList();
                }
            }
        }
    }
}
