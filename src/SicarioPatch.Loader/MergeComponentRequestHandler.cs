using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace SicarioPatch.Loader
{
    public class MergeComponentRequestHandler : IRequestHandler<MergeComponentRequest, IEnumerable<MergeComponent>>
    {
        private readonly IEnumerable<IMergeProvider> _providers;

        public MergeComponentRequestHandler(IEnumerable<IMergeProvider> providers) {
            _providers = providers;
        }
        public async Task<IEnumerable<MergeComponent>> Handle(MergeComponentRequest request, CancellationToken cancellationToken) {
            // this really doesn't need to be as DI and interface-reliant as it is
            // we could just instantiate the handful of required sources and implement them here
            // for now, this was a lot of work and I'm sticking to it
            // also consumers can just use this and it doesn't matter if I go back to the old way
            var results = _providers.SelectMany(p => p.GetMergeComponents(request.SearchPaths));
            return results;
        }
    }
}