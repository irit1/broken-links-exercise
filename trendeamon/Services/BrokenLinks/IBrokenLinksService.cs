using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using trendeamon.Models;

namespace trendeamon.Services.BrokenLinks
{
    public interface IBrokenLinksService
    {
        string Start(BrokenLinksStartRequestModel request);

        string GetStatus(string requestId);

        string GetNextRequestIdBlocking(CancellationToken cancellationToken);

        Task HandleRequest(string requestId);
    }
}
