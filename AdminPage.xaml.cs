using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OralHistoryRecorder.Models;
using System.Threading.Tasks;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OralHistoryRecorder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdminPage : Page
    {
        private ObservableCollection<StudentRecording> studentRecordingList = new ObservableCollection<StudentRecording>();
        public AdminPage()
        {
            this.InitializeComponent();

            InitializeAsync();
        }

        private void AddRange(ObservableCollection<StudentRecording> collection, List<StudentRecording> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        private async Task InitializeAsync()
        {
            var tempList = await RecordingManager.retrieveRecordings();
            AddRange(studentRecordingList, tempList);
        }
    }
}
