using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RICContentStudio
{
    public partial class ImageLoader : Grid
    {
        public ImageSource Source { get { return LoadedImage.Source; } set { LoadedImage.Source = value; } }
        public string UriSource { get; private set; }
        public delegate void ImageLoadedDelegate(string result);
        public event ImageLoadedDelegate ImageLoaded;
        bool SetNullIfEmpty { get; set; }
        bool ReportOK { get; set; }
        WebClient Client { get; set; }
        int LoadRetryCount { get; set; }
        public ImageLoader()
        {
            InitializeComponent();
            ImageLoaded += (s) => ControlsContainer_MouseLeave(null, null);
            Client = new WebClient();
            Client.DownloadDataCompleted += Client_DownloadDataCompleted;
        }

        void ProvideYourFile_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.ImageUploadUnimplemented.Replace(@"\n", "\n"),
                            Properties.Resources.Unimplemented, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Process.Start("http://PostImage.org");
        }

        void ControlsContainer_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)ControlsContainer.Resources["AppearStoryboard"]).Begin();
        }

        void ControlsContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!ProvideYourLinkTB.IsOpened)
                ((Storyboard)ControlsContainer.Resources["DisappearStoryboard"]).Begin();
        }

        void ProvideYourLinkTB_InputCompleted(string text)
        {
            LoadImage(text);
            ProvideYourLinkTB.Text = Properties.Resources.ProvideYourLink;
        }
        public void LoadImage(string uri, bool reportOK = true, bool setNullIfEmpty = false)
        {
            try
            {
                Uri uriResult;
                if (!string.IsNullOrWhiteSpace(uri) && uri != Properties.Resources.ProvideYourLink)
                    if (Uri.TryCreate(uri, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                    {
                        UriSource = uri;
                        ReportOK = reportOK;
                        if (Client.IsBusy) Client.CancelAsync();
                        else
                            Client.DownloadDataAsync(uriResult);
                    }
                    else if (ImageLoaded != null) ImageLoaded("NOT_WELL_FORMED");
            }
            catch (WebException e)
            {
                if (ImageLoaded != null)
                    ImageLoaded("WEB_ERROR:" + e.Message);
            }
            catch (IOException e)
            {
                if (ImageLoaded != null)
                    ImageLoaded("IO_ERROR:" + e.Message);
            }
            catch (NotSupportedException)
            {
                if (LoadRetryCount < 10)
                {
                    LoadImage(uri, reportOK, setNullIfEmpty);
                    LoadRetryCount++;
                }
                else
                {
                    if (ImageLoaded != null)
                        ImageLoaded("NOT_SUPPORTED");
                    LoadRetryCount = 0;
                }
            }
            catch(StackOverflowException)
            {
               
            }
            if (setNullIfEmpty)
                LoadedImage.Source = null;
        }

        private void Client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs EA)
        {
            if (!EA.Cancelled)
                try
                {
                    LoadedImage.Source = EA.Result.ToBitmap();
                    if (ReportOK && ImageLoaded != null) ImageLoaded("OK");
                    LoadRetryCount = 0;
                    return;
                }
                catch (TargetInvocationException) { }
            else
                Client.DownloadDataAsync(new Uri(UriSource));
            if (SetNullIfEmpty)
                LoadedImage.Source = null;
        }
    }
}
