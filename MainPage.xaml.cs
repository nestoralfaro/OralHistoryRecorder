using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OralHistoryRecorder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AudioRecorderLib _audioRecorder;
        public MainPage()
        {
            this.InitializeComponent();
            this._audioRecorder = new AudioRecorderLib();
        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            this._audioRecorder.Record();
        }

        //private void btnPlay_Click(object sender, RoutedEventArgs e)
        //{
        //    this._audioRecorder.Play();
        //}

        private async void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            await this._audioRecorder.PlayFromDisk(Dispatcher);
        }

        private void btnStopRecording_Click(object sender, RoutedEventArgs e)
        {
            this._audioRecorder.StopRecording();
        }
    }
}
