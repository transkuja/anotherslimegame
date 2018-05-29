using UnityEngine;

public class LoadingTip : MonoBehaviour {
    
	void Start () {
        if (LevelLoader.TargetLevelId != "Hub" || GameManager.Instance.ActivePlayersAtStart < 2)
            gameObject.SetActive(false);
	}
}
