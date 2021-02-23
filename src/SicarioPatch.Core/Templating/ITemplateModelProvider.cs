using System.Collections.Generic;

namespace SicarioPatch.Core.Templating
{
    public interface ITemplateModelProvider
    {
        IEnumerable<ITemplateModel> LoadModels();
    }
}