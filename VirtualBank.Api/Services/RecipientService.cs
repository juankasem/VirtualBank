using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VirtualBank.Core.ApiRequestModels.RecipientApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.RecipientApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api.Services
{
    public class RecipientService : IRecipientService
    {
        private readonly VirtualBankDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RecipientService(VirtualBankDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<RecipientListResponse>> GetBankAccountRecipientsAsync(int accountId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<RecipientListResponse>();

            var recipients = await _dbContext.Recipients.Where(r => r.AccountId == accountId).ToListAsync();

            var recipientList = new List<RecipientResponse>();

            foreach (var recipient in recipients)
            {
                recipientList.Add(CreateRecipientResponse(recipient));
            }

            responseModel.Data = new RecipientListResponse(recipientList.ToImmutableList(), recipientList.Count);

            return responseModel;
        }

        public Task<ApiResponse<RecipientResponse>> GetRecipientbyIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse> AddOrEditRecipientAsync(CreateRecipientRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        #region private helper methods
        private Recipient CreateRecipient(CreateRecipientRequest request)
        {
            if (request != null)
            {
                return new Recipient()
                {
                    AccountId = request.AccountId,
                    IBAN = request.IBAN,
                    FullName = request.FullName,
                    ShortName = request.ShortName
                };
            }

            return null;
        }


        private RecipientResponse CreateRecipientResponse(Recipient recipient)
        {
            if (recipient != null)
            {
                return new RecipientResponse(recipient.Id, recipient.IBAN, recipient.FullName, recipient.ShortName);
            }

            return null;
        }

        #endregion
    }
}
