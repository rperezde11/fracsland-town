using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// Manages all the dialogs that has to be displayed on the screen.
/// </summary>
public class DialogManager : MonoBehaviour {

    public static DialogManager instance;
    public GameObject dialogPrefab;
    private Dialog _dialog = null;
    private List<DialogRequest> _dialogRequests;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }else
        {
            instance = this;
            _dialogRequests = new List<DialogRequest>();
        }
        
    }

    void Update ()
    {
        try
        {
            if (_dialogRequests.Count > 0 && _dialog != null && !_dialog.isVisible)
            {
                DialogRequest dr = _dialogRequests[0];
                _dialogRequests.RemoveAt(0);
                Debug.Log("Num Dialog Requests" + _dialogRequests.Count.ToString());
                _dialog.Create(dr);
                _dialog.Open();
            }
        }catch(NullReferenceException  ex)
        {
            Debug.Log("Wooopsy!!");
        }

	}

    public void SpawnDialog()
    {
        //Spawn Dialog
        GameObject ui = GameObject.Find("UI");
        GameObject dialog = Instantiate(dialogPrefab) as GameObject;
        dialog.GetComponent<UIElement>().hide();
        _dialog = dialog.GetComponent<Dialog>();

        //Spawn Dialog Mask
        GameObject dm = Instantiate(_dialog.dialogMask);
        dm.GetComponent<UIElement>().hide();
        RectTransform rt = dm.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(Screen.width, Screen.height);
        dm.transform.SetParent(ui.transform);
        rt.localPosition = Vector3.zero;
        _dialog.setDialogMask(dm);
        dialog.transform.SetParent(ui.transform);
        dialog.GetComponent<RectTransform>().localPosition = Vector3.zero;
        
    }

	public void CloseDialog()
	{
		_dialog.Close ();
	}

    public struct DialogRequest
    {
        public Dialog.DIALOG_TYPE t;
        public Dialog.btnCallback cancelCallback, acceptCallback;
        public string title, content;
        public bool canBeClosed;

        public DialogRequest(Dialog.DIALOG_TYPE t, string title, string content, Dialog.btnCallback cancelCallback = null, Dialog.btnCallback acceptCallback = null, bool canBeClosed = false)
        {
            this.t = t;
            this.title = title;
            this.content = content;
            this.cancelCallback = cancelCallback;
            this.acceptCallback = acceptCallback;
			if(this.cancelCallback == null){
				this.cancelCallback = () => {DialogManager.instance.CloseDialog();};
			}
			if(this.acceptCallback == null){
				this.acceptCallback = () => {DialogManager.instance.CloseDialog();};
			}
            this.canBeClosed = canBeClosed;
        }

    }

    public void CreateDialogRequest(DialogRequest dr)
    {
        _dialogRequests.Add(dr);
    }

}
