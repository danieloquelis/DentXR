using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject m_panelIntro;

    [SerializeField]
    private GameObject m_panelModules;

    [SerializeField]
    private GameObject m_DentalSimulationContainer;

    [SerializeField]
    private GameObject m_panelScoreboard;

    void Start()
    {
        m_panelIntro?.SetActive(true);
        m_panelModules?.SetActive(false);
        m_DentalSimulationContainer?.SetActive(false);
        m_panelScoreboard?.SetActive(false);
    }

    public void ShowInputPanel()
    {
        m_panelIntro?.SetActive(false);
        m_panelModules?.SetActive(true);
    }

    public void ShowCavityPreparationSimulation()
    {
        m_DentalSimulationContainer?.SetActive(true);
        m_panelScoreboard?.SetActive(true);
    }

    public void HideSimulation()
    {
        m_DentalSimulationContainer?.SetActive(false);
        m_panelScoreboard?.SetActive(false);
    }
}
