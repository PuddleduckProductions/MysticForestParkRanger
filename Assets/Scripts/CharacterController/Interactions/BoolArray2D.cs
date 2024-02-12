//learned how to do this w/ this:
//https://github.com/Eldoir/Array2DEditor/tree/master

using UnityEngine;

[System.Serializable]
public class BoolArray2D
{
    [HideInInspector]
    public bool[] data;
    [HideInInspector]
    public int width;
    [HideInInspector]
    public int height;
    public bool defaultValue = true; // new field to hold init val

    public BoolArray2D(int width, int height) {
        this.width = width;
        this.height = height;
        data = new bool[width * height];
        Reset(defaultValue); // init all vals to the default
    }

    public void Reset(bool value) {
        for (int i = 0; i < data.Length; i++) {
            data[i] = value;
        }
    }

    public bool this[int x, int y] {
        get { return data[y * width + x]; }
        set { data[y * width + x] = value; }
    }
}
