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
    PoolChild poolchild;

    public float t1 = 4;
    public float t2 = 3;
    public float t3 = 2;
    public float t4 = 1;

    public void Start()
    {
        poolchild = GetComponent<PoolChild>();
        state = StateFruit.Safe;
    }

    //Les fruits pourrissent en fonction du temps
    //Ils donnent moins de points selon l'état du fruit
    public void Update()
    {

        if (GameManager.Instance.CurrentGameMode.minigameVersion == 1)
            return;

        if (GetComponent<Collectable>().type != CollectableType.Bonus)
        {
            if (typeFruit == Fruit.Clementine)
            {
                if (poolchild.CurrentTimer >= 8.0f)
                {
                    GetComponent<Renderer>().material = matClementine;
                    state = StateFruit.Safe;
                }

                else if (poolchild.CurrentTimer < t4)
                {
                    state = StateFruit.CompletelyRot;
                }
                else if (poolchild.CurrentTimer < t3)
                {
                    state = StateFruit.EndRot;
                }
                else if (poolchild.CurrentTimer < t2)
                {
                    state = StateFruit.MiddleRot;
                }
                else if (poolchild.CurrentTimer < t1)
                {
                    state = StateFruit.BeginRot;
                }


                if (poolchild.CurrentTimer <= t1)
                    GetComponent<Renderer>().material.Lerp(matClementine, rotFruit, (t1 - poolchild.CurrentTimer) / (t1));
            }

            if (typeFruit == Fruit.Pomme)
            {
                if (poolchild.CurrentTimer >= 8.0f)
                {
                    GetComponent<Renderer>().material = matPomme;
                    state = StateFruit.Safe;
                }
                else if (poolchild.CurrentTimer < t4)
                {
                    state = StateFruit.CompletelyRot;
                }
                else if (poolchild.CurrentTimer < t3)
                {
                    state = StateFruit.EndRot;
                }
                else if (poolchild.CurrentTimer < t2)
                {
                    state = StateFruit.MiddleRot;
                }
                else if (poolchild.CurrentTimer < t1)
                {
                    state = StateFruit.BeginRot;
                }

                if (poolchild.CurrentTimer <= t1)
                    GetComponent<Renderer>().material.Lerp(matPomme, rotFruit, (t1 - poolchild.CurrentTimer) / (t1));
            }

            if (typeFruit == Fruit.Kiwi)
            {
                if (poolchild.CurrentTimer >= 8.0f)
                {
                    GetComponent<Renderer>().material = matKiwi;
                    state = StateFruit.Safe;
                }
                else if (poolchild.CurrentTimer < t4)
                {
                    state = StateFruit.CompletelyRot;
                }
                else if (poolchild.CurrentTimer < t3)
                {
                    state = StateFruit.EndRot;
                }
                else if (poolchild.CurrentTimer < t2)
                {
                    state = StateFruit.MiddleRot;
                }
                else if (poolchild.CurrentTimer < t1)
                {
                    state = StateFruit.BeginRot;
                }

                if (poolchild.CurrentTimer <= t1)
                    GetComponent<Renderer>().material.Lerp(matKiwi, rotFruit, (t1 - poolchild.CurrentTimer) / (t1));
            }

            if (typeFruit == Fruit.Fraise)
            {
                if (poolchild.CurrentTimer >= 8.0f)
                {
                    GetComponent<Renderer>().material = matFraise;
                    state = StateFruit.Safe;
                }

                if (poolchild.CurrentTimer < t4)
                {
                    state = StateFruit.CompletelyRot;
                }
                else if (poolchild.CurrentTimer < t3)
                {
                    state = StateFruit.EndRot;
                }
                else if (poolchild.CurrentTimer < t2)
                {
                    state = StateFruit.MiddleRot;
                }
                else if (poolchild.CurrentTimer < t1)
                {
                    state = StateFruit.BeginRot;
                }


                if (poolchild.CurrentTimer <= t1)
                    GetComponent<Renderer>().material.Lerp(matFraise, rotFruit, (t1 - poolchild.CurrentTimer) / (t1));
            }

        }

    }
}
