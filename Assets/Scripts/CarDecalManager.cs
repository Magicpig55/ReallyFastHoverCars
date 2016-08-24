using UnityEngine;
using System.Collections;

public class CarDecalManager : MonoBehaviour {

    public Material Wrap {
        get {
            return renderer.enabled ? renderer.material : null;
        }
        set {
            if (value == null) {
                renderer.enabled = false;
            } else {
                renderer.material = value;
                renderer.enabled = true;
            }
        }
    }

    private Renderer _renderer;
    private new Renderer renderer {
        get {
            return _renderer != null ? _renderer : (_renderer = GetComponent<Renderer>());
        }
    }

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
