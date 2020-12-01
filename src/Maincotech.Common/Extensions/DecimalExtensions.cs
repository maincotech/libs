namespace System
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// 将decimal转为int，并四舍五入
        /// </summary>
        /// <param name="val"></param>
        /// <param name="roundedUp">是否四舍五入</param>
        /// <returns></returns>
        public static int ToInt(this decimal val, bool roundedUp)
        {
            var i = Convert.ToInt32(val);

            if (roundedUp && (Convert.ToDouble(val) - i) >= 0.5)
            {
                return i + 1;
            }

            return i;
        }
    }
}