using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseClass;

[ExecuteInEditMode]
public class CreateEnumFromDatabase : MonoBehaviour {

    [SerializeField, HideInInspector]
    public int HideInt = 0;

    [SerializeField, HideInInspector]
    public List<string> enumFromList;

    [SerializeField, HideInInspector]
    public Database db;
}
