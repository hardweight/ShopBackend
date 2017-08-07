using Shop.Commands.Users;
using Shop.Commands.Wallets;
using Shop.Commands.Wallets.CashTransfers;
using System;
using Shop.Commands.Wallets.BenevolenceTransfers;
using ModelCashTransfers = Shop.Domain.Models.Wallets.CashTransfers;
using ModelBenevolenceTransfers = Shop.Domain.Models.Wallets.BenevolenceTransfers;

namespace Shop.CommandHandlers
{
    /// <summary>
    /// 命令域枚举=》Domain枚举
    /// </summary>
    public static class EnumExtensions
    {
        #region Wallet
        public static Domain.Models.Wallets.WalletDirection ToWalletDirection(this WalletDirection source)
        {
            var value = Domain.Models.Wallets.WalletDirection.In;
            Enum.TryParse(source.ToString(), out value);
            return value;
        }

        public static ModelCashTransfers.CashTransferType ToCashTransferType(this CashTransferType source)
        {
            var value = ModelCashTransfers.CashTransferType.Transfer;
            Enum.TryParse(source.ToString(), out value);
            return value;
        }
        public static ModelCashTransfers.CashTransferStatus ToCashTransferStatus(this CashTransferStatus source)
        {
            var value = ModelCashTransfers.CashTransferStatus.Placed;
            Enum.TryParse(source.ToString(), out value);
            return value;
        }

        public static ModelBenevolenceTransfers.BenevolenceTransferType ToBenevolenceTransferType(this BenevolenceTransferType source)
        {
            var value = ModelBenevolenceTransfers.BenevolenceTransferType.Transfer;
            Enum.TryParse(source.ToString(), out value);
            return value;
        }
        public static ModelBenevolenceTransfers.BenevolenceTransferStatus ToBenevolenceTransferStatus(this BenevolenceTransferStatus source)
        {
            var value = ModelBenevolenceTransfers.BenevolenceTransferStatus.Placed;
            Enum.TryParse(source.ToString(), out value);
            return value;
        }
        #endregion


        public static Domain.Models.Partners.PartnerLevel ToPartnerLevel(this PartnerLevel source)
        {
            var value = Domain.Models.Partners.PartnerLevel.Province;
            Enum.TryParse(source.ToString(), out value);
            return value;
        }
    }
}
