using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DecalSample : MonoBehaviour {

    public int TextureID {
        get {
            return _texId;
        }
        set {
            GetComponent<Image>().overrideSprite = DecalLibrary.Get(value);
            _texId = value;
        }
    }
    private int _texId;

}
