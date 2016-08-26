using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DecalSelector : MonoBehaviour {

    private CarDecalCreator _decalCreator;
    private CarDecalCreator decalCreator {
        get {
            return _decalCreator == null ? (_decalCreator = FindObjectOfType<CarDecalCreator>()) : _decalCreator;
        }
    }
    private Animator _animator;
    private Animator animator {
        get {
            return _animator == null ? (_animator = GetComponent<Animator>()) : _animator;
        }
    }
    private ScrollRect _scrollRect;
    private ScrollRect scrollRect {
        get {
            return _scrollRect == null ? (_scrollRect = GetComponent<ScrollRect>()) : _scrollRect;
        }
    }

    private bool decalPickerOpen = true;

	// Use this for initialization
	void Start () {
        int index = 0;
        GameObject decalSamplePrefab = Resources.Load("DecalSampler") as GameObject;
        Transform content = GetComponent<ScrollRect>().content;
        foreach(Sprite spr in DecalLibrary.Instance.TexList) {
            GameObject t = Instantiate(decalSamplePrefab);
            t.transform.SetParent(content);
            t.GetComponent<DecalSample>().TextureID = index;
            t.GetComponent<Button>().onClick.AddListener(() => DecalClicked(t.GetComponent<DecalSample>()));
            index++;
        }
	}
    void Update() {
        if(!decalPickerOpen) {
            scrollRect.content.localPosition = Vector3.Lerp(scrollRect.content.localPosition, scrollPos, Time.deltaTime * 8f);
        }
    }

    private Vector3 scrollPos = Vector3.zero;

    void DecalClicked (DecalSample ds) {
        if(decalPickerOpen) {
            scrollPos = new Vector3(-ds.transform.localPosition.x, scrollRect.content.localPosition.y);
            scrollRect.StopMovement();
            scrollRect.enabled = false;
            animator.SetTrigger("GoPreview");
        } else {
            scrollRect.enabled = true;
            animator.SetTrigger("GoFull");
        }
        decalPickerOpen = !decalPickerOpen;
    }
}
