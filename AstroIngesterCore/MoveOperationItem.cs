using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroIngesterCore
{
    public class MoveOperationItem
    {
        public string SourcePath { get; }
        public string DestinationPath { get; }
        public string Name { get; }
        public int Year { get; }
        public int Month { get; }
        public int Day { get; }
        public string Type { get; }
        public string Comment { get; }

        public MoveOperationItem(string sourcePath, string destinationPath, string name, int year, int month, int day, string type, string comment)
        {
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            Name = name;
            Year = year;
            Month = month;
            Day = day;
            Type = type;
            Comment = comment;
        }
    }
}
