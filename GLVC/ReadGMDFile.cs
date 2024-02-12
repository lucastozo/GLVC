using System.IO.Compression;
using System.Text;
using System.Xml;

namespace GLVC
{
    public class ReadGmdFile : GjGameLevel
    {
        public void ReadFile(string filePath, string csvFilePath, bool printObjects, int version)
        {
            // load xml
            var xmlData = File.ReadAllText(filePath);
            xmlData = CleanInvalidXmlChars(xmlData);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlData);

            // default song
            AudioTrack = 1;
            // audio track is NG?
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
                        // Check song
                        // array, index = game version, value = max song ID
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
                        if (possible.Item1) // item 1 is bool
                        {
                            Objects.Add(obj);
                        }
                        else
                        {
                            // throw new Exception(possible.Item2); // item 2 is string
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

                    // print the objects
                    if (printObjects)
                    {
                        Console.Clear();
                        var print = new PrintObjects();
                        print.PrintHeader(Path.GetFileName(filePath), "Index", "ObjectID", "Position", "Rotation", "Scale");
                        var index = 1;
                        foreach (var obj in Objects)
                        {
                            var parts = obj.Split(',');
                            var objectId = "";
                            var positionX = "0";
                            var positionY = "0";
                            var rotation = "0";
                            var scale = "1";
                            for (var i = 0; i < parts.Length; i += 2)
                            {
                                switch (parts[i])
                                {
                                    case "1":
                                        objectId = parts[i + 1];
                                        break;
                                    case "2":
                                        positionX = parts[i + 1];
                                        break;
                                    case "3":
                                        positionY = parts[i + 1];
                                        break;
                                    case "6":
                                        rotation = parts[i + 1];
                                        break;
                                    case "32":
                                        scale = parts[i + 1];
                                        break;
                                }
                            }
                            var position = positionX + "X, " + positionY + "Y";
                            if (!string.IsNullOrEmpty(objectId))
                            {
                                print.PrintRow(index.ToString(), objectId, position, rotation, scale);
                                index++;
                            }
                        }
                    }

                }
            }

            // success
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success: Possible Level");
            Console.ResetColor();
        }

        public static string CleanInvalidXmlChars(string text)
        {
            var validXmlChars = text.Where(XmlConvert.IsXmlChar).ToArray();
            return new string(validXmlChars);
        }
    }
}