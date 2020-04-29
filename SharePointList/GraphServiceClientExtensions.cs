using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plumsail.DataSource.SharePointList
{
    internal static class GraphServiceClientExtensions
    {
        internal static async System.Threading.Tasks.Task<IListRequestBuilder> GetListAsync(this GraphServiceClient graph, string siteUrl, string listName)
        {
            var url = new Uri(siteUrl);
            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("select", "id"),
                new QueryOption("expand", "lists(select=id,name)")
            };
            var site = await graph.Sites.GetByPath(url.AbsolutePath, url.Host)
                .Request(queryOptions)
                .GetAsync();

            var listsPage = site.Lists;
            var list = listsPage.FirstOrDefault(list => list.Name == listName);
            while (list == null && listsPage.NextPageRequest != null)
            {
                listsPage = await listsPage.NextPageRequest.GetAsync();
                list = listsPage.FirstOrDefault(list => list.Name == listName);
            }

            return list != null
                ? graph.Sites[site.Id].Lists[list.Id]
                : null;
        }
    }
}
