using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class MinigameColorFloorGP : MonoBehaviour {

    public AnimationCurve jumpCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f, 4.9f, 4.9f), new Keyframe(0.5f, 1.0f, 0f, 0f), new Keyframe(1.0f, 0.0f, -4.9f, -4.9f));
    public float jumpHeight = 5f;
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
                if (GameManager.CurrentState != GameState.Normal)
                {
                    yield return null;
                    continue;
                }

                yield return new WaitForSeconds(gameMode.restrainedMovementTick/2);
                Move(true);
                yield return new WaitForSeconds(gameMode.restrainedMovementTick/2);
                Move();
            }
        }
    }

    void Move(bool _moveEvoAgileOnly = false)
    {
        if (GameManager.CurrentState != GameState.Normal)
            return;

        for (int i = 0; i < nbPlayers; i++)
        {
            bool hasAgileEvo = GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<EvolutionAgile>() != null;
            float speedMultiplier = 0f;
            if (hasAgileEvo)
            {
                speedMultiplier = 2.0f;
            }
            else
            {
                if (_moveEvoAgileOnly)
                    continue;

                speedMultiplier = 1.0f;
            }
            GameObject curPlayer = GameManager.Instance.PlayerStart.PlayersReference[i];
            curPlayer.GetComponent<PlayerCharacter>().Anim.SetTrigger("Jump");

            //StopCoroutine();
            StartCoroutine(Jump(curPlayer, speedMultiplier));
            controllerStates[i] = GamePad.GetState((PlayerIndex)i);

            float x = controllerStates[i].ThumbSticks.Left.X;
            float y = controllerStates[i].ThumbSticks.Left.Y;
            if (Utils.Abs(x) < 0.1f && Utils.Abs(y) < 0.1f)
            {
                continue;
            }

            Vector3 dir = (Utils.Abs(x) > Utils.Abs(y)) ? Vector3.right * x : Vector3.forward * y;
            dir.Normalize();
            RaycastHit hit;
            Vector3 fromPos = GameManager.Instance.PlayerStart.PlayersReference[i].transform.position;
            fromPos.y = 1f;
            if (Physics.Raycast(fromPos + Vector3.up, dir + Vector3.down*0.25f, out hit, 5.0f, restrainedMovementLayerMask))
            {
                if (!IsDestinationOccupied(hit.collider.gameObject))
                {
                    playerCurrentPositions[i] = hit.collider.gameObject;
                    Vector3 lookAtPos = hit.collider.transform.position;
                    lookAtPos.y = curPlayer.transform.position.y;
                    curPlayer.transform.LookAt(lookAtPos, Vector3.up);
                    StartCoroutine(ApplyMovement(i, curPlayer, hit.collider.transform, speedMultiplier));
                }
            }
        }
    }

    IEnumerator Jump(GameObject _player, float _speedMultiplier)
    {
        float timer = 0.0f;
        float maxTime = gameMode.restrainedMovementTick / _speedMultiplier;
        float startHeight = _player.transform.position.y;
        while (timer <= maxTime)
        {
            _player.GetComponent<PlayerCharacter>().Anim.SetFloat("JumpTime", timer/maxTime);
            Vector3 pos = _player.transform.position;
            timer += Time.deltaTime;
            pos.y = startHeight + jumpCurve.Evaluate(timer / maxTime) * jumpHeight;
            _player.transform.position = pos;
            yield return null;
        }
    }

    IEnumerator ApplyMovement(int playerId, GameObject _player, Transform _target, float _speedMultiplier)
    {
        float timer = 0.0f;
        float maxTime = gameMode.restrainedMovementTick / _speedMultiplier;
        Vector3 startPos = _player.transform.position;
        while( timer <= maxTime)
        {
            timer += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(startPos, _target.position, timer / maxTime);
            newPos.y = _player.transform.position.y;
            _player.transform.position = newPos;
            yield return null;
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
