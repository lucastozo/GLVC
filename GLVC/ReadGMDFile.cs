using System.IO.Compression;
using System.Text;
using System.Xml;

namespace GLVC
{
    public class ReadGmdFile : GjGameLevel
    {
        public void ReadFile(string filePath, string csvFilePath, int version)
        {

            string? xmlData;
            using (var reader = new StreamReader(filePath))
            {
                xmlData = reader.ReadToEnd();
            }
            xmlData = CleanInvalidXmlChars(xmlData);

            static string CleanInvalidXmlChars(string text)
            {
                var validXmlChars = text.Where(XmlConvert.IsXmlChar).ToArray();
                return new string(validXmlChars);
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            AudioTrack = 1;
            var audioTrackIsNg = false;

            // check k45 and k8
            foreach (XmlNode kNode in xmlDoc.SelectNodes("//k")!)
            {
                var key = kNode.InnerText;
                switch (key)
                {
                    case "k45":
                    {
                        AudioTrack = int.Parse(kNode.NextSibling!.InnerText.Trim());
                        if (version < 13)
                        {
                            // k45 key is custom song
                            throw new Exception($"Error: Illegal custom song ID: {AudioTrack}");
                        }
                        audioTrackIsNg = true;
                        break;
                    }
                    case "k8":
                    {
                        AudioTrack = int.Parse(kNode.NextSibling!.InnerText.Trim()) + 1;
                        // index = game version, value = max song ID
                        int[] maxAudiosPorVersao = { 0, 6, 6, 7, 7, 8, 2, 9, 10, 12, 13, 14, 15, 17, 19, 20, 2, 9, 2, 21 };
                        if (AudioTrack > maxAudiosPorVersao[version])
                        {
                            throw new Exception($"Error: Illegal song ID: {AudioTrack}");
                        }

                        break;
                    }
                }
            }

            // create IsPossibleVersion object
            var isPossibleVersion = new IsPossibleVersion(csvFilePath, version);

            // k4 key = objects
            foreach (XmlNode kNode in xmlDoc.SelectNodes("//k")!)
            {
                var key = kNode.InnerText;
                var value = kNode.NextSibling!.InnerText.Trim();

                if (key == "k4")
                {
                    LevelString = value;
                    var safe = value.Replace('-', '+').Replace('_', '/');
                    // Decode the base64 string
                    var compressedBytes = Convert.FromBase64String(safe);

                    // Decompress the bytes
                    using var stream = new GZipStream(new MemoryStream(compressedBytes), CompressionMode.Decompress);
                    const int size = 4096;
                    var buffer = new byte[size];
                    using var memory = new MemoryStream();
                    int count;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    var decodedString = Encoding.UTF8.GetString(memory.ToArray());

                    // Split the decoded string into objects
                    var objects = decodedString.Split(';');

                    // check single object
                    foreach (var obj in objects)
                    {
                        var possible = isPossibleVersion.CheckSingleObject(obj, version);
                        if (possible.Item1)
                        {
                            Objects.Add(obj);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(possible.Item2);
                            Console.ResetColor();
                            Console.WriteLine("Return .gmd file without illegal objects? (y/n): ");
                            var input = Console.ReadLine()!.Trim().ToLower() == "y";
                            if (input)
                            {
                                var returnGmd = new ReturnGmdFile();
                                returnGmd.ReturnFile(xmlData, objects, csvFilePath, version, audioTrackIsNg);
                                return;
                            }
                            Console.Clear();
                            throw new Exception(possible.Item2); // item 2 is string
                        }
                    }

                }
            }
            xmlData = null; // trying to free memory
            xmlDoc = null; // trying to free memory

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success: Possible Level");
            Console.ResetColor();
        }
    }
}