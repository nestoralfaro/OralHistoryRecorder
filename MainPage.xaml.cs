using NAudio.Wave;
using OralHistoryRecorder.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
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

        private DispatcherTimer dispatcherTimer, demoDispatcher;
        private DateTime startedTime, frozenTime;
        private TimeSpan timePassed, timeSinceLastStop;
        bool isStop = false;
        bool isPaused = false;
        StudentRecording student;

        public MainPage()
        {
            InitializeComponent();
            audioRecorder = new AudioRecorderLib();
            student = new StudentRecording();

            btnStartRecording.IsEnabled = false;
            btnPauseRecording.IsEnabled = false;
            btnRemoveRecording.IsEnabled = false;
            btnPlay.IsEnabled = false;

            btnEnterTag.IsEnabled = false;
        }
        private async void btnPauseRecording_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused == false)
            {
                // Should pause
                isPaused = true;
                demoDispatcher.Stop();
                dispatcherTimer.Stop();
                PauseText.Text = "Resume";
                PauseIcon.Symbol = Symbol.Back;

                //timePassed = timeSinceLastStop - timePassed;

                Debug.WriteLine("thou shall pause time now");
                Debug.WriteLine(timePassed);
                Debug.WriteLine(timeSinceLastStop);
                Debug.WriteLine(DateTime.Now);
                Debug.WriteLine(startedTime);

                //demoDispatcher.Tick -= DemoDispatcher_Tick;

                await audioRecorder.PauseRecording();
            } else
            {

                //demoDispatcher.Tick += DemoDispatcher_Tick;
                // Should resume
                isPaused = false;
                demoDispatcher.Start();
                dispatcherTimer.Start();

                Debug.WriteLine("thou shall resume time now");
                Debug.WriteLine(timePassed);
                Debug.WriteLine(timeSinceLastStop);
                Debug.WriteLine(DateTime.Now);
                Debug.WriteLine(startedTime);

                PauseText.Text = "Pause";
                PauseIcon.Symbol = Symbol.Pause;
                await audioRecorder.ResumeRecording();
            }

        }

        private async void btnStartRecording_Click(object sender, RoutedEventArgs e)
        {
            if (isStop == false)
            {
                isStop = true;
                startedTime = DateTime.Now;
                DispatcherTimerSetup();
                RecordingIcon.Symbol = Symbol.Stop;
                RecordingText.Text = "Stop";


                btnPauseRecording.IsEnabled = true;

                await audioRecorder.Record();
            }
            else
            {
                isStop = false;
                dispatcherTimer.Stop();
                demoDispatcher.Stop();
                timeText.Text = "00:10:00:000";
                RecordingIcon.Symbol = Symbol.Microphone;
                RecordingText.Text = "Start"; 

                btnRemoveRecording.IsEnabled = true;
                btnPlay.IsEnabled = true;
                btnEnterTag.IsEnabled = true;
                btnPauseRecording.IsEnabled = false;

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
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();

            timeSinceLastStop = new TimeSpan(0, 0, 10, 0, 0);

            timeText.Text = "00:10:00:000";
            demoDispatcher = new DispatcherTimer();
            demoDispatcher.Tick += DemoDispatcher_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            demoDispatcher.Start();
        }

        private void btnEnterTag_Click(object sender, RoutedEventArgs e)
        {
            var dir = ApplicationData.Current.LocalFolder.Path;
            Debug.WriteLine("the dir where it is being stored");
            Debug.WriteLine(dir);
            var tfile = TagLib.File.Create(dir + "\\" + audioRecorder.audioFileName);
            string title = tfile.Tag.Title;
            TimeSpan duration = tfile.Properties.Duration;
            Debug.WriteLine("Title: {0}, duration: {1}", title, duration);

            //tfile.Tag.Title = nameTextBox.Text + student.RecId;
            tfile.Tag.Title = audioRecorder.audioFileName.Replace(".mp3", "");

            ComboBoxItem selectedItem = decadeComboBox.SelectedItem as ComboBoxItem;
            string selectedOption = selectedItem.Content.ToString();

            tfile.Tag.Year = UInt32.Parse(selectedOption);
            Debug.WriteLine("this is the tag to add hopefully");
            Debug.WriteLine(student.tag);
            //tfile.Tag.Comment = student.tag;
            tfile.Tag.Comment = (bool)ChapelTag.IsChecked ? "Chapel," : "";
            tfile.Tag.Comment += (bool)DormTag.IsChecked ? "Dorm," : "";
            tfile.Tag.Comment += String.IsNullOrEmpty(enteredCustomTag.Text) ? "" : (enteredCustomTag.Text + ",");

            //Restore to default
            //nameTextBox.Text = String.Empty;
            enteredCustomTag.Text = String.Empty;
            ChapelTag.IsChecked = false;
            DormTag.IsChecked = false;
            //btnEnterTag.IsEnabled = false;

            ++student.RecId;

            // change title in the file
            tfile.Save();
        }

        private void Tag_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;
            string tag = toggleButton.Name.Replace("Tag", "");
            if (!student.tag.Contains(tag))
            {
                student.tag += toggleButton.Name.Replace("Tag", "") + ',';
            }
        }

        private void Tag_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;
            string tag = toggleButton.Name.Replace("Tag", "");

            if (student.tag.Contains(tag))
            {
                student.tag = student.tag.Replace(tag + ',', "");
            }
        }


        private void decadeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = decadeComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string selectedOption = selectedItem.Content.ToString();
                // Do something with the selected option...
            }
        }

        private async void CommandInvokedHandler(IUICommand command)
        {
            // Check which command was invoked
            if (command.Label == "Yes")
            {
                // Remove the file
                // TODO: Implement file removal logic here
                audioRecorder.RemoveAudioFile();

                // Show a message that the file was successfully removed
                var messageDialog = new MessageDialog($"{Regex.Replace(audioRecorder.audioFileName, @"\d*\.mp3", "")} was successfully removed.");
                await messageDialog.ShowAsync();
            }
            else
            {
                // Do nothing
            }
        }

        private async void btnRemoveRecording_Click(object sender, RoutedEventArgs e)
        {
            // Create the message dialog and set its content
            var messageDialog = new MessageDialog($"Are you sure you want to remove {Regex.Replace(audioRecorder.audioFileName, @"\d*\.mp3", "")}?");

            // Add commands and set their callbacks
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            messageDialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(this.CommandInvokedHandler)));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (nameTextBox.Text == "")
            {
                btnStartRecording.IsEnabled = false;
                btnPauseRecording.IsEnabled = false;
                btnRemoveRecording.IsEnabled = false;
                btnPlay.IsEnabled = false;
                btnEnterTag.IsEnabled = false;
            }
            else
            {
                btnStartRecording.IsEnabled = true;
                student.Title = nameTextBox.Text + student.RecId;
                audioRecorder.audioFileName = nameTextBox.Text + student.RecId + ".mp3";
 
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
            Debug.WriteLine("============");
            Debug.WriteLine("time passed from callback");
            Debug.WriteLine(timePassed);
            Debug.WriteLine(DateTime.Now);
            Debug.WriteLine(startedTime);
            Debug.WriteLine("============");


            timeText.Text = MakeDigitString((timeSinceLastStop - timePassed).Hours, 2) + ":"
                + MakeDigitString((timeSinceLastStop - timePassed).Minutes, 2) + ":"
                + MakeDigitString((timeSinceLastStop - timePassed).Seconds, 2) + ":"
                + MakeDigitString((timeSinceLastStop - timePassed).Milliseconds, 3);
        }
    }
}
