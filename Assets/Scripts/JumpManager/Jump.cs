using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/* Inspiré de "Math for Game Programmers: Building a Better Jump"
 *  https://www.youtube.com/watch?v=hG9SzQxaCm8                  */

/*
    Le saut permet de récupérer l'impusion
    et applique une gravité personnalisé.
*/


[System.Serializable]
public class Jump  {

    [SerializeField] private string name;

    public Parabola upParabola= null; // la parabole de base du saut
    [SerializeField] bool hasFallingParabola = false;
    [SerializeField] Parabola fallingParabola = null; // la parabole de descente du saut.

    [SerializeField] bool isContinue;
    [SerializeField] Parabola minJumpParabola = null; // la parabole minimale de saut.

    private Parabola curParabola = null; // parabole en cours -> set la gravité à appliquer.
    private float playerMaxGroundSpeed;
    Rigidbody controllerRb;

    #region Getters
    public bool IsContinue
    {
        get{return isContinue;}
        set {isContinue = value;}
    }
    public bool HasFallingParabola
    {
        get{return hasFallingParabola;}
        set { hasFallingParabola = value; }
    }
    public Parabola FallingParabola
    {
        get{return fallingParabola;}
    }

    public Parabola MinJumpParabola
    {
        get{return minJumpParabola;}
    }

    public string Name
    {
        get
        {
            return name;
        }

    }
    #endregion

    public void InitJump(Rigidbody rb,float _playerMaxGroundSpeed)
    {
        controllerRb = rb;
        playerMaxGroundSpeed = _playerMaxGroundSpeed;
        if (upParabola != null)
        {
            upParabola.ComputeValues(playerMaxGroundSpeed);
            rb.velocity = new Vector3(rb.velocity.x, upParabola.V0, rb.velocity.z);
            rb.useGravity = false;
        }
        curParabola = upParabola;
    }
    public void SwitchMinimalJump()
    {
        if (isContinue)
        {
            //Debug.Log("SwitchMinimalJump");
            curParabola = minJumpParabola;
            minJumpParabola.ComputeValues(playerMaxGroundSpeed);
        }
    }
    public void AtPeakOfJump()
    {
        //Debug.Log("heightAtPeak : " + controllerRb.position.y);
        if (isContinue)
        {
            curParabola = upParabola;
            //Debug.Log("Back to firstParabola");
        }
        if (hasFallingParabola == true)
        {
            //Debug.Log("Parabola Falling");
            curParabola = fallingParabola;
            fallingParabola.ComputeValues(playerMaxGroundSpeed);
        }
    }
    Vector3 lastVelocity;



    public void JumpFixedUpdate()
    {
            // on applique la gravité personnalisé pour saut.
        if (curParabola != null)
        {
            controllerRb.AddForce(curParabola.CurGravity * Vector3.up, ForceMode.Acceleration);
            if (controllerRb.velocity.y <= 0 && lastVelocity.y > 0 )
            {
                AtPeakOfJump();
            }
            lastVelocity = controllerRb.velocity;
        }
    }

}

//[CustomPropertyDrawer(typeof(Jump))]
//public class JumpDrawer : PropertyDrawer
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        // Using BeginProperty / EndProperty on the parent property means that
//        // prefab override logic works on the entire property.
//        EditorGUI.BeginProperty(position, label, property);

//        // Draw label
//        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

//        // Don't make child fields be indented
//        var indent = EditorGUI.indentLevel;
//        EditorGUI.indentLevel = 0;

//        // Calculate rects
//        var amountRect = new Rect(position.x, position.y, 30, position.height);
//        var unitRect = new Rect(position.x + 35, position.y, 50, position.height);
//        var nameRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);

//        // Draw fields - passs GUIContent.none to each so they are drawn without labels
//        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("hasFallingParabola"), GUIContent.none);


//        // Set indent back to what it was
//        EditorGUI.indentLevel = indent;

//        EditorGUI.EndProperty();
//    }
//}