using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static float timerTutoText = 5.0f;

    static int[] mapTypeMaxValue =
        {
            50,            // CollectableType Evolution3 (Strength)
            50,            // CollectableType Evolution3 (Platformist)
            50,            // CollectableType Evolution4 (Agile)
            50,            // CollectableType Evolution4 (Ghost)
            9999,          // Max points
            30,              // Max Runes
            99999           // Max money
        };

    static int[] defaultValueCollectable =
    {
        30,                // CollectableType Evolution3 (Strength)
        50,                // CollectableType Evolution4 (Platformist)
        50,                // CollectableType Evolution4 (Agile)
        50,                // CollectableType Evolution4 (Ghost)
        5,                // points
        1,                  // Rune
        5                  // Money
    };

    /*
     * Returns the maximum value for a collectableType
     */
    public static int GetMaxValueForCollectable(CollectableType collectableType)
    {
        return mapTypeMaxValue[(int)collectableType];
    }

    /*
     * Returns true if collectableType is linked to an evolution
     */
    public static bool IsAnEvolutionCollectable(CollectableType collectableType)
    {
        return collectableType == CollectableType.StrengthEvolution1
            || collectableType == CollectableType.PlatformistEvolution1
            || collectableType == CollectableType.AgileEvolution1
            || collectableType == CollectableType.GhostEvolution1;
    }

    public static int GetDefaultCollectableValue(int collectableType)
    {
        return defaultValueCollectable[collectableType];
    }

    public static bool CheckEvolutionAndCollectableTypeCompatibility(CollectableType _collectableType, EvolutionComponent _currentComponent)
    {
        return (_collectableType == CollectableType.AgileEvolution1 && _currentComponent is EvolutionAgile)
            || (_collectableType == CollectableType.PlatformistEvolution1 && _currentComponent is EvolutionPlatformist)
            || (_collectableType == CollectableType.GhostEvolution1 && _currentComponent is EvolutionGhost)
            || (_collectableType == CollectableType.StrengthEvolution1 && _currentComponent is EvolutionStrength);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random _random = new System.Random();

        T value;
        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            // NextDouble returns a random number between 0 and 1.
            // ... It is equivalent to Math.random() in Java.
            int r = i + (int)(_random.NextDouble() * (n - i));
            value = list[r];
            list[r] = list[i];
            list[i] = value;
        }

        //RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
        //int n = list.Count;
        //while (n > 1)
        //{
        //    byte[] box = new byte[1];
        //    do provider.GetBytes(box);
        //    while (!(box[0] < n * (Byte.MaxValue / n)));
        //    int k = (box[0] % n);
        //    n--;
        //    T value = list[k];
        //    list[k] = list[n];
        //    list[n] = value;
        //}
    }

    public static void PopTutoText(string _actionText, KeyboardControlType _button, string _desc, Player _player)
    {
        GameObject tutoEvolution = GameObject.Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabTutoEvolution, GameManager.UiReference.transform);
        tutoEvolution.transform.position = _player.cameraReference.GetComponentInChildren<Camera>().WorldToScreenPoint(_player.transform.position)
                                        + Vector3.up * ((GameManager.Instance.PlayerStart.PlayersReference.Count > 2) ? 80.0f : 160.0f);

        tutoEvolution.transform.GetChild(0).GetComponent<Text>().text = _actionText;
        tutoEvolution.transform.GetChild(1).GetComponent<Image>().sprite = ResourceUtils.Instance.spriteUtils.GetKeyboardControlSprite(_button);
        tutoEvolution.transform.GetChild(2).GetComponent<Text>().text = _desc;

        if (GameManager.Instance.activeTutoTextForAll != null)
        {
            tutoEvolution.SetActive(false);
            _player.PendingTutoText = tutoEvolution;
        }
        else
        {
            if (_player.activeTutoText != null)
                _player.activeTutoText.SetActive(false);

            _player.activeTutoText = tutoEvolution;
            GameObject.Destroy(tutoEvolution, timerTutoText);
        }
    }

    public static void PopTutoText(string _actionText, ControlType _button, string _desc, Player _player)
    {
        GameObject tutoEvolution = GameObject.Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabTutoEvolution, GameManager.UiReference.transform);
        tutoEvolution.transform.position = _player.cameraReference.GetComponentInChildren<Camera>().WorldToScreenPoint(_player.transform.position)
                                        + Vector3.up * ((GameManager.Instance.PlayerStart.PlayersReference.Count > 2) ? 80.0f : 160.0f);

        tutoEvolution.transform.GetChild(0).GetComponent<Text>().text = _actionText;
        tutoEvolution.transform.GetChild(1).GetComponent<Image>().sprite = ResourceUtils.Instance.spriteUtils.GetControlSprite(_button);
        tutoEvolution.transform.GetChild(2).GetComponent<Text>().text = _desc;

        if (GameManager.Instance.activeTutoTextForAll != null)
        {
            tutoEvolution.SetActive(false);
            _player.PendingTutoText = tutoEvolution;
        }
        else
        {
            if (_player.activeTutoText != null)
                _player.activeTutoText.SetActive(false);

            _player.activeTutoText = tutoEvolution;
            GameObject.Destroy(tutoEvolution, timerTutoText);
        }

    }

    public static void PopTutoTextForAll(string _text, string _textCmd = "")
    {
        GameObject tutoText = GameObject.Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabTutoTextForAll, GameManager.UiReference.transform);
        tutoText.transform.localPosition = Vector3.zero;

        tutoText.GetComponent<Text>().text = _text;
        tutoText.transform.GetChild(0).GetComponent<Text>().text = _textCmd;
        if (GameManager.Instance.activeTutoTextForAll != null)
            GameManager.Instance.activeTutoTextForAll.SetActive(false);

        GameManager.Instance.activeTutoTextForAll = tutoText;
        GameObject.Destroy(tutoText, timerTutoText);
    }

    public static float Abs(float _float)
    {
        return (_float < 0.0f) ? -_float : _float;
    }

    public static void CopyUnderwaterStateDataToPausedState(UnderwaterState _underwaterState, PausedState _pausedState)
    {
        _pausedState.waterLevel = _underwaterState.waterLevel;
        _pausedState.hasReachedTheSurface = _underwaterState.hasReachedTheSurface;
        _pausedState.hasStartedGoingUp = _underwaterState.hasStartedGoingUp;
    }

    public static void GetUnderwaterStateDataFromPausedState(UnderwaterState _underwaterState, PausedState _pausedState)
    {
        _underwaterState.waterLevel = _pausedState.waterLevel;
        _underwaterState.hasReachedTheSurface = _pausedState.hasReachedTheSurface;
        _underwaterState.hasStartedGoingUp = _pausedState.hasStartedGoingUp;
    }


    public static String GetRetryMessage(MessageTypeMinigame messageType)
    {
        switch (messageType)
        {
            case MessageTypeMinigame.AreYouReady:
                return "Ready ?";
            case MessageTypeMinigame.AreyoureadyPlayer1:
                return "P1 Ready ?";
            case MessageTypeMinigame.AreyoureadyPlayer2:
                return "P2 Ready ?";
            case MessageTypeMinigame.Retry:
                return "Retry ?";
            default:
                return "?????";
        }
    }

    public static string[] MinigameTypeStr = { "Coloring", "Runner", "Shapes", "Kart", "Clash", "Food" };
    public static string[] MinigameTypeSprite = { "screenshotColorType", "screenshotMinigameRunner", "screenshotColorShape", "screenshotKartType", "screenshotClashType", "screenshotMinigameFood" };
}