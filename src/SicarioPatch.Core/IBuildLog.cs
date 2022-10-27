namespace SicarioPatch.Core
{
    public interface IBuildLog
    {
        void SaveRequest(PatchRequestSummary summary);
    }
}