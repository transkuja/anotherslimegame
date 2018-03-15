using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public static class MinigameDataUtils
{
    public static string GetTitle(GameMode _curGameMode)
    {
        GameMode curGameMode = _curGameMode;
        if (curGameMode is ColorFloorGameMode)
        {
            return "Color Floor";
        }
        else if (curGameMode is KartGameMode)
        {
            return "Kart";
        }
        else if (curGameMode is Runner3DGameMode)
        {
            return "Runner";
        }
        else if (curGameMode is FruitGameMode)
        {
            return "";
        }
        else if (curGameMode is PushGameMode)
        {
            return "Arena Fighting";
        }
        return "";
    }

    public static string GetTitle(string _minigameId)
    {
        // "MinigameAntho", "MinigameKart", "MinigamePush", "Minigame3dRunner"
        if (_minigameId == "MinigameAntho")
        {
            return "Floor Coloring";
        }
        else if (_minigameId == "MinigameKart")
        {
            return "Kart";
        }
        else if (_minigameId == "Minigame3dRunner")
        {
            return "Runner";
        }
        else if (_minigameId == "Fruits")
        {
            return "";
        }
        else if (_minigameId == "MinigamePush")
        {
            return "Arena Fighting";
        }
        return "";
    }

    public static string GetDescription(GameMode _curGameMode)
    {
        GameMode curGameMode = _curGameMode;
        if (curGameMode is ColorFloorGameMode)
        {
            return "Color the floor and collect score pickup to earn points. Be the one with the most points to win!";
        }
        else if (curGameMode is KartGameMode)
        {
            return "Win the race.";
        }
        else if (curGameMode is Runner3DGameMode)
        {
            return "Run";
        }
        else if (curGameMode is FruitGameMode)
        {
            return "";
        }
        else if (curGameMode is PushGameMode)
        {
            return "Steal other players' coins!";
        }
        return "";
    }

    public static List<ControlDetails> GetControls(GameMode _curGameMode)
    {
        GameMode curGameMode = _curGameMode;
        List<ControlDetails> controls = new List<ControlDetails>();

        if (curGameMode is ColorFloorGameMode)
        {
            controls.Add(new ControlDetails(ControlType.Movement));
            controls.Add(new ControlDetails(ControlType.Jump));
            controls.Add(new ControlDetails(ControlType.Action, "Dash forward with X"));
        }
        else if (curGameMode is KartGameMode)
        {
            controls.Add(new ControlDetails(ControlType.DrivingForward));
            controls.Add(new ControlDetails(ControlType.DrivingReverse));
            controls.Add(new ControlDetails(ControlType.Steering));
            controls.Add(new ControlDetails(ControlType.Action, "Use your items with X"));
        }
        else if (curGameMode is Runner3DGameMode)
        {
            controls.Add(new ControlDetails(ControlType.Movement));
            controls.Add(new ControlDetails(ControlType.Jump));
        }
        else if (curGameMode is FruitGameMode)
        {
        }
        else if (curGameMode is PushGameMode)
        {
            controls.Add(new ControlDetails(ControlType.Movement));
            controls.Add(new ControlDetails(ControlType.Jump));
            controls.Add(new ControlDetails(ControlType.Action, "Dash forward with X"));
            controls.Add(new ControlDetails(ControlType.SpecialAction, "Stomp the ground with Y"));
        }
        return controls;
    }

    public static List<PossiblePickup> GetPossiblePickups(GameMode _curGameMode)
    {
        GameMode curGameMode = _curGameMode;
        List<PossiblePickup> possiblePickups = new List<PossiblePickup>();

        if (curGameMode is ColorFloorGameMode)
        {
            possiblePickups.Add(new PossiblePickup(PickUpType.Score, "Collect to score points"));
            possiblePickups.Add(new PossiblePickup(PickUpType.ColorAround, "Color nearby stones"));
            possiblePickups.Add(new PossiblePickup(PickUpType.ColorArrow, "Color stones in a direction"));
            possiblePickups.Add(new PossiblePickup(PickUpType.SpeedUp, "Speeds you up"));
        }
        else if (curGameMode is KartGameMode)
        {
        }
        else if (curGameMode is Runner3DGameMode)
        {
        }
        else if (curGameMode is FruitGameMode)
        {
        }
        return possiblePickups;
    }

    public static string GetRuneInformation(GameMode _curGameMode)
    {
        GameMode curGameMode = _curGameMode;
        if (curGameMode is ColorFloorGameMode)
        {
            return "Score " + ((ColorFloorGameMode)_curGameMode).necessaryPointsForRune * GameManager.Instance.ActivePlayersAtStart + " points.";
        }
        else if (curGameMode is KartGameMode)
        {
            return "Finish the race in less than 30s";
        }
        else if (curGameMode is Runner3DGameMode)
        {
            return "Beat 1:00";
        }
        else if (curGameMode is FruitGameMode)
        {
            return "";
        }
        return "";
    }


}
