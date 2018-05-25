using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitTeleporter : MonoBehaviour {

    public CollectableType evolutionType;

    bool isTeleporterActive = false;
    Color startColor;
    public bool teleportToMinigame = false;
    string minigameSceneToTeleportTo = "";
    MeshRenderer meshRenderer;

    // TODO: replace by an animation?
    float lerpValueAnim;
    Vector3 originPosition;
    Vector3 endPosition;

    [SerializeField]
    Transform respawnFromMinigame;

    GameObject[] refCanvas = new GameObject[2];
    GameObject[] BButtonShown = new GameObject[2];

    private void Start()
    {
        if (GetComponentInChildren<MeshRenderer>() != null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            startColor = ResourceUtils.Instance.teleporterEmissiveColor;
            meshRenderer.material.SetColor("_EmissionColor", ResourceUtils.Instance.teleporterEmissiveColor);
        }
    }

    private void OnEnable()
    {
        if (teleportToMinigame)
        {
            if (GetComponentInChildren<MeshRenderer>() != null)
            {
                meshRenderer = GetComponentInChildren<MeshRenderer>();
                startColor = ResourceUtils.Instance.teleporterEmissiveColor;
                meshRenderer.material.SetColor("_EmissionColor", ResourceUtils.Instance.teleporterEmissiveColor);
            }
            originPosition = transform.position;
            endPosition = originPosition + Vector3.up * 4.0f;
            lerpValueAnim = 0.0f;
        }
    }

    public void CreateButtonFeedback(int _playerIndex)
    {
        refCanvas[_playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabCanvasWithUiCameraAdapter, transform);
        refCanvas[_playerIndex].GetComponent<UICameraApdater>().PlayerIndex = _playerIndex;
        refCanvas[_playerIndex].transform.localPosition += Vector3.up * 5.0f;
        BButtonShown[_playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabBButton, refCanvas[_playerIndex].transform);
        BButtonShown[_playerIndex].SetActive(true);

        refCanvas[_playerIndex].layer = LayerMask.NameToLayer((_playerIndex == 0) ? "CameraP1" : "CameraP2");
    }

    public void DestroyButtonFeedback(int _playerIndex)
    {
        Destroy(BButtonShown[_playerIndex]);
        Destroy(refCanvas[_playerIndex]);
    }

    public void TeleportToMinigame(string sceneName)
    {
        teleportToMinigame = true;
        minigameSceneToTeleportTo = sceneName;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (teleportToMinigame)
            return;

        // Arrived at destination
        if (collision.transform.GetComponent<Player>() != null && collision.transform.GetComponent<Player>().HasBeenTeleported)
        {
            collision.transform.GetComponent<Player>().HasBeenTeleported = false;
            return;
        }

        // TODO; dev here the day we want multiple evolution component behaviour
        if (Utils.CheckEvolutionAndCollectableTypeCompatibility(evolutionType, collision.transform.GetComponent<EvolutionComponent>())
            || teleportToMinigame)
        {
            GetComponent<PlatformGameplay>().isATeleporter = true;
            isTeleporterActive = true;
        }
    }

    private void Update()
    {
        if (isTeleporterActive && !teleportToMinigame)
        {
            PlatformGameplay gameplayComponent = GetComponent<PlatformGameplay>();
            float lerpValue = (gameplayComponent.delayBetweenMovements - gameplayComponent.DelayTimer) / gameplayComponent.delayBetweenMovements;
            meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(startColor, Color.red, lerpValue));
            if (lerpValue >= 1.0f)
            {
                isTeleporterActive = false;
                Invoke("ResetPlatform", 0.1f); // WARNING! Should be call in any cases if we dont load scenes
            }
        }

        if (teleportToMinigame)
        {
            if (lerpValueAnim < 1.0f)
            {
                lerpValueAnim += Time.deltaTime;
                transform.position = Vector3.Lerp(originPosition, endPosition, lerpValueAnim);
            }
            else
            {
                isTeleporterActive = true;
            }
        }
    }

    void ResetPlatform()
    {
        GetComponent<PlatformGameplay>().isATeleporter = false;
        if (meshRenderer != null)
            meshRenderer.material.SetColor("_EmissionColor", startColor);
    }

    public void LoadMinigame()
    {
        //Just in case
        ReturnToNormalState();

        List<GameObject> players = GameManager.Instance.PlayerStart.PlayersReference;
        for (int i = 0; i < players.Count; i++)
        {
            Player currentPlayer = players[i].GetComponent<Player>();
            GameManager.Instance.playerCostAreaTutoShown[i] = currentPlayer.costAreaTutoShown;
            GameManager.Instance.playerEvolutionTutoShown[i] = currentPlayer.evolutionTutoShown;
        }
        if (respawnFromMinigame)
        {
            GameManager.Instance.savedPositionInHub = respawnFromMinigame.position;
            GameManager.Instance.savedRotationInHub = respawnFromMinigame.rotation;
        }
        else
        {
            GameManager.Instance.savedPositionInHub = transform.position + transform.forward * -6.0f;
            GameManager.Instance.savedRotationInHub = Quaternion.LookRotation(-transform.forward);
        }
        LevelLoader.LoadLevelWithFadeOut(minigameSceneToTeleportTo);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (teleportToMinigame)
            return;

        if (isTeleporterActive && (Utils.CheckEvolutionAndCollectableTypeCompatibility(evolutionType, collision.transform.GetComponent<EvolutionComponent>())
            || teleportToMinigame))
        {
            if (meshRenderer != null)
            {
                meshRenderer.material.SetColor("_EmissionColor", startColor);
            }
            isTeleporterActive = false;
            GetComponent<PlatformGameplay>().isATeleporter = false;
        }
    }


    public void ReturnToNormalState()
    {
        GameManager.ChangeState(GameState.Normal);
    }

}
