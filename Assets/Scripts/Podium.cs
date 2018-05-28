using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Podium : MonoBehaviour {

    SlimeDataContainer container;

	void Start () {
        container = SlimeDataContainer.instance;
        if (container == null)
            return;

        for (int i = 0; i < SlimeDataContainer.instance.nbPlayers; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            PlayerCosmetics playerCosmetics = transform.GetChild(i).GetComponentInChildren<PlayerCosmetics>();

            int indexRanked = container.lastRanks[i];

            if (GameManager.Instance.DataContainer.colorFadeSelected[indexRanked])
                playerCosmetics.ColorFadeType = ColorFadeType.Basic;
            else
                playerCosmetics.SetUniqueColor(GameManager.Instance.DataContainer.selectedColors[indexRanked]);
            playerCosmetics.FaceType = GameManager.Instance.DataContainer.selectedFaces[indexRanked];

            if (playerCosmetics.transform.GetComponentInChildren<CustomizableSockets>() != null)
            {
                Transform customizableParent = playerCosmetics.transform.GetComponentInChildren<CustomizableSockets>().transform;

                // Init mustaches //
                playerCosmetics.Mustache = GameManager.Instance.DataContainer.mustachesSelected[indexRanked];

                // Init ears //
                playerCosmetics.Ears = GameManager.Instance.DataContainer.earsSelected[indexRanked];

                // Init hats //
                playerCosmetics.Hat = GameManager.Instance.DataContainer.hatsSelected[indexRanked];

                // Init forehead //
                playerCosmetics.Forehead = GameManager.Instance.DataContainer.foreheadsSelected[indexRanked];

                // Init skin //
                playerCosmetics.Skin = GameManager.Instance.DataContainer.skinsSelected[indexRanked];

                // Init chin //
                playerCosmetics.Chin = GameManager.Instance.DataContainer.chinsSelected[indexRanked];

                // Init accessory //
                playerCosmetics.Accessory = GameManager.Instance.DataContainer.accessoriesSelected[indexRanked];
            }
        }
    }

}
