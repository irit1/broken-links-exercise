using System.Threading;
using trendeamon.Models;

namespace trendeamon.Services.BrokenLinksDataService
{
    public interface IBrokenLinksDataService
    {
        string AddRequest(BrokenLinksStartRequestModel request);
        string GetNextRequestBlocking(CancellationToken cancellationToken);
        BrokenLinksDataModel GetRequestData(string requestId);
        string GetRequestStatus(string requestId);
        int GetBrokenLinksCount(string requestId);
        void SetRequestStatus(string requestId, BrokenLinksStatus status);
        void SetBrokenLinksCount(string requestId, int count);
    }
}
