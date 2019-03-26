using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using trendeamon.Models;
using trendeamon.Services.BrokenLinksDataService;

namespace trendeamon.Services.BrokenLinks
{
    public class BrokenLinksService : IBrokenLinksService
    {
        private readonly IBrokenLinksDataService _brokenLinksDataService;
        private readonly HttpClient _httpClient;

        public BrokenLinksService(IBrokenLinksDataService brokenLinksDataService)
        {
            _brokenLinksDataService = brokenLinksDataService;
            _httpClient = new HttpClient();
        }

        public string Start(BrokenLinksStartRequestModel request)
        {
            return _brokenLinksDataService.AddRequest(request);
        }

        public string GetStatus(string requestId)
        {
            var status = _brokenLinksDataService.GetRequestStatus(requestId);
            if (status == "done")
            {
                status += " " + _brokenLinksDataService.GetBrokenLinksCount(requestId);
            }
            return status;
        }

        public string GetNextRequestIdBlocking(CancellationToken cancellationToken)
        {
            return _brokenLinksDataService.GetNextRequestBlocking(cancellationToken);
        }

        public async Task HandleRequest(string requestId)
        {
            // get request data
            var data = _brokenLinksDataService.GetRequestData(requestId);

            // get count
            _brokenLinksDataService.SetRequestStatus(requestId, BrokenLinksStatus.Running);
            var count = await CountBrokenLinks(data.Url);

            // update request as done
            _brokenLinksDataService.SetBrokenLinksCount(requestId, count);
            _brokenLinksDataService.SetRequestStatus(requestId, BrokenLinksStatus.Done);
            Debug.WriteLine($"*** BrokenLinksService request id {requestId} updated with count {count}");
        }

        //////////////////////////////////////////////////////////////////////
        // private methods

        private async Task<int> CountBrokenLinks(string url)
        {
            var urlHost = GetUrlPrefix(url);
            Debug.WriteLine($"***  url: {url}");
            Debug.WriteLine($"***  url host: {urlHost}");

            var links = GetUrlLinks(url);
            var count = 0;
            var brokenLinksCount = 0;
            var total = links.Count();
            foreach (var link in links)
            {
                count++;
                var linkStatus = await GetLinkStatusCode(link);
                Debug.WriteLine($"***** url link {count}/{total}: {link}, status: {linkStatus}");
                if (linkStatus != HttpStatusCode.OK)
                    brokenLinksCount++;
            }
            Debug.WriteLine("***  url broken links count: {0}/{1}", brokenLinksCount, total);
            return brokenLinksCount;
        }

        private async Task<HttpStatusCode> GetLinkStatusCode(string link)
        {
            try
            {
                var response = await _httpClient.GetAsync(link);
                return response.StatusCode;
            }
            catch (Exception ex)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        private IEnumerable<string> GetUrlLinks(string url)
        {
            var urlPrefix = GetUrlPrefix(url);
            var urlSchema = GetUrlSchema(url);

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);

            var links = htmlDoc.DocumentNode.SelectNodes("//a");

            return links.Select(x => x.GetAttributeValue("href", ""))
                .Where(href => !string.IsNullOrWhiteSpace(href) && href[0] != '#')
                // if url starts with '//' add http: or https: (according to the sites protocol)
                .Select(x => x.StartsWith("//") ? urlSchema + ":" + x : x)
                // if url starts with '/' ir means it's a link within the url's domain - add the domain (and protocol)
                .Select(x => x.StartsWith("/") ? urlPrefix + x : x);
        }

        private string GetUrlPrefix(string url)
        {
            var uri = new Uri(url);
            return uri.Scheme + "://" + uri.Host;
        }

        private string GetUrlSchema(string url)
        {
            var uri = new Uri(url);
            return uri.Scheme;
        }

    }
}
