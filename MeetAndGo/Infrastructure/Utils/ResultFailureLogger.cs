using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MeetAndGo.Infrastructure.Utils
{
    public interface IResultFailureLogger
    {
        void LogFailureError(Result res, object obj);
    }

    public class ResultFailureLogger : IResultFailureLogger
    {
        private readonly ILogger<ResultFailureLogger> _logger;

        public ResultFailureLogger(ILogger<ResultFailureLogger> logger)
        {
            _logger = logger;
        }

        public void LogFailureError(Result res, object obj)
        {
            if (!res.IsFailure) return;

            _logger.LogWarning($"Failure. Reason: {res.Error}. Body: {JsonConvert.SerializeObject(obj)}");
        }
    }
}
