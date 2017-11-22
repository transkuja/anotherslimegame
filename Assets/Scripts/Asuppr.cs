using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asuppr : MonoBehaviour {
    bool test =true;
    void OnPreRender()
    {
        if (test)
        {
            Debug.Log("cambegin" + gameObject.name);
        }
        foreach (GameObject player in GameManager.Instance.PlayerStart.PlayersReference)
        {
            //player.GetComponentInChildren<Text>().gameObject.transform.parent.LookAt(transform);

            player.transform.GetChild(2).GetComponentInChildren<Text>().gameObject.transform.LookAt(transform.parent.GetComponentInChildren<Cinemachine.CinemachineFreeLook>().transform);
            /*GameObject objec = player.transform.GetChild(2).GetComponentInChildren<Text>().gameObject.transform.gameObject;
            Debug.Log("");*/
            //textPlayer.transform.position = new Vector3(go.transform.position.x, go.transform.position.y + 5, go.transform.position.z);

        }
    }
    private void OnPostRender()
    {
        if (test)
        {
            Debug.Log("camEnd" + gameObject.name);
            test = false;
        }
    }
}
