using System;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualBank.Core.Enums;

namespace VirtualBank.Core.Models
{
    [NotMapped]
    public class BankAsset
    {
        public BankAssetType Type { get; set; }

        public string Value { get; set; }

    }
}

