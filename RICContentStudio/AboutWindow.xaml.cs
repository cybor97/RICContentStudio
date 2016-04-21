using System.Windows;
namespace RICContentStudio
{
    public partial class AboutWindow : Window
    {
        public string Text
        {
            get { return ContentTB.Text; }
            set { ContentTB.Text = value; }
        }
        AboutWindow()
        {
            InitializeComponent();
        }
        public static new void Show()
        {
            new AboutWindow { Text = Properties.Resources.About.Replace(@"\n","\n") }.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ClosingStoryboard.Begin();
        }
    }
}
