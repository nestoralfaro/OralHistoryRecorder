using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OralHistoryRecorder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AudioRecorderLib audioRecorder;
        RecordingClass recording = new RecordingClass();

        private DispatcherTimer dispatcherTimer, demoDispatcher;
        private DateTime startedTime;
        private TimeSpan timePassed, timeSinceLastStop;
        bool isStop = false;

        public MainPage()
        {
            InitializeComponent();
            audioRecorder = new AudioRecorderLib();
        }

        private async void btnRecord_Click(object sender, RoutedEventArgs e)
        {

            if (isStop == false)
            {
                isStop = true;
                startedTime = DateTime.Now;
                DispatcherTimerSetup();
                btnRecord.Content = "Stop";
                await audioRecorder.Record();
            }
            else
            {
                isStop = false;
                dispatcherTimer.Stop();
                demoDispatcher.Stop();
                timeText.Text = "00:00:00:000";
                btnRecord.Content = "Start";
                await audioRecorder.StopRecording();




            }

        }

        private async void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            await audioRecorder.PlayFromDisk(Dispatcher);
        }

        private void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();

            timeSinceLastStop = TimeSpan.Zero;
            timeText.Text = "00:00:00:000";
            demoDispatcher = new DispatcherTimer();
            demoDispatcher.Tick += DemoDispatcher_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            demoDispatcher.Start();
        }

        private void enterTagButton_Click(object sender, RoutedEventArgs e)
        {

            string curDir = Directory.GetCurrentDirectory();



            //var tfile = TagLib.File.Create(@"C:\Users\aurru\OneDrive\Desktop\NewRecording.mp3");
            var dir = ApplicationData.Current.LocalFolder.Path;
            var tfile = TagLib.File.Create(dir + "\\NewRecording.mp3");
            string title = tfile.Tag.Title;
            TimeSpan duration = tfile.Properties.Duration;
            Debug.WriteLine("Title: {0}, duration: {1}", title, duration);

            // change title in the file
            tfile.Tag.Title = "done with taglibsharp";
            tfile.Save();
        }

        private void Tag_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;
            toggleButton.Background = new SolidColorBrush(Colors.Black);
            toggleButton.Foreground = new SolidColorBrush(Colors.Gold);
        }

        private void Tag_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;
            toggleButton.Background = new SolidColorBrush(Colors.White);
            toggleButton.Foreground = new SolidColorBrush(Colors.Black);
        }


        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = ComboBox1.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string selectedOption = selectedItem.Content.ToString();
                // Do something with the selected option...
            }
        }


        private string MakeDigitString(int number, int count)
        {
            string result = "0";
            if (count == 2)
            {
                if (number < 10)
                {
                    result = "0" + number;
                }
                else
                {
                    result = number.ToString();
                }
            }
            else if (count == 3)
            {
                if (number < 10)
                    result = "00" + number;
                else if (number > 9 && number < 100)
                    result = "0" + number;
                else
                    result = number.ToString();
            }
            return result;
        }

        private void DemoDispatcher_Tick(object sender, object e)
        {
            timePassed = DateTime.Now - startedTime;
            timeText.Text = MakeDigitString((timeSinceLastStop + timePassed).Hours, 2) + ":"
                + MakeDigitString((timeSinceLastStop + timePassed).Minutes, 2) + ":"
                + MakeDigitString((timeSinceLastStop + timePassed).Seconds, 2) + ":"
                + MakeDigitString((timeSinceLastStop + timePassed).Milliseconds, 3);
        }
    }
}
