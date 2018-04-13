using System;
using UnityEngine;
using UnityEngine.UI;

public class PNJDefaultMessage : MonoBehaviour {

    // DEfault message
    public PNJMessages defaultMessage;
    public PNJName pnjName;

    // Ref sur les instances
    public GameObject refCanvasParent;
    private GameObject[] refCanvas = new GameObject[2];
    private GameObject[] BbuttonShown = new GameObject[2];
    private bool[] hasBeenInitialized = new bool[2];
    private GameObject[][] Message = new GameObject[2][];
    private int currentMessage = 0;

    // Event
    public bool needCallEvent = false;

    // Use this for initialization
    void Start () {
        defaultMessage = new PNJMessages(
            PNJDialogUtils.GetDefaultMessages(pnjName), 
            PNJDialogUtils.GetQuestMessages(pnjName), 
            PNJDialogUtils.GetDefaultEmotions(pnjName), 
            PNJDialogUtils.GetQuestEmotions(pnjName)
        );

        // Bon sa c'est un peu dégeulasse mais sa permet de savoir si les messages ont été créés
        hasBeenInitialized[0] = false;
        hasBeenInitialized[1] = false;
    }

    #region DefaultMessage
    // Trigger Enter
    public void CreateUIMessage(int playerIndex)
    {
        if (GetComponent<PNJDefaultBehavior>() != null && GetComponent<PNJDefaultBehavior>().IsEventOver())
            needCallEvent = false;

        if (Message != null)
        {
            DestroyUIMessage(0);
            DestroyUIMessage(1);
        }

        if (needCallEvent)
        {
            int nextMessagesLength = GetComponent<PNJDefaultBehavior>().GetNextMessagesLength();
            if (nextMessagesLength > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Message[i] = new GameObject[nextMessagesLength];
                }
            }
        }
        else
        {
            if (defaultMessage.DefaultMessagesLength() > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Message[i] = new GameObject[defaultMessage.DefaultMessagesLength()];
                }
            }
            else
                return;
        }

        // Donne au uicameraapter le playerindex utilise pour s'orienter face a la camera
        refCanvas[playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabCanvasWithUiCameraAdapter, refCanvasParent.transform);
        refCanvas[playerIndex].GetComponent<UICameraApdater>().PlayerIndex = playerIndex;

        // Instanciera tous les message a afficher
        for (int i = 0; i < Message[playerIndex].Length; i++)
        {
            Message[playerIndex][i] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabMessage, refCanvas[playerIndex].transform);
            Message[playerIndex][i].transform.GetChild(2).GetComponent<Text>().text = pnjName.ToString();
            if (needCallEvent)
                Message[playerIndex][i].transform.GetChild(3).GetComponent<Text>().text = GetComponent<PNJDefaultBehavior>().GetNextMessage(i);
            else
                Message[playerIndex][i].transform.GetChild(3).GetComponent<Text>().text = defaultMessage.GetDefaultMessages(0).messages[i];

            Message[playerIndex][i].SetActive(false);
        }

        // Instanciera le bouton
        BbuttonShown[playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabBButton, refCanvas[playerIndex].transform);
        BbuttonShown[playerIndex].SetActive(true);

        // Affecte les layers au joueurs
        refCanvas[playerIndex].layer = LayerMask.NameToLayer((playerIndex == 0) ? "CameraP1" : "CameraP2");

        hasBeenInitialized[playerIndex] = true;
    }

    // TriggerExit
    public void DestroyUIMessage(int playerIndex)
    {
        if (Message[playerIndex] == null)
            return;

        currentMessage = 0;
        for (int i = 0; i < Message[playerIndex].Length; i++)
        {
            Destroy(Message[playerIndex][i]);
        }
        Destroy(BbuttonShown[playerIndex]);
        Destroy(refCanvas[playerIndex]);
        hasBeenInitialized[playerIndex] = false;
    }

    public void DisplayMessage(int playerIndex)
    {
        if (!hasBeenInitialized[playerIndex])
            return;

        if (currentMessage < Message[playerIndex].Length)
        {
            // Se tourne vers le joueur
            transform.LookAt(new Vector3(GameManager.Instance.PlayerStart.PlayersReference[playerIndex].transform.position.x, transform.position.y, GameManager.Instance.PlayerStart.PlayersReference[playerIndex].transform.position.z));

            if (Message[playerIndex][currentMessage].gameObject.activeSelf)
            {
                // Cache le précédent message
                Message[playerIndex][currentMessage].SetActive(false);
                currentMessage++;

                // Affiche le suivant
                if (currentMessage < Message[playerIndex].Length)
                {
                    Message[playerIndex][currentMessage].SetActive(true);
                }

                // Comportement sur le dernier message
                if( currentMessage == Message[playerIndex].Length)
                {
                    if(needCallEvent)
                    {
                        // Special event
                        MyCustomEvent();
                    }
                    else
                    {
                        // Cas standard
                        GameManager.ChangeState(GameState.Normal);
                    }

                    BbuttonShown[playerIndex].SetActive(true);
                    currentMessage = 0;
                    hasBeenInitialized[playerIndex] = false;
                    CreateUIMessage(playerIndex);
                }
            }
            else
            {
                Message[playerIndex][currentMessage].SetActive(true);
                BbuttonShown[playerIndex].SetActive(false);

                // Disable inputs except camera et change le comportement dans le player controller : si c'est en normal l'interaction se fait via B sinon A
                GameManager.ChangeState(GameState.ForcedPauseMGRules);

                // Freeze player character
                PlayerCharacterHub pc = GameManager.Instance.PlayerStart.PlayersReference[playerIndex].GetComponent<PlayerCharacterHub>();
                pc.Rb.drag = 25.0f;
                pc.Rb.velocity = Vector3.zero;
            }
        }
    }
    #endregion

    public void MyCustomEvent()
    {
        // My custom Event on last message.
        if (GetComponent<PNJDefaultBehavior>())
        {
            GetComponent<PNJDefaultBehavior>().InitNextStep();
        }


        GameManager.ChangeState(GameState.Normal);
    }
}
