using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UwCore.Extensions
{
    public static class ByteArrayExtensions
    {
        public static ImageSource ToImage(this byte[] self)
        {
            if (self == null)
                return null;

            if (self.Length == 0)
                return null;

            var stream = new InMemoryRandomAccessStream();
            stream.WriteAsync(self.AsBuffer()).AsTask().Wait();
            stream.Seek(0);

            var myImage = new BitmapImage();
            myImage.SetSource(stream);

            return myImage;
        }
    }
}