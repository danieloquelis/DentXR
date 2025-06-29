using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolController : MonoBehaviour
{
    [Header("Tools")]
    [SerializeField] private List<GameObject> tools;

    [Header("Dependencies")]
    [SerializeField] private CustomStylusHandler customStylusHandler;

    public UnityEvent onSwitchTool;
    
    private int _currentToolIndex = 0;
    private GameObject _selectedTool;

    private void Start()
    {
        customStylusHandler.OnFrontPressed += SwitchNextTool;
        customStylusHandler.OnBackPressed += SwitchPreviousTool;

        if (tools == null || tools.Count == 0)
        {
            Debug.LogError("ToolController: No tools assigned.");
            return;
        }

        ActivateTool(_currentToolIndex);
    }

    private void SwitchNextTool()
    {
        SwitchTool((_currentToolIndex + 1) % tools.Count);
    }

    private void SwitchPreviousTool()
    {
        int newIndex = (_currentToolIndex - 1 + tools.Count) % tools.Count; // wrap around backward
        SwitchTool(newIndex);
    }

    private void SwitchTool(int newIndex)
    {
        if (_selectedTool != null)
        {
            _selectedTool.SetActive(false);
        }

        _currentToolIndex = newIndex;
        ActivateTool(_currentToolIndex);
        onSwitchTool?.Invoke();
    }

    private void ActivateTool(int index)
    {
        _selectedTool = tools[index];
        _selectedTool.SetActive(true);
    }

    private void OnDestroy()
    {
        if (customStylusHandler != null)
        {
            customStylusHandler.OnFrontPressed -= SwitchNextTool;
            customStylusHandler.OnBackPressed -= SwitchPreviousTool;
        }
    }
}