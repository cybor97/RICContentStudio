using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace RICContentStudio
{
    public partial class MainWindow
    {
        enum ResizeDirection { Left = 61441, Right = 61442, Top = 61443, TopLeft = 61444, TopRight = 61445, Bottom = 61446, BottomLeft = 61447, BottomRight = 61448, }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        private void WindowResize(object sender, MouseButtonEventArgs e) 
        {
            SendMessage(((HwndSource)PresentationSource.FromVisual((Visual)sender)).Handle, 0x112, (IntPtr)ResizeDirection.TopRight, IntPtr.Zero);
        }
    }
}
