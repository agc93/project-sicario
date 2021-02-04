using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Components;
using SicarioPatch.Core;

namespace SicarioPatch.App
{
    public static class CoreExtensions
    {
        public static IDictionary<string, string> FallbackToDefaults(this IDictionary<string, string> dict,
            IEnumerable<PatchParameter> parameters)
        {
            if (dict.Any() && parameters.Any())
            {
                return dict.Select(kv =>
                {
                    if (string.IsNullOrWhiteSpace(kv.Value))
                    {
                        return (parameters.FirstOrDefault(p => p.Id == kv.Key) is var matchingParam &&
                                matchingParam != null)
                            ? new KeyValuePair<string, string>(kv.Key, matchingParam.Default)
                            : kv;
                    }

                    return kv;
                }).ToDictionary(k => k.Key, v => v.Value);
            }
            return dict;
        }
    }
    public static class NavigationExtensions
        {
            public static NameValueCollection QueryString(this NavigationManager navigationManager)
            {
                return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
            }

            public static string QueryString(this NavigationManager navigationManager, string key)
            {
                return navigationManager.QueryString()[key];
            }
        }
}