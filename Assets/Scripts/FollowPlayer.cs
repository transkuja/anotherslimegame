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
    public Text textPlayer1;
    public Text textPlayer2;
    public Text textPlayer3;

    public int count;
    public float fieldOfView;

    public string layer1;
    public string layer2;
    public string layer3;
	// Use this for initialization
	void Start ()
    {
        count = GameManager.Instance.PlayerStart.PlayersReference.Count;
        layer1 = LayerMask.LayerToName(textPlayer1.gameObject.layer);
        layer2 = LayerMask.LayerToName(textPlayer2.gameObject.layer);
        layer3 = LayerMask.LayerToName(textPlayer3.gameObject.layer);
        //fieldOfView = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView;
        //Initialisation des textes pour le joueur 1
        if (layer1 == "CameraP1" && layer2 == "CameraP1" && layer3 == "CameraP1")
        {
            Debug.Log("coucou");
            if (count == 4)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
            }
            else if (count == 3)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

            }
            else if (count == 2)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
            }
        }

        //Initialisation des textes pour le joueur 2
        if (layer1 == "CameraP2" && layer2 == "CameraP2" && layer3 == "CameraP2")
        {
            if (count == 4)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
            }
            else if (count == 3)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

            }
            else if (count == 2)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);
            }
        }


        //Initialisation des textes pour le joueur 3
        if (layer1 == "CameraP3" && layer2 == "CameraP3" && layer3 == "CameraP3")
        {
            if (count == 4)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
            }
            else if (count == 3)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

            }
        }

        //Initialisation des textes pour le joueur 4
        if (layer1 == "CameraP4" && layer2 == "CameraP4" && layer3 == "CameraP4")
        {
            if (count == 4)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Initialisation des textes pour le joueur 1
        if (layer1 == "CameraP1" && layer2 == "CameraP1" && layer3 == "CameraP1")
        {
            if (count == 4)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
            }
            else if (count == 3)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

            }
            else if (count == 2)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
            }
        }

        //Initialisation des textes pour le joueur 2
        if (layer1 == "CameraP2" && layer2 == "CameraP2" && layer3 == "CameraP2")
        {
            if (count == 4)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
            }
            else if (count == 3)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

            }
            else if (count == 2)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);
            }
        }


        //Initialisation des textes pour le joueur 3
        if (layer1 == "CameraP3" && layer2 == "CameraP3" && layer3 == "CameraP3")
        {
            if (count == 4)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
            }
            else if (count == 3)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

            }
        }

        //Initialisation des textes pour le joueur 4
        if (layer1 == "CameraP4" && layer2 == "CameraP4" && layer3 == "CameraP4")
        {
            if (count == 4)
            {
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
            }
        }
    }
}
