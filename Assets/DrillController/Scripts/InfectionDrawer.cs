#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace DrillSystem
{
    [ExecuteInEditMode]
    public class InfectionDrawer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The radius of the detection")]
        private float sphereRadius;

        [SerializeField]
        [Tooltip("The layer of the infected pieces")]
        private int infectedLayer;

        [SerializeField]
        [Tooltip("The material of infection")]
        private Material infectionMaterial;

        private void Start()
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                GetComponent<SphereCollider>().radius = sphereRadius;

                Collider[] hits = Physics.OverlapSphere(transform.position, sphereRadius);

                foreach (Collider hit in hits)
                {
                    if (hit.gameObject != gameObject && hit.gameObject.CompareTag("Piece"))
                    {
                        hit.gameObject.layer = infectedLayer;
                        hit.gameObject.GetComponent<Renderer>().material = infectionMaterial;

                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                    }
                }
            }
        }
    }
}

#endif