using UnityEngine;

using UnityEngine.UI;

public class ReadySetGo : MonoBehaviour {

    float timer;
    Image img;
	void Start() {
        img = GetComponent<Image>();
        img.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber3;
        timer = 3.0f;
    }

    void Update () {
        timer -= Time.deltaTime;

        if (timer < 2.0f && img.sprite == ResourceUtils.Instance.spriteUtils.victoryNumber3)
            img.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber2;
        if (timer < 1.0f && img.sprite == ResourceUtils.Instance.spriteUtils.victoryNumber2)
            img.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber1;
        if (timer < 0.0f && img.sprite == ResourceUtils.Instance.spriteUtils.victoryNumber1)
        {
            GameManager.ChangeState(GameState.Normal);
            Destroy(gameObject);
        }

    }
}
