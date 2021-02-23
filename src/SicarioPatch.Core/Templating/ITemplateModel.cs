using System.Collections.Generic;

namespace SicarioPatch.Core.Templating
{
    public interface ITemplateModel
    {
        string Name { get; }
        Dictionary<string, string> GetModel();
    }
}