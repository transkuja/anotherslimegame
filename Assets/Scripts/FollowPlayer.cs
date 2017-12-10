using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/***
 *TODO : Verif position > Si hors champs, passer en mode fleche qui tourne sur les bords
 * 
 */
public class FollowPlayer : MonoBehaviour
{
    public Canvas canvasText;

    public Text textPlayer1;
    public Text textPlayer2;
    public Text textPlayer3;
    public Text textPlayer4;

    public int count;
    public float fieldOfView;
	// Use this for initialization
	void Start ()
    {
        count = GameManager.Instance.PlayerStart.PlayersReference.Count;
        fieldOfView = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView;
        if(count == 4)
        {
            textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0,3,0);

            textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
            textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

            textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
            textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

            textPlayer4.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
            textPlayer4.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);

        }
        else if(count == 3)
        {
            textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

            textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
            textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

            textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
            textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
        }
        else if(count == 2)
        {
            textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

            textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
            textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        /*if ()
        {*/
            if (count == 4)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

                textPlayer4.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
                textPlayer4.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);

            }
            else if (count == 3)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
            }
            else if (count == 2)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
            }
       // }
        /*else
        {
            //GameManager.Instance.PlayerStart.PlayersReference[i].transform.GetChild((int)PlayerChildren.Canvas).GetChild(i).gameObject.SetActive(false);
        }*/
    }
}
