using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fluid;
using MediatR;
using SicarioPatch.Core;

namespace SicarioPatch.Templating
{
    public class PatchTemplateBehaviour : IPipelineBehavior<PatchRequest, FileInfo>
    {
        private readonly FluidParser _parser;
        private IEnumerable<ITemplateFilter> Filters { get; } = new List<ITemplateFilter>();
        private IEnumerable<ITemplateModel> Models { get; } = new List<ITemplateModel>();
        

        public PatchTemplateBehaviour()
        {
            _parser = new FluidParser().AddTags();
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
                var modelVars = RenderVariables(request, mod);
                var dict = mod.FilePatches.ToDictionary(k => k.Key, kvp =>
                {
                    var finalPatches = kvp.Value.Where(psList =>
                    {
                        // REMEMBER: this is to keep the step, so have to return false to skip it
                        if (mod.ModInfo.StepsEnabled.ContainsKey(psList.Name) && _parser.TryParse(mod.ModInfo.StepsEnabled[psList.Name], out var skipTemplate))
                        {
                            var rendered = skipTemplate.Render(GetInputContext(request, modelVars));
                            var result = !bool.TryParse(rendered, out var skip) || skip;
                            // var result = bool.TryParse(rendered, out var skip) || skip;
                            // do NOT invert result: result *is* inverted
                            return result;
                        }
                        return true;
                    }).Select(psList =>
                    {
                        psList.Patches = psList.Patches.Select(p =>
                        {
                            if (_parser.TryParse(p.Substitution, out var subTemplate))
                            {
                                p.Substitution = subTemplate.Render(GetInputContext(request, modelVars));
                            }
                            if (_parser.TryParse(p.Template, out var template))
                            {
                                p.Template = template.Render(GetInputContext(request, modelVars));
                            }

                            if (p.Window != null)
                            {
                                p.Window.After = SafeRender(request, p.Window.After, modelVars);
                                p.Window.Before = SafeRender(request, p.Window.Before, modelVars);
                            }
                            return p;
                        }).ToList();
                        return psList;
                    }).ToList();
                    return finalPatches;
                });
                mod.FilePatches = dict.Where(kvp => kvp.Value.Any()).ToDictionary(k => k.Key, v => v.Value);
            }
            return await next();
        }

        private string SafeRender(PatchRequest request, string inputKey, Dictionary<string, string> modelVars = null)
        {
            modelVars ??= new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(inputKey) && _parser.TryParse(inputKey, out var template))
            {
                return template.Render(GetInputContext(request, modelVars));
            }
            else
            {
                return inputKey;
            }
        }

        private TemplateContext GetContext(object templateInputs, Dictionary<string, string> additionalVars = null)
        {
            var templCtx = new TemplateContext(templateInputs).AddFilters();
            foreach (var templateModel in Models)
            {
                templCtx.SetValue(templateModel.Name, templateModel.GetModel());
            }
            if (additionalVars != null)
            {
                templCtx.SetValue("vars", additionalVars);
            }
            foreach (var filter in Filters)
            {
                templCtx.Filters.AddFilter(filter.Name, filter.RunFilter);
            }
            return templCtx;
        }

        private Dictionary<string, string> RenderVariables(PatchRequest request, WingmanMod mod)
        {
            var validVars = new Dictionary<string, string>();
            if (mod.Variables != null)
            {
                foreach (var (varName, varTemplate) in mod.Variables)
                {
                    if (_parser.TryParse(varTemplate, out var subTemplate))
                    {
                        validVars.Add(varName, subTemplate.Render(GetInputContext(request, validVars)));
                    }
                }
            }
            return validVars;
        }

        private TemplateContext GetInputContext(PatchRequest request, Dictionary<string, string> additionalVars = null)
        {
            return GetContext(new {inputs = request.TemplateInputs}, additionalVars);
        }
    }
}