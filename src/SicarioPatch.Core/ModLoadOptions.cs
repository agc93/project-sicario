using System.Collections.Generic;

namespace SicarioPatch.Core
{
    public class ModLoadOptions
    {
        public List<string> Sources { get; set; } = new List<string>();
        public string Filter { get; set; } = "*.dtm";
    }
}