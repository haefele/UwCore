using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UwCore.Common
{
    internal static class VisualTreeHelperEx
    {
        public static T GetParent<T>(DependencyObject obj)
            where T : DependencyObject
        {
            while (obj is T == false)
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            return obj as T;
        }
    }
}