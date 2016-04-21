using System.Windows;
using System.Windows.Documents;

namespace RICContentStudio
{
    public class TextEditorWindowResult
    {
        public bool Ok { get; set; }
        public string Data { get; set; }
    }
    public partial class TextEditorWindow : Window
    {
        bool TextEditingResult { get; set; }
        TextEditorWindow()
        {
            InitializeComponent();
        }
        public static TextEditorWindowResult Show(string with)
        {
            var window = new TextEditorWindow();
            window.ContentTB.AppendText(with);
            if (window.ShowDialog().Value)
                return new TextEditorWindowResult
                {
                    Ok = true,
                    Data = new TextRange(window.ContentTB.Document.ContentStart, window.ContentTB.Document.ContentEnd).Text
                };
            else return new TextEditorWindowResult { Ok = false };
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            TextEditingResult = true;
            ClosingStoryboard.Begin();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClosingStoryboard.Begin();
        }
    }
}