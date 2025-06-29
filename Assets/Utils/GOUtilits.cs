using System.Linq;
using UnityEngine;

namespace Utils
{
    public class GOUtils: MonoBehaviour
    {
        public static GameObject[] FindGameObjectsWithLayer(int layer) 
        {
            var goArray = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            var goList = goArray.Where(t => t.layer == layer).ToList();
            return goList.ToArray();
        }
    }
}

