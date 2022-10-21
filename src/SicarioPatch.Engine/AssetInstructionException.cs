using System;
using System.Runtime.Serialization;

namespace SicarioPatch.Engine
{
    [Serializable]
    public class AssetInstructionException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public AssetInstructionException() {
        }

        internal AssetInstructionException(PatchRunContext context) : base(ExceptionMessage(context)) {
        }

        private static string ExceptionMessage(PatchRunContext context) {
            return $"Error executing instructions for {(context.PatchType?.Type ?? "unknown patch type")} in {(context.Loader?.Name ?? "unknown")} loader";
        }

        internal AssetInstructionException(PatchRunContext context, Exception inner) : base(ExceptionMessage(context), inner) {
        }

        protected AssetInstructionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {
        }
    }
}