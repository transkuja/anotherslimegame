using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class MinigameColorFloorGP : MonoBehaviour {

    ColorFloorPickupHandler pickupHandler;
    public ColorFloorGameMode gameMode;

    uint nbPlayers;
    GamePadState[] controllerStates = new GamePadState[4];

    GameObject[] playerCurrentPositions = new GameObject[4];

    [SerializeField]
    LayerMask restrainedMovementLayerMask;

    private IEnumerator Start()
    {
        pickupHandler = GetComponent<ColorFloorPickupHandler>();
        nbPlayers = GameManager.Instance.ActivePlayersAtStart;

        if (!gameMode.freeMovement)
        {
            for (int i = 0; i < nbPlayers; i++)
                playerCurrentPositions[i] = gameMode.RestrainedMovementStarters[i];

            while (true)
            {
                yield return new WaitForSeconds(gameMode.restrainedMovementTick);
                Move();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponentInParent<PlayerController>() != null)
        {
            PlayerController pc = collision.transform.GetComponentInParent<PlayerController>();
            Collider thisCollider = collision.contacts[0].thisCollider;
            if (thisCollider.transform.childCount > 0)
            {
                ColorFloorPickUp pickupComponent = thisCollider.transform.GetChild(0).GetComponent<ColorFloorPickUp>();
                if (pickupComponent.pickupType == ColorFloorPickUpType.Score)
                    ColorFloorHandler.ScorePoints((int)pc.playerIndex);

                Destroy(pickupComponent.gameObject);
                pickupHandler.pickupSpawned--;
            }

            ColorFloorHandler.RegisterFloor((int)pc.playerIndex, thisCollider);
            thisCollider.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", GameManager.Instance.PlayerStart.colorPlayer[(int)pc.playerIndex]);
        }
    }

    void Move()
    {
        for (int i = 0; i < nbPlayers; i++)
        {
            controllerStates[i] = GamePad.GetState((PlayerIndex)i);

            float x = controllerStates[i].ThumbSticks.Left.X;
            float y = controllerStates[i].ThumbSticks.Left.Y;
            if (Utils.Abs(x) < 0.1f && Utils.Abs(y) < 0.1f)
                continue;

            Vector3 dir = (Utils.Abs(x) > Utils.Abs(y)) ? Vector3.right * x : Vector3.forward * y;
            dir.Normalize();

            Transform from = GameManager.Instance.PlayerStart.PlayersReference[i].transform;
            RaycastHit hit;

            if (Physics.Raycast(from.position + Vector3.up, dir + Vector3.down*0.25f, out hit, 5.0f, restrainedMovementLayerMask))
            {
                if (!IsDestinationOccupied(hit.collider.gameObject))
                {
                    playerCurrentPositions[i] = hit.collider.gameObject;

                    // TODO:Anim + proper move
                    GameObject curPlayer = GameManager.Instance.PlayerStart.PlayersReference[i];
                    curPlayer.transform.LookAt(hit.collider.transform);
                    curPlayer.transform.position = hit.collider.transform.position;
                }
            }
            
            
        }
    }

    bool IsDestinationOccupied(GameObject _destination)
    {
        for (int i = 0; i < nbPlayers; i++)
        {
            if (playerCurrentPositions[i] == _destination)
                return true;
        }
        return false;
    }
}
