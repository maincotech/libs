namespace Maincotech.Common.Security
{
    public class RandomNumber
    {
        private readonly int _MaxNumber;
        private readonly Random _Random;		// the random number

        /// <summary>
        ///
        /// </summary>
        /// <param name="maxLength">The length of the number. Needs to be 1-9.</param>
        public RandomNumber(int maxLength = 9)
        {
            Check.Require(maxLength, nameof(maxLength), Check.Between<int>(1, 9, true));
            // determine the maximum number.
            _MaxNumber = 0;
            for (int i = 0; i < maxLength; i++)
            {
                _MaxNumber = _MaxNumber * 10 + 9;
            }
            _Random = new Random(unchecked((int)DateTime.Now.Ticks));
        }

        public int Get()
        {
            return _Random.Next(_MaxNumber);
        }
    }
}