using System;
using AutoMapper;
using VirtualBank.Core.ApiResponseModels.FastTransactionApiResponses;
using VirtualBank.Core.Entities;

namespace VirtualBank.Api.MappingProfiles
{
    public class FastTransactionProfile : Profile
    {
        public FastTransactionProfile()
        {
            CreateMap<FastTransaction, FastTransactionResponse>();
        }
    }
}
