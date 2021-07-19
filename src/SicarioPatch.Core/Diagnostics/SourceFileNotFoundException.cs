using System;
using System.Runtime.Serialization;

namespace SicarioPatch.Core.Diagnostics
{
    [Serializable]
    public class SourceFileNotFoundException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public SourceFileNotFoundException() {
        }

        public SourceFileNotFoundException(string filePath) : base($"Could not load {filePath} from any configured file sources.") {
        }

        public SourceFileNotFoundException(string filePath, Exception inner) : base($"Could not load {filePath} from any configured file sources.", inner) {
        }

        protected SourceFileNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {
        }
    }
}