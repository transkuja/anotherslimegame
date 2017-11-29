using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asuppr : MonoBehaviour
{


    private void OnPreRender()
    {

        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            if(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Renderer>().isVisible)
            {
                GameManager.Instance.PlayerStart.PlayersReference[i].transform.GetChild(3).GetChild(i).GetComponentInChildren<Text>().gameObject.transform.LookAt(transform);
            }
            else
            {
                GameManager.Instance.PlayerStart.PlayersReference[i].transform.GetChild((int)PlayerChildren.Canvas).GetChild(i).gameObject.SetActive(false);

            }
            //player.GetComponentInChildren<Text>().gameObject.transform.parent.LookAt(transform);

            //player.transform.GetChild(3).GetComponentInChildren<Text>().gameObject.transform.LookAt(transform.parent.GetComponentInChildren<Cinemachine.CinemachineFreeLook>().transform);
            /*GameObject objec = player.transform.GetChild(2).GetComponentInChildren<Text>().gameObject.transform.gameObject;
            Debug.Log("");*/
        }
    }
    private void OnPostRender()
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            if (GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Renderer>().isVisible)
            {
                GameManager.Instance.PlayerStart.PlayersReference[i].transform.GetChild(3).GetChild(i).GetComponentInChildren<Text>().gameObject.transform.LookAt(transform);
            }
            else
            {
                GameManager.Instance.PlayerStart.PlayersReference[i].transform.GetChild((int)PlayerChildren.Canvas).GetChild(i).gameObject.SetActive(false);

            }
            //GameObject player = GameManager.Instance.PlayerStart.PlayersReference[i];
            //player.GetComponentInChildren<Text>().gameObject.transform.parent.LookAt(transform);


            /*player.transform.GetChild(3).GetComponentInChildren<Text>().gameObject.transform.LookAt(transform.parent.GetComponentInChildren<Cinemachine.CinemachineFreeLook>().transform);
            GameObject objec = player.transform.GetChild(2).GetComponentInChildren<Text>().gameObject.transform.gameObject;
            Debug.Log("");*/
        }
    }
}