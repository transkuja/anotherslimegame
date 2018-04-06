using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

using Object = UnityEngine.Object;

namespace QuickUtility
{
    public class QuickUtilityTools : EditorWindow
    {
        /*
            Planned Tools:

            - Mirror movements between 2 objects (if you move the right side object to the right, move the left side object to the left...) on an axis, or on a plane

            - Hide some objects in editor view (Special button for canvases)

            v- Create a new shared parent for the selected objects

            o- Recenter parent from children positions

            v- Select all children

            - Rename children

            v- Select parents

            v- Fix children movement (Allows to move parent without moving children when enabled)

            v- Move objects to the floor level

            v- Deselect all

            v- Clear parent

            - Apply changes to prefab

            - Break prefab instance?

            - Set as first / last sibling (May be useless just use drag and drop)

            v- Replace object with prefab
        */

        Object obj;
        GameObject[] selectedObjects = null;

        string parentName = "NewParent";

        bool errorNoSelection = false;
        bool centerPivotPoint = true;

        enum Tabs
        {
            Prefab,
            Other,
            Help
        }

        string[] tabs = new string[] { "Prefab", "Other", "Help" };
        int selectedTab = 0;

        [MenuItem("Tools/Quick Utility Tools/Open Window", priority = 1)]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            QuickUtilityTools window = (QuickUtilityTools)EditorWindow.GetWindow(typeof(QuickUtilityTools), false, "Quick Utility");
            window.Show();
        }

        void OnSelectionChange()
        {
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            selectedObjects = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                selectedObjects[i] = selection[i] as GameObject;
            }
            Repaint();
        }

        void OnGUI()
        {
            if (selectedObjects == null)
                OnSelectionChange();

            errorNoSelection = false;

            if (selectedObjects == null || selectedObjects.Length < 1)
                errorNoSelection = true;

            selectedTab = GUILayout.Toolbar(selectedTab, tabs);
            switch((Tabs)selectedTab)
            {
                case Tabs.Other:
                    OtherTab();
                    break;
                case Tabs.Prefab:
                    PrefabTab();
                    break;
                case Tabs.Help:
                    ShowHelp();
                    break;
            }
        }

        void OtherTab()
        {
            if (errorNoSelection)
                GUI.enabled = false;

            string currentWindow = focusedWindow.ToString();
            GUILayout.Label("Focused window: " + currentWindow);
            parentName = EditorGUILayout.TextField("New parent name", parentName);

            centerPivotPoint = EditorGUILayout.Toggle("Center parent pivot point", centerPivotPoint);

            if (GUILayout.Button("Create parent"))
            {
                CreateParent();
            }
            obj = EditorGUILayout.ObjectField("Object to extract methods from", obj, typeof(Transform), true);
            // obj = null;
            if (GUILayout.Button("Log methods"))
            {
                Debug.Log(getMethodsOn(obj));
            }
            if (GUILayout.Button("Log members"))
            {
                Debug.Log(getMembersOf(obj));
            }
        }

        void ShowHelp()
        {
            GUILayout.Label("Selection", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Select Object + immediate children : Ctrl + H", MessageType.Info);
            EditorGUILayout.HelpBox("Select Parent : Ctrl + G", MessageType.Info);
            EditorGUILayout.HelpBox("Select Object + all children : Ctrl + L", MessageType.Info);
            EditorGUILayout.HelpBox("Select Top Level Objects : Ctrl + T", MessageType.Info);
            EditorGUILayout.HelpBox("Select Root Object : Ctrl + U", MessageType.Info);
            EditorGUILayout.HelpBox("Select Whole Objects : Ctrl + W", MessageType.Info);
            EditorGUILayout.HelpBox("Deselect All : Ctrl + I", MessageType.Info);
        }


        GameObject basePrefab;
        bool copyScale = false;
        bool overrideChildren = true;

        void PrefabTab()
        {
            GUILayout.Label("Replace with Prefab", EditorStyles.boldLabel);
            basePrefab = EditorGUILayout.ObjectField("Prefab", basePrefab, typeof(GameObject), false) as GameObject;

            copyScale = EditorGUILayout.Toggle("Copy Scale", copyScale);

            overrideChildren = EditorGUILayout.Toggle("Override Children", overrideChildren);

            if (basePrefab == null || errorNoSelection)
                GUI.enabled = false;

            if(GUILayout.Button("Replace Selected objects with prefab"))
            {
                ReplaceObjects();
            }
        }

        void ReplaceObjects()
        {
            Array.Sort(selectedObjects, new SortFromHierarchyDepthComparer());

            for (int i = 0; i < selectedObjects.Length; i++)
            {
                GameObject newObject = PrefabUtility.InstantiatePrefab(basePrefab) as GameObject;
                Undo.RegisterCreatedObjectUndo(newObject, "NewPrefab" + i);
                newObject.transform.SetParent(selectedObjects[i].transform.parent);
                newObject.transform.SetSiblingIndex(selectedObjects[i].transform.GetSiblingIndex());
                newObject.transform.position = selectedObjects[i].transform.position;
                newObject.transform.rotation = selectedObjects[i].transform.rotation;
                if(copyScale)
                    newObject.transform.localScale = selectedObjects[i].transform.localScale;

                if(!overrideChildren)
                    while (selectedObjects[i].transform.childCount > 0)
                    {
                        Undo.SetTransformParent(selectedObjects[i].transform.GetChild(0), newObject.transform, "NewPrefabSetParent"+i);
                    }
                Undo.DestroyObjectImmediate(selectedObjects[i]);
                selectedObjects[i] = newObject;
            }
            Selection.objects = selectedObjects;
        }

        #region SelectionHotkeyMenuItems
        [MenuItem("Tools/Quick Utility Tools/Selection/Select only TopLevel %t")]
        static void SelectOnlyTopLevelItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)" && EditorWindow.focusedWindow.ToString() != " (QuickUtility.QuickUtilityTools)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel | SelectionMode.ExcludePrefab);
            GameObject[] goSelect = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                goSelect[i] = selection[i] as GameObject;
            }

            Selection.objects = goSelect;
        }

        [MenuItem("Tools/Quick Utility Tools/Selection/Select Roots %u")]
        static void SelectRootsItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel | SelectionMode.ExcludePrefab);
            GameObject[] goSelect = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                goSelect[i] = ((GameObject)selection[i]).transform.root.gameObject;
            }

            Selection.objects = goSelect;
        }

        [MenuItem("Tools/Quick Utility Tools/Selection/Select All Children Only %h")]
        static void SelectChildrenOnlyItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < selection.Length; i++)
            {
                Transform childTransform = ((GameObject)selection[i]).transform;
                for (int j = 0; j < childTransform.transform.childCount; j++)
                {
                    goList.Add(childTransform.GetChild(j).gameObject);
                }
            }
            Selection.objects = goList.ToArray();
        }
        [MenuItem("Tools/Quick Utility Tools/Selection/Select Object and Children %l")]
        static void SelectChildrenItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < selection.Length; i++)
            {
                Transform childTransform = ((GameObject)selection[i]).transform;
                goList.Add(childTransform.gameObject);
                for (int j = 0; j < childTransform.transform.childCount; j++)
                {
                    goList.Add(childTransform.GetChild(j).gameObject);
                }
            }
            Selection.objects = goList.ToArray();
        }
        [MenuItem("Tools/Quick Utility Tools/Selection/Select Parents %g")]
        static void SelectParentsItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < selection.Length; i++)
            {
                if (((GameObject)selection[i]).transform.parent == null)
                    continue;
                GameObject toAdd = ((GameObject)selection[i]).transform.parent.gameObject;
                if (!goList.Contains(toAdd))
                    goList.Add(toAdd);
            }
            Selection.objects = goList.ToArray();
        }

        [MenuItem("Tools/Quick Utility Tools/Selection/Select Whole Object %w", false, -10)]
        static void SelectWholeContextItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)" && EditorWindow.focusedWindow.ToString() != " (QuickUtility.QuickUtilityTools)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel | SelectionMode.ExcludePrefab);
            GameObject[] goSelect = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                goSelect[i] = selection[i] as GameObject;
            }
            List<GameObject> rootObjects = new List<GameObject>();

            for (int i = 0; i < goSelect.Length; i++)
            {
                if (!rootObjects.Contains(goSelect[i].transform.root.gameObject))
                    rootObjects.Add(goSelect[i].transform.root.gameObject);
            }

            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < rootObjects.Count; i++)
            {
                goList.AddRange(GetGameObjectWithAllChildren(rootObjects[i]));
            }
            Selection.objects = goList.ToArray();
        }

        [MenuItem("Tools/Quick Utility Tools/Selection/Deselect All %i")]
        static void DeselectAll()
        {
            Selection.objects = new Object[0];
        }
        #endregion

        #region SelectionContextMenu
        [MenuItem("GameObject/Selections/Select Whole Object", false)]
        static void SelectWholeItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)" && EditorWindow.focusedWindow.ToString() != " (QuickUtility.QuickUtilityTools)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel | SelectionMode.ExcludePrefab);
            GameObject[] goSelect = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                goSelect[i] = selection[i] as GameObject;
            }
            List<GameObject> rootObjects = new List<GameObject>();

            for (int i = 0; i < goSelect.Length; i++)
            {
                if (!rootObjects.Contains(goSelect[i].transform.root.gameObject))
                    rootObjects.Add(goSelect[i].transform.root.gameObject);
            }

            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < rootObjects.Count; i++)
            {
                goList.AddRange(GetGameObjectWithAllChildren(rootObjects[i]));
            }
            Selection.objects = goList.ToArray();
        }

        [MenuItem("GameObject/Selections/Select Object With All Children", false, -10)]
        static void SelectChildrenAndObjectContextItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)" && EditorWindow.focusedWindow.ToString() != " (QuickUtility.QuickUtilityTools)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel | SelectionMode.ExcludePrefab);
            GameObject[] goSelect = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                goSelect[i] = selection[i] as GameObject;
            }
            List<GameObject> goList = new List<GameObject>();
            for(int i = 0; i < goSelect.Length; i++)
            {
                goList.AddRange(GetGameObjectWithAllChildren(goSelect[i]));
            }
            Selection.objects = goList.ToArray();
        }

        [MenuItem("GameObject/Selections/Select All Children Only", false, -10)]
        static void SelectAllChildrenContextItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel | SelectionMode.ExcludePrefab);
            GameObject[] goSelect = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                goSelect[i] = ((GameObject)selection[i]).transform.root.gameObject;
            }
            List<GameObject> goList = new List<GameObject>();

            for (int i = 0; i < goSelect.Length; i++)
            {
                List<GameObject> gl = new List<GameObject>();
                gl.AddRange(GetGameObjectWithAllChildren(goSelect[i]));
                gl.RemoveAt(0);
                goList.AddRange(gl);
            }

            Selection.objects = goList.ToArray();
        }

        [MenuItem("GameObject/Selections/Select Direct Children Only", false, -10)]
        static void SelectDirectChildrenContextItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < selection.Length; i++)
            {
                Transform childTransform = ((GameObject)selection[i]).transform;
                for (int j = 0; j < childTransform.transform.childCount; j++)
                {
                    goList.Add(childTransform.GetChild(j).gameObject);
                }
            }
            Selection.objects = goList.ToArray();
        }
        [MenuItem("GameObject/Selections/Select Object and Direct Children", false, -10)]
        static void SelectObjectAndChildrenContextItem()
        {
            if (EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneHierarchyWindow)" && EditorWindow.focusedWindow.ToString() != " (UnityEditor.SceneView)") //The space before the name is needed
                return;
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < selection.Length; i++)
            {
                Transform childTransform = ((GameObject)selection[i]).transform;
                goList.Add(childTransform.gameObject);
                for (int j = 0; j < childTransform.transform.childCount; j++)
                {
                    goList.Add(childTransform.GetChild(j).gameObject);
                }
            }
            Selection.objects = goList.ToArray();
        }
        [MenuItem("GameObject/Selections/Select Parent", false, -10)]
        static void SelectParentsContextItem()
        {
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            List<GameObject> goList = new List<GameObject>();
            for (int i = 0; i < selection.Length; i++)
            {
                if (((GameObject)selection[i]).transform.parent == null)
                    continue;
                GameObject toAdd = ((GameObject)selection[i]).transform.parent.gameObject;
                if (!goList.Contains(toAdd))
                    goList.Add(toAdd);
            }
            Selection.objects = goList.ToArray();
        }

        [MenuItem("GameObject/Selections/Deselect All", false, -10)]
        static void DeselectAllContext()
        {
            Selection.objects = new Object[0];
        }
        #endregion

        #region ParentOperations
        // ---------- Create parent ----------
        [MenuItem("GameObject/ Create Parent", true)]
        static bool ValidateCreateParentItem()
        {
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
                return false;
            return true;
        }
        [MenuItem("GameObject/ Create Parent", false, 0)]
        static void CreateParentItem(MenuCommand menuCommand)
        {
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
                return;
            GameObject[] selectedObjects = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                selectedObjects[i] = selection[i] as GameObject;
            }

            // Only execute once, not for each object
            if (selectedObjects.Length > 1)
            {
                if (((GameObject)menuCommand.context) != selectedObjects[0])
                {
                    return;
                }
            }

            GameObject parent = new GameObject("Parent");

            Undo.RegisterCreatedObjectUndo(parent, "CreatedParent");
            GameObject closestToRoot = FindClosestToRootSelectedGameObjectStatic(selectedObjects);
            parent.transform.SetParent(closestToRoot.transform.parent);
            parent.transform.position = selectedObjects[0].transform.position;
            parent.transform.SetSiblingIndex(closestToRoot.transform.GetSiblingIndex());
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                Undo.SetTransformParent(selectedObjects[i].transform, parent.transform, "SetParent" + i);
            }
            Selection.activeObject = parent;

            EditorApplication.ExecuteMenuItem("GameObject/ Center On.../All Children");

            Selection.objects = selection;
        }
        // -----------------------------------

        // ---------- Clear parent ----------
        [MenuItem("GameObject/ Clear Parent", true)]
        static bool ValidateClearParentItem()
        {
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
                return false;
            return true;
        }

        [MenuItem("GameObject/ Clear Parent", false, 0)]
        static void ClearParentItem()
        {
            EditorApplication.ExecuteMenuItem("GameObject/Clear Parent");
        }
        // ----------------------------------

        [MenuItem("GameObject/ Center On.../Immediate Children", true)]
        static bool ValidateCenterOnChildrenItem()
        {
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
                return false;
            for(int i = 0; i < selection.Length; i++)
            {
                if (((GameObject)selection[i]).transform.childCount > 0)
                    return true;
            }
            return false;
        }
        [MenuItem("GameObject/ Center On.../Immediate Children", false, 0)]
        static void CenterOnChildrenItem()
        {
            EditorApplication.ExecuteMenuItem("GameObject/Center On Children");
        }

        [MenuItem("GameObject/ Center On.../All Children", true)]
        static bool ValidateCenterOnAllChildrenItem()
        {
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
                return false;
            for (int i = 0; i < selection.Length; i++)
            {
                if (((GameObject)selection[i]).transform.childCount > 0)
                    return true;
            }
            return false;
        }
        [MenuItem("GameObject/ Center On.../All Children", false, 0)]
        static void CenterOnAllChildrenItem()
        {
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab | SelectionMode.TopLevel);
            if (selection == null || selection.Length == 0)
                return;
            for (int i = 0; i < selection.Length; i++)
            {
                GameObject curGo = selection[i] as GameObject;
                if (curGo.transform.childCount == 0)
                    continue;
                Undo.RegisterCompleteObjectUndo(curGo.transform, "CenterOnChildren"+i);
                Undo.FlushUndoRecordObjects();
                GameObject go = new GameObject("Temp");
                go.transform.position = curGo.transform.position;
                go.transform.rotation = curGo.transform.rotation;
                go.transform.SetParent(curGo.transform.parent);
                go.transform.localScale = curGo.transform.localScale;

                while (curGo.transform.childCount > 0)
                {
                    curGo.transform.GetChild(0).SetParent(go.transform);
                }

                curGo.transform.position = FindCenterOfChildren(go);

                while (go.transform.childCount > 0)
                {
                    go.transform.GetChild(0).SetParent(curGo.transform);
                }

                DestroyImmediate(go);
            }
            
        }

        void CreateParent()
        {
            GameObject parent = new GameObject(parentName);
           

            Undo.RegisterCreatedObjectUndo(parent, "CreatedParent");
            GameObject closestToRoot = FindClosestToRootSelectedGameObject();
            parent.transform.SetParent(closestToRoot.transform.parent);
            parent.transform.SetSiblingIndex(closestToRoot.transform.GetSiblingIndex());
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                Undo.SetTransformParent(selectedObjects[i].transform, parent.transform, "SetParent" + i);
            }

            Selection.activeObject = parent;

            if (centerPivotPoint)
            {
                Vector3 pos = FindCenterOfChildren(parent);
                GameObject temp = new GameObject("temp");
                foreach(Transform child in parent.transform)
                {
                    child.SetParent(temp.transform);
                }
                parent.transform.position = pos;
                foreach (Transform child in temp.transform)
                {
                    child.SetParent(parent.transform);
                }
                DestroyImmediate(temp);
            }
                
            //if (centerPivotPoint)
            //    CenterOnChildrenItem();
        }
        #endregion

        #region ContextOperations
        [MenuItem("GameObject/ Move To Floor Level", true)]
        static bool ValidateMoveToFloorItem()
        {
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
                return false;
            //for(int i = 0; i < selection.Length; i++)
            //{
            //    if(!((GameObject)selection[i]).GetComponent<Collider>())
            //        return false;
            //}
            return true;
        }
        [MenuItem("GameObject/ Move To Floor Level", false, 0)]
        static void MoveToFloorItem()
        {
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            if (selection == null || selection.Length == 0)
                return;
            GameObject[] selectedObjects = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                selectedObjects[i] = selection[i] as GameObject;
            }

            Array.Sort(selectedObjects, new SortFromHierarchyDepthComparer());

            GameObject firstToMove = null;
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                if (!selectedObjects[i].GetComponent<Collider>())
                    continue;
                if (!firstToMove)
                    firstToMove = selectedObjects[i];
                RaycastHit hit;
                float heightFactor = 0.0f; // if we're below the floor level, this factor will be higher to compensate the difference
                Vector3 position = selectedObjects[i].transform.position;
                if (Physics.Raycast(position, Vector3.down, out hit))
                    heightFactor = 1.0f;
                else if (Physics.Raycast(position, Vector3.up, out hit))
                    heightFactor = 2.0f;
                else if (Physics.Raycast(new Vector3(position.x, firstToMove.transform.position.y, position.z), Vector3.down, out hit)) // Check from the first object that has moved height
                    heightFactor = 2.0f;

                if (hit.collider != null)
                {
                    Undo.RecordObject(selectedObjects[i].transform, "MoveToFloor" + i);
                    Debug.Log(hit.collider.name);
                    selectedObjects[i].transform.position = new Vector3(selectedObjects[i].transform.position.x, (hit.point.y + selectedObjects[i].GetComponent<Collider>().bounds.extents.y * heightFactor), selectedObjects[i].transform.position.z);
                }
            }
        }

        class SortFromHierarchyDepthComparer : IComparer<GameObject>
        {
            public int Compare(GameObject x, GameObject y)
            {
                int xDepth = GetDepthInHierarchy(x);
                int yDepth = GetDepthInHierarchy(y);
                if (xDepth > yDepth)
                    return 1;
                else if (yDepth > xDepth)
                    return -1;
                else return 0;
            }
        }

        #endregion

        #region NonStaticUtilities
        Vector3 FindSelectionCenter()
        {
            Vector3 center = Vector3.zero;
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Deep | SelectionMode.ExcludePrefab);
            GameObject[] goSelection = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                goSelection[i] = selection[i] as GameObject;
            }


            for (int i = 0; i < goSelection.Length; i++)
            {
                center += goSelection[i].transform.position;
            }
            center /= goSelection.Length;

            return center;
        }

        GameObject FindClosestToRootSelectedGameObject()
        {
            if (selectedObjects == null || selectedObjects.Length == 0)
                return null;
            int smallestDistanceFromRoot = 10000;
            GameObject closestObjectFromRoot = null;
            GameObject currentObject = null;
            int currentDistance = 0;
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                currentObject = selectedObjects[i];
                currentDistance = 0;
                while (currentObject.transform.parent != null)
                {
                    currentDistance++;
                    currentObject = currentObject.transform.parent.gameObject;
                }
                if (currentDistance < smallestDistanceFromRoot)
                {
                    smallestDistanceFromRoot = currentDistance;
                    closestObjectFromRoot = selectedObjects[i];
                }
            }

            return closestObjectFromRoot;
        }
        #endregion

        #region StaticUtilities

        static GameObject FindClosestToRootSelectedGameObjectStatic(GameObject[] selectedObjects)
        {
            Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab);
            selectedObjects = new GameObject[selection.Length];
            for (int i = 0; i < selection.Length; i++)
            {
                selectedObjects[i] = selection[i] as GameObject;
            }

            int smallestDistanceFromRoot = 10000;
            GameObject closestObjectFromRoot = null;
            GameObject currentObject = null;
            int currentDistance = 0;
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                currentObject = selectedObjects[i];
                currentDistance = 0;
                while (currentObject.transform.parent != null)
                {
                    currentDistance++;
                    currentObject = currentObject.transform.parent.gameObject;
                }
                if (currentDistance < smallestDistanceFromRoot)
                {
                    smallestDistanceFromRoot = currentDistance;
                    closestObjectFromRoot = selectedObjects[i];
                }
            }

            return closestObjectFromRoot;
        }

        public static Vector3 FindCenterOfChildren(GameObject go)
        {
            Vector3 toReturn = Vector3.zero;

            GameObject[] children = GetAllChildren(go);

            for(int i = 0; i < children.Length; i++)
            {
                toReturn += children[i].transform.position;
            }

            return toReturn/(children.Length);
        }

        public static int GetDepthInHierarchy(GameObject go)
        {
            int i = 0;
            GameObject cursor = go;
            while (cursor.transform.parent != null)
            {
                cursor = cursor.transform.parent.gameObject;
                i++;
            }
            return i;
        }

        public static GameObject[] GetGameObjectWithAllChildren(GameObject go)
        {
            List<GameObject> list = new List<GameObject>();
            list.Add(go.gameObject);
            for(int i = 0; i < go.transform.childCount; i++)
            {
                list.AddRange(GetGameObjectWithAllChildren(go.transform.GetChild(i).gameObject));
            }
            return list.ToArray();
        }

        public static GameObject[] GetAllChildren(GameObject go)
        {
            List<GameObject> list = new List<GameObject>();

            list.AddRange(GetGameObjectWithAllChildren(go));
            list.RemoveAt(0);

            return list.ToArray();
        }

        public static string getMethodsOn(System.Object t)
        {
            Type obj = t.GetType();
            string log = "METHODS FOR : " + obj.Name;
            MethodInfo[] method_info = obj.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo method in method_info)
            {
                string parameters = "";

                ParameterInfo[] param_info = method.GetParameters();
                if (0 < param_info.Length)
                {
                    for (int i = 0; i < param_info.Length; i++)
                    {
                        parameters += param_info[i].ParameterType.Name;
                        parameters += (i < (param_info.Length - 1)) ? ", " : "";
                    }
                }
                log += "\nFunction :" + method.Name + "(" + parameters + ")";
            }
            return log;
        }

        public static string getMembersOf(System.Object t)
        {
            Type obj = t.GetType();
            string log = "MEMBERS OF : " + obj.Name;
            MemberInfo[] member_info = obj.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MemberInfo member in member_info)
            {
                log += "\nMember :" + member.Name;
            }
            return log;
        }
#endregion
    }
}
