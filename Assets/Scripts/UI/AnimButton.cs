using UnityEngine;
using UnityEngine.UI;

public class AnimButton : MonoBehaviour {

    Button button;
    bool scaleIncreasing = true;
    Vector3 initialScale;

    void Start()
    {
        button = GetComponent<Button>();
        initialScale = transform.localScale;
    }

    void Update()
    {
        button.transform.localScale = new Vector3(button.transform.localScale.x + ((scaleIncreasing) ? Time.deltaTime : -Time.deltaTime) / 4,
                                                    button.transform.localScale.y + ((scaleIncreasing) ? Time.deltaTime : -Time.deltaTime) / 4,
                                                    1);
        if (button.transform.localScale.x > initialScale.x * 1.05f) scaleIncreasing = false;
        else if (button.transform.localScale.x < initialScale.x * 0.95f) scaleIncreasing = true;
    }

    private void OnDisable()
    {
        button.transform.localScale = initialScale;
    }
}
