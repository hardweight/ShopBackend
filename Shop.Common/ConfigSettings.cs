﻿using System;
using System.Configuration;

namespace Shop.Common
{
    /// <summary>
    /// 公共配置
    /// </summary>
    public class ConfigSettings
    {
        /// <summary>
        /// 订单预定自动过期时间 30分钟
        /// </summary>
        public static TimeSpan ReservationAutoExpiration = TimeSpan.FromMinutes(30);
        /// <summary>
        /// 订单商品服务自动过期时间 10天
        /// </summary>
        public static TimeSpan OrderGoodsServiceAutoExpiration = TimeSpan.FromDays(10);
        /// <summary>
        /// ENode 数据库链接字符串
        /// </summary>
        public static string ENodeConnectionString { get; set; }
        /// <summary>
        ///  数据库链接字符串
        /// </summary>
        public static string ConnectionString { get; set; }

        /// <summary>
        /// 成为传递使者 最低推荐人数
        /// </summary>
        public static int ToPasserRecommendCount { get; set; }
        /// <summary>
        /// 成为传递使者 最低消费金额
        /// </summary>
        public static decimal ToPasserSpendingAmount { get; set; }
        /// <summary>
        /// 成为传递大使 缴纳年份金额
        /// </summary>
        public static decimal ToAmbassadorChargeAmount { get; set; }

        /// <summary>
        /// 一个善心的价值
        /// </summary>
        public static decimal BenevolenceValue { get; set; }

        /// <summary>
        /// 消费者消费激励倍数
        /// </summary>
        public static int ConsumerMultiple { get; set; }
        public static int StoreMultiple { get; set; }

        /// <summary>
        /// 拿推荐商家销售商品额的百分比3%
        /// </summary>
        public static decimal RecommandStoreGetPercent { get; set; }

        //表名称
        public static string UserTable { get; set; }
        public static string UserMobileIndexTable { get; set; }
        public static string ExpressAddressTable { get; set; }
        public static string UserGiftTable { get; set; }
        public static string PartnerApplyTable { get; set; }

        public static string CartTable { get; set; }
        public static string AnnouncementTable { get; set; }
        public static string CategoryTable { get; set; }

        public static string GoodsTable { get; set; }
        public static string SpecificationTable { get; set; }
        public static string GoodsParamTable { get; set; }
        public static string GoodsCommentsTable { get; set; }
        public static string ReservationItemsTable { get; set; }

        public static string OrderTable { get; set; }
        public static string OrderLineTable { get; set; }
        public static string PaymentTable { get; set; }
        public static string PaymentItemTable { get; set; }

        public static string StoreTable { get; set; }
        public static string StoreOrderTable { get; set; }
        public static string OrderGoodsTable { get; set; }
        public static string ApplyServiceTable { get; set; }
        public static string ServiceExpressTable { get; set; }

        public static string SectionTable { get; set; }
        public static string PartnerTable { get; set; }

        public static string WalletTable { get; set; }
        public static string CashTransferTable { get; set; }
        public static string BenevolenceTransferTable { get; set; }
        public static string BankCardTable { get; set; }

        public static string GranteeTable { get; set; }
        public static string GranteeMoneyHelpsTable { get; set; }
        public static string GranteeTestifysTable { get; set; }

        public static int NameServerPort { get; set; }
        public static int BrokerProducerPort { get; set; }
        public static int BrokerConsumerPort { get; set; }
        public static int BrokerAdminPort { get; set; }

        /// <summary>
        /// 初始化配置
        /// </summary>
        public static void Initialize()
        {
            if (ConfigurationManager.ConnectionStrings["enode"] != null)
            {
                ENodeConnectionString = ConfigurationManager.ConnectionStrings["enode"].ConnectionString;
            }
            if (ConfigurationManager.ConnectionStrings["shop"] != null)
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["shop"].ConnectionString;
            }

            ToPasserRecommendCount = 10;
            ToPasserSpendingAmount = 99;
            ToAmbassadorChargeAmount = 365;

            BenevolenceValue = 100;
            ConsumerMultiple = 5;
            StoreMultiple = 2;

            RecommandStoreGetPercent = 0.03M;//3%

            UserTable = "Users";
            UserMobileIndexTable = "UserMobiles";
            ExpressAddressTable = "UserExpressAddresses";
            UserGiftTable = "UserGifts";
            PartnerApplyTable = "PartnerApplys";

            CartTable = "Carts";
            CategoryTable = "Categorys";
            AnnouncementTable = "Announcements";

            GoodsTable = "Goodses";
            SpecificationTable = "Specificationes";
            GoodsParamTable = "GoodsParams";
            GoodsCommentsTable = "GoodsComments";
            ReservationItemsTable = "ReservationItems";

            StoreTable = "Stores";
            StoreOrderTable = "StoreOrders";
            OrderGoodsTable = "OrderGoodses";
            ApplyServiceTable = "ApplyServices";
            ServiceExpressTable = "ServiceExpresses";

            SectionTable = "StoreSections";
            PartnerTable = "Partners";

            OrderTable = "Orders";
            OrderLineTable = "OrderLines";
            WalletTable = "Wallets";
            CashTransferTable = "CashTransfers";
            BenevolenceTransferTable = "BenevolenceTransfers";
            BankCardTable = "BankCards";

            GranteeTable = "Grantees";
            GranteeMoneyHelpsTable = "GranteeMoneyHelps";
            GranteeTestifysTable = "GranteeTestifys";

            PaymentTable = "Payments";
            PaymentItemTable = "PaymentItems";

            NameServerPort = 11100;

            BrokerProducerPort = 11101;
            BrokerConsumerPort = 11102;
            BrokerAdminPort = 11103;
        }
    }
}
