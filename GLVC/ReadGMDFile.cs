using System.IO.Compression;
using System.Text;
using System.Xml;

namespace GLVC
{
    public class ReadGMDFile : GJGameLevel
    {
        public void ReadFile(string filePath, string csvFilePath, bool printObjects, int version)
        {
            try
            {
                // load xml
                string xmlData = File.ReadAllText(filePath);
                xmlData = CleanInvalidXmlChars(xmlData);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlData);

                // default song
                AudioTrack = 1;

                // check k45 and k8
                foreach (XmlNode kNode in xmlDoc.SelectNodes("//k")!)
                {
                    string key = kNode.InnerText;

                    if (key == "k45")
                    {
                        int songID = int.Parse(kNode.NextSibling!.InnerText.Trim());
                        if (version < 13)
                        {
                            // k45 key is custom song
                            throw new Exception($"Error: Illegal custom song ID: {songID}");
                        }
                    }
                    else if (key == "k8")
                    {
                        AudioTrack = int.Parse(kNode.NextSibling!.InnerText.Trim()) + 1;
                        // Check song
                        // array, index = game version, value = max song ID
                        int[] maxAudiosPorVersao = { 0, 6, 6, 7, 7, 8, 2, 9, 10, 12, 13, 14, 15, 17, 19, 20, 2, 9, 2, 21 };
                        if (AudioTrack > maxAudiosPorVersao[version])
                        {
                            throw new Exception($"Error: Illegal song ID: {AudioTrack}");
                        }
                    }
                }

                // create IsPossibleVersion object
                IsPossibleVersion isPossibleVersion = new IsPossibleVersion(csvFilePath, version);

                // k4 key = objects
                foreach (XmlNode kNode in xmlDoc.SelectNodes("//k")!)
                {
                    string key = kNode.InnerText;
                    string value = kNode.NextSibling!.InnerText.Trim();

                    if (key == "k4")
                    {
                        LevelString = value;

                        string safe = value.Replace('-', '+').Replace('_', '/');
                        // Decode the base64 string
                        byte[] compressedBytes = Convert.FromBase64String(safe);

                        // Decompress the bytes
                        using (GZipStream stream = new GZipStream(new MemoryStream(compressedBytes), CompressionMode.Decompress))
                        {
                            const int size = 4096;
                            byte[] buffer = new byte[size];
                            using (MemoryStream memory = new MemoryStream())
                            {
                                int count = 0;
                                do
                                {
                                    count = stream.Read(buffer, 0, size);
                                    if (count > 0)
                                    {
                                        memory.Write(buffer, 0, count);
                                    }
                                }
                                while (count > 0);
                                string decodedString = Encoding.UTF8.GetString(memory.ToArray());

                                // Split the decoded string into objects
                                string[] objects = decodedString.Split(';');

                                // check single object
                                foreach (string obj in objects)
                                {
                                    var possible = isPossibleVersion.CheckSingleObject(obj, version);
                                    if (possible.Item1) // item 1 is bool
                                    {
                                        Objects.Add(obj);
                                    }
                                    else
                                    {
                                        throw new Exception(possible.Item2); // item 2 is string
                                    }
                                }

                                // print the objects
                                if (printObjects)
                                {
                                    Console.Clear();
                                    PrintObjects print = new PrintObjects();
                                    print.PrintHeader(Path.GetFileName(filePath), "Index", "ObjectID", "Rotation");
                                    int index = 1;
                                    foreach (string obj in Objects)
                                    {
                                        string[] parts = obj.Split(',');
                                        string objectId = "";
                                        string rotation = "";
                                        for (int i = 0; i < parts.Length; i += 2)
                                        {
                                            if (parts[i] == "1")
                                            {
                                                objectId = parts[i + 1];
                                            }
                                            else if (parts[i] == "6")
                                            {
                                                rotation = parts[i + 1];
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(objectId))
                                        {
                                            print.PrintRow(index.ToString(), objectId, rotation);
                                            index++;
                                        }
                                        // Reset objectId and rotation for the next object
                                        objectId = "";
                                        rotation = "";
                                    }
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
            catch
            {
                throw;
            }
        }

        public static string CleanInvalidXmlChars(string text)
        {
            var validXmlChars = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            return new string(validXmlChars);
        }
    }
}