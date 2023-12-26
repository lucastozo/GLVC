using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GLVC
{
    public class ReturnGMDFile : GJGameLevel
    {
        //variables
        string levelName = "";
        string levelDescription = "";
        string levelString = "";
        string songKey = "";

        public void ReturnFile(string xmlData, string[] objects, string csvFilePath, int version, int AudioTrack, bool audioTrackIsNG)
        {
            FillLevel(xmlData, objects, csvFilePath, version, AudioTrack, audioTrackIsNG);
            DumpLevel(levelName, levelDescription, levelString, songKey, audioTrackIsNG);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Level {levelName} dumped successfully in 'levels' inside the program folder.");
            Console.ResetColor();
        }

        public void FillLevel(string xmlData, string[] objects, string csvFilePath, int version, int AudioTrack, bool audioTrackIsNG)
        {

            // load xml
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            // create IsPossibleVersion object
            IsPossibleVersion isPossibleVersion = new IsPossibleVersion(csvFilePath, version);

            // fill info
            foreach (XmlNode kNode in xmlDoc.SelectNodes("//k")!)
            {
                string key = kNode.InnerText;

                // k2 key = level name
                if (key == "k2")
                {
                    levelName = kNode.NextSibling!.InnerText.Trim();
                }
                // k3 key = level description
                else if (key == "k3")
                {
                    levelDescription = kNode.NextSibling!.InnerText.Trim();
                }
                else if (key == "k45")
                {
                    songKey = kNode.NextSibling!.InnerText.Trim();
                }
                else if (key == "k8")
                {
                    songKey = kNode.NextSibling!.InnerText.Trim();
                }
                else if (key == "k4")
                {
                    // fill objects
                    foreach (string obj in objects)
                    {
                        var possible = isPossibleVersion.CheckSingleObject(obj, version);
                        if (possible.Item1) // item 1 is bool
                        {
                            Objects.Add(obj);
                        }
                    }
                }
            }

            // Join all objects into a single string
            levelString = (string.Join(";", Objects));

            // Convert the string to bytes
            byte[] originalBytes = Encoding.UTF8.GetBytes(levelString);

            using (MemoryStream compressedStream = new MemoryStream())
            {
                // Compress the bytes
                using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
                {
                    gzipStream.Write(originalBytes, 0, originalBytes.Length);
                }

                // Convert the compressed bytes to base64
                byte[] compressedBytes = compressedStream.ToArray();
                levelString = Convert.ToBase64String(compressedBytes);

                // convert to url base64
                levelString = levelString.Replace('+', '-').Replace('/', '_');
            }
        }

        public void DumpLevel(string levelName, string levelDescription, string levelString, string songKey, bool audioTrackIsNG)
        {
            if(!Directory.Exists("levels"))
            {
                Directory.CreateDirectory("levels");
            }
            string filePath = $"levels/{levelName}.gmd";
            using StreamWriter sw = new StreamWriter(filePath);
            {
                sw.WriteLine("<d>");
                sw.WriteLine("<k>kCEK</k><i>4</i>");
                sw.WriteLine($"<k>k2</k><s>{levelName}</s>");
                sw.WriteLine($"<k>k3</k><s>{levelDescription}</s>");
                sw.WriteLine($"<k>k4</k><s>{levelString}</s>");
                if (songKey != "")
                {
                    if(audioTrackIsNG)
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
