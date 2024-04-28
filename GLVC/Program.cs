using System.Reflection;

namespace GLVC
{
    internal class Program
    {
        private static void Main()
        {
            const string csvFilePath = "objects.csv";

            while (true)
            {
                Console.Clear();
                PrintHeader();
                try
                {
                    string gmdPath;
                    bool possibleFile;
                    do
                    {
                        Console.Write("Insert a .GMD file path: ");
                        gmdPath = Console.ReadLine()!.Trim();
                        possibleFile = File.Exists(gmdPath) && Path.GetExtension(gmdPath) == ".gmd";
                        if (!possibleFile)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("File was not found or is not a .gmd file.");
                            Console.ResetColor();
                        }
                    } while(!possibleFile);
                    Console.WriteLine($"using level {Path.GetFileNameWithoutExtension(gmdPath)}");
                    Console.WriteLine();

                    //version picker
                    PrintVersions();
                    var possibleVersion = true;
                    var version = 0;
                    do
                    {
                        Console.Write("Pick a version: ");
                        version = Convert.ToInt32(Console.ReadLine());
                        if (version < 1 || version > 19)
                        {
                            Console.WriteLine("Invalid version.");
                            Console.WriteLine();
                            possibleVersion = false;
                        }
                    } while(!possibleVersion);

                    var gmd = new ReadGmdFile();
                    gmd.ReadFile(gmdPath, csvFilePath, version);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }

                // reset GJGameLevel
                var level = new GjGameLevel
                {
                    LevelString = "",
                    AudioTrack = 1
                };
                level.Objects.Clear();

                Console.WriteLine("Press any key to restart the program...");
                Console.ReadKey();
            }
        }

        private static void PrintVersions()
        {
            List<string> versions = new()
            {
                "1.0",
                "1.02",
                "1.1",
                "1.11",
                "1.2",
                "Lite 1.30",
                "1.3",
                "1.4",
                "1.51",
                "1.6",
                "1.7",
                "1.80",
                "1.93",
                "2.00",
                "2.111",
                "SZ 2017",
                "GDW 2019",
                "SZ 2022",
                "2.2"
            };

            Console.WriteLine("-----------------------");
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (var i = 0; i < versions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {versions[i]}");
            }
            Console.WriteLine("-----------------------");
        }

        private static void PrintHeader()
        {
            Console.WriteLine("GLVC");
            //github link
            Console.WriteLine("github.com/lucastozo/GLVC");
            //load version
            Assembly.GetExecutingAssembly();
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            version = new Version(version!.Major, version.Minor, version.Build);
            Console.WriteLine($"version: {version}");
            Console.WriteLine();
        }
    }
}