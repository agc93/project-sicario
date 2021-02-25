using System.Collections.Generic;

namespace SicarioPatch.Templating
{
    public interface ITemplateFilterProvider
    {
        IEnumerable<ITemplateFilter> LoadFilters();
    }
}