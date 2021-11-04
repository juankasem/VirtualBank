
using VirtualBank.Core.Domain.Models;

namespace VirtualBank.Api.Mappers.Response
{
    public interface IUtilityPaymentsMapper
    {
        Core.Models.Responses.UtilityPayment MapToResponseModel(UtilityPayment utilityPayment);

    }
}