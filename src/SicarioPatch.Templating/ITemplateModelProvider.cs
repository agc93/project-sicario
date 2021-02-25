using System.Collections.Generic;

namespace SicarioPatch.Templating
{
    public interface ITemplateModelProvider
    {
        IEnumerable<ITemplateModel> LoadModels();
    }
}