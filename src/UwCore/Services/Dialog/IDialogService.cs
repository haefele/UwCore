using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace UwCore.Services.Dialog
{
    public interface IDialogService
    {
        Task<IUICommand> ShowAsync(string message, string title, IEnumerable<IUICommand> commands);
    }
}