using Fluid;
using Fluid.Values;

namespace SicarioPatch.Templating
{
    public interface ITemplateFilter
    {
        string Name { get; }
        FluidValue RunFilter(FluidValue input, FilterArguments arguments, TemplateContext ctx);
    }
}