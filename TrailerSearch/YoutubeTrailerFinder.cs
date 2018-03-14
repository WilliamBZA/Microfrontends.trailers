using HtmlAgilityPack;
using ITOps.Composition;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
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

            var element = doc.GetElementbyId("video-title");
            var href = element.GetAttributeValue("href", null);

            var trailerUrl = $"https://www.youtube.com{href}";

            return trailerUrl;
        }

        public async Task PopulateData(dynamic viewModel, RouteData routeData, HttpRequest request)
        {
            viewModel.trailerUrl = await GetTrailerUrl(request.Query["moviedetails"]);
        }
    }
}
