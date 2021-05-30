using System;
namespace VirtualBank.Api.Helpers.ErrorsHelper
{
    public class MessageCreator
    {
        public static string AddNotFound(string fieldName, string message = null) =>
           !string.IsNullOrEmpty(message) ? $"{fieldName} {message}" : $"{fieldName} not found";


        public static string AddUnauthorized(string fieldName, string message) =>
           !string.IsNullOrEmpty(message) ? $"{fieldName} {message}" : $"{fieldName} unauthorized";


        public static string AddBadRequest(string fieldName, string message) =>
           !string.IsNullOrEmpty(message) ? $"{fieldName} {message}" : $"{fieldName} Bad Request";
          

       public static string AddUnprocessableEntity(string fieldName, string message) =>
           !string.IsNullOrEmpty(message) ? $"{fieldName} {message}" : $"{fieldName} Unprocessable Entity";

        public static string AddForbidden() => "Forbidden";

        public static string AddInternalServerError(string message) => !string.IsNullOrEmpty(message) ? message : "Internal Server Error";

       public MessageCreator()
        {
        }
    }
}
