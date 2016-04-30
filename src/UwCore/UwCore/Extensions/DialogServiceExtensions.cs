using System.Threading.Tasks;
using UwCore.Services.Dialog;

namespace UwCore.Extensions
{
    public static class DialogServiceExtensions
    {
        public static Task ShowAsync(this IDialogService self, string message)
        {
            return self.ShowAsync(message, null, null);
        }

        public static Task ShowAsync(this IDialogService self, string message, string title)
        {
            return self.ShowAsync(message, title, null);
        }
    }
}