using UnityEngine;
using UnityEngine.UI;

// Para poder ver la clase en el Inspector de Unity
[System.Serializable]
public class TransformCoordinates {
    public Vector3 buttonCoordinates;
}

public class hideButtons : MonoBehaviour {

    public GameObject panelToHide;
    public bool otherObject;
    public GameObject panelToHide2;
    public Texture hideImage;
    public Texture showImage;
    public TransformCoordinates coordinates;


    private Button btn;
    private bool isShowing;

    private void Start() {
        isShowing = true;
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    private void TaskOnClick() {
        isShowing = !isShowing;
        panelToHide.SetActive(isShowing);
        if (otherObject) panelToHide2.SetActive(isShowing);
        UpdateImage(btn.GetComponent<RawImage>());
        UpdatePosition(btn.GetComponent<RectTransform>());
    }

    private void UpdateImage(RawImage img) {
        if (isShowing) img.texture = (Texture) hideImage;
        else img.texture = (Texture) showImage;
    }

    private void UpdatePosition(RectTransform transform) {
        if (isShowing) transform.localPosition = new Vector3(transform.localPosition.x - coordinates.buttonCoordinates.x, transform.localPosition.y, transform.localPosition.y);
        else transform.localPosition = new Vector3(transform.localPosition.x + coordinates.buttonCoordinates.x, transform.localPosition.y, transform.localPosition.y);

        string str = string.Format("transform.localPosition: {0}", transform.localPosition);
        Debug.Log(str);
    }
}
