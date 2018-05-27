using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveDoor : MonoBehaviour {

    private DoorActivable door;
    private bool bActive = false;

	// Use this for initialization
	void Start () {
        door = GetComponent<DoorActivable>();

        if (DatabaseManager.Db.NbRunes > 1)
        {
            door.Active(true);
            bActive = true;
            return;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(!bActive)
        {
            if (DatabaseManager.Db.NbRunes > 1)
            {
                door.Active(true);
                bActive = true;
                return;
            }
        }

    }
}
