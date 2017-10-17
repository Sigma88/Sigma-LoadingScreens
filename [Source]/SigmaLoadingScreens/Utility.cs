using System;
using System.Collections.Generic;
using UnityEngine;


namespace Sigma88LoadingScreensPlugin
{
    public static class Utility
    {
        public static Texture2D LoadDDS(byte[] bytes)
        {
            try
            {
                if (bytes[4] != 124) return null; //this byte should be 124 for DDS image files

                int height = bytes[13] * 256 + bytes[12];
                int width = bytes[17] * 256 + bytes[16];

                int header = 128;
                byte[] data = new byte[bytes.Length - header];
                Buffer.BlockCopy(bytes, header, data, 0, bytes.Length - header);

                TextureFormat format;
                switch ((double)height * width / data.Length)
                {
                    case 1d:
                        format = TextureFormat.DXT5;
                        break;
                    case 2d:
                        format = TextureFormat.DXT1;
                        break;
                    default:
                        return null;
                }

                Texture2D texture = new Texture2D(width, height, format, false);

                texture.LoadRawTextureData(data);
                texture.Apply();

                return (texture);
            }
            catch
            {
                return null;
            }
        }

        public static void Clone(LoadingScreen.LoadingScreenState original, LoadingScreen.LoadingScreenState copy)
        {
            if (original != null && copy != null)
            {
                copy.displayTime = original.displayTime;
                copy.fadeInTime = original.fadeInTime;
                copy.fadeOutTime = original.fadeOutTime;
                copy.tipTime = original.tipTime;
            }
        }

        public static void Scramble<T>(this List<T> list)
        {
            if (list?.Count > 0)
            {
                System.Random rnd = new System.Random();
                List<T> oldList = new List<T>();
                oldList.AddRange(list);
                list.Clear();

                int count = oldList.Count;

                for (int i = 0; i < count; i++)
                {
                    int index = rnd.Next(oldList.Count);
                    list.Add(oldList[index]);
                    oldList.RemoveAt(index);
                }
            }
        }
    }
}
