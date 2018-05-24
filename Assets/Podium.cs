using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Podium : MonoBehaviour {

    SlimeDataContainer container;

	void Start () {
        container = SlimeDataContainer.instance;

        for (int i = 0; i < SlimeDataContainer.instance.nbPlayers; i++)
        {
            PlayerCosmetics playerCosmetics = transform.GetChild(i).GetComponentInChildren<PlayerCosmetics>();

            if (GameManager.Instance.DataContainer.colorFadeSelected[i])
                playerCosmetics.ColorFadeType = ColorFadeType.Basic;
            else
                playerCosmetics.SetUniqueColor(GameManager.Instance.DataContainer.selectedColors[i]);
            playerCosmetics.FaceType = GameManager.Instance.DataContainer.selectedFaces[i];

            if (playerCosmetics.transform.GetComponentInChildren<CustomizableSockets>() != null)
            {
                Transform customizableParent = playerCosmetics.transform.GetComponentInChildren<CustomizableSockets>().transform;

                // Init mustaches //
                playerCosmetics.Mustache = GameManager.Instance.DataContainer.mustachesSelected[i];

                // Init ears //
                playerCosmetics.Ears = GameManager.Instance.DataContainer.earsSelected[i];

                // Init hats //
                playerCosmetics.Hat = GameManager.Instance.DataContainer.hatsSelected[i];

                // Init forehead //
                playerCosmetics.Forehead = GameManager.Instance.DataContainer.foreheadsSelected[i];

                // Init skin //
                playerCosmetics.Skin = GameManager.Instance.DataContainer.skinsSelected[i];

                // Init chin //
                playerCosmetics.Chin = GameManager.Instance.DataContainer.chinsSelected[i];

                // Init accessory //
                playerCosmetics.Accessory = GameManager.Instance.DataContainer.accessoriesSelected[i];
            }
        }
    }

}
