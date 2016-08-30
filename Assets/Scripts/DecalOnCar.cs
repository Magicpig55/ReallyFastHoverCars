using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DecalOnCar : MonoBehaviour {

    private CarDecal _currentDecal = null;
    public CarDecal CurrentDecal {
        set {
            rectTransform.position = value.Position;
            rectTransform.Rotate(rectTransform.forward, value.Rotation);
            rectTransform.localScale = value.Scale;
            image.color = value.Color;
            image.overrideSprite = DecalLibrary.Get(value.TextureID);
            _currentDecal = value;
        }
        get {
            return _currentDecal;
        }
    }

    private RectTransform _rectTransform;
    private RectTransform rectTransform {
        get {
            return _rectTransform == null ? (_rectTransform = GetComponent<RectTransform>()) : _rectTransform;
        }
    }
    private Image _image;
    private Image image {
        get {
            return _image == null ? (_image = GetComponent<Image>()) : _image;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (CurrentDecal != null) {
            rectTransform.localPosition = CurrentDecal.Position;
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, CurrentDecal.Rotation));
            rectTransform.localScale = CurrentDecal.Scale;
            image.color = CurrentDecal.Color;
        }
    }
}
