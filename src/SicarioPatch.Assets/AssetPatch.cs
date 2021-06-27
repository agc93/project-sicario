namespace SicarioPatch.Assets
{
    public class AssetPatch
    {
        public int Version { get; set; } = 1;
        public string Description {get;set;}
        public string Template {get;set;}
        public string Value {get;set;}
        public string Type {get;set;} = string.Empty;
    }
}