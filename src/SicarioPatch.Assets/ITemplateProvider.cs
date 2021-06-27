using System.Collections.Generic;
using Parlot.Fluent;
using SicarioPatch.Assets.Fragments;

namespace SicarioPatch.Assets
{
    public interface ITemplateProvider
    {
        public IEnumerable<Parser<IAssetParserFragment>> GetParsers();
    }
}