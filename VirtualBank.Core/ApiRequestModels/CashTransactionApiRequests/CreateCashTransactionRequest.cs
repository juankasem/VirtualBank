﻿using System;
using VirtualBank.Core.ArgumentChecks;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.ApiRequestModels.CashTransactionApiRequests
{
    #nullable enable
    public class CreateCashTransactionRequest
    {
        public CashTransactionType Type { get; set; }

        public BankAssetType InitiatedBy { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public decimal Amount { get; set; }
     
        public decimal SenderRemainingBalance { get; set; }

        public decimal RecipientRemainingBalance { get; set; }

        public string Description { get; set; }

        public PaymentType PaymentType { get; set; }

        public DateTime TransactionDate { get; set; }

        public string RecipeintFirstName { get; set; }

        public string RecipeintLastName { get; set; }

        public string? CreditCardNo { get; set; }

        public string? DebitCardNo { get; set; }

        public string? PIN { get; set; }



        public CreateCashTransactionRequest(CashTransactionType type, BankAssetType initiatedBy, string from, string to,
                                            decimal senderRemainingBalance, decimal recipientRemainingBalance, string description,
                                            PaymentType paymentType, DateTime transactionDate, string recipeintFirstName, string recipeintLastName,
                                            string creditCardNo, string debitCardNo, string pin)
        {
            Type = Throw.ArgumentNullException.IfNull(type, nameof(type));
            InitiatedBy = Throw.ArgumentNullException.IfNull(initiatedBy, nameof(initiatedBy));
            From = from;
            To = to;
            SenderRemainingBalance = senderRemainingBalance;
            RecipientRemainingBalance = recipientRemainingBalance;
            Description = description;
            PaymentType = Throw.ArgumentNullException.IfNull(paymentType, nameof(paymentType));
            TransactionDate = Throw.ArgumentNullException.IfNull(transactionDate, nameof(transactionDate));
            RecipeintFirstName = recipeintFirstName;
            RecipeintLastName = recipeintLastName;
            CreditCardNo = creditCardNo;
            DebitCardNo = debitCardNo;
            PIN = pin;
        }
    }
}   
