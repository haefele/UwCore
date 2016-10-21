using System.Threading.Tasks;
using Windows.UI.Popups;
using UwCore.Services.Dialog;

namespace UwCore.Extensions
{
    public static class DialogServiceExtensions
    {
        public static Task<IUICommand> ShowAsync(this IDialogService self, string message)
        {
            return self.ShowAsync(message, null, null);
        }

        public static Task<IUICommand> ShowAsync(this IDialogService self, string message, string title)
        {
            return self.ShowAsync(message, title, null);
        }
    }
}