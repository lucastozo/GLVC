namespace GLVC
{
    public class PrintObjects
    {
        private const int TableWidth = 105;
        public static void PrintLine()
        {
            Console.WriteLine(new string('-', TableWidth));
        }

        public void PrintHeader(string header, params string[] columns)
        {
            PrintLine();
            PrintRow(header);
            PrintRow(columns);
        }

        public void PrintRow(params string[] columns)
        {
            var width = (TableWidth - columns.Length) / columns.Length;
            var row = "|";

            foreach (var column in columns)
            {
                row += AlignCenter(column, width) + "|";
            }

            Console.WriteLine(row);
            PrintLine();
        }

        public static string AlignCenter(string text, int width)
        {
            if (text.Length > width)
            {
                text = text[..(width - 3)] + "...";
            }

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }
    }
}
