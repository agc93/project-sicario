using System.Collections.Generic;
using MediatR;
using ModEngine.Merge;
using SicarioPatch.Core;

namespace SicarioPatch.Loader
{
    public class MergeComponentRequest : IRequest<IEnumerable<MergeComponent<WingmanMod>>>
    {
        public List<string>? SearchPaths { get; init; }
    }
}