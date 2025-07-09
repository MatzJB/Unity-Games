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


        public static void Randomize<T>(this IList<T> list)
        {
            var rand = new System.Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        // keep matrix overload if you still want it
        public static void Randomize<T>(this T[,] values)
        {
            int rows = values.GetLength(0);
            int cols = values.GetLength(1);

            var flat = new List<T>(rows * cols);
            foreach (var v in values) flat.Add(v);

            flat.Randomize();                          // uses the list version above

            int idx = 0;
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    values[r, c] = flat[idx++];
        }
    }
}
