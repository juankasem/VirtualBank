
namespace VirtualBank.Core.ApiResponseModels
{
    public class ErrorResponse
    {
        public int Code { get; set; }

        public string FieldName { get; set; }

        public string Message { get; set; }


        public ErrorResponse()
        {

        }

        public ErrorResponse(string message)
        {
            Message = message;
        }

        public ErrorResponse(int code, string fieldName, string message)
        {
            Code = code;
            FieldName = fieldName;
            Message = message;
        }
    }
}
