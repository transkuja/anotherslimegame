using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public static class MinigameDataUtils
{
    private static string ColorFloorTitle = "Floor Coloring";
    private static string ClassicKartTitle = "Classic Kart";
    private static string RunnerTitle = "Classic Runner";
    private static string SnowKartTitle = "Snow Kart";
    private static string FruitTitle = "Frootballs";
    private static string ClassicClashTitle = "Classic Clash";
    private static string SuperRunnerTitle = "Super Runner"; 

    public static string GetTitle(GameMode _curGameMode)
    {
        GameMode curGameMode = _curGameMode;
        if (curGameMode is ColorFloorGameMode)
        {
            return ColorFloorTitle;
        }
        else if (curGameMode is KartGameMode)
        {
            return ClassicKartTitle;
        }
        else if (curGameMode is Runner3DGameMode)
        {
            return RunnerTitle;
        }
        else if (curGameMode is FruitGameMode)
        {
            return FruitTitle;
        }
        else if(curGameMode is FruitGameMode2)
        {
            return FruitTitle;
        }
        else if (curGameMode is PushGameMode)
        {
            return ClassicClashTitle;
        }
        return "";
    }

    public static string GetTitle(string _minigameId)
    {
        // "MinigameAntho", "MinigameKart", "MinigamePush", "Minigame3dRunner"
        if (_minigameId == "MinigameAntho")
        {
            return ColorFloorTitle;
        }
        else if (_minigameId == "MinigameKart")
        {
            return ClassicKartTitle;
        }
        else if (_minigameId == "MinigameKart 2")
        {
            return SnowKartTitle;
        }
        else if (_minigameId == "Minigame3dRunner")
        {
            return RunnerTitle;
        }
        else if (_minigameId == "Minigame3dRunner 1")
        {
            return SuperRunnerTitle;
        }
        else if (_minigameId == "MiniGameFruits")
        {
            return FruitTitle;
        }
        else if (_minigameId == "MiniGameFruits 2")
        {
            return FruitTitle;
        }
        else if (_minigameId == "MinigamePush")
        {
            return ClassicClashTitle;
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
            return "RUN !";
        }
        else if (curGameMode is FruitGameMode)
        {
            return "Collect fruits which are associated to your color";
        }
        else if (curGameMode is FruitGameMode2)
        {
            return "Collect fruits which are associated to your color";
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
            controls.Add(new ControlDetails(ControlType.Action, "Boost with X"));
        }
        else if (curGameMode is Runner3DGameMode)
        {
            controls.Add(new ControlDetails(ControlType.Movement));
            controls.Add(new ControlDetails(ControlType.Jump));
            controls.Add(new ControlDetails(ControlType.Action, "Dash forward with X"));
        }
        else if (curGameMode is FruitGameMode)
        {
            controls.Add(new ControlDetails(ControlType.Movement));
            controls.Add(new ControlDetails(ControlType.Jump));
            controls.Add(new ControlDetails(ControlType.Action, "Dash forward with X"));
        }
        else if (curGameMode is FruitGameMode2)
        {
            controls.Add(new ControlDetails(ControlType.Movement));
            controls.Add(new ControlDetails(ControlType.Jump));
            controls.Add(new ControlDetails(ControlType.Action, "Dash forward with X"));
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
            possiblePickups.Add(new PossiblePickup(PickUpType.Changer, "Change all fruits in scene to yours"));
            possiblePickups.Add(new PossiblePickup(PickUpType.Aspirator, "Collect all your fruits"));
        }
        else if (curGameMode is FruitGameMode2)
        {
            possiblePickups.Add(new PossiblePickup(PickUpType.Changer, "Change all fruits in scene to yours"));
            possiblePickups.Add(new PossiblePickup(PickUpType.Aspirator, "Collect all your fruits"));
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
            return "Finish the race in less than " + TimeFormatUtils.GetFormattedTime(((KartGameMode)_curGameMode).necessaryTimeForRune, TimeFormat.Sec) + "s";
        }
        else if (curGameMode is Runner3DGameMode)
        {
            return "Beat 1:00";
        }
        else if (curGameMode is FruitGameMode)
        {
            return "Score " + ((FruitGameMode)_curGameMode).necessaryPointsForRune;
        }
        else if (curGameMode is FruitGameMode2)
        {
            return "Score " + ((FruitGameMode2)_curGameMode).necessaryPointsForRune;
        }
        return "";
    }


}
