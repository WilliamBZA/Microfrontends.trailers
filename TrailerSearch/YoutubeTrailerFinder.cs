using HtmlAgilityPack;
using ITOps.Composition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TrailerSearch
{
    public class YoutubeTrailerFinder : IProvideData
    {
        public bool Matches(RouteData routeData, HttpRequest request)
        {
            var controller = ((string)routeData.Values["controller"]).ToLowerInvariant();
            var action = ((string)routeData.Values["action"]).ToLowerInvariant();

            return controller == "data" && request.Query.ContainsKey("moviedetails");
        }

        public async Task<string> GetTrailerUrl(string movieId)
        {
            var query = $"https://www.youtube.com/results?search_query={movieId}";

            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(query);

            var classes = doc.DocumentNode.Descendants().Where(e => e.Attributes.Contains("class") && e.Attributes.Contains("href") && e.Attributes["class"].Value.Contains("yt-uix-sessionlink") && e.Attributes["href"].Value.Contains("/watch?v="));

            var element = classes.FirstOrDefault();

            var href = element.GetAttributeValue("href", null);
            return href.Substring(href.IndexOf("v=") + 2);
        }

        public async Task PopulateData(dynamic viewModel, RouteData routeData, HttpRequest request)
        {
            viewModel.trailerUrl = await GetTrailerUrl(request.Query["moviedetails"]);
        }
    }
}
