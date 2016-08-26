using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DecalPreview : MonoBehaviour {

    private CarDecal _carDecal;
    public CarDecal CurrentDecal {
        get {
            return _carDecal;
        }
        set {
            _carDecal = value;
            image.overrideSprite = DecalLibrary.Get(_carDecal.TextureID);
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
        image.color = CurrentDecal.Color;
	}
}
