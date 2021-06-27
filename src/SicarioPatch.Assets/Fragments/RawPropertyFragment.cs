using System;
using System.Collections.Generic;
using UAssetAPI.PropertyTypes;

namespace SicarioPatch.Assets.Fragments
{
    public class RawPropertyFragment : IAssetParserFragment
    {
        public string? MatchPattern { get; init; }
        public PropertyData Match(PropertyData input) {
            if (string.IsNullOrWhiteSpace(MatchPattern)) {
                throw new InvalidOperationException("No match value provided for property parser!");
            }

            throw new InvalidOperationException("Raw property data!");
        }

        public IEnumerable<PropertyData> Match(IEnumerable<PropertyData> initialInput) {
            throw new NotImplementedException();
        }
    }
}