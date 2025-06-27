#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class HealthyConverter : MonoBehaviour
{
    [SerializeField]
    private GameObject parent;

    [SerializeField]
    private int healthyLayer;

    public void Convert()
    {
        foreach(Transform child in parent.transform)
        {
            child.gameObject.layer = healthyLayer;
        }

        Debug.Log("Objects converted");
    }
}

[CustomEditor(typeof(HealthyConverter))]
public class HealthyConverterEditor : Editor
{
    public override void OnInspectorGUI()
    {
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

#endif