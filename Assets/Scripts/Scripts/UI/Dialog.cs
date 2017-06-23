using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to create a dialog to inform the player of something like errors, or info.
/// it's like a web dialog, who ask you something and have some options.
/// </summary>
public class Dialog : MonoBehaviour {

    private bool _isVisible = false;
    public bool isVisible { get { return _isVisible; } }

    public enum DIALOG_TYPE { INFORM, MODAL, ALERT }
    public GameObject dialogMask;
    public GameObject _dialogMask;

    public Sprite iconAlert, iconInfo, iconModal;
    private Button _btnAccept, _btnCancel, _btnClose;
    private Text _txtTitle, _txtContent;
    private UIElement _uiElement;
    private Image _imgIcon;
    private Image _glowImage;

    public delegate void btnCallback();

    void Awake()
    {
        _btnAccept = transform.FindChild("btnAccept").GetComponent<Button>();
        _btnCancel = transform.FindChild("btnCancel").GetComponent<Button>();
        _btnClose = transform.FindChild("topBar/btnClose").GetComponent<Button>();
        _txtTitle = transform.FindChild("txtTitle").GetComponent<Text>();
        _txtContent = transform.FindChild("txtContent").GetComponent<Text>();
        _uiElement = GetComponent<UIElement>();
        _imgIcon = transform.FindChild("topBar/icon").GetComponent<Image>();
    }

    public void Open()
    {
        _uiElement.show();
        _dialogMask.GetComponent<UIElement>().show();
        _isVisible = true;
    }

    public void Close()
    {
		_btnAccept.onClick.RemoveAllListeners ();
		_btnClose.onClick.RemoveAllListeners ();

		UnityEngine.Events.UnityAction closeAction = () => { Close();  };
		_btnAccept.onClick.AddListener(closeAction);
		_btnClose.onClick.AddListener(closeAction);

        _uiElement.hide();
        _dialogMask.GetComponent<UIElement>().hide();
        _isVisible = false;
    }

    public void setDialogMask(GameObject dm)
    {
        _dialogMask = dm;
        _glowImage = _dialogMask.transform.FindChild("Glow").GetComponent<Image>();
    }

    public void Create(DialogManager.DialogRequest request)
    {
        Create(request.t, request.title, request.content, request.cancelCallback, request.acceptCallback, request.canBeClosed);
    }

    public void Create(DIALOG_TYPE type, string title, string content, btnCallback cancelCallback=null, btnCallback acceptCallback=null, bool canBeClosed=false)
    {
        //Set the dialog Icon
        switch (type)
        {
            case DIALOG_TYPE.ALERT:
                _imgIcon.sprite = iconAlert;
                _glowImage.color = Color.red;
                break;
            case DIALOG_TYPE.INFORM:
                _imgIcon.sprite = iconInfo;
                _glowImage.color = Color.blue;
                break;
            case DIALOG_TYPE.MODAL:
                _imgIcon.sprite = iconModal;
                _glowImage.color = Color.green;
                break;
            default:
                break;
        }

        _txtTitle.text = title;
        _txtContent.text = content;

        if (cancelCallback != null)
        {
			UnityEngine.Events.UnityAction cancelAction = () => { Close();  cancelCallback();  };
            _btnCancel.onClick.AddListener(cancelAction);
            _btnCancel.gameObject.SetActive(true);
        }
        else
        {
            _btnCancel.gameObject.SetActive(false);
        }

        if (acceptCallback != null)
        {
			UnityEngine.Events.UnityAction acceptAction = () => { Close(); acceptCallback(); };
            _btnAccept.onClick.AddListener(acceptAction);
            _btnAccept.gameObject.SetActive(true);
        }
        else
        {
            _btnAccept.gameObject.SetActive(true);
            UnityEngine.Events.UnityAction closeAction = () => { Close(); };
            _btnAccept.onClick.AddListener(closeAction);
        }

        if(canBeClosed)
        {
            UnityEngine.Events.UnityAction closeAction = () => { Close(); };
            _btnClose.onClick.AddListener(closeAction);
            _btnClose.gameObject.SetActive(true);
        }
        else
        {
            _btnClose.gameObject.SetActive(false);
        }
    
    }
}
