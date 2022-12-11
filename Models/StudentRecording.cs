using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

// Model definition inspired by Collin Blake - https://www.youtube.com/watch?v=sCz_lqovLRg

namespace OralHistoryRecorder.Models
{
    public class StudentRecording
    {
        public int RecId { get; set; }
        public String Title { get; set; }
        public String tag { get; set; }

        public float duration { get; set; }
        public string decade { get; set; }
        public StudentRecording()
        {
            tag = "";
            RecId = 0;
        }
    }


    public class RecordingManager
    {
        public static List<StudentRecording> retrieveRecordings()
        {
            var stuRecordings =  new List<StudentRecording>();
            
            //Adding a new recording into the List
            stuRecordings.Add(new StudentRecording {
                RecId = 1, 
                Title = "Dorm Adventures", 
                tag = "Dorm Life", 
                duration = 4.56f,
                decade = "12/09/2022"});
            
            stuRecordings.Add(new StudentRecording
            {
                RecId = 1,
                Title = "Clubs are great!",
                tag = "Club Stuff",
                duration = 7.34f,
                decade = "10/09/2022"
            });

            return stuRecordings;
        }
    }
}
