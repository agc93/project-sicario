using System.Collections.Generic;
using System.IO;
using System.Linq;
using IniParser;
using Microsoft.Extensions.Configuration;
using SicarioPatch.Templating;

namespace SicarioPatch.App
{
    public class IniModelProvider : ITemplateModelProvider
    {
        public IniModelProvider(IConfiguration configuration)
        {
            FileName = configuration.GetValue<string>("TemplateModelsFile", string.Empty);
        }

        public string FileName { get; set; }

        public IEnumerable<ITemplateModel> LoadModels()
        {
            
            if (!string.IsNullOrWhiteSpace(FileName) && new FileInfo(FileName) is var fi && fi.Exists)
            {
                var parser = new FileIniDataParser();
                var data = parser.ReadFile(fi.FullName);
                foreach (var section in data.Sections)
                {
                    yield return new BasicTemplateModel()
                    {
                        Name = section.SectionName, 
                        Values = section.Keys.ToDictionary(k => k.KeyName, v => v.Value)
                    };
                }
            }
        }
    }
}