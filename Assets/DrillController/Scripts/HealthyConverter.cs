#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace DrillSystem
{
    public class HealthyConverter : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The parent where the tooth pieces are placed")]
        private GameObject parent;

        [SerializeField]
        [Tooltip("The layer of the healthy pieces")]
        private int healthyLayer;

        [SerializeField]
        [Tooltip("The material to paint the healthy material")]
        private Material healthyMaterial;

        public void Convert()
        {
            foreach (Transform child in parent.transform)
            {
                child.gameObject.layer = healthyLayer;
                child.gameObject.tag = "Piece";

                if (child.gameObject.GetComponent<MeshCollider>() == null)
                    child.gameObject.AddComponent<MeshCollider>();

                // Uncomment if we need to add lit material to the tooth
                //child.GetComponent<MeshRenderer>().material = healthyMaterial;
            }

            Debug.Log("Objects converted");
        }
    }

    [CustomEditor(typeof(HealthyConverter))]
    public class HealthyConverterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("The layer to read all the pieces is \"Piece\"", MessageType.Info);

            GUILayout.Space(10);

            base.OnInspectorGUI();

            HealthyConverter manager = (HealthyConverter)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Convert"))
            {
                manager.Convert();

                Scene activeScene = SceneManager.GetActiveScene();
                EditorSceneManager.MarkSceneDirty(activeScene);
            }
        }
    }
}

#endif