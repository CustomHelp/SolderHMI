using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LOET_HMI.SystemPages.PopUps
{
    /// <summary>
    /// Interaktionslogik für WindowPopUpVideoPlayer.xaml
    /// </summary>
    public partial class WindowPopUpVideoPlayer : Window
    {

        private string videoPath = GlobalVar.videoPath;
        private string path;

        public WindowPopUpVideoPlayer(string filename)
        {
            InitializeComponent();

            videoPlayer.Source = new Uri(filename,UriKind.Absolute);
            videoPlayer.LoadedBehavior = MediaState.Manual;
            videoPlayer.UnloadedBehavior = MediaState.Manual;
            SetUpVideoList();
            videoPlayer.Play();
            string[] splitList = filename.Split('\\');
            string name = splitList.Last();
            videoLabel.Content = name.Substring(0, name.Length - 4);
            for (int i = 0; i < videoList.Items.Count; i++)
            {
                ListBoxItem item = videoList.Items[i] as ListBoxItem;
                if (item.Content.ToString() == videoLabel.Content.ToString())
                {
                    videoList.SelectedIndex = i;
                }
            }
        }

        private void SetUpVideoList()
        {
            DirectoryInfo d = new DirectoryInfo(videoPath);
            FileInfo[] files = d.GetFiles();

            Style style = new Style(typeof(ListBoxItem));

            Trigger triggerIsSelected = new Trigger { Property = ListBoxItem.IsSelectedProperty, Value = true };
            triggerIsSelected.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, Brushes.White));
            triggerIsSelected.Setters.Add(new Setter(ListBoxItem.ForegroundProperty, Brushes.Indigo));
            Trigger triggerIsUnSelected = new Trigger { Property = ListBoxItem.IsSelectedProperty, Value = false };
            triggerIsUnSelected.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, Brushes.Indigo));
            triggerIsUnSelected.Setters.Add(new Setter(ListBoxItem.ForegroundProperty, Brushes.White));
            Trigger triggerIsMouseOver = new Trigger { Property = ListBoxItem.IsMouseOverProperty, Value = true };
            triggerIsMouseOver.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, Brushes.Transparent));
            triggerIsMouseOver.Setters.Add(new Setter(ListBoxItem.ForegroundProperty, Brushes.Indigo));
            style.Triggers.Add(triggerIsMouseOver);
            style.Triggers.Add(triggerIsSelected);
            style.Triggers.Add(triggerIsUnSelected);
            foreach (FileInfo file in files)
            {
                ListBoxItem listItem = new ListBoxItem();
                listItem.Content = file.Name.Substring(0, file.Name.Length - 4);
                listItem.Style = style;
                listItem.Height = 30;
                videoList.Items.Add(listItem);
            }
        }

        private void VideoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = sender as ListBox;
            ListBoxItem item = box.Items[box.SelectedIndex] as ListBoxItem;
            string name = item.Content.ToString();
            path = "";
            path = videoPath + name + ".mov";
            videoPlayer.Source = new Uri(path,UriKind.Absolute);
            videoLabel.Content = name;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Stretch = Stretch.Fill;
            videoPlayer.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Stop();
        }


        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            videoPlayer.Stop();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Position += new TimeSpan(0, 0, 5);
        }

        private void BackwardButton_Click(object sender, RoutedEventArgs e)
        {
            videoPlayer.Position -= new TimeSpan(0, 0, 5);
        }

        private void VideoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.ToString());
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
