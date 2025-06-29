using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRestarter : MonoBehaviour
{
    public void buttonPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
