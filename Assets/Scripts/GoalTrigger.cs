using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour {

    public GameObject rune;

    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<TheBall>())
        {
            if(! DatabaseManager.Db.IsUnlock<DatabaseClass.RuneData>(rune.GetComponent<Collectable>().idRune))
                DropRuneOnTheGround();
        }
    }

    public void DropRuneOnTheGround()
    {
        rune.SetActive(true);
        rune.transform.position += Vector3.up;
    }
}
