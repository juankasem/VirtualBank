using System;
using AutoMapper;
using VirtualBank.Core.ApiResponseModels.BankAccountApiResponses;
using VirtualBank.Core.Entities;

namespace VirtualBank.Api.MappingProfiles
{
    public class BankAccountProfile : Profile
    {
        public BankAccountProfile()
        {
            CreateMap<BankAccount, BankAccountResponse>();

        }
    }
}
