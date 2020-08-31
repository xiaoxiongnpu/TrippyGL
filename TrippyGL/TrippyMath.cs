using System;

#pragma warning disable CA1062 // Validate arguments of public methods

namespace TrippyGL
{
    /// <summary>
    /// Provides some general help math-related functions and values.
    /// </summary>
    public static class TrippyMath
    {
        /// <summary>
        /// Calculates the size to use for an array that needs resizing, where the new size
        /// will be a power of two times the previous capacity.
        /// </summary>
        /// <param name="currentCapacity">The current length of the array.</param>
        /// <param name="requiredCapacity">The minimum required length for the array.</param>
        /// <remarks>
        /// This is calculated with the following equation:<para/>
        /// <code>
        /// newCapacity = currentCapacity * pow(2, ceiling(log2(requiredCapacity/currentCapacity)));
        /// </code>
        /// </remarks>
        public static int GetNextCapacity(int currentCapacity, int requiredCapacity)
        {
            // Finds the smallest number that is greater than requiredCapacity and satisfies this equation:
            // " newCapacity = oldCapacity * 2^X " where X is an integer

            const double log2 = 0.69314718055994530941723212145818;
            int power = (int)Math.Ceiling(Math.Log(requiredCapacity / (double)currentCapacity) / log2);
            return currentCapacity * IntegerPow(2, power);
        }

        public static int IntegerPow(int a, int b)
        {
            int r = 1;
            while (b > 0)
            {
                if ((b & 1) == 1)
                    r *= a;
                b >>= 1;
                a *= a;
            }
            return r;
        }

        public static long IntegerLongPow(long a, long b)
        {
            long r = 1;
            while (b > 0)
            {
                if ((b & 1) == 1)
                    r *= a;
                b >>= 1;
                a *= a;
            }
            return r;
        }

        public static uint UnsignedIntegerPow(uint a, uint b)
        {
            uint r = 1;
            while (b > 0)
            {
                if ((b & 1) == 1)
                    r *= a;
                b >>= 1;
                a *= a;
            }
            return r;
        }

        public static ulong UnsignedIntegerLongPow(ulong a, ulong b)
        {
            ulong r = 1;
            while (b > 0)
            {
                if ((b & 1) == 1)
                    r *= a;
                b >>= 1;
                a *= a;
            }
            return r;
        }

        public static float NextFloat(this Random random, float max)
        {
            return (float)random.NextDouble() * max;
        }

        public static float NextFloat(this Random random, float min, float max)
        {
            return (float)random.NextDouble() * (max - min) + min;
        }
    }
}
