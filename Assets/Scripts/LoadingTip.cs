using UnityEngine;
using UnityEngine.UI;

public class LoadingTip : MonoBehaviour {
    
	void Start () {

        if (SlimeDataContainer.instance != null)
        {
            if (LevelLoader.TargetLevelId != "Hub" || SlimeDataContainer.instance .nbPlayers < 2)
                gameObject.SetActive(false);
        }

        if (Controls.IsKeyboardUsed())
        {
            transform.GetComponentInChildren<Text>().text = "Tip\n    Hold       and       or       to teleport above your partner";
            transform.GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            transform.GetComponentInChildren<Text>().text = "Tip\n    Hold       and       to teleport above your partner";
            transform.GetChild(3).gameObject.SetActive(false);
        }

    }
}
