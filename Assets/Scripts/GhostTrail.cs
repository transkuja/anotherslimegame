using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour {

    // WARNING: May get tricky if it isn't recreated between scenes
    static Dictionary<PlayerController, int> dicPlayersInTriggers;
    public PlayerController owner;
    public List<PlayerController> currentlyInMe;

    public static Dictionary<PlayerController, int> DicPlayersInTriggers
    {
        get
        {
            return dicPlayersInTriggers;
        }

        set
        {
            dicPlayersInTriggers = value;
        }
    }

    void Start()
    {
        currentlyInMe = new List<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if(player && player != owner)
        {
            if (!currentlyInMe.Contains(player))
            {
                currentlyInMe.Add(player);
                if (DicPlayersInTriggers == null)
                DicPlayersInTriggers = new Dictionary<PlayerController, int>();
                if(DicPlayersInTriggers.ContainsKey(player))
                {
                    DicPlayersInTriggers[player]++;
                }
                else
                {
                    DicPlayersInTriggers.Add(player, 1);
                }                
            }

            player.PlayerState = player.restrainedByGhostState;
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player && player != owner)
        {
            if(currentlyInMe.Contains(player))
            {
                currentlyInMe.Remove(player);
            }
            //Need a function
            if (DicPlayersInTriggers != null && DicPlayersInTriggers.ContainsKey(player))
            {
                DicPlayersInTriggers[player]--;
                if(DicPlayersInTriggers[player] <= 0)
                {
                    DicPlayersInTriggers[player] = 0;
                    player.PlayerState = player.freeState;
                }
            }
            else
            {
                player.PlayerState = player.freeState;
            }
        }
    }

    void OnDestroy()
    {
        if(currentlyInMe.Count > 0)
        {
            for(int i = 0; i < currentlyInMe.Count; i++)
            {
                PlayerController player = currentlyInMe[i];
                if(player != null)
                {
                    if (DicPlayersInTriggers != null && DicPlayersInTriggers.ContainsKey(player))
                    {
                        DicPlayersInTriggers[player]--;
                        if (DicPlayersInTriggers[player] <= 0)
                        {
                            DicPlayersInTriggers[player] = 0;
                            player.PlayerState = player.freeState;
                        }
                    }
                    else
                    {
                        player.PlayerState = player.freeState;
                    }
                }

            }
        }
    }
}
