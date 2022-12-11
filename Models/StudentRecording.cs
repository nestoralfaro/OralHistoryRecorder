using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

// Model definition inspired by Collin Blake - https://www.youtube.com/watch?v=sCz_lqovLRg

namespace OralHistoryRecorder.Models
{
    public class StudentRecording
    {
        public int RecId { get; set; }
        public String Title { get; set; }
        public String tag { get; set; }

        public TimeSpan duration { get; set; }
        public string decade { get; set; }
        public StudentRecording()
        {
            tag = "";
            RecId = 0;
        }
    }


    public class RecordingManager
    {
        public async static Task<List<StudentRecording>> retrieveRecordings()
        {
            var stuRecordings =  new List<StudentRecording>();
            
            //Adding a new recording into the List
            stuRecordings.Add(new StudentRecording {
                RecId = 1, 
                Title = "Dorm Adventures", 
                tag = "Dorm Life", 
                duration = new TimeSpan(10),
                decade = "12/09/2022"});
            
            stuRecordings.Add(new StudentRecording
            {
                RecId = 1,
                Title = "Clubs are great!",
                tag = "Club Stuff",
                duration = new TimeSpan(7),
                decade = "10/09/2022"
            });

            var recFolder = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path);

            var files = await recFolder.GetFilesAsync();


            int id = 0;

            foreach (var file in files)
            {

                var dir = ApplicationData.Current.LocalFolder.Path;


                var tempFile = TagLib.File.Create(file.Path);

                stuRecordings.Add(new StudentRecording { RecId = id, 
                                                         Title = String.IsNullOrEmpty(tempFile.Tag.Title) ? "something" : tempFile.Tag.Title,
                                                         tag = String.IsNullOrEmpty(tempFile.Tag.Comment) ? "something" : tempFile.Tag.Comment,
                                                         duration = tempFile.Properties.Duration, 
                                                         decade = tempFile.Tag.Year.ToString() });

                id++;   
            }


            return stuRecordings;
        }
    }
}
