using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class CarWrap {
    public string WrapName;
    public List<CarDecal> Decals;
}

[Serializable]
public class CarDecal {
    public string Texture;
    public Vector2 Position;
    public Vector2 Scale;
    public float Rotation;
    public CarDecalPart Part;
}

public enum CarDecalPart {
    Hood = 0, Rear, Top, Left, Right
}