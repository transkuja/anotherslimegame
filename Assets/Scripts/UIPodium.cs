using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPodium : MonoBehaviour {

    SlimeDataContainer container;
    [SerializeField]
    Material runeMat;

    float replayScreenDelay = 2.5f;
    Podium podium;

    void Start()
    {
        podium = FindObjectOfType<Podium>();
        container = SlimeDataContainer.instance;
        if (container == null)
            return;

        for (int i = 0; i < SlimeDataContainer.instance.nbPlayers; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            transform.GetChild(i).GetComponentInChildren<Image>().color = container.playerColorsMenu[container.lastRanks[i]];
            Text[] texts = transform.GetChild(i).GetComponentsInChildren<Text>();
            texts[0].text = "P" + (container.lastRanks[i] + 1);
            if (container.minigameType == MinigameType.Kart)
                texts[1].text = TimeFormatUtils.GetFormattedTime(container.lastScores[container.lastRanks[i]], TimeFormat.MinSecMil);
            else
                texts[1].text = container.lastScores[container.lastRanks[i]].ToString();

            ObtainMoneyBasedOnScore(container);
        }

        DatabaseClass.RuneData runeData = DatabaseManager.Db.GetRuneFromMinigame(container.minigameType, container.minigameVersion);

        if (runeData == null || container.launchedFromMinigameScreen)
        {
            StartCoroutine(ShowReplayScreen());
        }
        else
        {
            RuneObjective runeObjective = runeData.objective;

            if (!runeData.isUnlocked)
            {
                transform.GetChild(4).gameObject.SetActive(true);
                podium.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);

                Text[] runeObjectiveTexts = transform.GetChild(4).GetComponentsInChildren<Text>();

                // Initialize rune objective screen values
                if (runeObjective == RuneObjective.Points)
                {
                    runeObjectiveTexts[1].text = (MinigameDataUtils.GetRuneScoreObjective(container.minigameType, container.minigameVersion) * container.nbPlayers).ToString();
                    runeObjectiveTexts[3].text = (container.lastScores[0] + container.lastScores[1]).ToString();
                }
                else if (runeObjective == RuneObjective.Time)
                {
                    if (container.minigameType == MinigameType.Kart)
                    {
                        runeObjectiveTexts[1].text = TimeFormatUtils.GetFormattedTime(MinigameDataUtils.GetRuneScoreObjective(container.minigameType, container.minigameVersion), TimeFormat.MinSecMil);

                        //TODO: Skip to failed screen
                        bool atLeastOnePlayerHasFinishedTheRace = false;
                        for (int i = 0; i < container.nbPlayers; i++)
                        {
                            if (container.lastScores[i] > 0.0f)
                            {
                                atLeastOnePlayerHasFinishedTheRace = true;
                                break;
                            }
                        }
                        if (!atLeastOnePlayerHasFinishedTheRace)
                            runeObjectiveTexts[3].text = "Try Again";
                        else
                        {
                            if (container.lastScores[0] > 0.0f && container.lastScores[1] > 0.0f)
                                runeObjectiveTexts[3].text = TimeFormatUtils.GetFormattedTime(Mathf.Min(container.lastScores[0], container.lastScores[1]), TimeFormat.MinSecMil);
                            else
                            {
                                if (container.lastScores[0] > 0.0f)
                                    runeObjectiveTexts[3].text = TimeFormatUtils.GetFormattedTime(container.lastScores[0], TimeFormat.MinSecMil);
                                else
                                    runeObjectiveTexts[3].text = TimeFormatUtils.GetFormattedTime(container.lastScores[1], TimeFormat.MinSecMil);
                            }
                        }
                    }
                }

                if (CheckRuneObjective(container))
                {
                    runeObjectiveTexts[3].color = Color.green;
                    StartCoroutine(ActivateRune());
                }
                else
                {
                    runeObjectiveTexts[3].color = Color.red;
                    StartCoroutine(ShowReplayScreen());
                }
            }
            else
            {
                StartCoroutine(ShowReplayScreen());
            }
        }
    }

    IEnumerator ActivateRune()
    {
        yield return new WaitForSeconds(2.0f);
        podium.transform.GetChild(0).GetChild(1).GetComponentInChildren<MeshRenderer>().material = runeMat;
        podium.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(true);
        DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(DatabaseManager.Db.GetRuneFromMinigame(container.minigameType, container.minigameVersion).Id, true);
        GameManager.Instance.Runes += 1;

        StartCoroutine(ShowReplayScreen());
    }

    IEnumerator ShowReplayScreen()
    {
        yield return new WaitForSeconds(replayScreenDelay);
        transform.GetChild(5).gameObject.SetActive(true);
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

        GameObject feedback = Instantiate(ResourceUtils.Instance.feedbacksManager.scorePointsPrefab, null);
        if (result >= 0)
        {
            feedback.GetComponentInChildren<Outline>().effectColor = Color.green;
            feedback.GetComponentInChildren<Text>().text = "+ ";
        }

        feedback.transform.GetChild(0).position = Camera.main.WorldToScreenPoint(GameManager.UiReference.transform.GetChild(0).position)+ (Vector3.right * 300)+(Vector3.down * 60);
     
        feedback.GetComponentInChildren<Text>().text += Utils.Abs(result).ToString();
        feedback.GetComponentInChildren<Text>().enabled = true;

        GameManager.Instance.GlobalMoney += (int)(result / _container.nbPlayers);
    }


    bool CheckRuneObjective(SlimeDataContainer container)
    {
        if (container.minigameType == MinigameType.Kart)
            return CheckRuneObjectiveKart(container);
        if (container.minigameType == MinigameType.Clash)
            return CheckRuneObjectiveClash(container);

        return CheckRuneObjectiveDefault(container);
    }

    bool CheckRuneObjectiveKart(SlimeDataContainer container)
    {
        return 
            (container.lastScores[0] <= MinigameDataUtils.GetRuneScoreObjective(MinigameType.Kart, container.minigameVersion) && container.lastScores[0] > 0) 
                || (container.nbPlayers > 1 && container.lastScores[1] <= MinigameDataUtils.GetRuneScoreObjective(MinigameType.Kart, container.minigameVersion) && container.lastScores[1] > 0);
    }

    bool CheckRuneObjectiveClash(SlimeDataContainer container)
    {
        int pointsObjective = 0;
        int curScore = 0;

        if (container.minigameVersion != 4)
        {
            for (int i = 0; i < container.nbPlayers; i++)
            {
                curScore += (int)container.lastScores[i];
                pointsObjective += MinigameDataUtils.GetRuneScoreObjective(MinigameType.Clash, container.minigameVersion);
            }
        }
        else
        {
            pointsObjective += MinigameDataUtils.GetRuneScoreObjective(MinigameType.Clash, container.minigameVersion);
            if (container.nbPlayers == 2)
                curScore = Mathf.Max((int)container.lastScores[0], (int)container.lastScores[1]);
            else
                curScore = (int)container.lastScores[0];
        }

        return curScore >= pointsObjective;
    }

    bool CheckRuneObjectiveDefault(SlimeDataContainer container)
    {
        int pointsObjective = 0;
        int curScore = 0;
        for (int i = 0; i < container.nbPlayers; i++)
        {
            curScore += (int)container.lastScores[i];
            pointsObjective += MinigameDataUtils.GetRuneScoreObjective(container.minigameType, container.minigameVersion);
        }
        return curScore >= pointsObjective;
    }
}
