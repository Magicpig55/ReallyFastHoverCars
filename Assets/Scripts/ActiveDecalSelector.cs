using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActiveDecalSelector : MonoBehaviour {

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

    private List<CarDecal> DecalList = new List<CarDecal>();

    private GameObject decalPreviewPrefab = Resources.Load<GameObject>("DecalPreview");

    public void AddDecal(CarDecal cd) {
        DecalList.Add(cd);
        GameObject t = Instantiate(decalPreviewPrefab);
        t.transform.SetParent(scrollRect.content);
        t.GetComponent<DecalPreview>().CurrentDecal = cd;
        t.GetComponent<Button>().onClick.AddListener(() => { DecalClicked(cd); });
    }

    void DecalClicked(CarDecal decal) {
        decalCreator.SelectedDecal = decal.associatedObject;
        decalCreator.SelectedSurface = decal.Part;
    }
}
