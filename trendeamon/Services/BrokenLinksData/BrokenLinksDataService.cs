using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using trendeamon.Models;

namespace trendeamon.Services.BrokenLinksDataService
{
    public class BrokenLinksDataService : IBrokenLinksDataService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly BlockingCollection<string> _queue;
        private readonly object _lock = new object();

        public BrokenLinksDataService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _queue = new BlockingCollection<string>();
        }

        public string AddRequest(BrokenLinksStartRequestModel request)
        {
            var data = new BrokenLinksDataModel
            {
                Url = request.Url,
                Status = BrokenLinksStatus.Pending,
                BrokenLinksCount = 0
            };
            var requestId = Guid.NewGuid().ToString();
            _memoryCache.Set<BrokenLinksDataModel>(requestId, data, TimeSpan.FromDays(1));
            _queue.Add(requestId);
            return requestId;
        }

        public string GetNextRequestBlocking(CancellationToken cancellationToken)
        {
            return _queue.Take();
        }

        public BrokenLinksDataModel GetRequestData(string requestId)
        {
            return _memoryCache.Get<BrokenLinksDataModel>(requestId);
        }

        public string GetRequestStatus(string requestId)
        {
            lock (_lock)
            {
                var success = _memoryCache.TryGetValue<BrokenLinksDataModel>(requestId, out BrokenLinksDataModel data);
                return success ? data.Status.ToString().ToLower() : "not_found";
            }
        }

        public int GetBrokenLinksCount(string requestId)
        {
            lock (_lock)
            {
                var success = _memoryCache.TryGetValue<BrokenLinksDataModel>(requestId, out BrokenLinksDataModel data);
                return success ? data.BrokenLinksCount : 0;
            }
        }

        public void SetRequestStatus(string requestId, BrokenLinksStatus status)
        {
            lock (_lock)
            {
                var success = _memoryCache.TryGetValue<BrokenLinksDataModel>(requestId, out BrokenLinksDataModel data);
                if (success)
                    data.Status = status;
            }
        }

        public void SetBrokenLinksCount(string requestId, int count)
        {
            lock (_lock)
            {
                var success = _memoryCache.TryGetValue<BrokenLinksDataModel>(requestId, out BrokenLinksDataModel data);
                if (success)
                    data.BrokenLinksCount = count;
            }
        }
    }
}
