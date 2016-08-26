using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CarDecalCreator : MonoBehaviour {

    public bool EditMode = false;
    [HideInInspector]
    public RenderTexture EditorRenderTexture;
    public int EditorRenderResolution = 1024; // Soon this'll be a property, once I get options implemented
    public CarDecalManager CarToManage;
    [HideInInspector]
    public CarDecalPart SelectedSurface = CarDecalPart.None;

    private Transform _selectedDecal;
    private Transform _selectionMarker;
    private Transform selectionMarker {
        get {
            return _selectionMarker == null ? (_selectionMarker = GameObject.Find("SelectionMarker").transform) : _selectionMarker;
        }
    }
    public Transform SelectedDecal {
        get {
            return _selectedDecal;
        }
        set {
            if (value == null) {
                //selectionMarker.gameObject.SetActive(false);
                _selectedDecal = null;
            } else {
                _selectedDecal = value;
                //selectionMarker.gameObject.SetActive(true);
                //selectionMarker.SetParent(_selectedDecal, false);
                //selectionMarker.GetComponent<Renderer>().enabled = true;
            }
        }
    }

    private CarDecal _selectedDecalTexture;
    public CarDecal SelectedDecalTexture {
        get {
            return _selectedDecalTexture;
        }
        set {
            _selectedDecalTexture = value;
        }
    }

    private ActiveDecalSelector _activeDecalSelector;
    public ActiveDecalSelector ActiveCarDecalSelector {
        get {
            return _activeDecalSelector == null ? (_activeDecalSelector = FindObjectOfType<ActiveDecalSelector>()) : _activeDecalSelector;
        }
    }

    private Camera _camera;
    private new Camera camera {
        get {
            return _camera == null ? (_camera = GetComponent<Camera>()) : _camera;
        }
    }

    private Transform _mainCamControl;
    private Transform mainCamControl {
        get {
            return _mainCamControl == null ? (_mainCamControl = GameObject.Find("CamControl").transform) : _mainCamControl;
        }
    }

    private Transform MaskTop;
    private Transform MaskLeft;
    private Transform MaskRight;
    private Transform MaskHood;
    private Transform MaskRear;

    private Transform CurrentMaskedSide {
        get {
            switch (SelectedSurface) {
                case CarDecalPart.Hood: return MaskHood;
                case CarDecalPart.Rear: return MaskRear;
                case CarDecalPart.Left: return MaskLeft;
                case CarDecalPart.Right: return MaskRight;
                case CarDecalPart.Top: return MaskTop;
                default: return null;
            }
        }
    }

    // Use this for initialization
    void Start () {
        if(EditMode) {
            SelectedDecal = null;
            EditorRenderTexture = new RenderTexture(EditorRenderResolution, EditorRenderResolution, 24);
            EditorRenderTexture.isPowerOfTwo = true;
            EditorRenderTexture.Create();
            CarToManage.renderer.sharedMaterial.mainTexture = EditorRenderTexture;

            camera.targetTexture = EditorRenderTexture;
            RenderTexture.active = EditorRenderTexture;
            camera.Render();
        }
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    private GameObject DecalOnCarPrefab = Resources.Load<GameObject>("DecalOnCar");

    public void AddDecal(CarDecal decal) {
        GameObject t = Instantiate(DecalOnCarPrefab);
        t.transform.SetParent(CurrentMaskedSide, false);
        t.GetComponent<DecalOnCar>().CurrentDecal = decal;
        ActiveCarDecalSelector.AddDecal(decal);
    }

    public Texture2D GenerateCarWrapTexture(int Resolution) {
        RenderTexture RT = new RenderTexture(Resolution, Resolution, 24); // Must have stencil buffer for Decal Masks to work properly
        RT.isPowerOfTwo = true;
        RT.Create();

        if (EditMode) {
            //selectionMarker.GetComponent<Renderer>().enabled = false;
        }

        camera.targetTexture = RT;
        camera.Render();

        RenderTexture.active = RT;
        Texture2D CG = new Texture2D(Resolution, Resolution, TextureFormat.ARGB32, true); // Generate texture, since it'll be a car texture
        CG.ReadPixels(new Rect(0, 0, Resolution, Resolution), 0, 0);
        RenderTexture.active = null;
        camera.targetTexture = null;
        Destroy(RT);

        return CG;
    }
    public void Load(string WrapString) {

    }
    public string Save() {
        return "notyetimplemented";
    }

    static GameObject WrapCreatorPrefab = (GameObject)Resources.Load("WrapCreator", typeof(GameObject));
    public static CarDecalCreator NewWrapCreator() {
        GameObject instance = Instantiate(WrapCreatorPrefab);
        return instance.GetComponent<CarDecalCreator>();
    }

    // IMPORTANT: Make sure the Resolution is a power of 2!!
    public static Texture2D QuickGenerateCarWrap(string WrapString, int Resolution) {
        CarDecalCreator instance = NewWrapCreator();
        instance.Load(WrapString);
        Texture2D tex = instance.GenerateCarWrapTexture(Resolution);
        Destroy(instance);
        return tex;
    }
}
