using System.Collections.Generic;
using BuildEngine;
using HexPatch;
using MediatR;

namespace SicarioPatch.Core
{
    public class ModsRequest : IRequest<Dictionary<string, Mod>>
    {
        
    }
}
