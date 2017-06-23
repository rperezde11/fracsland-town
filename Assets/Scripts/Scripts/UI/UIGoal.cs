using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to show a goal into the UI.
/// </summary>
public class UIGoal : MonoBehaviour {

    public enum GOALTYPE {VIOLENT, NON_VIOLENT}

    public GOALTYPE Type;
    public Text Title;
    public Image Icon, Background;

    public Sprite ViolentIcon, NonViolentIcon;

    private GameObject _target;
    private Vector3 _location;

    private bool _complete = false;
    private RectTransform _rect;

    public Quest RelatedQuest;

    private float _blinkingTime = 1f;
    private int _blinkingTimes = 3;

    private Color _whiteColor = Color.white;
    private Color _fullColor = Color.black;

    float t;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Setup(GOALTYPE t, string title, GameObject target, Quest quest)
    {
        Type = t;
        Title.text = title;
        _target = target;
        RelatedQuest = quest;
        ShowGoal();
    }

    public void Setup(GOALTYPE t, string title, Vector3 location)
    {
        Type = t;
        Title.text = title;
        _location = location;
        ShowGoal();
    }

    public void ShowGoal()
    {

        switch (Type)
        {
            case GOALTYPE.VIOLENT:
                Icon.sprite = ViolentIcon;
                break;
            case GOALTYPE.NON_VIOLENT:
                Icon.sprite = NonViolentIcon;
                break;
        }
    }

    void Update()
    {
        BlinkingUpdate();
        if (!_complete) return;
        CompleteUpdate();
    }

    void BlinkingUpdate()
    {
        if (_blinkingTimes > 0)
        {
            t += Time.deltaTime;

            if (t < _blinkingTime)
            {
                Title.color = Color.Lerp(_whiteColor, _fullColor, t / _blinkingTime);
            }
            else
            {
                _blinkingTimes--;
                Title.color = _whiteColor;
                t = 0;
            }
        }
    }

    void CompleteUpdate()
    {
        Title.color = new Color(Title.color.r, Title.color.g, Title.color.b, Title.color.a * 0.99f);
        Icon.color = new Color(Icon.color.r, Icon.color.g, Icon.color.b, Icon.color.a * 0.99f);
    }

    public void ShowComplete()
    {
        GetComponent<LayoutElement>().ignoreLayout = true;
        _complete = true;
        Title.GetComponent<Outline>().enabled = false;
        Destroy(gameObject, 3f);
    }
}
