using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        if (UIrefLeft == null || UIrefRight == null)
            return;

        ShowXPlayersUI(GameManager.Instance.PlayerStart.ActivePlayersAtStart);
        foreach (GameObject p in GameManager.Instance.PlayerStart.PlayersReference)
            p.GetComponent<Player>().OnValuesChange = new UIfct[(int)PlayerUIStat.Size];
        HandleTextChangeInit(GameManager.Instance.PlayerStart.ActivePlayersAtStart);
        UpdateSpriteColorBasedOnSelection();

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
                ShowPlayerUi(UIrefLeft.transform.GetChild(0), false);
                ShowPlayerUi(UIrefLeft.transform.GetChild(1), true);
                ShowPlayerUi(UIrefRight.transform.GetChild(0), false);
                ShowPlayerUi(UIrefRight.transform.GetChild(1), false);
                break;
            // 2P: - X 00:00 X -
            case 2:
                ShowPlayerUi(UIrefLeft.transform.GetChild(0), false);
                ShowPlayerUi(UIrefLeft.transform.GetChild(1), true);
                ShowPlayerUi(UIrefRight.transform.GetChild(0), true);
                ShowPlayerUi(UIrefRight.transform.GetChild(1), false);
                break;
            // 3P: X X 00:00 X -
            case 3:
                ShowPlayerUi(UIrefLeft.transform.GetChild(0), true);
                ShowPlayerUi(UIrefLeft.transform.GetChild(1), true);
                ShowPlayerUi(UIrefRight.transform.GetChild(0), true);
                ShowPlayerUi(UIrefRight.transform.GetChild(1), false);
                break;
            // 4P: X X 00:00 X -
            default:
                ShowPlayerUi(UIrefLeft.transform.GetChild(0), true);
                ShowPlayerUi(UIrefLeft.transform.GetChild(1), true);
                ShowPlayerUi(UIrefRight.transform.GetChild(0), true);
                ShowPlayerUi(UIrefRight.transform.GetChild(1), true);
                break;
        }
    }

    void ShowPlayerUi(Transform _uiPlayer, bool _enable)
    {
        _uiPlayer.GetChild(0).gameObject.SetActive(_enable);
        _uiPlayer.GetChild(1).gameObject.SetActive(_enable);
    }

    void HandleTextChangeInit(uint _nbrPlayers)
    {
        switch (_nbrPlayers)
        {
            // 1P: - X 00:00 - -
            case 1:
                UIrefLeft.transform.GetChild(1).GetChild(1).GetComponent<TextChange>().Init(0);
                UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "P1";
                break;
            // 2P: - X 00:00 X -
            case 2:
                UIrefLeft.transform.GetChild(1).GetChild(1).GetComponent<TextChange>().Init(0);
                UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "P1";
                UIrefRight.transform.GetChild(0).GetChild(1).GetComponent<TextChange>().Init(1);
                UIrefRight.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "P2";
                break;
            // 3P: X X 00:00 X -
            case 3:
                UIrefLeft.transform.GetChild(0).GetChild(1).GetComponent<TextChange>().Init(0);
                UIrefLeft.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "P1";
                UIrefLeft.transform.GetChild(1).GetChild(1).GetComponent<TextChange>().Init(1);
                UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "P2";
                UIrefRight.transform.GetChild(0).GetChild(1).GetComponent<TextChange>().Init(2);
                UIrefRight.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "P3";
                break;
            // 4P: X X 00:00 X -
            default:
                UIrefLeft.transform.GetChild(0).GetChild(1).GetComponent<TextChange>().Init(0);
                UIrefLeft.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "P1";
                UIrefLeft.transform.GetChild(1).GetChild(1).GetComponent<TextChange>().Init(1);
                UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "P2";
                UIrefRight.transform.GetChild(0).GetChild(1).GetComponent<TextChange>().Init(2);
                UIrefRight.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "P3";
                UIrefRight.transform.GetChild(1).GetChild(1).GetComponent<TextChange>().Init(3);
                UIrefRight.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "P4";
                break;
        }
    }

    private void UpdateSpriteColorBasedOnSelection()
    {
        if (SlimeDataContainer.instance != null)
        {
            //UIref.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = (SlimeDataContainer.instance.colorFadeSelected[i] ? SlimeDataContainer.instance.selectedColors[i] : Color.white);

            // TMP: Should use color from slimedatacontainer instead
            switch (GameManager.Instance.PlayerStart.ActivePlayersAtStart)
            {
                case 1:
                    UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = GameManager.Instance.PlayerStart.colorPlayer[0];
                    break;
                case 2:
                    Debug.Log(GameManager.Instance.PlayerStart.colorPlayer[0]);
                    UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = GameManager.Instance.PlayerStart.colorPlayer[0];
                    UIrefRight.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = GameManager.Instance.PlayerStart.colorPlayer[1];
                    break;
                case 3:
                    UIrefLeft.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = GameManager.Instance.PlayerStart.colorPlayer[0];
                    UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = GameManager.Instance.PlayerStart.colorPlayer[1];
                    UIrefRight.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = GameManager.Instance.PlayerStart.colorPlayer[2];
                    break;
                default:
                    UIrefLeft.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = GameManager.Instance.PlayerStart.colorPlayer[0];
                    UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = GameManager.Instance.PlayerStart.colorPlayer[1];
                    UIrefRight.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = GameManager.Instance.PlayerStart.colorPlayer[2];
                    UIrefRight.transform.GetChild(1).GetChild(0).GetComponent<Text>().color = GameManager.Instance.PlayerStart.colorPlayer[3];
                    break;
            }
        }
    }
}
