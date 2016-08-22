using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour {

    public HoverBalancer CarToTrack;

    private Animator _anim;
    Animator anim {
        get {
            return _anim == null ? (_anim = GetComponent<Animator>()) : _anim;
        }
    }

    private Text _text;
    Text text {
        get {
            return _text == null ? (_text = GetComponentInChildren<Text>()) : _text;
        }
    }
	
	// Update is called once per frame
	void Update () {
        anim.SetFloat("speedPercentage", CarToTrack.SpeedPercent);
        text.text = string.Format("{0:N0} #DICKSOUT4HARAMBE", CarToTrack.CurrentSpeed);
	}
}
