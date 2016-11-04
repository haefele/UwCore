using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UwCore.Extensions
{
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Determines whether this <paramref name="element"/> is loaded.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>true if the element is loaded; otherwise, false. </returns>
        public static bool IsLoaded(this FrameworkElement element)
        {
            try
            {
                //It is loaded if it has a parent
                if ((element.Parent ?? VisualTreeHelper.GetParent(element)) != null)
                {
                    return true;
                }

                //Or if it's the current window root
                var rootVisual = Window.Current.Content;
                if (rootVisual != null)
                {
                    return element == rootVisual;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
