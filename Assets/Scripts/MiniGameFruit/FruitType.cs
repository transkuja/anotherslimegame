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

    //Les fruits pourrissent au fur du temps
    //Ils donnent moins de points selon l'état du fruit
    public IEnumerator fruitsStartToRot(GameObject fruit)
    {
        fruit.GetComponent<Renderer>().material.Lerp(rotFruit, fruit.GetComponent<Renderer>().material, 1.0f);

        yield return new WaitForSeconds(4.5f);
        fruit.GetComponent<Renderer>().material.Lerp(fruit.GetComponent<Renderer>().material, rotFruit, 0.25f);
        fruit.GetComponent<FruitType>().state = StateFruit.BeginRot;

        yield return new WaitForSeconds(1.5f);
        fruit.GetComponent<Renderer>().material.Lerp(fruit.GetComponent<Renderer>().material, rotFruit, 0.50f);
        fruit.GetComponent<FruitType>().state = StateFruit.MiddleRot;

        yield return new WaitForSeconds(1.5f);
        fruit.GetComponent<Renderer>().material.Lerp(fruit.GetComponent<Renderer>().material, rotFruit, 0.75f);
        fruit.GetComponent<FruitType>().state = StateFruit.EndRot;

        yield return new WaitForSeconds(1.5f);
        fruit.GetComponent<Renderer>().material.Lerp(fruit.GetComponent<Renderer>().material, rotFruit, 1.0f);
        fruit.GetComponent<FruitType>().state = StateFruit.CompletelyRot;
    }
}
