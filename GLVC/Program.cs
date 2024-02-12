using Figgle;
using System.Reflection;

namespace GLVC
{
    internal class Program
    {
        private static void Main()
        {
            // put the program inside a loop
            while (true)
            {
                Console.Clear();
                PrintHeader();

                try
                {
                    //csv file path
                    Console.WriteLine("leave blank to use csv inside the program folder");
                    Console.WriteLine("type '/help' for help");
                    Console.Write("Insert .CSV file path (");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("docs.google.com/spreadsheets/d/18R2_qBXa9iCZZ6bhfSpxEah3YLRGeBSk");
                    Console.ResetColor();
                    Console.Write("): ");
                    var csvFilePath = Console.ReadLine()!.Trim();
                    switch (csvFilePath)
                    {
                        case "":
                            Console.WriteLine("using default CSV file.");
                            Console.WriteLine();
                            csvFilePath = "objid.csv";
                            break;
                        case "/help":
                            CsvHelp();
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            continue;
                        default:
                        {
                            if (!File.Exists(csvFilePath))
                            {
                                throw new FileNotFoundException();
                            }
                            Console.WriteLine("using custom CSV file.");
                            Console.WriteLine();
                            break;
                        }
                    }

                    //gmd file path
                    var gmdPath = "";
                    var possibleFile = false;
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
                    string versionString;
                    do
                    {
                        Console.Write("Pick a version: ");
                        versionString = Console.ReadLine()!;
                        if (Convert.ToInt32(versionString) < 1 || Convert.ToInt32(versionString) > 19)
                        {
                            Console.WriteLine("Invalid version.");
                            Console.WriteLine();
                            possibleVersion = false;
                        }
                    } while(!possibleVersion);
                    var version = Convert.ToInt32(versionString);

                    // print objects?
                    Console.Write("Print objects? (y/n): ");
                    var printObjects = Console.ReadLine()!.Trim().ToLower() == "y";

                    var gmd = new ReadGmdFile();
                    gmd.ReadFile(gmdPath, csvFilePath, printObjects, version);
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
                // garbage collector
                GC.Collect();

                Console.WriteLine("Press any key to restart the program...");
                Console.ReadKey();
            }
        }

        private static void PrintVersions()
        {
            Console.WriteLine("-----------------------");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1 - 1.0");
            Console.WriteLine("2 - 1.02");
            Console.WriteLine("3 - 1.1");
            Console.WriteLine("4 - 1.11");
            Console.WriteLine("5 - 1.2");
            Console.WriteLine("6 - Lite 1.30");
            Console.WriteLine("7 - 1.3");
            Console.WriteLine("8 - 1.4");
            Console.WriteLine("9 - 1.51");
            Console.WriteLine("10 - 1.6");
            Console.WriteLine("11 - 1.7");
            Console.WriteLine("12 - 1.80");
            Console.WriteLine("13 - 1.93");
            Console.WriteLine("14 - 2.00");
            Console.WriteLine("15 - 2.111");
            Console.WriteLine("16 - SZ 2017");
            Console.WriteLine("17 - GDW 2019");
            Console.WriteLine("18 - SZ 2022");
            Console.WriteLine("19 - 2.2");
            Console.ResetColor();
            Console.WriteLine("-----------------------");
        }

        private static void PrintHeader()
        {
            //Figgle Text
            Console.Write(FiggleFonts.Standard.Render("GLVC"));
            //github link
            Console.WriteLine("github.com/lucastozo/GLVC");
            //load version
            Assembly.GetExecutingAssembly();
            Console.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine();
        }

        private static void CsvHelp()
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("------------------------------");
            Console.WriteLine("How to download .csv file (object id database):");
            Console.Write("1. go to: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("docs.google.com/spreadsheets/d/18R2_qBXa9iCZZ6bhfSpxEah3YLRGeBSk");
            Console.ResetColor();
            Console.WriteLine("2. download the spreadsheet as .csv");
            Console.Write("3. put the .csv file inside the program folder named as ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("objid.csv");
            Console.ResetColor();
            Console.WriteLine(" or, if you want to, locate it manually.");
        }
    }
}