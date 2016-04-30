using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace UwCore.Services.Dialog
{
    public interface IDialogService
    {
        Task ShowAsync(string message, string title, IEnumerable<UICommand> commands);
    }
}