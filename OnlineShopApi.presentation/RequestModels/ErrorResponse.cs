using Newtonsoft.Json;

namespace OnlineShopApi.presentation.RequestModels
{
    public class ErrorResponse
    {
        public string Title { get; set; }
        public int? Status { get; set; }
        public string Detail { get; set; }
        public string TraceId { get; set; }
        public object Errors { get; set; }

        public ErrorResponse(int statusCode, string message = null, string errorId = null)
        {
            Title = GetDefaultTitleForStatusCode(statusCode);
            Status = statusCode;
            Detail = message ?? GetDefaultMessageForStatusCode(statusCode);
            TraceId = errorId;
        }

        private static string GetDefaultTitleForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return "Bad Request";
                case 404:
                    return "Not Found";
                case 500:
                    return "Internal Server Error";
                default:
                    return null;
            }
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    return "Bad Request";
                case 404:
                    return "Resource not found";
                case 500:
                    return "Something went wrong!";
                default:
                    return null;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
