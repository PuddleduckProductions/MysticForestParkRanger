//learned how to do this w/ this:
//https://github.com/Eldoir/Array2DEditor/tree/master

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(test))]
public class BoolArray2DDrawer : Editor
{
    SerializedProperty shapeProperty;

    void OnEnable() {
        shapeProperty = serializedObject.FindProperty("shape");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        SerializedProperty widthProp = shapeProperty.FindPropertyRelative("width");
        SerializedProperty heightProp = shapeProperty.FindPropertyRelative("height");
        SerializedProperty defaultValueProp = shapeProperty.FindPropertyRelative("defaultValue");

        EditorGUILayout.PropertyField(defaultValueProp, new GUIContent("Default Value"));

        // width buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("-", GUILayout.Width(30))) {
            widthProp.intValue = Mathf.Max(0, widthProp.intValue - 1);
            ResizeDataArrayIfNeeded(shapeProperty, defaultValueProp.boolValue, isWidthChanged: true);
        }
        EditorGUILayout.PropertyField(widthProp);
        if (GUILayout.Button("+", GUILayout.Width(30))) {
            widthProp.intValue += 1;
            ResizeDataArrayIfNeeded(shapeProperty, defaultValueProp.boolValue, isWidthChanged: true);
        }
        EditorGUILayout.EndHorizontal();

        // height buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("-", GUILayout.Width(30))) {
            heightProp.intValue = Mathf.Max(0, heightProp.intValue - 1);
            ResizeDataArrayIfNeeded(shapeProperty, defaultValueProp.boolValue, isWidthChanged: false);
        }
        EditorGUILayout.PropertyField(heightProp);
        if (GUILayout.Button("+", GUILayout.Width(30))) {
            heightProp.intValue += 1;
            ResizeDataArrayIfNeeded(shapeProperty, defaultValueProp.boolValue, isWidthChanged: false);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Grid to Default Value")) {
            ResetGridToDefaultValue(shapeProperty, defaultValueProp.boolValue);
        }

        if (EditorGUI.EndChangeCheck()) {
            //ResizeDataArrayIfNeeded(shapeProperty, defaultValueProp.boolValue);
        }

        SerializedProperty dataProp = shapeProperty.FindPropertyRelative("data");
        DisplayGrid(widthProp.intValue, heightProp.intValue, dataProp);

        serializedObject.ApplyModifiedProperties();
    }

    private void ResetGridToDefaultValue(SerializedProperty shapeProperty, bool defaultValue) {
        SerializedProperty dataProp = shapeProperty.FindPropertyRelative("data");
        for (int i = 0; i < dataProp.arraySize; i++) {
            dataProp.GetArrayElementAtIndex(i).boolValue = defaultValue;
        }
    }

    private void AdjustWidth(SerializedProperty shapeProperty, bool defaultValue, int newWidth, int newHeight) {
        SerializedProperty dataProp = shapeProperty.FindPropertyRelative("data");
        int oldTotalCells = dataProp.arraySize;
        int oldWidth = oldTotalCells / newHeight; // calcs based on newHeight
        int newSize = newWidth * newHeight;

        bool[] tempData = new bool[newSize];
        for (int i = 0; i < newSize; i++) {
            tempData[i] = defaultValue; // inits w/ default value.
        }

        // copies existing vals into new array, adjsuting for new width.
        for (int y = 0; y < newHeight; y++) {
            for (int x = 0; x < oldWidth && x < newWidth; x++) { // ensures we don't exceed old or new width
                if (y * oldWidth + x < oldTotalCells) { // checks if w/in old total cells
                    tempData[y * newWidth + x] = dataProp.GetArrayElementAtIndex(y * oldWidth + x).boolValue;
                }
            }
        }

        UpdateSerializedProperty(dataProp, tempData);
    }

    private void AdjustHeight(SerializedProperty shapeProperty, bool defaultValue, int newWidth, int newHeight) {
        SerializedProperty dataProp = shapeProperty.FindPropertyRelative("data");
        int oldWidth = newWidth; // assumes width remains constant when adjusting height
        int oldHeight = dataProp.arraySize / oldWidth; // calcs old height based on constant width
        int newSize = newWidth * newHeight;

        bool[] tempData = new bool[newSize];
        for (int i = 0; i < newSize; i++) {
            tempData[i] = defaultValue; // inits w/ default val
        }

        // copies existing vals, adjusting for the new height.
        for (int y = 0; y < oldHeight && y < newHeight; y++) { // ensures we don't exceed old or new height
            for (int x = 0; x < newWidth; x++) { // loops thru each column
                if (y * oldWidth + x < dataProp.arraySize) { // checks if w/in old total cells
                    tempData[y * newWidth + x] = dataProp.GetArrayElementAtIndex(y * oldWidth + x).boolValue;
                }
            }
        }

        UpdateSerializedProperty(dataProp, tempData);
    }


    private void UpdateSerializedProperty(SerializedProperty dataProp, bool[] tempData) {
        dataProp.arraySize = tempData.Length;
        for (int i = 0; i < tempData.Length; i++) {
            dataProp.GetArrayElementAtIndex(i).boolValue = tempData[i];
        }
    }

    private void ResizeDataArrayIfNeeded(SerializedProperty shapeProperty, bool defaultValue, bool isWidthChanged) {
        if(isWidthChanged) return;

        if (isWidthChanged) {
            AdjustWidth(shapeProperty, defaultValue, shapeProperty.FindPropertyRelative("width").intValue, shapeProperty.FindPropertyRelative("height").intValue);
        }
        else {
            AdjustHeight(shapeProperty, defaultValue, shapeProperty.FindPropertyRelative("width").intValue, shapeProperty.FindPropertyRelative("height").intValue);
        }
    }



    private void DisplayGrid(int width, int height, SerializedProperty dataProp) {
        EditorGUILayout.LabelField("BoolArray2D Grid");
        for (int y = 0; y < height; y++) {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < width; x++) {
                int index = y * width + x;
                if (index < dataProp.arraySize) {
                    SerializedProperty cellProp = dataProp.GetArrayElementAtIndex(index);
                    bool newValue = EditorGUILayout.Toggle(cellProp.boolValue, GUILayout.Width(25));
                    cellProp.boolValue = newValue;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
