using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ModEngine.Merge;
using SicarioPatch.Core;

namespace SicarioPatch.Loader
{
    public class MergeComponentRequestHandler : IRequestHandler<MergeComponentRequest, IEnumerable<MergeComponent<WingmanMod>>>
    {
        private readonly IEnumerable<IMergeProvider<WingmanMod>> _providers;

        public MergeComponentRequestHandler(IEnumerable<IMergeProvider<WingmanMod>> providers) {
            _providers = providers;
        }
        public async Task<IEnumerable<MergeComponent<WingmanMod>>> Handle(MergeComponentRequest request, CancellationToken cancellationToken) {
            // this really doesn't need to be as DI and interface-reliant as it is
            // we could just instantiate the handful of required sources and implement them here
            // for now, this was a lot of work and I'm sticking to it
            // also consumers can just use this and it doesn't matter if I go back to the old way
            var results = _providers.SelectMany(p => p.GetMergeComponents(request.SearchPaths));
            return results;
        }
    }
}