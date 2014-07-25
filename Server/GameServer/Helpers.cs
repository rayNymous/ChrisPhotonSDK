using System;
using System.IO;
using System.Text;
using ProtoBuf;

namespace GameServer
{
    public class Helpers
    {
        public static T Deserialize<T>(byte[] bytes)
        {
            T deserialized;

            using (var ms = new MemoryStream(bytes))
            {
                deserialized = Serializer.Deserialize<T>(ms);
            }

            return deserialized;
        }

        public static byte[] Serialize<T>(T obj)
        {
            byte[] b = null;

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                b = new byte[ms.Position];
                byte[] fullB = ms.GetBuffer();
                Array.Copy(fullB, b, b.Length);
            }

            return b;
        }

        public static string ReadFile(string path)
        {
            var sb = new StringBuilder();
            using (var sr = new StreamReader(Path.GetFullPath(path)))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            string allines = sb.ToString();

            return allines;
        }
    }
}