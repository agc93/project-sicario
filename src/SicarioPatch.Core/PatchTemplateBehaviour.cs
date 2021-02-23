using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fluid;
using MediatR;
using SicarioPatch.Core.Templating;

namespace SicarioPatch.Core
{
    public class PatchTemplateBehaviour : IPipelineBehavior<PatchRequest, FileInfo>
    {
        private readonly FluidParser _parser;
        private IEnumerable<ITemplateFilter> Filters { get; } = new List<ITemplateFilter>();
        private IEnumerable<ITemplateModel> Models { get; } = new List<ITemplateModel>();
        

        public PatchTemplateBehaviour()
        {
            _parser = new FluidParser();
        }

        public PatchTemplateBehaviour(IEnumerable<ITemplateFilterProvider> templates, IEnumerable<ITemplateModelProvider> modelProviders) : this()
        {
            if (templates.Any())
            {
                Filters = templates.SelectMany(provider => provider.LoadFilters());
            }

            if (modelProviders.Any())
            {
                Models = modelProviders.SelectMany(provider => provider.LoadModels());
            }
        }

        public async Task<FileInfo> Handle(PatchRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<FileInfo> next)
        {
            request.TemplateInputs ??= new Dictionary<string, string>();
            //var templateObj = DictionaryToObject(request.TemplateInputs.ToDictionary(k => k.Key, v => (object)v.Value));
            
            foreach (var mod in request.Mods)
            {
                mod.FilePatches = mod.FilePatches.ToDictionary(k => k.Key, kvp =>
                {
                    return kvp.Value.Select(psList =>
                    {
                        var setList = psList.Patches.Select(p =>
                        {
                            if (_parser.TryParse(p.Substitution, out var subTemplate))
                            {
                                p.Substitution = subTemplate.Render(GetContext(new { inputs = request.TemplateInputs}));
                            }

                            if (_parser.TryParse(p.Template, out var template))
                            {
                                p.Template = template.Render(GetContext(new { inputs = request.TemplateInputs}));
                            }
                            return p;
                        }).ToList();
                        return psList;
                    }).ToList();
                });
            }
            return await next();
        }

        private TemplateContext GetContext(object templateInputs)
        {
            var templCtx = new TemplateContext(templateInputs).AddFilters();
            foreach (var templateModel in Models)
            {
                templCtx.SetValue(templateModel.Name, templateModel.GetModel());
            }
            foreach (var filter in Filters)
            {
                templCtx.Filters.AddFilter(filter.Name, filter.RunFilter);
            }
            return templCtx;
        }
    }
}