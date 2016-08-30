using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CarDecalCreator : MonoBehaviour {

    public bool EditMode = false;
    [HideInInspector]
    public RenderTexture EditorRenderTexture;
    public int EditorRenderResolution = 1024; // Soon this'll be a property, once I get options implemented
    public CarDecalManager CarToManage;
    public CarDecalPart SelectedSurface = CarDecalPart.Top;

    private CarDecal _selectedDecal;
    private Transform _selectionMarker;
    private Transform selectionMarker {
        get {
            return _selectionMarker == null ? (_selectionMarker = transform.Find("Canvas/SelectionMarker")) : _selectionMarker;
        }
    }
    public CarDecal SelectedDecal {
        get {
            return _selectedDecal;
        }
        set {
            if (value == null) {
                selectionMarker.gameObject.SetActive(false);
                _selectedDecal = null;
            } else {
                _selectedDecal = value;
                selectionMarker.gameObject.SetActive(true);
                selectionMarker.SetParent(_selectedDecal.associatedObject.transform, false);
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
            if (_mainCamControl == null) {
                _mainCamControl = GameObject.Find("CamControl").transform;
                TopView = _mainCamControl.Find("TopView");
                RightView = _mainCamControl.Find("RightView");
                LeftView = _mainCamControl.Find("LeftView");
                HoodView = _mainCamControl.Find("HoodView");
                RearView = _mainCamControl.Find("RearView");
                DefaultView = _mainCamControl.Find("DefaultViewHelper/SecondaryViewHelper/DefaultView");
                FocalPoint = _mainCamControl.Find("FocalPoint");
            }
            return _mainCamControl;
        }
    }

    private Transform TopView;
    private Transform LeftView;
    private Transform RightView;
    private Transform HoodView;
    private Transform RearView;
    private Transform DefaultView;
    private Transform FocalPoint;

    private Transform _currentView;
    private Transform CurrentView {
        get {
            if (mainCamControl == null) ;
            switch (SelectedSurface) {
                case CarDecalPart.Hood: return HoodView;
                case CarDecalPart.Top: return TopView;
                case CarDecalPart.Right: return RightView;
                case CarDecalPart.Left: return LeftView;
                case CarDecalPart.Rear: return RearView;
                default: return DefaultView;
            }
        }
    }

    public Transform MaskTop;
    public Transform MaskLeft;
    public Transform MaskRight;
    public Transform MaskHood;
    public Transform MaskRear;

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

    private bool rotating = false;
    private float baseAngle = 0;

    private bool scaling = false;
    private Vector2 baseScale = Vector2.one;
    private float baseDist = 0;

	// Update is called once per frame
	void Update () {
        //if(Input.GetKeyDown(KeyCode.S)) {
        //    if(SelectedDecal != null) {
        //        SelectedDecal.Scale = Vector2.one * Random.Range(0.2f, 3.0f);
        //        SelectedDecal.Rotation += Random.Range(10f, 60f);
        //    }
        //}
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, CurrentView.position, Time.deltaTime * 5f);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, CurrentView.rotation, Time.deltaTime * 5f);
        if(Input.GetMouseButton(2)) {
            //mainCamControl.Rotate(Vector3.up, Input.GetAxis("Mouse X") * 5);
            if(CurrentView == TopView) {
                CurrentView.Rotate(Vector3.forward, Input.GetAxis("Mouse X") * 5);
            } else if (CurrentView == LeftView || CurrentView == RightView) {
                CurrentView.position += CurrentView.right * Input.GetAxis("Mouse X");
                CurrentView.position += CurrentView.up * Input.GetAxis("Mouse Y");
            } else if (CurrentView == DefaultView) {
                CurrentView.parent.parent.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 5f);
                CurrentView.parent.Rotate(Vector3.right * Input.GetAxis("Mouse Y") * 5f);
                Vector3 bep = CurrentView.parent.localEulerAngles;
                if (bep.x > 80 && bep.x < 180) bep.x = 80;
                if (bep.x < 0 || bep.x > 180) bep.x = 0;
                CurrentView.parent.localEulerAngles = bep;
                //CurrentView.RotateAround(FocalPoint.position, CurrentView.right, Input.GetAxis("Mouse Y") * 5);
            }
        }
        // Scaling
        if (Input.GetKeyDown(KeyCode.S)) {
            scaling = true;
            baseScale = SelectedDecal.Scale;
            baseDist = Vector2.Distance(Camera.main.WorldToScreenPoint(SelectedDecal.roughPosition), Input.mousePosition);
        }
        if (Input.GetKey(KeyCode.S)) {
            SelectedDecal.Scale = baseScale * (Vector2.Distance(Camera.main.WorldToScreenPoint(SelectedDecal.roughPosition), Input.mousePosition) / baseDist);
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            scaling = false;
        }
        // Rotation
        if (Input.GetKeyDown(KeyCode.R)) {
            rotating = true;
            baseAngle = SelectedDecal.Rotation - 270;
        }
        if (Input.GetKey(KeyCode.R) && rotating) {
            float a = ((360 - Vec2ToAngle(Vector2.up, (Camera.main.WorldToScreenPoint(SelectedDecal.roughPosition) - Input.mousePosition).normalized)) + baseAngle) % 360;
            SelectedDecal.Rotation = a;
        }
        if (Input.GetKeyUp(KeyCode.R)) {
            rotating = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectedSurface = CarDecalPart.None;
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectedSurface = CarDecalPart.Hood;
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectedSurface = CarDecalPart.Left;
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectedSurface = CarDecalPart.Right;
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectedSurface = CarDecalPart.Rear;
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectedSurface = CarDecalPart.Top;
    }

    public static float Vec2ToAngle(Vector2 a, Vector2 b) {
        float ang = Vector2.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);
        if (cross.z > 0) ang = 360 - ang;
        return ang;
    }

    void FixedUpdate () {
        if (EditMode) {
            //if (Input.GetMouseButton(0)) {
            //    if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
            //        RaycastHit rayHit;
            //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //        if (Physics.Raycast(ray, out rayHit)) {
            //            if (rayHit.transform == CarToManage.transform) {
            //                //selectionMarker.localPosition = new Vector3((rayHit.textureCoord.x - .5f) * 1024, (rayHit.textureCoord.y - .5f) * 1024, 0);
            //            }
            //        }
            //    }
            //}
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit rayHit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out rayHit)) {
                    CarDecal d = new CarDecal(SelectedDecalTexture.TextureID, new Vector2((rayHit.textureCoord.x - .5f) * 1024, (rayHit.textureCoord.y - .5f) * 1024), SelectedSurface);
                    d.roughPosition = rayHit.point;
                    print("Attempted to add decal");
                    AddDecal(d);
                } else {
                    SelectedDecal = null;
                }
            }
        }
    }

    private GameObject DecalOnCarPrefab = null;

    public void AddDecal(CarDecal decal) {
        if(DecalOnCarPrefab == null) DecalOnCarPrefab = Resources.Load<GameObject>("DecalOnCar");
        GameObject t = Instantiate(DecalOnCarPrefab);
        t.transform.SetParent(CurrentMaskedSide, false);
        decal.associatedObject = t.transform;
        t.GetComponent<DecalOnCar>().CurrentDecal = decal;
        ActiveCarDecalSelector.AddDecal(decal);
        SelectedDecal = decal;
    }

    public Texture2D GenerateCarWrapTexture(int Resolution) {
        RenderTexture RT = new RenderTexture(Resolution, Resolution, 24); // Must have stencil buffer for Decal Masks to work properly
        RT.isPowerOfTwo = true;
        RT.Create();

        if (EditMode) {
            selectionMarker.gameObject.SetActive(false);
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

    static GameObject WrapCreatorPrefab = null;
    public static CarDecalCreator NewWrapCreator() {
        if (WrapCreatorPrefab = null) WrapCreatorPrefab = (GameObject)Resources.Load("WrapCreator", typeof(GameObject));
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
