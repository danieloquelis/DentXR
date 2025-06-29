using System.Collections;
using TMPro;
using UnityEngine;
using DrillSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject errorDialog;
    [SerializeField] private TMP_Text errorTitle;
    [SerializeField] private TMP_Text errorMessage;

    private bool _isShowingError = false;
    private Coroutine _hideCoroutine;

    void Start()
    {
        errorDialog.SetActive(false);
        DrillController.OnNerveTouched += _ => ShowErrorDialog("Warning!", "You are touching the nerve");
    }

    public void ShowErrorDialog(string title, string message)
    {
        if (_isShowingError) return;

        _isShowingError = true;
        errorTitle.text = title;
        errorMessage.text = message;
        errorDialog.SetActive(true);

        _hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        errorDialog.SetActive(false);
        _isShowingError = false;
    }
}