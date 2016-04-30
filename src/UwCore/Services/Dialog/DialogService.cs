using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using UwCore.Extensions;

namespace UwCore.Services.Dialog
{
    public class DialogService : IDialogService
    {
        public async Task ShowAsync(string message, string title, IEnumerable<UICommand> commands)
        {
            var dialog = new MessageDialog(message, title ?? string.Empty);

            foreach (var command in commands.EmptyIfNull())
            {
                dialog.Commands.Add(command);
            }

            await dialog.ShowAsync();
        }
    }
}