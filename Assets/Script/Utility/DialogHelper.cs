using System;
using UnityEngine;

public class DialogHelper : MonoBehaviour
{
    public GameObject PanGameOver;

    // Possible Callbacks
    private Action<bool> CallbackBool;

    #region Confirm Dialog
    public void CloseConfirm()
    {
        gameObject.SetActive(false);
        PanGameOver.SetActive(false);
    }

    public void ShowGameOverDialog(Action<bool> callback = null)
    {
        CallbackBool = callback;

        gameObject.SetActive(true);
        PanGameOver.SetActive(true);

    }

    public void ReceiveConfirm(bool response)
    {
        CloseConfirm();
        if (CallbackBool != null)
        {
            CallbackBool(response);
            CallbackBool = null;
        }
    }
    #endregion
}
