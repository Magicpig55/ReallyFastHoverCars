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
    public int TextureID; // I'd really hate to use an ID system, but it seems to be the only way at the moment
    public Vector2 Position;
    public Vector2 Scale;
    public float Rotation;
    public Color Color;
    public CarDecalPart Part;

    public CarDecal(int texID, Vector2 pos, Vector2 scale, float rot, Color col, CarDecalPart part) {
        TextureID = texID;
        Position = pos;
        Scale = scale;
        Rotation = rot;
        Color = col;
        Part = part;
    }
    // Usefull when creating a decal at a user designated point
    public CarDecal(int texID, Vector2 pos, CarDecalPart part) : this(texID, pos, Vector2.one, 0f, Color.white, part) { }

    [NonSerialized]
    public Transform associatedObject;
    [NonSerialized]
    public Vector2 roughPosition;

    public bool CanMirror {
        get {
            return Part == CarDecalPart.Left || Part == CarDecalPart.Right;
        }
    }
    public CarDecal MirrorCounterpart {
        get {
            if (CanMirror) {
                return new CarDecal(TextureID, Position, Scale, Rotation, Color, Part);
            } else {
                return null;
            }
        }
    }
}

public enum CarDecalPart {
    None = 0, Hood, Rear, Top, Left, Right
}