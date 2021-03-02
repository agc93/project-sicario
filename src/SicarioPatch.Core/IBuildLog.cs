using System;
using System.Collections.Generic;

namespace SicarioPatch.Core
{
    public interface IBuildLog
    {
        void SaveRequest(PatchRequestSummary summary);
    }

    public class PatchRequestSummary
    {
        public PatchRequestSummary(string id)
        {
            Id = id;
            BuildTime = DateTime.UtcNow;
        }
        public string Id { get; }

        public DateTime BuildTime { get;}
        public List<string> IncludedPatches { get; init; } = new List<string>();
        public Dictionary<string, string> Inputs { get; init; } = new Dictionary<string, string>();
        public string FileName { get; set; }
        public string UserName { get; set; }
    }
}