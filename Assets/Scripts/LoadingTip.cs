using UnityEngine;

public class LoadingTip : MonoBehaviour {
    
	void Start () {

        if( SlimeDataContainer.instance != null)
        {
            if (LevelLoader.TargetLevelId != "Hub" || SlimeDataContainer.instance .nbPlayers < 2)
                gameObject.SetActive(false);
        }


	}
}
