using UnityEngine;

using UnityEngine.UI;

public class ReadySetGo : MonoBehaviour {

    float timer;
    Image img;

    float step = 0.75f;

	void Start() {
        img = GetComponent<Image>();
        img.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber3;
        timer = step * 3;
        if (AudioManager.Instance != null && AudioManager.Instance.readySetGoFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.readySetGoFx);
    }

    // Should be handled with a coroutine
    void Update () {
        timer -= Time.deltaTime;

        if (timer < step * 2 && img.sprite == ResourceUtils.Instance.spriteUtils.victoryNumber3)
        {
            img.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber2;
            if (AudioManager.Instance != null && AudioManager.Instance.readySetGoFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.readySetGoFx);
        }
        if (timer < step && img.sprite == ResourceUtils.Instance.spriteUtils.victoryNumber2)
        {
            img.sprite = ResourceUtils.Instance.spriteUtils.victoryNumber1;
            if (AudioManager.Instance != null && AudioManager.Instance.readySetGoFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.readySetGoFx);
        }
        if (timer < 0.0f && img.sprite == ResourceUtils.Instance.spriteUtils.victoryNumber1)
        {
            // TODO: sound should be different here
            if (AudioManager.Instance != null && AudioManager.Instance.readySetGoFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.readySetGoFx);
            GameManager.ChangeState(GameState.Normal);
            Destroy(gameObject);
        }

    }
}
