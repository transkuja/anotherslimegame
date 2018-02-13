using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Reverse engineered UnityEditor.TransformInspector from Unity3D wiki

[CanEditMultipleObjects, CustomEditor(typeof(Transform))]
public class TransformInspector : Editor
{

    //private const float FIELD_WIDTH = 212.0f;
    private const bool WIDE_MODE = true;

    private const float POSITION_MAX = 100000.0f;

    private static GUIContent positionGUIContent = new GUIContent(LocalString("Position")
                                                                 , LocalString("The local position of this Game Object relative to the parent."));
    private static GUIContent rotationGUIContent = new GUIContent(LocalString("Rotation")
                                                                 , LocalString("The local rotation of this Game Object relative to the parent."));
    private static GUIContent scaleGUIContent = new GUIContent(LocalString("Scale")
                                                                 , LocalString("The local scaling of this Game Object relative to the parent."));

    private static string positionWarningText = LocalString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");

    private SerializedProperty positionProperty;
    private SerializedProperty rotationProperty;
    private SerializedProperty scaleProperty;

    Transform transform;
    bool freezeChildren = false;
    public bool FreezeChildren
    {
        get
        {
            return freezeChildren;
        }
        set
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (value)
                {
                    Undo.RegisterCompleteObjectUndo(child.transform, "FreezeChild");
                    if (!child.GetComponent<TransformLocker>())
                        child.AddComponent<TransformLocker>();
                }
                else
                {
                    if (child.GetComponent<TransformLocker>())
                    {
                        DestroyImmediate(child.GetComponent<TransformLocker>()); //Problem with undo
                    }
                        
                }
            }
            Undo.FlushUndoRecordObjects();
            freezeChildren = value;
        }
    }

    private static string LocalString(string text)
    {
        return LocalizationDatabase.GetLocalizedString(text);
    }

    public void OnEnable()
    {
        this.positionProperty = this.serializedObject.FindProperty("m_LocalPosition");
        this.rotationProperty = this.serializedObject.FindProperty("m_LocalRotation");
        this.scaleProperty = this.serializedObject.FindProperty("m_LocalScale");
        transform = (Transform)serializedObject.targetObject;
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.wideMode = TransformInspector.WIDE_MODE;
        EditorGUIUtility.labelWidth = 4 *EditorGUIUtility.currentViewWidth/10.0f; // align field to right of inspector
        EditorGUIUtility.fieldWidth =  6 * EditorGUIUtility.currentViewWidth / 10.0f;
        this.serializedObject.Update();

        EditorGUILayout.PropertyField(this.positionProperty, positionGUIContent);
        this.RotationPropertyField(this.rotationProperty, rotationGUIContent);
        EditorGUILayout.PropertyField(this.scaleProperty, scaleGUIContent);

        if (!ValidatePosition(((Transform)this.target).position))
        {
            EditorGUILayout.HelpBox(positionWarningText, MessageType.Warning);
        }
        if(transform.childCount > 0 && !serializedObject.isEditingMultipleObjects)
            FreezeChildren = EditorGUILayout.Toggle("Freeze Children", FreezeChildren);
        this.serializedObject.ApplyModifiedProperties();
    }

    private void OnDisable()
    {
        // Disable FreezeChildren on Deselection to prevent confusion if forgotten
        if(FreezeChildren)
            FreezeChildren = false;
    }

    private bool ValidatePosition(Vector3 position)
    {
        if (Mathf.Abs(position.x) > TransformInspector.POSITION_MAX) return false;
        if (Mathf.Abs(position.y) > TransformInspector.POSITION_MAX) return false;
        if (Mathf.Abs(position.z) > TransformInspector.POSITION_MAX) return false;
        return true;
    }

    private void RotationPropertyField(SerializedProperty rotationProperty, GUIContent content)
    {
        Transform transform = (Transform)this.targets[0];
        Quaternion localRotation = transform.localRotation;
        foreach (UnityEngine.Object t in (UnityEngine.Object[])this.targets)
        {
            if (!SameRotation(localRotation, ((Transform)t).localRotation))
            {
                EditorGUI.showMixedValue = true;
                break;
            }
        }

        EditorGUI.BeginChangeCheck();

        Vector3 eulerAngles = EditorGUILayout.Vector3Field(content, localRotation.eulerAngles);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(this.targets, "Rotation Changed");
            foreach (UnityEngine.Object obj in this.targets)
            {
                Transform t = (Transform)obj;
                t.localEulerAngles = eulerAngles;
            }
            rotationProperty.serializedObject.SetIsDifferentCacheDirty();
        }

        EditorGUI.showMixedValue = false;
    }

    private bool SameRotation(Quaternion rot1, Quaternion rot2)
    {
        if (rot1.x != rot2.x) return false;
        if (rot1.y != rot2.y) return false;
        if (rot1.z != rot2.z) return false;
        if (rot1.w != rot2.w) return false;
        return true;
    }
}
