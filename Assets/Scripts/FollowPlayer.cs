using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/***
 *TODO LIST
 * 
 * 4 variables pour les 4 Cameras > Les recupérées. --> Tester avec WorldToScreenPoint pour ensuite verif position --> Si hors champs, passer en mode fleche qui tourne sur les bords
 * 
 *
 * 
 */
public class FollowPlayer : MonoBehaviour
{
    public Text textPlayer1;
    public Text textPlayer2;
    public Text textPlayer3;


    public int count;
    public float fieldOfView;
    public float distancePlayer1Text1;
    public float distancePlayer1Text2;
    public float distancePlayer1Text3;

    public float distancePlayer2Text1;
    public float distancePlayer2Text2;
    public float distancePlayer2Text3;

    public float distancePlayer3Text1;
    public float distancePlayer3Text2;
    public float distancePlayer3Text3;

    public float distancePlayer4Text1;
    public float distancePlayer4Text2;
    public float distancePlayer4Text3;

    public Dictionary<int, float> distancePlayers = new Dictionary<int, float>();
    private Text[] textPlayer = new Text[3];

    public string layerName;
    public GameObject[] camReferences;

    //Les 4 cameras
    Camera cam1;
    Camera cam2;
    Camera cam3;
    Camera cam4;
    // Use this for initialization
    void Start()
    {
        count = GameManager.Instance.PlayerStart.PlayersReference.Count;
        layerName = LayerMask.LayerToName(textPlayer1.gameObject.layer);
        camReferences = GameManager.Instance.PlayerStart.cameraPlayerReferences;
        //fieldOfView = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView;
        fieldOfView = 95.0f;

        //Ici Recuperation des cameras (en evitant le FindGameOjbectWithTag (trop couteux).

        //for (int i = 0; i < transform.childCount; i++)
        //    textPlayer[i] = transform.GetChild(i).GetComponent<Text>();

        //int j = 0;
        //for (int i = 0; i < count; i++)
        //{

        //    if (i != transform.parent.GetSiblingIndex())
        //    {
        //        textPlayer[j].text = GameManager.Instance.PlayerStart.PlayersReference[i].name;
        //        textPlayer[j].transform.position = GameManager.Instance.PlayerStart.PlayersReference[i].transform.position + new Vector3(0, 3, 0);
        //        j++;
        //    }
        //}

        switch (layerName)
        {
            case "CameraP1":
                switch (count)
                {
                    case 4:
                        textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                        textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                        textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                        textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

                        textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
                        textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
                        break;
                    case 3:
                        textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                        textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                        textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                        textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
                        break;
                    case 2:
                        textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                        textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
                        break;
                    default:
                        break;
                }
                break;
            case "CameraP2":
                switch (count)
                {
                    case 4:
                        textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                        textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                        textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                        textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);

                        textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
                        textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
                        break;
                    case 3:
                        textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                        textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                        textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                        textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
                        break;
                    case 2:
                        textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                        textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);
                        break;
                    default:
                        break;
                }
                break;
            case "CameraP3":
                switch (count)
                {
                    case 4:
                        textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                        textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                        textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                        textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                        textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[3].name;
                        textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
                        break;
                    case 3:
                        textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                        textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                        textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                        textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
                        break;
                    default:
                        break;
                }
                break;
            case "CameraP4":
                textPlayer1.text = GameManager.Instance.PlayerStart.PlayersReference[0].name;
                textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);

                textPlayer2.text = GameManager.Instance.PlayerStart.PlayersReference[1].name;
                textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);

                textPlayer3.text = GameManager.Instance.PlayerStart.PlayersReference[2].name;
                textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        distancePlayer1Text1 = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position.z - textPlayer1.transform.position.z;
        distancePlayer1Text2 = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position.z - textPlayer2.transform.position.z;
        distancePlayer1Text3 = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position.z - textPlayer3.transform.position.z;

        distancePlayer2Text1 = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position.z - textPlayer1.transform.position.z;
        distancePlayer2Text2 = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position.z - textPlayer2.transform.position.z;
        distancePlayer2Text3 = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position.z - textPlayer3.transform.position.z;

        distancePlayer3Text1 = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position.z - textPlayer1.transform.position.z;
        distancePlayer3Text2 = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position.z - textPlayer2.transform.position.z;
        distancePlayer3Text3 = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position.z - textPlayer3.transform.position.z;

        distancePlayer4Text1 = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position.z - textPlayer1.transform.position.z;
        distancePlayer4Text2 = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position.z - textPlayer2.transform.position.z;
        distancePlayer4Text3 = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position.z - textPlayer3.transform.position.z;
        //Position pour les test et positionner les textes comme il le faut
        Vector3 pos = cam1.WorldToScreenPoint(textPlayer1.transform.position);
        switch (layerName)
        {
            //JOUEUR 1
            case "CameraP1":
                switch (count)
                {
                    case 4:
                        if (distancePlayer1Text1 <= fieldOfView)
                        {
                            textPlayer1.gameObject.SetActive(true);
                            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            //Pour le moment ça ne marche pas
                            //Placement du texte si il est en dehors de l'écran (droite ou gauche)
                            //if (textPlayer1.transform.position.z >= Screen.width / 2.0f)
                            //{
                            //    textPlayer1.transform.position.Set(GameManager.Instance.PlayerStart.PlayersReference[1].transform.position.x, GameManager.Instance.PlayerStart.PlayersReference[1].transform.position.y, Screen.width / 2.0f);
                            //}

                            //Placement du texte si il est trop haut ou trop bas
                            //if (textPlayer1.transform.position.y >= Screen.height)
                            //{
                            //    textPlayer1.transform.position.Set(GameManager.Instance.PlayerStart.PlayersReference[1].transform.position.x, Screen.height, GameManager.Instance.PlayerStart.PlayersReference[1].transform.position.z);
                            //}
                            //if (textPlayer1.transform.position.y <= Screen.height / 2.0f)
                            //{
                            //    textPlayer1.transform.position.Set(GameManager.Instance.PlayerStart.PlayersReference[1].transform.position.x, Screen.height / 2.0f, GameManager.Instance.PlayerStart.PlayersReference[1].transform.position.z);
                            //}
                            textPlayer1.gameObject.SetActive(false);
                        }

                        if (distancePlayer1Text2 <= fieldOfView)
                        {
                            textPlayer2.gameObject.SetActive(true);
                            textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer2.gameObject.SetActive(false);
                        }

                        if (distancePlayer1Text3 <= fieldOfView)
                        {
                            textPlayer3.gameObject.SetActive(true);
                            textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer3.gameObject.SetActive(false);
                        }
                        break;
                    case 3:
                        if (distancePlayer1Text1 <= fieldOfView)
                        {
                            textPlayer1.gameObject.SetActive(true);
                            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer1.gameObject.SetActive(false);
                        }

                        if (distancePlayer1Text2 <= fieldOfView)
                        {
                            textPlayer2.gameObject.SetActive(true);
                            textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer2.gameObject.SetActive(false);
                        }
                        break;
                    case 2:
                        if (distancePlayer1Text1 <= fieldOfView)
                        {
                            textPlayer1.gameObject.SetActive(true);
                            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer1.gameObject.SetActive(false);
                        }
                        break;
                    default:
                        break;
                }
                break;

            //JOUEUR 2 
            case "CameraP2":
                switch (count)
                {
                    case 4:
                        if (distancePlayer2Text1 <= fieldOfView)
                        {
                            textPlayer1.gameObject.SetActive(true);
                            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer1.gameObject.SetActive(false);
                        }

                        if (distancePlayer2Text2 <= fieldOfView)
                        {
                            textPlayer2.gameObject.SetActive(true);
                            textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer2.gameObject.SetActive(false);
                        }

                        if (distancePlayer2Text3 <= fieldOfView)
                        {
                            textPlayer3.gameObject.SetActive(true);
                            textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer3.gameObject.SetActive(false);
                        }
                        break;
                    case 3:
                        if (distancePlayer2Text1 <= fieldOfView)
                        {
                            textPlayer1.gameObject.SetActive(true);
                            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer1.gameObject.SetActive(false);
                        }

                        if (distancePlayer2Text2 <= fieldOfView)
                        {
                            textPlayer2.gameObject.SetActive(true);
                            textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer2.gameObject.SetActive(false);
                        }
                        break;
                    case 2:
                        if (distancePlayer2Text1 <= fieldOfView)
                        {
                            textPlayer1.gameObject.SetActive(true);
                            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer1.gameObject.SetActive(false);
                        }
                        break;
                    default:
                        break;
                }
                break;
            //JOUEUR 3
            case "CameraP3":
                switch (count)
                {
                    case 4:
                        if (distancePlayer3Text1 <= fieldOfView)
                        {
                            textPlayer1.gameObject.SetActive(true);
                            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer1.gameObject.SetActive(false);
                        }

                        if (distancePlayer3Text2 <= fieldOfView)
                        {
                            textPlayer2.gameObject.SetActive(true);
                            textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer2.gameObject.SetActive(false);
                        }

                        if (distancePlayer3Text3 <= fieldOfView)
                        {
                            textPlayer3.gameObject.SetActive(true);
                            textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[3].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer3.gameObject.SetActive(false);
                        }
                        break;
                    case 3:
                        if (distancePlayer3Text1 <= fieldOfView)
                        {
                            textPlayer1.gameObject.SetActive(true);
                            textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer1.gameObject.SetActive(false);
                        }

                        if (distancePlayer3Text2 <= fieldOfView)
                        {
                            textPlayer2.gameObject.SetActive(true);
                            textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
                        }
                        else
                        {
                            textPlayer2.gameObject.SetActive(false);
                        }
                        break;
                    default:
                        break;
                }
                break;
            //JOUEUR 4
            case "CameraP4":
                if (distancePlayer4Text1 <= fieldOfView)
                {
                    textPlayer1.gameObject.SetActive(true);
                    textPlayer1.transform.position = GameManager.Instance.PlayerStart.PlayersReference[0].transform.position + new Vector3(0, 3, 0);
                }
                else
                {
                    textPlayer1.gameObject.SetActive(false);
                }

                if (distancePlayer4Text2 <= fieldOfView)
                {
                    textPlayer2.gameObject.SetActive(true);
                    textPlayer2.transform.position = GameManager.Instance.PlayerStart.PlayersReference[1].transform.position + new Vector3(0, 3, 0);
                }
                else
                {
                    textPlayer2.gameObject.SetActive(false);
                }
                if (distancePlayer4Text3 <= fieldOfView)
                {
                    textPlayer3.gameObject.SetActive(true);
                    textPlayer3.transform.position = GameManager.Instance.PlayerStart.PlayersReference[2].transform.position + new Vector3(0, 3, 0);
                }
                else
                {
                    textPlayer3.gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }
}
