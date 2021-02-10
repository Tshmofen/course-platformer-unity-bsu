using System;

namespace Assets.Scripts.Util
{
    public static class MathUtil
    {
        #region Public

        public static float MinAbs(params float[] numbers)
        {
            int minInd = 0;
            float min = Math.Abs(numbers[0]);
            for (int i = 0; i < numbers.Length; i++)
            {
                float current = Math.Abs(numbers[i]);
                if (current < min)
                {
                    min = current;
                    minInd = i;
                }
            }

            return numbers[minInd];
        }

        public static int Sign(float number)
        {
            if (number < 0) return -1;
            if (number > 0) return 1;
            return 0;
        }

        #endregion
    }
}