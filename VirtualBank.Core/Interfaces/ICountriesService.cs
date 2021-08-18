﻿using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualBank.Core.ApiRequestModels.CountryApiRequests;
using VirtualBank.Core.ApiResponseModels;
using VirtualBank.Core.ApiResponseModels.CountryApiResponse;

namespace VirtualBank.Core.Interfaces
{
    public interface ICountriesService
    {
        Task<ApiResponse<CountryListResponse>> GetAllCountriesAsync(bool includeCities = false, CancellationToken cancellationToken = default);

        Task<ApiResponse<CountryResponse>> GetCountryByIdAsync(int countryId, bool includeCities = false, CancellationToken cancellationToken = default);

        Task<ApiResponse<CountryResponse>> AddOrEditCountryAsync(int countryId, CreateCountryRequest request, CancellationToken cancellationToken = default);
    }
}
