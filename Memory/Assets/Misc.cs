using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    static class Misc
    {

        public static void NormalizeSize(this Transform t, float targetSize = 1f)
        {
            var rends = t.GetComponentsInChildren<Renderer>();
            if (rends.Length == 0)
            {
                Debug.LogWarning("No renderers found to compute bounds");
                return;
            }

            var bounds = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++)
                bounds.Encapsulate(rends[i].bounds);

            float maxDim = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            if (maxDim <= 0f)
            {
                Debug.LogWarning("Bounds have zero size");
                return;
            }

            float scaleFactor = targetSize / maxDim;

            t.localScale = t.localScale * scaleFactor;
        }


        public static void Randomize<T>(this T[,] values)
        {
            // Get the dimensions.
            int num_rows = values.GetUpperBound(0) + 1;
            int num_cols = values.GetUpperBound(1) + 1;
            int num_cells = num_rows * num_cols;

            // Randomize the array.
            System.Random rand = new System.Random();
            for (int i = 0; i < num_cells - 1; i++)
            {
                // Pick a random cell between i and the end of the array.
                int j = rand.Next(i, num_cells);

                // Convert to row/column indexes.
                int row_i = i / num_cols;
                int col_i = i % num_cols;
                int row_j = j / num_cols;
                int col_j = j % num_cols;

                // Swap cells i and j.
                T temp = values[row_i, col_i];
                values[row_i, col_i] = values[row_j, col_j];
                values[row_j, col_j] = temp;
            }
        }
    }
}
