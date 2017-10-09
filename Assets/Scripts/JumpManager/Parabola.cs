using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Inspiré de "Math for Game Programmers: Building a Better Jump"
 *  https://www.youtube.com/watch?v=hG9SzQxaCm8                  */

/* 
    Permet de créer une parabole paramétrable :  
    --> Calcule la force d'impulsion à appliquer pour sauter.
 */
[System.Serializable]
public class Parabola {

    [SerializeField] private float height; // height of jump (used to model jump)
    private float xz_speed; // max speed on ground (defined by player characteristics)
    [SerializeField] private float xz_h;// ground distance to peak of jump (used to model jump)
    private float curGravity;
    private float v0;
    public float V0 { get { return v0; } }
    public float CurGravity { get { return curGravity; } }

    public float Xz_h
    {   get{return xz_h;}}

    public Parabola()
    {
        height = 5;
        xz_speed = 1;
        xz_h = 5;
        ComputeValues(10);
    }
        // Enregistre la speed maximale du joueur, la force à appliquer pour sauter.
    public void ComputeValues(float maxSpeed)
    {
        xz_speed = maxSpeed;
        v0 = (2 * height * xz_speed) / xz_h;
        curGravity = (-2 * height * xz_speed * xz_speed) / (xz_h * xz_h);
    }
   
    public float getPosition(float t)
    {
        return 0.5f * curGravity * t * t + v0*t;
    }
    public float getT_h() // time to reach peak of jump
    {
        return xz_h / xz_speed;
    }

    /*
     * maths : 
     * t_h : le temps au pic du jump
     * xz_h : la distance pour atteindre le pic du jump
     * h = la hauteur du jump

     * v0 = 2h/ t_h
     * g = -2h/t_h*t_h
     * 

     * 
     * t_h = xzh / XZSpeed
     * v0 = 2*h*XZSpeed / xzh 
     * g = -2h * XZSpeed * XZSpeed /xz_h*xz_h
     * */
}
