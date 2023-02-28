using RTP.SQLWriter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTP.BioToSQL
{
    public class EmotionDataProvider
    {
        [TableName("SimSessionBioEmotions")]
        public class EmotionsSql
        {
            Root _root;
            public EmotionsSql(Root root)
            {
                _root = root;
            }
            [ColumnName("SessionID")]
            public long SessionID { get; set; }

            [ColumnName("TIME")]
            public double TIME { get; set; }

            [ColumnName("PHASE")]
            public int PHASE { get; set; }

            [ColumnName("facetracker_status")]
            public double facetracker_status { get => _root.status =="ok" ? 1.0d : 0.0d; }

            [ColumnName("left_eye_closed")]
            public double left_eye_closed { get => _root.rects.Count > 0 ? (_root.rects[0].left_eye_closed == "false" ? 1.0d : 0.0d) : -1.0d; }

            [ColumnName("right_eye_closed")]
            public double right_eye_closed { get => _root.rects.Count > 0 ? (_root.rects[0].right_eye_closed == "false" ? 1.0d : 0.0d) : -1.0d; }

            [ColumnName("major_emotion")]
            public double major_emotion { get => _root.rects.Count > 0 ? Emotions.GetEmotionNum(_root.rects[0].major_emotion) : -1.0d; }

            [ColumnName("angry")]
            public double angry { get => _root.rects.Count>0 ? ((double)_root.rects[0].emotions["angry"]) : 0.0d; }

            [ColumnName("disgust")]
            public double disgust { get => _root.rects.Count > 0 ? ((double)_root.rects[0].emotions["disgust"]) : 0.0d; }

            [ColumnName("fear")]
            public double fear { get => _root.rects.Count > 0 ? ((double)_root.rects[0].emotions["fear"]) : 0.0d; }

            [ColumnName("happy")]
            public double happy { get => _root.rects.Count > 0 ? ((double)_root.rects[0].emotions["happy"]) : 0.0d; }

            [ColumnName("sad")]
            public double sad { get => _root.rects.Count > 0 ? ((double)_root.rects[0].emotions["sad"]) : 0.0d; }

            [ColumnName("surprise")]
            public double surprise { get => _root.rects.Count > 0 ? ((double)_root.rects[0].emotions["surprise"]) : 0.0d; }

            [ColumnName("neutral")]
            public double neutral { get => _root.rects.Count > 0 ? ((double)_root.rects[0].emotions["neutral"]) : 0.0d; }

        }
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Emotions
        {
            public string angry { get; set; }
            public string disgust { get; set; }
            public string fear { get; set; }
            public string happy { get; set; }
            public string sad { get; set; }
            public string surprise { get; set; }
            public string neutral { get; set; }

            public static double GetEmotionNum(string emo)
            {
                if (emo == "angry")
                    return 0.0d;
                else if (emo == "disgust")
                    return 1.0d;
                else if (emo == "fear")
                    return 2.0d;
                else if (emo == "happy")
                    return 3.0d;
                else if (emo == "sad")
                    return 4.0d;
                else if (emo == "surprise")
                    return 5.0d;
                else if (emo == "neutral")
                    return 6.0d;
                else
                    return -1.0d;
            }
        }

        public class Rect
        {
            public List<List<int>> rect { get; set; }
            public List<List<int>> shape { get; set; }
            public string left_eye_closed { get; set; }
            public string right_eye_closed { get; set; }
            public Dictionary<string, decimal> emotions { get; set; }
            public string major_emotion { get; set; }
        }

        public class Root
        {
            public string status { get; set; }
            public int faces { get; set; }
            public List<Rect> rects { get; set; }
        }


    }
}
