using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle players' ui in minigames. The desired behaviour is as follow, with a X for "ui shown" and a - for "ui not shown":
/// 1P: - X 00:00 - -
/// 2P: - X 00:00 X -
/// 3P: X X 00:00 X -
/// 4P: X X 00:00 X -
/// /// </summary>
public abstract class APlayerUI : MonoBehaviour {

    [Tooltip("The 2 players' ui shown in the top left corner.")]
    public GameObject UIrefLeft;
    [Tooltip("The 2 players' ui shown in the top right corner.")]
    public GameObject UIrefRight;

    public void Awake()
    {
        GameManager.Instance.RegisterAPlayerUI(this);
    }

    public virtual void Init()
    {
        ShowXPlayersUI(GameManager.Instance.PlayerStart.ActivePlayersAtStart);
        foreach (GameObject p in GameManager.Instance.PlayerStart.PlayersReference)
            p.GetComponent<Player>().OnValuesChange = new UIfct[(int)PlayerUIStat.Size];
        HandleTextChangeInit(GameManager.Instance.PlayerStart.ActivePlayersAtStart);

    }

    public virtual void OnValueChange(TextChange text)
    {
        // feedback 
    }

    void ShowXPlayersUI(uint _nbrPlayers)
    {
        switch (_nbrPlayers)
        {
            // 1P: - X 00:00 - -
            case 1:
                UIrefLeft.transform.GetChild(0).gameObject.SetActive(false);
                UIrefLeft.transform.GetChild(1).gameObject.SetActive(true);
                UIrefRight.transform.GetChild(0).gameObject.SetActive(false);
                UIrefRight.transform.GetChild(1).gameObject.SetActive(false);
                break;
            // 2P: - X 00:00 X -
            case 2:
                UIrefLeft.transform.GetChild(0).gameObject.SetActive(false);
                UIrefLeft.transform.GetChild(1).gameObject.SetActive(true);
                UIrefRight.transform.GetChild(0).gameObject.SetActive(true);
                UIrefRight.transform.GetChild(1).gameObject.SetActive(false);
                break;
            // 3P: X X 00:00 X -
            case 3:
                UIrefLeft.transform.GetChild(0).gameObject.SetActive(true);
                UIrefLeft.transform.GetChild(1).gameObject.SetActive(true);
                UIrefRight.transform.GetChild(0).gameObject.SetActive(true);
                UIrefRight.transform.GetChild(1).gameObject.SetActive(false);
                break;
            // 4P: X X 00:00 X -
            default:
                UIrefLeft.transform.GetChild(0).gameObject.SetActive(true);
                UIrefLeft.transform.GetChild(1).gameObject.SetActive(true);
                UIrefRight.transform.GetChild(0).gameObject.SetActive(true);
                UIrefRight.transform.GetChild(1).gameObject.SetActive(true);
                break;
        }
    }

    void HandleTextChangeInit(uint _nbrPlayers)
    {
        int i = 0;
        foreach (TextChange obj in GameObject.FindObjectsOfType<TextChange>())
        {
            if (obj.transform.parent.gameObject.activeSelf)
            {
                obj.Init(i);
                i++;
            }
        }
    }
}
