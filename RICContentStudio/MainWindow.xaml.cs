using System;
using System.Collections.Generic;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace RICContentStudio
{
    public partial class MainWindow : Window
    {
        List<Article> CurrentData = new List<Article>();
        bool CriticalMessageTBExists = false;
        static string LastSelectedArticle { get; set; }
        public MainWindow()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("ru-RU");
            InitializeComponent();
            Closing += (s, EA) => Environment.Exit(0);
            RICClient.Init(KeyInputDialog.Show());
            RICClient.InitCompleted += RICClient_InitCompleted;
        }

        void RICClient_InitCompleted(bool isAdmin)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                if (isAdmin)
                {
                    AdminCrown.Fill = new ImageBrush(new BitmapImage(new Uri(GetImageUri("CrownGold"))));
                    AddArticleButton.IsEnabled = RemoveArticleButton.IsEnabled = true;
                    SendButton.Content = Properties.Resources.SendButton;
                }
                else
                {
                    AdminCrown.Fill = new ImageBrush(new BitmapImage(new Uri(GetImageUri("CrownGray"))));
                    AddArticleButton.IsEnabled = RemoveArticleButton.IsEnabled = false;
                    SendButton.Content = Properties.Resources.ProposeButton;
                }
                SendButton.IsEnabled = CancelButton.IsEnabled = true;
            }));
            UpdateDisplayData();
        }
        void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (HeadlinesLV.SelectedIndex >= 0)
                RICEmulator.Emulator.Start(ArticleTitleTB.Text,
                    ArticleImage.Source,
                    ArticleTextTB.Text);
        }

        void AdminCrown_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RICClient.Stop();
            RICClient.Init(KeyInputDialog.Show());
        }
        void UpdateDisplayData()
        {
            UpdateDisplayData(RICRequestProcessor.Take(20));
        }
        void UpdateDisplayData(List<Article> items)
        {
            CurrentData.Clear();
            Dispatcher.Invoke((Action)(HeadlinesLV.Items.Clear));
            foreach (var current in items)
                if (current != null)
                {
                    Dispatcher.Invoke((Action)(() => HeadlinesLV.Items.Add(current.Title)), DispatcherPriority.Background);
                    CurrentData.Add(current);
                }
            Dispatcher.Invoke((Action)(() =>
            {
                if (HeadlinesLV.Items.Count > 0 && HeadlinesLV.SelectedIndex < 0)
                    if (LastSelectedArticle != null && HeadlinesLV.Items.Contains(LastSelectedArticle))
                        HeadlinesLV.SelectedValue = LastSelectedArticle;
                    else
                        HeadlinesLV.SelectedIndex = 0;
                SendButton.IsEnabled = CancelButton.IsEnabled = true;
            }));
        }

        void HeadlinesLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HeadlinesLV.SelectedIndex >= 0)
            {
                ArticleTitleTB.Text = LastSelectedArticle = CurrentData[HeadlinesLV.SelectedIndex].Title;
                ArticleImage.LoadImage(CurrentData[HeadlinesLV.SelectedIndex].Images, false, true);
                ArticleTextTB.Text = CurrentData[HeadlinesLV.SelectedIndex].Text;
            }
        }
        string GetImageUri(string imageName)
        {
            return string.Format("pack://application:,,,/{0};component/Resources/{1}.png", Assembly.GetExecutingAssembly().FullName, imageName);
        }

        void RemoveArticleButton_Click(object sender, RoutedEventArgs e)
        {
            if (HeadlinesLV.SelectedIndex >= 0)
                ProcessRequestResult(RICRequestProcessor.Remove(CurrentData[HeadlinesLV.SelectedIndex].ID).Trim());
            UpdateDisplayData();
        }

        void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var tags = ArticleTextTB.Text.Split('\n').Last(c => !string.IsNullOrWhiteSpace(c));
            if (tags.Contains(" #"))
            {
                SendButton.IsEnabled = CancelButton.IsEnabled = false;
                ProcessRequestResult(RICRequestProcessor.Add(new Article
                {
                    Title = ArticleTitleTB.Text,
                    Text = ArticleTextTB.Text,
                    Tags = tags.Split(' ').Where(c => c.StartsWith("#")).Merge(),
                    Images = ArticleImage.UriSource
                }));
                UpdateDisplayData();
            }
            else NotifyUser(Properties.Resources.TagsMissingError, true);
        }

        private void ArticleImage_ImageLoaded(string result)
        {
            NotifyUser(result, result != "OK");
        }

        bool ProcessRequestResult(string result)
        {
            switch (result)
            {
                case "OK":
                    NotifyUser("OK", false);
                    return true;
                case "403":
                    NotifyUser("ACCESS DENIED!", true);
                    return false;
                default:
                    NotifyUser("UNKNOWN ERROR!", true);
                    return false;
            }
        }

        void NotifyUser(string with, bool isError = false)
        {
            if (!CriticalMessageTBExists)
            {
                var criticalMessageTB = (TextBlock)DataViewGrid.Resources["CriticalMessageTB"];
                DataViewGrid.Children.Add(criticalMessageTB);
                DataViewGrid.RegisterName("CriticalMessageTB", criticalMessageTB);
                CriticalMessageTBExists = true;
                criticalMessageTB.Background = new SolidColorBrush(isError ? Colors.Red : Colors.Green);
                criticalMessageTB.Text = with;
                var criticalMessageStoryboard = (Storyboard)DataViewGrid.Resources["CriticalMessageStoryboard"];
                ((DoubleAnimation)criticalMessageStoryboard.Children[0]).Completed += (s, EA) =>
                {
                    DataViewGrid.Children.Remove(criticalMessageTB);
                    CriticalMessageTBExists = false;
                };
                criticalMessageStoryboard.Begin();
                new SoundPlayer(isError ? Properties.Resources.Error : Properties.Resources.OK).Play();
            }
        }

        private void AddArticleButton_Click(object sender, RoutedEventArgs e)
        {
            ArticleTitleTB.Text = ArticleTextTB.Text = "";
            ArticleImage.Source = null;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            int selection = HeadlinesLV.SelectedIndex;
            UpdateDisplayData();
            HeadlinesLV.SelectedIndex = selection;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTB.Text)) UpdateDisplayData();
            else
                switch (SearchByCB.SelectedIndex)
                {
                    case 0:
                        UpdateDisplayData(RICRequestProcessor.GetArticlesByTitle(SearchTB.Text));
                        break;
                    case 1:
                        UpdateDisplayData(RICRequestProcessor.GetArticlesByText(SearchTB.Text));
                        break;
                    case 2:
                        UpdateDisplayData(RICRequestProcessor.GetArticlesByTags(SearchTB.Text));
                        break;
                    case 3:
                        MessageBox.Show(Properties.Resources.UnimplementedMessage, Properties.Resources.Unimplemented);
                        SearchByCB.SelectedIndex = 0;
                        break;
                }
        }

        private void SearchByCB_Selected(object sender, RoutedEventArgs e)
        {
            switch (SearchByCB.SelectedIndex)
            {
                case 3:
                    MessageBox.Show(Properties.Resources.UnimplementedMessage, Properties.Resources.Unimplemented);
                    SearchByCB.SelectedIndex = 0;
                    break;
            }
        }
    }
}
