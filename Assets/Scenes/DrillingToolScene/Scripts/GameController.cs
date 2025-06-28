using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameController : MonoBehaviour
{
    private GameObject m_panelIntro;
    private GameObject m_panelModules;

    void Start()
    {
        m_panelIntro = GameObject.Find("PanelIntro");
        m_panelModules = GameObject.Find("PanelModules");

        m_panelIntro?.SetActive(true);
        m_panelModules?.SetActive(false);
    }

    public void ShowInputPanel()
    {
        m_panelIntro?.SetActive(false);
        m_panelModules?.SetActive(true);
    }
}
