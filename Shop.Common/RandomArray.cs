using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xia.Common.Extensions;

namespace Shop.Common
{
    public class RandomArray
    {
        /// <summary>
        /// 善心指数
        /// </summary>
        /// <returns></returns>
        public static decimal BenevolenceIndex()
        {
            var randomArray = new decimal[] { 0.0023M, 0.00232M, 0.00234M, 0.00236M,0.00233M,0.00237M,
                0.0024M, 0.00241M, 0.00245M, 0.00242M,0.00243M,0.00247M,0.00248M,0.00249M,0.00251M,0.00266M,0.0027M,
                0.00256M, 0.00257M,0.00252M,0.00255M,0.00251M,0.00254M,0.00255M,0.00263M,0.00269M,
                0.0026M, 0.00261M, 0.00264M,0.00265M,0.00266M,0.00267M,0.00268M,0.00269M,
                0.0027M,0.00273M,0.00274M,0.00275M,0.00279M,
                0.0028M };
            return new Random().NextItem(randomArray);
        }

        /// <summary>
        /// 新用户红包
        /// </summary>
        /// <returns></returns>
        public static decimal NewUserRedPacket()
        {
            var randomArray = new decimal[] {
                1M,1.22M,1.34M,1.4M,1.44M,1.5M, 1.8M, 1.83M,1.9M, 1.91M,1.99M,
                2M, 2.21M, 2.22M,2.31M,2.3M, 2.4M,2.45M,2.55M,2.66M,2.72M,2.73M,2.78M,2.79M,2.8M,2.9M,
                3M, 3.1M,3.22M,3.23M,3.3M,3.33M,3.55M,3.6M,3.7M,3.8M,3.9M,3.99M,
                4.01M, 4.2M,4.26M,
                5M};
            return new Random().NextItem(randomArray);
        }

    }
}
