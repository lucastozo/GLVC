using CsvHelper;
using System.Globalization;

namespace GLVC
{
    public class IsPossibleVersion
    {
        private Dictionary<string, string> csvData;

        public IsPossibleVersion(string csvFilePath, int version)
        {
            csvData = new Dictionary<string, string>();
            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                while (csv.Read())
                {
                    var objectId = csv.GetField<string>(0);
                    var isPresentInVersion = csv.GetField<string>(version);
                    csvData[objectId!] = isPresentInVersion!;
                }
            }
        }

        public (bool, string) CheckSingleObject(string obj, int version)
        {
            string[] parts = obj.Split(',');
            string objectId = "";
            string rotation = "";
            string positionX = "";
            string positionY = "";
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
                else if (parts[i] == "2")
                {
                    positionX = parts[i + 1];
                }
                else if (parts[i] == "3")
                {
                    positionY = parts[i + 1];
                }
            }
            if (csvData.ContainsKey(objectId) && csvData[objectId] == "no")
            {
                return (false, $"Error: Illegal object ID: {objectId}, Position X,Y: {positionX}, {positionY}");
            }
            if (!string.IsNullOrEmpty(rotation))
            {
                int rotationValue = Math.Abs(int.Parse(rotation));
                if (rotationValue != 90 && rotationValue != 180 && rotationValue != 270 && version < 12)
                {
                    return (false, $"Error: Object ID: {objectId} has custom rotation value, Position X,Y: {positionX}, {positionY}");
                }
            }
            return (true, "Success: Possible Level");
        }
    }
}
