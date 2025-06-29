using System.Collections.Generic;
using UnityEngine;

namespace DrillSystem
{
    public class PrepareHealtyColor : MonoBehaviour
    {
        [SerializeField]
        private Material m_healthyTeethPartMaterial;

        void Start()
        {
            if (m_healthyTeethPartMaterial == null)
                return; 

            List<GameObject> objectsInLayer = GetAllGameObjectsInLayer("Healthy");
            foreach (GameObject obj in objectsInLayer)
            {
                var renderer = obj.GetComponent<Renderer>();
                renderer.material = m_healthyTeethPartMaterial;
            }
        }
        
        private List<GameObject> GetAllGameObjectsInLayer(string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName); // Convert layer name to layer number
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(); // Find all game objects
            List<GameObject> objectsInLayer = new List<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.layer == layer) // Check if the object's layer matches the target layer
                {
                    objectsInLayer.Add(obj); // Add matching objects to the list
                }
            }

            return objectsInLayer; // Return the list of game objects found in the layer
        }
    }
}