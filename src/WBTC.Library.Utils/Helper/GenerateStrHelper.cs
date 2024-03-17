using AutoMapper;
using System;
using System.Collections.Generic;

namespace WBTC.Library.Utils.Helper
{
    
    public static class GenerateStrHelper
    {
        #region 生成数字
        /// <summary>
        /// 生成数字字符串
        /// </summary>
        /// <param name="minvalue">最小值</param>
        /// <param name="maxvalue">最大值</param>
        /// <returns>返回生成值 </returns>
        public static int GenerateRandomNum(int minvalue, int maxvalue)
        {
            if (maxvalue > Int32.MaxValue || minvalue > maxvalue)
            {
                throw new ArgumentException("参数有误");
            }
            return new Random().Next(minvalue, maxvalue);
        }
        /// <summary>
        /// 按顺序生成数字
        /// </summary>
        /// <param name="minvalue"></param>
        /// <param name="maxvalue"></param>
        /// <param name="action">执行的动作</param>
        public static void GenerateNum(int minvalue, int maxvalue, Action<int> action)
        {
            if (maxvalue > Int32.MaxValue || minvalue > maxvalue)
            {
                throw new ArgumentException("参数有误");
            }
            for (int i = minvalue; i < maxvalue; i++)
            {
                action?.Invoke(i);
            }
        }
        /// <summary>
        /// 获得指定位数的最小值最大值
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Tuple<int, int> GetValueFromLength(int length)
        {
            if (length < 1)
            {
                throw new ArgumentException("位数必须大于0");
            }
            int minValue = (int)Math.Pow(10, length - 1);
            int maxValue = 0;
            for (int i = 0; i < length; i++)
            {
                maxValue = (maxValue * 10) + 9;
            }
            return new Tuple<int, int>(minValue, maxValue);
        }
        #endregion
    }
}
