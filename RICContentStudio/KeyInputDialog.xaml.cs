using System.Windows;
using System.Windows.Input;

namespace RICContentStudio
{
    public partial class KeyInputDialog : Window
    {
        KeyInputDialog()
        {
            InitializeComponent();
            KeyTextBox.Focus();
        }
        public static new string Show()
        {
            var dialog = new KeyInputDialog();
            return dialog.ShowDialog().Value ? !string.IsNullOrWhiteSpace(dialog.KeyTextBox.Password) ? dialog.KeyTextBox.Password : " " : " ";
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void _this_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) OKButton_Click(null, null);
        }
    }
}
