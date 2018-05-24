using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPodium : MonoBehaviour {

    SlimeDataContainer container;

    void Start()
    {
        container = SlimeDataContainer.instance;
        if (container == null)
            return;

        for (int i = 0; i < SlimeDataContainer.instance.nbPlayers; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            transform.GetComponentInChildren<Image>().color = container.playerColorsMenu[container.lastRanks[i]];
            Text[] texts = transform.GetComponentsInChildren<Text>();
            texts[0].text = "P" + (container.lastRanks[i] + 1);
            if (container.minigameType == MinigameType.Kart)
                texts[1].text = TimeFormatUtils.GetFormattedTime(container.lastScores[container.lastRanks[i]], TimeFormat.MinSecMil);
            else
                texts[1].text = container.lastScores[container.lastRanks[i]].ToString();

            ObtainMoneyBasedOnScore(container);
        }
    }


    void ObtainMoneyBasedOnScore(SlimeDataContainer _container)
    {
        float result = 0;
        if (_container.launchedFromMinigameScreen)
        {
            int[] minmax = MinigameDataUtils.GetMinMaxGoldTargetValues(_container.minigameType, _container.minigameVersion);
            for (int i = 0; i < _container.nbPlayers; i++)
            {
                int span = minmax[1] - minmax[0];
                float lerpParam;
                if (_container.minigameType == MinigameType.Kart)
                {
                    lerpParam = 1 - (_container.lastScores[i] - minmax[1]) / (float)span;
                }
                else
                {
                    lerpParam = (_container.lastScores[i] - minmax[0]) / (float)span;
                }

                float tmp = Mathf.Lerp(0, 50 + 25 * _container.nbPlayers, Mathf.Clamp(lerpParam, 0, 1));
                tmp = Mathf.Clamp(tmp, 0, 500);
                result += tmp;
            }
        }

        GameManager.Instance.GlobalMoney += (int)(result / _container.nbPlayers);
    }
}
