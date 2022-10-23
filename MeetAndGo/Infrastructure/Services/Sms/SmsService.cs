using System;
using System.Threading.Tasks;
using MeetAndGo.Infrastructure.Providers;
using MeetAndGo.Infrastructure.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace MeetAndGo.Infrastructure.Services.Sms
{
    public interface ISmsService
    {
        Task<Result> SendSmsAsync(SmsRequest sms);
    }

    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly IOptions<SmsSettings> _smsSettings;

        public SmsService(
            ILogger<SmsService> logger,
            IOptions<SmsSettings> smsSettings)
        {
            _logger = logger;
            _smsSettings = smsSettings;
        }

        public async Task<Result> SendSmsAsync(SmsRequest sms)
        {
            try
            {
                var auth = System.Text.Encoding.ASCII.GetBytes($"{_smsSettings.Value.AppKey}:{_smsSettings.Value.SecretKey}");
                var authToBase64 = Convert.ToBase64String(auth);

                var client = new RestClient("https://api.smslabs.net.pl/");

                var request = new RestRequest("apiSms/sendSms", Method.PUT);
                request.AddParameter("flash", "0");
                request.AddParameter("expiration", "0");
                request.AddParameter("phone_number", sms.PhoneNumber);
                request.AddParameter("sender_id", "meetgo");
                request.AddParameter("message", sms.Message);
                request.AddHeader("Authorization", "Basic " + authToBase64);

                var response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {
                    _logger.LogError($"APP_ERROR: Could not sent sms. Reason: {response.ErrorMessage}");
                    return Result.Fail("SMS_FAILURE");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"APP_ERROR: Could not sent sms. Reason: {ex.Message}");
                return Result.Fail("SMS_FAILURE");
            }

            return Result.Ok();
        }
    }
}