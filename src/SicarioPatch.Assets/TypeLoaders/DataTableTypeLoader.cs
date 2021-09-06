using System;
using System.Collections.Generic;
using System.Linq;
using UAssetAPI;
using UAssetAPI.PropertyTypes;
using UAssetAPI.StructTypes;

namespace SicarioPatch.Assets.TypeLoaders
{
    public class DataTableTypeLoader : IAssetTypeLoader
    {
        public string Name => "datatable";
        public IEnumerable<PropertyData> LoadData(AssetReader reader) {
            if (reader.categories[0] is not DataTableCategory dtCategory) {
                throw new InvalidOperationException("Could not load requested file as a datatable blueprint!");
            }
            var rows = dtCategory.Data2.Table;
            return rows.Select(r => r.Data);
        }

        public AssetWriter RunInstructions(AssetWriter writer, IEnumerable<AssetInstruction> instructions) {
            var additionalData = instructions.Where(i => i.Type == InstructionType.Add).ToList();

            foreach (var propertyPair in additionalData.SelectMany(ad => ad.Properties)) {
                if (propertyPair.Value is StructPropertyData structPropertyData) {
                    if (string.IsNullOrWhiteSpace(propertyPair.Key)) {
                        //just slap that shit in there
                        if (!writer.data.HeaderReferenceContains(structPropertyData.Name)) {
                            writer.data.AddHeaderReference(structPropertyData.Name);
                        }
                        writer.GetDataTableCategory().Data2.Table.Add(new DataTableEntry(structPropertyData, 0));
                    }
                    else {
                        if (!writer.data.HeaderReferenceContains(structPropertyData.Name)) {
                            writer.data.AddHeaderReference(structPropertyData.Name);
                        }

                        if (int.TryParse(propertyPair.Key, out var targetIndex)) {
                            if (writer.GetDataTable().All(dte => int.TryParse(dte.Data.Name, out _))) {
                                //we're going to have to do the whole indexing bullshittery
                                writer.GetDataTableCategory().Data2.Table.Insert(targetIndex, new DataTableEntry(structPropertyData, 0));
                                foreach (var tableEntry in writer.GetDataTableCategory().Data2.Table.Skip(targetIndex + 1)) {
                                    tableEntry.Data.Name = (int.Parse(tableEntry.Data.Name) + 1).ToString();
                                    if (!writer.data.HeaderReferenceContains(tableEntry.Data.Name)) {
                                        writer.data.AddHeaderReference(tableEntry.Data.Name);
                                    }
                                }
                            }
                            else {
                                //handling duplicate indexes properly
                                writer.GetDataTableCategory().Data2.Table.Add(new DataTableEntry(structPropertyData, targetIndex));
                            }
                        }
                        else {
                            if (!writer.data.HeaderReferenceContains(propertyPair.Key)) {
                                writer.data.AddHeaderReference(propertyPair.Key);
                            }
                            writer.GetDataTableCategory().Data2.Table.Add(new DataTableEntry(structPropertyData, 0));
                        }
                        /*var headerRef = writer.data.HeaderReferenceContains(structPropertyData.Name)
                            ? writer.data.SearchHeaderReference(structPropertyData.Name)
                            : writer.data.AddHeaderReference(structPropertyData.Name);*/
                        //named/fixed position
                    }
                }
            }

            var removalData = instructions.Where(i => i.Type == InstructionType.Remove);
            foreach (var removeInstruction in removalData) {
                foreach (var (removeName, removalProp) in removeInstruction.Properties) {
                    var match = writer.GetDataTable().FirstOrDefault(r => r.Data == removalProp);
                    if (match.Data == null) {
                        match = writer.GetDataTable().FirstOrDefault(r => r.Data.Name == removeName);
                    }
                    if (match.Data != null) {
                        writer.GetDataTable().Remove(writer.GetDataTable().FirstOrDefault());
                    }
                }
            }

            /*var directRuns = instructions.Where(i => i.Type == InstructionType.Direct);
            foreach (var instruction in directRuns) {
                instruction.WriterAction?.Invoke(writer);
            }*/

            return writer;
        }
    }
}