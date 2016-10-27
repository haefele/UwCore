using System;

namespace UwCore.Controls
{
    public class PopupOverlayClosingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}