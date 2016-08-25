using UnityEngine;
using System.Collections;

public class DecalLibrary : MonoBehaviour {

    private static DecalLibrary _instance;
    public static DecalLibrary Instance {
        get {
            return _instance == null ? ((_instance = FindObjectOfType(typeof(DecalLibrary)) as DecalLibrary) != null ? _instance : _instance = Resources.Load<GameObject>("Prefabs/DecalLibrary").GetComponent<DecalLibrary>()) : _instance;
        }
    }

    public static Sprite Get (int id) {
        return Instance.TexList[id];
    }

    public Sprite[] TexList;

}
