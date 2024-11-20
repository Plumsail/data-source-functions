using Microsoft.Graph;
using Microsoft.Graph.Sites.Item.Lists.Item;
using System;
using System.Linq;

namespace Plumsail.DataSource.SharePoint
{
    internal static class GraphServiceClientExtensions
    {
        internal static async System.Threading.Tasks.Task<ListItemRequestBuilder> GetListAsync(this GraphServiceClient graph, string siteUrl, string listName)
        {
            var url = new Uri(siteUrl);
            var site = await graph.Sites[$"{url.Host}:{url.AbsolutePath}"].GetAsync((requestConfiguration) =>
            {
                requestConfiguration.QueryParameters.Select = ["id"];
                requestConfiguration.QueryParameters.Expand = ["lists($select=id,name)"];
            });

            var list = site.Lists.FirstOrDefault(list => list.Name == listName);
            return list != null
                ? graph.Sites[site.Id].Lists[list.Id]
                : null;
        }
    }
}
