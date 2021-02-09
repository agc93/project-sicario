using System.Collections.Generic;
using BuildEngine;
using HexPatch;
using MediatR;

namespace SicarioPatch.Core
{
    public class ModsRequest : IRequest<Dictionary<string, WingmanMod>>
    {
        public string UserName { get; set; }
        public bool IncludePrivate { get; set; }
        public bool OnlyOwnMods { get; set; }
    }
}
