using System.Collections.Generic;
using Parlot.Fluent;
using SicarioPatch.Engine.Fragments;

namespace SicarioPatch.Engine
{
    public interface ITemplateProvider
    {
        public IEnumerable<Parser<IAssetParserFragment>> GetParsers();
    }
}