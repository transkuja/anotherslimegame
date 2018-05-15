using System;
using UnityEngine;
using UnityEngine.UI;

public class PNJMessage : MonoBehaviour {

    private PNJDefaultBehavior myBehavior;
    [HideInInspector]
    public PlayerCharacterHub myCharacter;

    // DEfault message
    private PNJMessages defaultMessage;
    public PNJName pnjName;

    // Ref sur les instances
    public GameObject[] BbuttonShown = new GameObject[2];
    [HideInInspector]
    public GameObject[] Message;
    public int currentMessage = 0;

    // Use this for initialization
    void Start () {
        defaultMessage = new PNJMessages(
            PNJDialogUtils.GetDefaultMessages(pnjName), 
            PNJDialogUtils.GetQuestMessages(pnjName), 
            PNJDialogUtils.GetDefaultEmotions(pnjName), 
            PNJDialogUtils.GetQuestEmotions(pnjName)
        );

        Message = new GameObject[2];

        myBehavior = GetComponent<PNJDefaultBehavior>();
        myCharacter = GetComponent<PlayerCharacterHub>();

        // Odd stuff
        if (GameManager.Instance && GameManager.Instance.PlayerStart && GameManager.Instance.ActivePlayersAtStart == 2)
        {
                Message[1] = GameManager.UiReference.dialog2.gameObject;
                Message[0] = GameManager.UiReference.dialog3.gameObject;
        }
        else
            Message[0] = GameManager.UiReference.dialog1.gameObject;
    }

    #region DefaultMessage
    

    // Trigger Enter
    public void OnEnterTrigger(int playerIndex)
    {
        if (myCharacter.dialogState == DialogState.Dialog)
            return;

        BbuttonShown[playerIndex].SetActive(true);
    }

    public bool OnExitTrigger(int playerIndex)
    {
        if (myCharacter.dialogState == DialogState.Dialog)
            return false;

        if (Message[playerIndex])
            Message[playerIndex].SetActive(false);
        GameManager.Instance.PlayerStart.PlayersReference[playerIndex].GetComponent<PlayerCharacterHub>().dialogState = DialogState.Normal;

        if (BbuttonShown[playerIndex])
            BbuttonShown[playerIndex].SetActive(false);

        return true;
    }

    public void DisplayMessage(int playerIndex)
    {
        currentMessage++;

        // how to behave
        if (myBehavior && !myBehavior.IsEventOver() && currentMessage >= myBehavior.GetNextMessagesLength())
        {
            PNJDialogUtils.EndDialog(myCharacter, playerIndex);
            BbuttonShown[playerIndex].SetActive(true);

            if (myBehavior && !myBehavior.IsEventOver())
            {
                myBehavior.InitNextStep(playerIndex);

                BbuttonShown[playerIndex].SetActive(false);
            }
        }
        else
        {
            // Next Message
            NextMessage(playerIndex);
        }

        // GetBack to default behavior
        if (myBehavior && myBehavior.IsEventOver()
            || (!myBehavior && currentMessage >= defaultMessage.GetDefaultMessages(0).messages.Length)
            )
        {
            BbuttonShown[playerIndex].SetActive(false);
            PNJDialogUtils.EndDialog(myCharacter, playerIndex);
        }
    }

    // First interaction
    public void Interact(int playerIndex)
    {
        if (myCharacter.dialogState == DialogState.Dialog)
            return;

        // Both character hub dialogState change to Dialog
        PlayerCharacterHub joueurQuiluiParle = GameManager.Instance.PlayerStart.PlayersReference[playerIndex].GetComponent<PlayerCharacterHub>();
        if (joueurQuiluiParle)
        {
            joueurQuiluiParle.dialogState = DialogState.Dialog;
            transform.LookAt(new Vector3(joueurQuiluiParle.transform.position.x, transform.position.y, joueurQuiluiParle.transform.position.z));
        }

        myCharacter.dialogState = DialogState.Dialog;
   
        // Display Message Hide Button B
        BbuttonShown[playerIndex].SetActive(false);
        Message[playerIndex].SetActive(true);


        // First Message
        if ((myBehavior && myBehavior.IsEventOver()) || !myBehavior)
        {
            currentMessage = 0;
            if( myBehavior)
                myBehavior.Reset();
        }
        NextMessage(playerIndex);
    }

    public void NextMessage(int playerIndex)
    {
        Message[playerIndex].transform.GetChild(1).GetComponent<Text>().text = pnjName.ToString();
        if (myBehavior && !myBehavior.IsEventOver())
        {
            Message[playerIndex].transform.GetChild(2).GetComponent<Text>().text = myBehavior.GetNextMessage(currentMessage);
        }
        else
        {
            Message[playerIndex].transform.GetChild(2).GetComponent<Text>().text = defaultMessage.GetDefaultMessages(0).GetNextMessage(currentMessage);
        }

    }
    #endregion
}
