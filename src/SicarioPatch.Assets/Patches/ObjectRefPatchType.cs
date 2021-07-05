using System;
using System.Collections.Generic;
using System.Linq;
using Parlot.Fluent;
using SicarioPatch.Assets.TypeLoaders;
using UAssetAPI;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.Patches
{
    public class ObjectRefPatchType : AssetPatchType<ObjectRefPatchType.ObjectReference>
    {
        public record ObjectReference
        {
            public string TargetPropertyName { get; init; }
            public string Name { get; init; }
            public string Path { get; init; }
        }
        public override string Type => "objectRef";
        protected override IEnumerable<AssetInstruction>? RunPatch(IEnumerable<PropertyData> propData, ObjectReference parsedValue) {
            foreach (var objectProp in propData.Where(pd => pd is ObjectPropertyData).Cast<ObjectPropertyData>()) {
                AddObjectRef(parsedValue, objectProp);
            }

            foreach (var arrayPropertyData in propData.Where(pd => pd is ArrayPropertyData).Cast<ArrayPropertyData>().Where(apd => apd.ArrayType == "ObjectProperty")) {
                var inputMatch = arrayPropertyData.Value.ElementAtOrDefault(0);
                if (inputMatch != null) {
                    // inputMatch.Name = parsedValue.TargetName;
                    var arrayValues = arrayPropertyData.Value.ToList();
                    arrayValues.Add(inputMatch);
                    arrayPropertyData.Value = arrayValues.ToArray();
                }

                var modMatch = arrayPropertyData.Value.Cast<ObjectPropertyData>().LastOrDefault();
                if (modMatch != null) {
                    AddObjectRef(parsedValue, modMatch);
                }
            }

            return new List<AssetInstruction>();
        }

        private static void AddObjectRef(ObjectReference parsedValue, ObjectPropertyData objectProp) {
            var existingNameLinkIndex = objectProp.Asset.SearchForLink(objectProp.Value.Property);
            var existingNameLink = objectProp.Asset.GetLinkAt(existingNameLinkIndex);
            var existingPathLinkHeaderRef =
                objectProp.Asset.links[Math.Abs(objectProp.Value.Linkage) - 1].Property;
            var existingPathLinkIndex = objectProp.Asset.SearchForLink(existingPathLinkHeaderRef);
            var existingPathLink = objectProp.Asset.GetLinkAt(existingPathLinkIndex);

            var pathLinkRef = objectProp.Asset.AddHeaderReference(parsedValue.Path);
            var pathLinkIndex = objectProp.Asset.SearchForLink((ulong) pathLinkRef);
            var pathLink = pathLinkIndex == default
                ? new Link(existingPathLink.Base, existingPathLink.Class, 0, (ulong) pathLinkRef)
                : objectProp.Asset.GetLinkAt(pathLinkIndex);
            pathLinkIndex = objectProp.Asset.AddLink(pathLink);

            var nameHeaderRef = objectProp.Asset.AddHeaderReference(parsedValue.Name);
            var nameLinkIndex = objectProp.Asset.SearchForLink((ulong) nameHeaderRef);
            var nameLink = nameLinkIndex == default
                ? new Link(existingNameLink.Base, existingNameLink.Class, pathLinkIndex, (ulong) nameHeaderRef)
                : objectProp.Asset.GetLinkAt(nameLinkIndex);
            nameLinkIndex = objectProp.Asset.AddLink(nameLink);


            // objectProp.Asset.links.FirstOrDefault(l => l.Linkage == -)
            objectProp.Value = objectProp.Asset.GetLinkAt(nameLinkIndex);
            objectProp.LinkValue = nameLinkIndex;
        }

        protected override Parser<ObjectReference> ValueParser =>
            Parsers.Terms.String(StringLiteralQuotes.Single)
                .AndSkip(Parsers.Terms.Char(':')).And(Parsers.Terms.String(StringLiteralQuotes.Single)).Then(res =>
                    new ObjectReference {
                        Name = res.Item1.ToString(), Path = res.Item2.ToString()
                    });
    }
}