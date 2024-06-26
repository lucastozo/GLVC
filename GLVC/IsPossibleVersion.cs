﻿using CsvHelper;
using System.Globalization;

namespace GLVC
{
    public class IsPossibleVersion
    {
        private readonly Dictionary<string, string> _csvData;

        public IsPossibleVersion(string csvFilePath, int version)
        {
            _csvData = new Dictionary<string, string>();
            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            while (csv.Read())
            {
                var objectId = csv.GetField<string>(0);
                var isPresentInVersion = csv.GetField<string>(version);
                _csvData[objectId!] = isPresentInVersion!;
            }
        }

        public bool IsSongPossible(int version, int audioTrack, string key)
        {
            // key
            // k45 = newgrounds
            // k8 = default songs
            switch (key)
            {
                case "k45":
                    if (version < 13)
                    {
                        return false;
                    }
                    return true;
                case "k8":
                    int[] maxAudiosPorVersao = { 0, 7, 7, 8, 8, 9, 7, 10, 11, 12, 14, 15, 16, 18, 20, 21, 2, 9, 2, 22 };
                    if (audioTrack > maxAudiosPorVersao[version])
                    {
                        return false;
                    }
                    return true;
            }
            return true;
        }

        public (bool, string) CheckSingleObject(string obj, int version)
        {
            var parts = obj.Split(',');
            var objectId = "";
            var positionX = "";
            var positionY = "";
            var rotation = "";
            var scale = "";
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
            if (_csvData.TryGetValue(objectId, out var value) && value == "no")
            {
                return (false, $"Error: Illegal object ID: {objectId}, Position X,Y: {positionX}, {positionY}");
            }
            if (!string.IsNullOrEmpty(rotation))
            {
                var rotationValue = Math.Abs(int.Parse(rotation));
                if (rotationValue != 90 && rotationValue != 180 && rotationValue != 270 && version < 12)
                {
                    return (false,
                        $"Error: Object ID: {objectId} has custom rotation value, Position X,Y: {positionX}, {positionY}");
                }
            }
            if (!string.IsNullOrEmpty(scale))
            {
                if (version < 14)
                {
                    return (false, $"Error: Object ID: {objectId} has custom scale value, Position X,Y: {positionX}, {positionY}");
                }
            }
            return (true, string.Empty);
        }
    }
}