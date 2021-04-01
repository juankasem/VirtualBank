using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests
{
    public class CreateCashTransactionRequest
    {
        public CashTransaction CashTransaction { get; set; }

        public string RecipeintFirstName { get; set; }

        public string RecipeintLastName { get; set; }


        public CreateCashTransactionRequest(CashTransaction cashTransaction, string recipeintFirstName, string recipeintLastName)
        {
            CashTransaction = Throw.ArgumentNullException.IfNull(cashTransaction, nameof(cashTransaction));
            RecipeintFirstName = recipeintFirstName;
            RecipeintLastName = recipeintLastName;
        }
    }
}   
