using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitTeleporter : MonoBehaviour {
    public CollectableType evolutionType;
    bool isTeleporterActive = false;
    Color startColor;
    bool teleportToMinigame = false;
    string minigameSceneToTeleportTo = "";
    MeshRenderer meshRenderer;

    // TODO: replace by an animation?
    float lerpValueAnim;
    Vector3 originPosition;
    Vector3 endPosition;

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

    public void TeleportToMinigame(string sceneName)
    {
        teleportToMinigame = true;
        minigameSceneToTeleportTo = sceneName;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Wait for animation to end
        if (teleportToMinigame && lerpValueAnim < 1.0f)
            return;

        // Arrived at destination
        if (collision.transform.GetComponent<Player>() != null && collision.transform.GetComponent<Player>().hasBeenTeleported)
        {
            collision.transform.GetComponent<Player>().hasBeenTeleported = false;
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
        if (isTeleporterActive)
        {
            PlatformGameplay gameplayComponent = GetComponent<PlatformGameplay>();
            float lerpValue = (gameplayComponent.delayBetweenMovements - gameplayComponent.DelayTimer) / gameplayComponent.delayBetweenMovements;
            meshRenderer.material.SetColor("_EmissionColor", Color.Lerp(startColor, Color.red, lerpValue));
            if (lerpValue >= 1.0f)
            {
                isTeleporterActive = false;
                if (teleportToMinigame)
                    LoadMinigame();
                else
                    Invoke("ResetPlatform", 0.1f); // WARNING! Should be call in any cases if we dont load scenes
            }
        }

        if (teleportToMinigame && lerpValueAnim < 1.0f)
        {
            lerpValueAnim += Time.deltaTime;
            transform.position = Vector3.Lerp(originPosition, endPosition, lerpValueAnim);
        }
    }

    void ResetPlatform()
    {
        GetComponent<PlatformGameplay>().isATeleporter = false;
        if (meshRenderer != null)
            meshRenderer.material.SetColor("_EmissionColor", startColor);
    }

    void LoadMinigame()
    {
        List<GameObject> players = GameManager.Instance.PlayerStart.PlayersReference;
        for (int i = 0; i < players.Count; i++)
        {
            GameManager.Instance.playerCollectables[i] = players[i].GetComponent<Player>().Collectables;
        }
        SceneManager.LoadScene(minigameSceneToTeleportTo);
    }

    private void OnCollisionExit(Collision collision)
    {
        // Wait for animation to end
        if (teleportToMinigame && lerpValueAnim < 1.0f)
            return;

        if (isTeleporterActive && 
            (Utils.CheckEvolutionAndCollectableTypeCompatibility(evolutionType, collision.transform.GetComponent<EvolutionComponent>())
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


}
