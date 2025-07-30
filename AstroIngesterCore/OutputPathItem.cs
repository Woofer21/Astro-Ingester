using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstroIngesterCore
{
    public class OutputPathItem
    {
        public string Path { get; set; }
        public List<string> Extentions { get; set; } = [];
        public List<string> Comments { get; set; } = [];

        public OutputPathItem(string path)
        {
            Path = path;
        }

        public OutputPathItem(string path, List<string> extentions, List<string> comments)
        {
            Path = path;
            Extentions = extentions;
            Comments = comments;
        }
    }
}
