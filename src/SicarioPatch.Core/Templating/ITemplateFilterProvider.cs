using System.Collections.Generic;
using System.Threading.Tasks;

namespace SicarioPatch.Core.Templating
{
    public interface ITemplateFilterProvider
    {
        IEnumerable<ITemplateFilter> LoadFilters();
    }
}