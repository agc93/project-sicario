using System.Collections.Generic;

namespace SicarioPatch.App.Infrastructure
{
    public class AccessOptions
    {
        public List<string> AllowedUsers { get; set; } = new List<string>();

        public List<string> AllowedUploaders { get; set; } = new List<string>();
    }
}