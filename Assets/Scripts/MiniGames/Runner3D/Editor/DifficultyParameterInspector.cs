using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(DifficultyParameters))]
public class DifficultyParameterInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        Rect rowLabelRect = new Rect(position.x, position.y, 70, 30);
        Rect rowRect = new Rect(position.x+60, position.y, 30, 30);

        Rect columnLabelRect = new Rect(position.x+100, position.y, 70, 30);
        Rect columnRect = new Rect(position.x+170, position.y, 30, 30);

            // Choose Array Size
        GUI.Label(rowLabelRect, "nbPalier");
        EditorGUI.PropertyField(rowRect, property.FindPropertyRelative("nbPalier"),GUIContent.none);

        GUI.Label(columnLabelRect, "nbOutput");
        EditorGUI.PropertyField(columnRect, property.FindPropertyRelative("nbOutput"), GUIContent.none);



        int ySize = property.FindPropertyRelative("nbPalier").intValue;
        int xSize = property.FindPropertyRelative("nbOutput").intValue;
        property.FindPropertyRelative("table").arraySize = xSize * ySize;
        property.FindPropertyRelative("palierTab").arraySize =  ySize;

        position.y += 30;

                // choose pallier values :
        
        int width = 30;
        int height = 30;

        for (int y = 0; y < ySize; y++)
        {
            Rect rect = new Rect(position.x, position.y + y * height, 40, 30);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("palierTab").GetArrayElementAtIndex(y), GUIContent.none);
        }
        position.x += 40;
            // Draw table Values :
       
                // showing tab
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize+2; x++)
            {
                Rect rect = new Rect(position.x + x * width, position.y + y * height, 30, 30);
                if (x == 0)
                {
                    GUI.Label(rect, "P" + y);   
                }
                else if (x == xSize+1)
                {
                    rect.width = 100;
                    if (GUI.Button(rect, "Normalize"))
                    {
                        NormalizeValues(property, y);
                    }
                }
                else
                {
                    int arrayIndex = (x-1) + y * xSize;
                    EditorGUI.PropertyField(rect, property.FindPropertyRelative("table").GetArrayElementAtIndex(arrayIndex), GUIContent.none);
                }
            }
        }
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return property.FindPropertyRelative("nbPalier").intValue*30+30;
    }
    public void NormalizeValues(SerializedProperty property,int row)
    {
        float magnitude = 0;
        for (int i = 0; i < property.FindPropertyRelative("nbOutput").intValue; i++)
        {
            int index = row * property.FindPropertyRelative("nbOutput").intValue + i;
            magnitude += property.FindPropertyRelative("table").GetArrayElementAtIndex(index).floatValue;
        }
        if (magnitude == 0)
        {
            property.FindPropertyRelative("table").GetArrayElementAtIndex(row * property.FindPropertyRelative("nbOutput").intValue).floatValue = 100;
            return;
        }
        if (magnitude == 100)
            return;
        for (int i = 0; i < property.FindPropertyRelative("nbOutput").intValue; i++)
        {
            int index = row * property.FindPropertyRelative("nbOutput").intValue + i;
            float normalizedValue =Mathf.Round( property.FindPropertyRelative("table").GetArrayElementAtIndex(index).floatValue*100 / magnitude);
            property.FindPropertyRelative("table").GetArrayElementAtIndex(index).floatValue = normalizedValue;
        }
    }
    
}
// resizing array : 
//if (property.FindPropertyRelative("table").arraySize < xSize * ySize)
//{
//    for (int i = property.FindPropertyRelative("table").arraySize; i < xSize * ySize;i++)
//    {
//        property.FindPropertyRelative("table").InsertArrayElementAtIndex(i);
//    }
//}
//else if (property.FindPropertyRelative("table").arraySize > xSize * ySize)
//{
//    while (xSize * ySize != property.FindPropertyRelative("table").arraySize)
//    {
//        property.FindPropertyRelative("table").DeleteArrayElementAtIndex(xSize * ySize);
//    }
//}