using System;
using System.Collections.Generic;
using System.Linq;

namespace SicarioPatch.Components
{
    public static class ComponentExtensions
    {
        internal static void Move<T>(this List<T> list, T item, int newIndex)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex > -1 && newIndex > -1 && newIndex < list.Count)
                {
                    list.RemoveAt(oldIndex);

                    if (newIndex > list.Count) newIndex = list.Count;
                    // the actual index could have shifted due to the removal

                    list.Insert(newIndex, item);
                }
            }

        }
        
        public static string ToolName(this IBrandProvider brand, NameFormat format = NameFormat.Normal) {
            var toolName = $"{brand.ProjectName} {brand.ToolName}";
            var acronym = string.Join(string.Empty,
                toolName.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(s => s[0])
            );
            return format switch {
                NameFormat.Normal => toolName,
                NameFormat.Short => acronym,
                NameFormat.Long => $"{toolName} ({acronym})",
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        public static string SiteName(this IBrandProvider brand, NameFormat format = NameFormat.Normal) {
            var toolName = $"{brand.ProjectName} {brand.AppName}";
            var acronym = string.Join(string.Empty,
                toolName.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(s => s[0])
            );
            return format switch {
                NameFormat.Normal => toolName,
                NameFormat.Short => acronym,
                NameFormat.Long => $"{toolName} ({acronym})",
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }
    }
    
    public enum NameFormat
    {
        Normal,
        Short,
        Long
    }
}