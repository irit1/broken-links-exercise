using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using trendeamon.Services.BrokenLinks;

namespace trendeamon.Services.BrokenLinksHostedService
{
    public class BrokenLinksHostedService : BackgroundService
    {
        IBrokenLinksService _brokenLinksService;

        public BrokenLinksHostedService(IBrokenLinksService brokenLinksService)
        {
            _brokenLinksService = brokenLinksService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Debug.WriteLine("*** BrokenLinksHostedService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                // get next request (blocking)
                var requestId = _brokenLinksService.GetNextRequestIdBlocking(stoppingToken);
                Debug.WriteLine($"*** BrokenLinksHostedService got request id {requestId}");
                // handle request
                await _brokenLinksService.HandleRequest(requestId);
            }

            Debug.WriteLine("*** BrokenLinksHostedService ended");
        }
    }
}
