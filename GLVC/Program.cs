using Figgle;
using System.Reflection;

namespace GLVC
{
    class Program
    {
        static void Main()
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
                    string csvFilePath = Console.ReadLine()!.Trim();
                    if (csvFilePath == "")
                    {
                        Console.WriteLine("using default CSV file.");
                        Console.WriteLine();
                        csvFilePath = "objid.csv";
                    }
                    else if (csvFilePath == "/help")
                    {
                        CsvHelp();
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        continue;
                    }
                    else
                    {
                        if (!File.Exists(csvFilePath))
                        {
                            throw new FileNotFoundException();
                        }
                        Console.WriteLine("using custom CSV file.");
                        Console.WriteLine();
                    }

                    //gmd file path
                    Console.Write("Insert a .GMD file path: ");
                    string gmdPath = Console.ReadLine()!.Trim();
                    if (!File.Exists(gmdPath))
                    {
                        throw new FileNotFoundException();
                    }
                    Console.WriteLine($"using level {Path.GetFileNameWithoutExtension(gmdPath)}");
                    Console.WriteLine();

                    //version picker
                    PrintVersions();
                    Console.Write("Pick a version: ");
                    int version;
                    string versionString = Console.ReadLine()!;
                    try
                    {
                        if (Convert.ToInt32(versionString) < 1 || Convert.ToInt32(versionString) > 19)
                        {
                            throw new Exception("Invalid version.");
                        }
                        version = Convert.ToInt32(versionString);
                    }
                    catch
                    {
                        throw;
                    }

                    // print objects?
                    Console.Write("Print objects? (y/n): ");
                    bool printObjects = Console.ReadLine()!.Trim().ToLower() == "y" ? true : false;

                    ReadGMDFile gmd = new ReadGMDFile();
                    try
                    {
                        gmd.ReadFile(gmdPath, csvFilePath, printObjects, version);
                    }
                    catch
                    {
                        throw;
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                }

                // reset GJGameLevel
                GJGameLevel level = new GJGameLevel();
                level.LevelString = "";
                level.AudioTrack = 1;
                level.Objects.Clear();
                // garbage collector
                GC.Collect();

                Console.WriteLine("Press any key to restart the program...");
                Console.ReadKey();
            }
        }

        static void PrintVersions()
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
        static void PrintHeader()
        {
            //Figgle Text
            Console.Write(FiggleFonts.Standard.Render("GLVC"));
            //github link
            Console.WriteLine("github.com/lucastozo/GLVC");
            //load version
            Assembly assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine($"Version: {Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine();
        }
        static void CsvHelp()
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