using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RICContentStudio
{
    public partial class EditableLabel : Grid
    {
        public bool ClearThisTime { get; set; }
        public bool IsOpened { get { return ContentTextBox.Visibility == Visibility.Visible; } }
        public string Text
        {
            get
            {
                return ContentTB.Text;
            }
            set
            {
                ContentTB.Text = ContentTextBox.Text = value;
            }
        }
        public double FontSize
        {
            get
            {
                return ContentTB.FontSize;
            }
            set
            {
                ContentTB.FontSize = ContentTextBox.FontSize = value;
            }
        }
        public TextAlignment TextAlignment
        {
            get
            {
                return ContentTB.TextAlignment;
            }
            set
            {
                ContentTB.TextAlignment = ContentTextBox.TextAlignment = value;
            }
        }
        public Brush Foreground
        {
            get
            {
                return ContentTB.Foreground;
            }
            set
            {
                ContentTB.Foreground = ContentTextBox.Foreground = value;
            }
        }
        public TextWrapping TextWrapping
        {
            get
            {
                return ContentTB.TextWrapping;
            }
            set
            {
                ContentTB.TextWrapping = ContentTextBox.TextWrapping = value;
            }
        }
        public delegate void InputCompletedDelegate(string text);
        public event InputCompletedDelegate InputCompleted;
        public EditableLabel()
        {
            InitializeComponent();
        }
        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            OKButton.Visibility = EditButton.Visibility = ContentTextBox.Visibility = Visibility.Hidden;
            ContentTB.Visibility = Visibility.Visible;
            ContentTB.Text = ContentTextBox.Text;
            if (InputCompleted != null)
                InputCompleted(ContentTB.Text);
        }

        void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) OKButton_Click(null, null);
            else if (e.Key == Key.Escape)
            {
                OKButton.Visibility = EditButton.Visibility = ContentTextBox.Visibility = Visibility.Hidden;
                ContentTB.Visibility = Visibility.Visible;
                ContentTextBox.Text = ContentTB.Text;
            }
        }

        void ContentTB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OKButton.Visibility = EditButton.Visibility = ContentTextBox.Visibility = Visibility.Visible;
            ContentTB.Visibility = Visibility.Hidden;
            if (ClearThisTime)
            {
                ContentTB.Text = "";
                ClearThisTime = false;
            }
            ContentTextBox.Text = ContentTB.Text;
        }
        public void UnFocus()
        {
            OKButton_Click(null, null);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var result = TextEditorWindow.Show(ContentTextBox.Text);
            if (result.Ok)
                ContentTextBox.Text = result.Data;
        }
    }
}
