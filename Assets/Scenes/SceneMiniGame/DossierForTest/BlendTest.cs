using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendTest : MonoBehaviour {


    public Material matTest1;
    public Material matTest2;
    public float duration = 2.0f;
    public Renderer renderTest;
    bool test;

    // Use this for initialization
    void Start () {
        renderTest = GetComponent<Renderer>();
        renderTest.material = matTest1;
        test = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        /*float lerp = Mathf.PingPong(Time.time, duration) / duration;
        if (lerp <= 0.9f)
        {
            Debug.Log(lerp);
            renderTest.material.Lerp(matTest1, matTest2, lerp);
        }*/
        if(test)
        {
            test = false;
            StartCoroutine(testBlendMaterial());
        }
	}

    IEnumerator testBlendMaterial()
    {
        yield return new WaitForSeconds(1.0f);
        renderTest.material.Lerp(matTest1, matTest2, 0.25f);

        yield return new WaitForSeconds(1.0f);
        renderTest.material.Lerp(matTest1, matTest2, 0.50f);

        yield return new WaitForSeconds(1.0f);
        renderTest.material.Lerp(matTest1, matTest2, 0.75f);

        yield return new WaitForSeconds(1.0f);
        renderTest.material.Lerp(matTest1, matTest2, 1.0f);
    }
}
