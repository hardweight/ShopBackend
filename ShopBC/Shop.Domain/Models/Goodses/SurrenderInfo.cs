using Shop.Common;
using System;

namespace Shop.Domain.Models.Goodses
{
    /// <summary>
    /// 商品的让利信息 
    /// </summary>
    [Serializable]
    public class SurrenderInfo
    {
        /// <summary>
        /// 让利比例
        /// </summary>
        public decimal SurrenderPersent { get; private set; }

        /// <summary>
        /// 消费者购买商品获取的 善心量
        /// </summary>
        public decimal ConsumerBenevolence { get; private set; }

        /// <summary>
        /// 售出商品 商家获取的 善心量
        /// </summary>
        public decimal StoreBenevolence { get; private set; }

        /// <summary>
        /// 创建让利信息
        /// </summary>
        /// <param name="price">商品价格</param>
        /// <param name="surrenderPersent">让利比例 如：0.1让利10%</param>
        public SurrenderInfo(decimal price, decimal surrenderPersent)
        {
            if (surrenderPersent <= 0.05M)
            {
                throw new Exception("产品让利不得低于5%");
            }

            SurrenderPersent = surrenderPersent;
            ///消费者都是5倍让利
            ConsumerBenevolence = Math.Round((price * surrenderPersent * ConfigSettings.ConsumerMultiple / ConfigSettings.BenevolenceValue), 2);
            if (surrenderPersent >= 0.15M)
            {//让利超过15% 商家双倍让利
                StoreBenevolence = Math.Round((price * surrenderPersent * ConfigSettings.StoreMultiple / ConfigSettings.BenevolenceValue), 2);
            }
            else
            {//不超过15% 商家单倍让利
                StoreBenevolence = Math.Round((price * surrenderPersent / ConfigSettings.BenevolenceValue), 2);
            }
        }
    }
}
