using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DecalSelector : MonoBehaviour {



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
	
    void DecalClicked (DecalSample ds) {
        print(ds.TextureID);
    }
}
