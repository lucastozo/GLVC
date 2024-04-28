using System.IO.Compression;
using System.Text;
using System.Xml;

namespace GLVC
{
    public class ReturnGmdFile : GjGameLevel
    {
        //variables
        private string _levelName = "";
        private string _levelDescription = "";
        private string _levelString = "";
        private string _songKey = "";

        public void ReturnFile(string xmlData, string[] objects, string csvFilePath, int version, bool audioTrackIsNg)
        {
            FillLevel(xmlData, objects, csvFilePath, version);
            DumpLevel(_levelName, _levelDescription, _levelString, _songKey, audioTrackIsNg);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Level {_levelName} dumped successfully in 'levels' inside the program folder.");
            Console.ResetColor();
        }

        public void FillLevel(string xmlData, string[] objects, string csvFilePath, int version)
        {

            // load xml
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            // create IsPossibleVersion object
            var isPossibleVersion = new IsPossibleVersion(csvFilePath, version);

            // fill info
            foreach (XmlNode kNode in xmlDoc.SelectNodes("//k")!)
            {
                var key = kNode.InnerText;

                switch (key)
                {
                    // k2 key = level name
                    case "k2":
                        _levelName = kNode.NextSibling!.InnerText.Trim();
                        break;
                    // k3 key = level description
                    case "k3":
                        _levelDescription = kNode.NextSibling!.InnerText.Trim();
                        break;
                    case "k45":
                    case "k8":
                        _songKey = kNode.NextSibling!.InnerText.Trim();
                        break;
                    case "k4":
                    {
                        // fill objects
                        foreach (var obj in objects)
                        {
                            var possible = isPossibleVersion.CheckSingleObject(obj, version);
                            if (possible.Item1)
                            {
                                Objects.Add(obj);
                            }
                        }

                        break;
                    }
                }
            }

            // Join all objects into a single string
            _levelString = (string.Join(";", Objects));

            // Convert the string to bytes
            var originalBytes = Encoding.UTF8.GetBytes(_levelString);

            using var compressedStream = new MemoryStream();
            // Compress the bytes
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                gzipStream.Write(originalBytes, 0, originalBytes.Length);
            }

            // Convert the compressed bytes to base64
            var compressedBytes = compressedStream.ToArray();
            _levelString = Convert.ToBase64String(compressedBytes);

            // convert to url base64
            _levelString = _levelString.Replace('+', '-').Replace('/', '_');
        }

        public static void DumpLevel(string levelName, string levelDescription, string levelString, string songKey, bool audioTrackIsNg)
        {
            if(!Directory.Exists("levels"))
            {
                Directory.CreateDirectory("levels");
            }
            var filePath = $"levels/{levelName}.gmd";
            using var sw = new StreamWriter(filePath);
            {
                sw.WriteLine("<d>");
                sw.WriteLine("<k>kCEK</k><i>4</i>");
                sw.WriteLine($"<k>k2</k><s>{levelName}</s>");
                sw.WriteLine($"<k>k3</k><s>{levelDescription}</s>");
                sw.WriteLine($"<k>k4</k><s>{levelString}</s>");
                if (songKey != "")
                {
                    if(audioTrackIsNg)
                    {
                        sw.WriteLine($"<k>k45</k><i>{songKey}</i>");
                    }
                    else
                    {
                        sw.WriteLine($"<k>k8</k><i>{songKey}</i>");
                    }
                }
                sw.WriteLine("<k>k13</k><t/>");
                sw.WriteLine("<k>k21</k><i>2</i>");
                sw.WriteLine("<k>k50</k><i>24</i>");
                sw.WriteLine("</d>");
            }

        }
    }
}