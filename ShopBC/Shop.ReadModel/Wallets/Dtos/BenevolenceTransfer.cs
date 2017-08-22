using Shop.Common.Enums;
using System;
using System.ComponentModel;

namespace Shop.ReadModel.Wallets.Dtos
{
    public class BenevolenceTransfer
    {
        public Guid Id { get; set; }
        private Guid WalletId;//钱包Id
        public string Number { get; set; }
        public decimal Amount { get; private set; }
        public decimal Fee { get; private set; }
        public BenevolenceTransferType Type { get; set; }
        public BenevolenceTransferStatus Status { get; set; }
        public WalletDirection Direction { get; private set; }
        public DateTime CreatedOn { get; set; }
        public string Remark { get; private set; }
    }
    
}
