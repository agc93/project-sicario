using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using SicarioPatch.Core.Templating;

namespace SicarioPatch.App
{
    public class ConfigModelProvider : ITemplateModelProvider
    {
        public ConfigModelProvider(IConfiguration configuration)
        {
            Section = configuration.GetSection("TemplateModels");
        }

        private IConfigurationSection Section { get; set; }

        public IEnumerable<ITemplateModel> LoadModels()
        {
            if (Section.Exists())
            {
                var keys = Section.GetChildren();
                foreach (var modelKey in keys)
                {
                    yield return new BasicTemplateModel()
                        {Name = modelKey.Key, Values = modelKey.Get<Dictionary<string, string>>()};
                }
            }
        }
    }
}