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

        // Fisher-Yates
        public static void Randomize<T>(this IList<T> list, System.Random rng = null)
        {
            rng ??= new System.Random();
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }


        public static Bounds GetBounds(string name)
        {
            var go = GameObject.Find(name);
            if (go == null)
            {
                Debug.LogError($"No {name}s were found");
                return new Bounds();
            }
            var rends = go.GetComponentsInChildren<Renderer>();
            if (rends.Length == 0)
            {
                Debug.LogError($"No Renderer on {name} or its children");
                return new Bounds();
            }

            var bounds = rends[0].bounds;
            for (int i = 1; i < rends.Length; i++)
                bounds.Encapsulate(rends[i].bounds);

            return bounds;
        }
    }
}
