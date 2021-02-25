using System.Collections.Generic;

namespace SicarioPatch.Templating
{
    public interface ITemplateModel
    {
        string Name { get; }
        Dictionary<string, string> GetModel();
    }
}