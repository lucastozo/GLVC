using System.IO.Compression;
using System.Text;
using System.Xml;

namespace GLVC
{
    public class ReadGmdFile : GjGameLevel
    {
        public string? xmlData;
        public XmlDocument? xmlDoc;

        public void ReadFile(string filePath, string csvFilePath, int version)
        {
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

            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            var isPossibleVersion = new IsPossibleVersion(csvFilePath, version);

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
                        if(!isPossibleVersion.IsSongPossible(version, AudioTrack, key))
                        {
                            throw new Exception($"Error: Illegal custom song ID: {AudioTrack}");
                        }
                        audioTrackIsNg = true;
                        break;
                    }
                    case "k8":
                    {
                        AudioTrack = int.Parse(kNode.NextSibling!.InnerText.Trim());
                        if (!isPossibleVersion.IsSongPossible(version, AudioTrack, key))
                        {
                            throw new Exception($"Error: Illegal song ID: {AudioTrack}");
                        }
                        break;
                    }
                }
            }

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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success: Possible Level");
            Console.ResetColor();
        }
    }
}