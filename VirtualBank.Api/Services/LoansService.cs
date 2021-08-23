using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Core.ApiRequestModels.LoanApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.LoanApiResponses;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class LoansService : ILoansService
    {
        private readonly ILoansRepository _loansRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoansService(ILoansRepository loansRepo,
                            IHttpContextAccessor httpContextAccessor)
        {
            _loansRepo = loansRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// retrieve all loans
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<LoanListResponse>> GetAllLoansAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<LoanListResponse>();

            var allLoans = await _loansRepo.GetAllAsync();

            if (!allLoans.Any())
            {
                return responseModel;
            }

            var loanList = allLoans.OrderByDescending(c => c.CreatedOn).Skip((pageNumber - 1) * pageSize)
                                                                       .Take(pageSize)
                                                                       .Select(x => CreateLoanResponse(x))
                                                                       .ToImmutableList();

            responseModel.Data = new LoanListResponse(loanList, loanList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve loans by customer id
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<LoanListResponse>> GetLoansByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<LoanListResponse>();

            var loans = await _loansRepo.GetByCustomerIdAsync(customerId);

            if (!loans.Any())
            {
                return responseModel;
            }

            var loanList = new List<LoanResponse>();

            foreach (var loan in loans)
            {
                loanList.Add(CreateLoanResponse(loan));
            }

            responseModel.Data = new LoanListResponse(loanList.ToImmutableList(), loanList.Count);

            return responseModel;
        }



        /// <summary>
        /// Retrive loan by iban
        /// </summary>
        /// <param name="iban"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<LoanListResponse>> GetLoansByIBANAsync(string iban, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<LoanListResponse>();

            var loans = await _loansRepo.GetByIBANdAsync(iban);

            if (!loans.Any())
            {
                return responseModel;
            }

            var loanList = new List<LoanResponse>();

            foreach (var loan in loans)
            {
                loanList.Add(CreateLoanResponse(loan));
            }

            responseModel.Data = new LoanListResponse(loanList.ToImmutableList(), loanList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve loam by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<LoanResponse>> GetLoanByIdsync(int id, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<LoanResponse>();

            var loan = await _loansRepo.FindByIdAsync(id);


            if (loan == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(loan), $"loan Id: {id} not found"));
                return responseModel;
            }

            responseModel.Data = CreateLoanResponse(loan);

            return responseModel;
        }


        /// <summary>
        /// Add or edit loan
        /// </summary>
        /// <param name="loanId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> AddOrEditLoanAsync(int loanId, CreateLoanRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new Response();


            if (loanId != 0)
            {
                var loan = await _loansRepo.FindByIdAsync(loanId);

                try
                {
                    if (loan != null)
                    {
                        loan.CustomerId = request.CustomerId;
                        loan.BankAccountId = request.BankAccountId;
                        loan.LoanType = request.LoanType;
                        loan.Amount = request.Amount;
                        loan.DueDate = request.DueDate;

                        await _loansRepo.UpdateAsync(loan);
                    }
                    else
                    {
                        responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(loan), $"loan of id: { loanId} not found"));
                        return responseModel;
                    }
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                }
            }
            else
            {
                try
                {
                    await _loansRepo.AddAsync(CreateLoan(request));
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));
                }
            }

            return responseModel;
        }

        private Loan CreateLoan(CreateLoanRequest request)
        {
            return new Loan()
            {
                CustomerId = request.CustomerId,
                BankAccountId = request.BankAccountId,
                LoanType = request.LoanType,
                Amount = request.Amount,
                DueDate = request.DueDate
            };
        }


        #region private helper methods
        private LoanResponse CreateLoanResponse(Loan loan)
        {

            return new LoanResponse(loan.Id,
                                    loan.Customer?.FirstName + " " + loan.Customer?.LastName,
                                    loan.BankAccount?.IBAN,
                                    loan.LoanType, loan.Amount,
                                    loan.InterestRate, loan.DueDate);

        }
        #endregion
    }
}
