using UnityEngine;

public static class WeightedRNG
{
    public static int Get(int[] weights)
    {
        int[] weightsSum = weights;
        int length = weightsSum.Length;
        for (int i = 1; i < length; i++)
        {
            weightsSum[i] += weightsSum[i - 1];
        }
        int rng = Random.Range(0, weightsSum[length - 1]);
        int result = UpperBound(weightsSum, rng);
        return result;
    }

    public static int UpperBound(int[] array, int target)
    {
        int low = 0;
        int high = array.Length;

        while (low < high)
        {
            int mid = low + ((high - low) >> 1);

            // For Upper Bound, if the mid value is less than or EQUAL to target, 
            // the strict upper bound must lie to the right.
            if (array[mid] <= target)
            {
                low = mid + 1;
            }
            else
            {
                high = mid;
            }
        }
        return low; // Returns array.Length if no element is greater than target
    }
}
