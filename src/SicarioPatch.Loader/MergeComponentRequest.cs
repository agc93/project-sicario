using System.Collections.Generic;
using MediatR;

namespace SicarioPatch.Loader
{
    public class MergeComponentRequest : IRequest<IEnumerable<MergeComponent>>
    {
        public List<string>? SearchPaths { get; init; }
    }
}