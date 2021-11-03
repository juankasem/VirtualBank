using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VirtualBank.Api.Helpers.ErrorsHelper;
using VirtualBank.Api.Helpers.Methods;
using VirtualBank.Api.Mappers.Response;
using VirtualBank.Core.ApiRequestModels.LoanApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.LoanApiResponses;
using VirtualBank.Core.Domain.Models;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data.Interfaces;

namespace VirtualBank.Api.Services
{
    public class LoansService : ILoansService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoansMapper _loansMapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoansService(IUnitOfWork unitOfWork,
                            ILoansMapper loansMapper,
                            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _loansMapper = loansMapper;
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

            var allLoans = await _unitOfWork.Loans.GetAllAsync();

            if (!allLoans.Any())
            {
                return responseModel;
            }

            var loanList = allLoans.OrderByDescending(c => c.CreationInfo.CreatedOn)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .Select(loan => _loansMapper.MapToResponseModel(loan))
                                   .ToImmutableList();

            responseModel.Data = new(loanList, loanList.Count);

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

            var loans = await _unitOfWork.Loans.GetByCustomerIdAsync(customerId);

            if (!loans.Any())
            {
                return responseModel;
            }

            var loanList = loans.OrderByDescending(loan => loan.ModificationInfo.LastModifiedOn)
                                .Select(loan => _loansMapper.MapToResponseModel(loan))
                                .ToImmutableList();

            responseModel.Data = new(loanList, loanList.Count);

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

            var loans = await _unitOfWork.Loans.GetByIBANdAsync(iban);

            if (!loans.Any())
            {
                return responseModel;
            }

            var loanList = loans.OrderByDescending(loan => loan.ModificationInfo.LastModifiedOn)
                                .Select(loan => _loansMapper.MapToResponseModel(loan))
                                .ToImmutableList();

            responseModel.Data = new(loanList, loanList.Count);

            return responseModel;
        }


        /// <summary>
        /// Retrieve loan by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ApiResponse<LoanResponse>> GetLoanByIdsync(Guid loanId, CancellationToken cancellationToken = default)
        {
            var responseModel = new ApiResponse<LoanResponse>();

            var loan = await _unitOfWork.Loans.FindByIdAsync(loanId);

            if (loan == null)
            {
                responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(loan), $"loan id: {loanId} not found"));

                return responseModel;
            }

            responseModel.Data = new(_loansMapper.MapToResponseModel(loan));

            return responseModel;
        }

        /// <summary>
        /// Add or edit loan
        /// </summary>
        /// <param name="loanId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response> AddOrEditLoanAsync(Guid loanId, CreateLoanRequest request, CancellationToken cancellationToken = default)
        {
            var responseModel = new Response();


            if (loanId != null && Convert.ToInt32(loanId) != 0)
            {
                var loan = await _unitOfWork.Loans.FindByIdAsync(loanId);

                try
                {
                    if (loan != null)
                    {
                        loan.BankAccountCustomer = CreateBankAccountCustomer(request.CustomerId, request.CustomerName, request.IBAN);
                        loan.LoanType = request.LoanType;
                        loan.Amount = request.Amount;
                        loan.DueDate = request.DueDate;
                        loan.ModificationInfo = Utils.CreateModificationInfo(request.CreationInfo.CreatedBy, request.CreationInfo.CreatedOn);

                        await _unitOfWork.Loans.UpdateAsync(loan);
                        await _unitOfWork.SaveAsync();
                    }
                    else

                        responseModel.AddError(ExceptionCreator.CreateNotFoundError(nameof(loan), $"loan of id: { loanId} not found"));

                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                    return responseModel;
                }
            }
            else
            {
                try
                {
                    await _unitOfWork.Loans.AddAsync(CreateLoan(request));
                    await _unitOfWork.SaveAsync();
                }
                catch (Exception ex)
                {
                    responseModel.AddError(ExceptionCreator.CreateInternalServerError(ex.ToString()));

                    return responseModel;
                }
            }

            return responseModel;
        }


        #region private helper methods
        private Loan CreateLoan(CreateLoanRequest request) =>
            new(Guid.NewGuid(),
                CreateBankAccountCustomer(request.CustomerId, request.CustomerName, request.IBAN),
                request.LoanType,
                request.Amount,
                request.InterestRate,
                request.DueDate,
                Utils.CreateCreationInfo(request.CreationInfo.CreatedBy, request.CreationInfo.CreatedOn),
                Utils.CreateModificationInfo(request.CreationInfo.CreatedBy, request.CreationInfo.CreatedOn));


        private BankAccountCustomer CreateBankAccountCustomer(int customerId, string customerName, string iban) =>
                new(customerId, customerName, iban);

        #endregion
    }
}
