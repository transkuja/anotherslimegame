using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateFruit
{
    Safe = 0,
    BeginRot = 2,
    MiddleRot,
    EndRot,
    CompletelyRot = 6
};

public class FruitType : MonoBehaviour {

    public Fruit typeFruit;

    public StateFruit state;

    public Material rotFruit;
    public Material matClementine;
    public Material matPomme;
    public Material matKiwi;
    public Material matFraise;

    [SerializeField]
    float timer;

    [SerializeField]
    PoolChild poolchild;

    public void Start()
    {
        poolchild = GetComponent<PoolChild>();
        state = StateFruit.Safe;
    }

    //Les fruits pourrissent en fonction du temps
    //Ils donnent moins de points selon l'état du fruit
    public void Update()
    {
        //timer = poolchild.CurrentTimer;
        if (GetComponent<Collectable>().type != CollectableType.Bonus)
        {
            if (typeFruit == Fruit.Clementine)
            {
                if (poolchild.CurrentTimer > 8.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matClementine, rotFruit, 0.0f);
                    state = StateFruit.Safe;
                }

                if (poolchild.CurrentTimer <= 0.2f)
                {
                    GetComponent<Renderer>().material.Lerp(matClementine, rotFruit, 0.0f);
                    state = StateFruit.Safe;
                }
                else if (poolchild.CurrentTimer < 2.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matClementine, rotFruit, 1.0f);
                    state = StateFruit.CompletelyRot;
                }
                else if (poolchild.CurrentTimer < 3.5f)
                {
                    GetComponent<Renderer>().material.Lerp(matClementine, rotFruit, 0.75f);
                    state = StateFruit.EndRot;
                }
                else if (poolchild.CurrentTimer < 5.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matClementine, rotFruit, 0.50f);
                    state = StateFruit.MiddleRot;
                }
                else if (poolchild.CurrentTimer < 6.5f)
                {
                    GetComponent<Renderer>().material.Lerp(matClementine, rotFruit, 0.25f);
                    state = StateFruit.BeginRot;
                }
            }

            if (typeFruit == Fruit.Pomme)
            {
                if (poolchild.CurrentTimer > 8.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matPomme, rotFruit, 0.0f);
                    state = StateFruit.Safe;
                }

                if (poolchild.CurrentTimer <= 0.2f)
                {
                    GetComponent<Renderer>().material.Lerp(matPomme, rotFruit, 0.0f);
                    state = StateFruit.Safe;
                }
                else if (poolchild.CurrentTimer < 2.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matPomme, rotFruit, 1.0f);
                    state = StateFruit.CompletelyRot;
                }
                else if (poolchild.CurrentTimer < 3.5f)
                {
                    GetComponent<Renderer>().material.Lerp(matPomme, rotFruit, 0.75f);
                    state = StateFruit.EndRot;
                }
                else if (poolchild.CurrentTimer < 5.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matPomme, rotFruit, 0.50f);
                    state = StateFruit.MiddleRot;
                }
                else if (poolchild.CurrentTimer < 6.5f)
                {
                    GetComponent<Renderer>().material.Lerp(matPomme, rotFruit, 0.25f);
                    state = StateFruit.BeginRot;
                }
            }

            if (typeFruit == Fruit.Kiwi)
            {
                if (poolchild.CurrentTimer > 8.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matKiwi, rotFruit, 0.0f);
                    state = StateFruit.Safe;
                }

                if (poolchild.CurrentTimer <= 0.2f)
                {
                    GetComponent<Renderer>().material.Lerp(matKiwi, rotFruit, 0.0f);
                    state = StateFruit.Safe;
                }
                else if (poolchild.CurrentTimer < 2.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matKiwi, rotFruit, 1.0f);
                    state = StateFruit.CompletelyRot;
                }
                else if (poolchild.CurrentTimer < 3.5f)
                {
                    GetComponent<Renderer>().material.Lerp(matKiwi, rotFruit, 0.75f);
                    state = StateFruit.EndRot;
                }
                else if (poolchild.CurrentTimer < 5.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matKiwi, rotFruit, 0.50f);
                    state = StateFruit.MiddleRot;
                }
                else if (poolchild.CurrentTimer < 6.5f)
                {
                    GetComponent<Renderer>().material.Lerp(matKiwi, rotFruit, 0.25f);
                    state = StateFruit.BeginRot;
                }
            }

            if (typeFruit == Fruit.Fraise)
            {
                if (poolchild.CurrentTimer > 8.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matFraise, rotFruit, 0.0f);
                    state = StateFruit.Safe;
                }

                if (poolchild.CurrentTimer <= 0.2f)
                {
                    GetComponent<Renderer>().material.Lerp(matFraise, rotFruit, 0.0f);
                    state = StateFruit.Safe;
                }
                else if (poolchild.CurrentTimer < 2.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matFraise, rotFruit, 1.0f);
                    state = StateFruit.CompletelyRot;
                }
                else if (poolchild.CurrentTimer < 3.5f)
                {
                    GetComponent<Renderer>().material.Lerp(matFraise, rotFruit, 0.75f);
                    state = StateFruit.EndRot;
                }
                else if (poolchild.CurrentTimer < 5.0f)
                {
                    GetComponent<Renderer>().material.Lerp(matFraise, rotFruit, 0.50f);
                    state = StateFruit.MiddleRot;
                }
                else if (poolchild.CurrentTimer < 6.5f)
                {
                    GetComponent<Renderer>().material.Lerp(matFraise, rotFruit, 0.25f);
                    state = StateFruit.BeginRot;
                }
            }
        }
    }
}
