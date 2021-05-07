using System;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VirtualBank.Api.Exceptions;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Data.ActionResults;

namespace VirtualBank.Api.ActionResults
{
    public interface IActionResultMapper<TController>
    {
        IActionResult Map(Exception exception);
    }

    public class ActionResultMapper<TController> : IActionResultMapper<TController>
    {
        private readonly IActionResultProvider _actionResultProvider;
        private readonly ILogger<TController> _logger;

        public ActionResultMapper(IActionResultProvider actionResultProvider, ILogger<TController> logger)
        {
            _actionResultProvider = Throw.ArgumentNullException.IfNull(actionResultProvider, nameof(actionResultProvider));
            _logger = Throw.ArgumentNullException.IfNull(logger, nameof(logger));
        }


        public IActionResult Map(Exception exception)
        {
            Throw.ArgumentNullException.IfNull(exception, nameof(exception));

            switch (exception)
            {
                case UnprocessableEntityException _:
                    _logger.LogError(exception, exception.Message);
                    return _actionResultProvider.GetUnprocessableEntityErrorResponse(exception.Message);

                case NotModifiedException _:
                    _logger.LogWarning(exception, exception.Message);
                    return _actionResultProvider.GetNotModifiedResponse();

                case ConflictException _:
                    _logger.LogWarning(exception, exception.Message);
                    return _actionResultProvider.GetConflictErrorResponse(exception.Message);

                case BadRequestException badRequestException:
                    _logger.LogError(exception, exception.Message);
                    return _actionResultProvider.GetBadRequestErrorResponse(badRequestException.Message,
                        badRequestException.Target);

                case NotFoundException _:
                    _logger.LogError(exception, exception.Message);
                    return _actionResultProvider.GetNotfoundErrorResponse(exception.Message);


                default:
                    ExceptionDispatchInfo.Capture(exception).Throw();
                    break;
            }

            throw new InvalidOperationException();
        }
    }
}
