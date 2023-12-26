namespace GLVC
{
    public class PrintObjects
    {
        const int tableWidth = 73;
        public void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        public void PrintHeader(string header, params string[] columns)
        {
            PrintLine();
            PrintRow(header);
            PrintRow(columns);
        }

        public void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
            PrintLine();
        }

        public string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }
    }
}
