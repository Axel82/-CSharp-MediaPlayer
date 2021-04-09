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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace MediaPlayer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path;
        string memoVideoName;

        public MainWindow()
        {
            InitializeComponent();
        }

        // A timer to display the video's location.
        private DispatcherTimer timerVideoTime;

        // Create the timer and otherwise get ready.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Timer
            timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromSeconds(0.1);
            timerVideoTime.Tick += new EventHandler(timer_Tick);

            // Video folder
            path = Directory.GetCurrentDirectory();
            string pathTempo = path + "/Videos";
            Directory.CreateDirectory(pathTempo);

            // Add all video file into cboxVideosList
            DirectoryInfo directoryInformation = new DirectoryInfo(pathTempo);
            FileInfo[] videosFile = directoryInformation.GetFiles();

            foreach (FileInfo file in videosFile)
            {
                cboxVideosList.Items.Add(file.Name);
            }

            // Media player
            myMediaPlayer.Stop();
            SpeedRatio.Content = "Speed : " + Math.Round(myMediaPlayer.SpeedRatio, 2);
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            sbarPosition.Minimum = 0;
            sbarPosition.Maximum = myMediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            sbarPosition.Visibility = Visibility.Visible;
        }

        // Show the play position in the ScrollBar and TextBox.
        private void ShowPosition()
        {
            sbarPosition.Value = myMediaPlayer.Position.TotalSeconds;
            txtPosition.Text = myMediaPlayer.Position.TotalSeconds.ToString("0.0");
        }

        // Enable and disable appropriate buttons.
        private void EnableButtons(bool is_playing)
        {
            if (is_playing)
            {
                btnPlay.IsEnabled = false;
                btnPause.IsEnabled = true;
                btnPlay.Opacity = 0.5;
                btnPause.Opacity = 1.0;
            }
            else
            {
                btnPlay.IsEnabled = true;
                btnPause.IsEnabled = false;
                btnPlay.Opacity = 1.0;
                btnPause.Opacity = 0.5;
            }
            timerVideoTime.IsEnabled = is_playing;
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            string videoName = (string)cboxVideosList.SelectedItem;

            if(videoName != null)
            {
                if(memoVideoName != videoName)
                {
                    // Get video path and add it to media player source
                    string videoPath = path + "/Videos/" + videoName;
                    myMediaPlayer.Source = new Uri(videoPath, UriKind.Relative);

                    memoVideoName = videoName;
                }

                // Start media player
                myMediaPlayer.Play();
                EnableButtons(true);
            }
            else
            {
                MessageBox.Show("No video selected.", "Video selection error", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            myMediaPlayer.Pause();
            EnableButtons(false);
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            myMediaPlayer.Stop();
            EnableButtons(false);
            ShowPosition();
        }
        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            myMediaPlayer.Stop();
            myMediaPlayer.Play();
            EnableButtons(true);
        }
        private void btnFaster_Click(object sender, RoutedEventArgs e)
        {
            myMediaPlayer.SpeedRatio *= 1.5;
            SpeedRatio.Content = "Speed : " + Math.Round(myMediaPlayer.SpeedRatio, 2);
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            myMediaPlayer.Position += TimeSpan.FromSeconds(10);
            ShowPosition();
        }
        private void btnSlower_Click(object sender, RoutedEventArgs e)
        {
            myMediaPlayer.SpeedRatio /= 1.5;
            SpeedRatio.Content = "Speed : " + Math.Round(myMediaPlayer.SpeedRatio, 2);
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            myMediaPlayer.Position -= TimeSpan.FromSeconds(10);
            ShowPosition();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ShowPosition();
        }

        private void btnSetPosition_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan timespan = TimeSpan.FromSeconds(double.Parse(txtPosition.Text));
            myMediaPlayer.Position = timespan;
            ShowPosition();
        }
    }
}
