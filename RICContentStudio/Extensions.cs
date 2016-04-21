using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace RICContentStudio
{
    public static class Extensions
    {
        public static byte[] ToByteArray(this BitmapImage image)
        {
            byte[] buffer = null;
            using (BinaryReader br = new BinaryReader(image.StreamSource))
                buffer = br.ReadBytes((int)image.StreamSource.Length);
            return buffer;
        }
        public static BitmapImage ToBitmap(this byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
        public static string Merge(this IEnumerable<string> data)
        {
            var result = "";
            foreach (var current in data)
                result += current;
            return result;
        }
    }
}
