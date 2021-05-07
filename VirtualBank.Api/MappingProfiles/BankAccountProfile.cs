using System;
using AutoMapper;
using VirtualBank.Core.ApiResponseModels.AccountApiResponses;
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
