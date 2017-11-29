using System.Collections;
using System.Collections.Generic;
using UWPAndXInput;
using UnityEngine;

public class TheButton : MonoBehaviour {

    public GameObject water;

    [Range(0.01f, 0.5f)]
    public float speed = 0.01f;

    [Header("height + water.position")]
    public float heightToReach = 32;

    private bool moveWater = false;
    private Vector3 positionToReach;
    Vector3 lerpStartValue;

    float lerpValue;

    public void Start()
    {
        positionToReach = water.transform.position + heightToReach * Vector3.up;
        lerpValue = 0.0f;
        lerpStartValue = water.transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<Animator>().SetBool("test", true);
            moveWater = true;
            for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++){
                GamePad.SetVibration((PlayerIndex)i, 1, 1);
            }
        }
    }

    public void Update()
    {
        if (moveWater)
        {
            lerpValue += speed * Time.deltaTime;
            water.transform.position = Vector3.Lerp(lerpStartValue, positionToReach, lerpValue);
            if (lerpValue >= 1.0f)
            {
                moveWater = false;
                for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
                    GamePad.SetVibration((PlayerIndex)i, 0, 0);
            }
        }

       
    }

    private void OnDestroy()
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
            GamePad.SetVibration((PlayerIndex)i, 0, 0);
    }
}
