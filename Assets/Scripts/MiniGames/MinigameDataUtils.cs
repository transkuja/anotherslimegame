using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public static class MinigameDataUtils
{
    private static string ColorFloorTitle = "Floor Coloring";
    private static string ColorFloorTitleV3 = "Trapped Floor Coloring";
    private static string ColorFloorTitleV5 = "Restrained Floor Coloring";
    private static string ColorFloorTitleV7 = "Trapped & Restrained Floor Coloring";

    private static string ColorFloorTitleV2 = "Shape Coloring";
    private static string ColorFloorTitleV4 = "Trapped Shape Coloring";
    private static string ColorFloorTitleV6 = "Restrained Shape Coloring";
    private static string ColorFloorTitleV8 = "Trapped & Restrained Shape Coloring";

    private static string ClassicKartTitle = "Classic Kart";
    private static string RunnerTitle = "Classic Runner";
    private static string SnowKartTitle = "Snow Kart";
    private static string FruitTitle = "Classic Fruits";
    private static string FruitTitle2 = "Super Fruits";
    private static string ClassicClashTitle = "Classic Clash";
    private static string SuperRunnerTitle = "Super Runner";
    private static string FoodTitle = "Eat Them All";

    public static string GetTitle(GameMode _curGameMode, int _version = 0)
    {
        GameMode curGameMode = _curGameMode;
        if (curGameMode is ColorFloorGameMode)
        {
            if (_version == 1)
                return ColorFloorTitleV2;
            if (_version == 2)
                return ColorFloorTitleV3;
            if (_version == 3)
                return ColorFloorTitleV4;
            if (_version == 4)
                return ColorFloorTitleV5;
            if (_version == 5)
                return ColorFloorTitleV6;
            if (_version == 6)
                return ColorFloorTitleV7;
            if (_version == 7)
                return ColorFloorTitleV8;

            return ColorFloorTitle;
        }
        else if (curGameMode is KartGameMode)
        {
            if (_version == 1)
                return SnowKartTitle; 
            return ClassicKartTitle;
        }
        else if (curGameMode is Runner3DGameMode)
        {
            if (_version == 1)
                return SuperRunnerTitle;
            return RunnerTitle;
        }
        else if (curGameMode is FruitGameMode)
        {
            if (_version == 1)
                return FruitTitle2;
            return FruitTitle;
        }
        else if (curGameMode is PushGameMode)
        {
            return ClassicClashTitle;
        }
        else if (curGameMode is FoodGameMode)
        {
            return FoodTitle;
        }
        return "";
    }

    public static string GetTitle(string _minigameId, int _version)
    {
        // "MinigameAntho", "MinigameKart", "MinigamePush", "Minigame3dRunner"
        if (_minigameId == "MinigameAntho")
        {
            if (_version == 2)
                return ColorFloorTitleV3;
            if (_version == 4)
                return ColorFloorTitleV5;
            if (_version == 6)
                return ColorFloorTitleV7;
            return ColorFloorTitle;

        }
        else if (_minigameId == "MinigameAnthourte")
        {
            if (_version == 3)
                return ColorFloorTitleV4;
            if (_version == 5)
                return ColorFloorTitleV6;
            if (_version == 7)
                return ColorFloorTitleV8;

            return ColorFloorTitleV2;
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
        else if (_minigameId == "MiniGameFruits2")
        {
            return FruitTitle2;
        }
        else if (_minigameId == "MinigamePush")
        {
            return ClassicClashTitle;
        }
        else if (_minigameId == "MinigameFood")
        {
            return FoodTitle;
        }
        return "";
    }

    public static string GetDescription(GameMode _curGameMode, int _version = 0)
    {
        GameMode curGameMode = _curGameMode;
        if (curGameMode is ColorFloorGameMode)
        {
            if (_version % 2 == 1)
                return "Encircle areas with your color to capture the center and earn points. The one with the most points win!";

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
            return "Collect fruits which are of your color\n\n Player 1 : Red\n Player 2 : Blue\n Player 3 : Green\n Player 4 : Yellow";
        }
        else if (curGameMode is PushGameMode)
        {
            return "Steal other players' coins!";
        }
        else if (curGameMode is FoodGameMode)
        {
            return "Eat as much as you can by pressing the correct buttons. But beware, it's not good eating too much too fast!";
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
            controls.Add(new ControlDetails(ControlType.RightTrigger));
            controls.Add(new ControlDetails(ControlType.LeftTrigger));
            controls.Add(new ControlDetails(ControlType.Steering));
            controls.Add(new ControlDetails(ControlType.Action, "Boost with X"));
        }
        else if (curGameMode is Runner3DGameMode)
        {
            controls.Add(new ControlDetails(ControlType.Movement));
            controls.Add(new ControlDetails(ControlType.Jump));
            controls.Add(new ControlDetails(ControlType.Action, "Dash forward with X"));
            if (_curGameMode.minigameVersion == 1)
            {
                controls.Add(new ControlDetails(ControlType.RightTrigger, "Create platforms with RT"));
            }

        }
        else if (curGameMode is FruitGameMode)
        {
            controls.Add(new ControlDetails(ControlType.Movement));
            controls.Add(new ControlDetails(ControlType.Jump));
            controls.Add(new ControlDetails(ControlType.Action, "Dash forward with X"));
            if (_curGameMode.minigameVersion == 1)
                controls.Add(new ControlDetails(ControlType.LeftTrigger, "Spray incapacitant with LT"));
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

    public static List<PossiblePickup> GetPossiblePickups(GameMode _curGameMode, int _version = 0)
    {
        GameMode curGameMode = _curGameMode;
        List<PossiblePickup> possiblePickups = new List<PossiblePickup>();

        if (curGameMode is ColorFloorGameMode)
        {
            if (_version%2 == 0)
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
        return possiblePickups;
    }

    public static string GetRuneInformation(GameMode _curGameMode, int _version = 0)
    {
        GameMode curGameMode = _curGameMode;
        if (curGameMode is ColorFloorGameMode)
        {
            if (_version%2 == 1)
                return "Score " + ((ColorFloorGameMode)_curGameMode).necessaryPointsForRune * GameManager.Instance.ActivePlayersAtStart + " points.";

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
        else if (curGameMode is FoodGameMode)
        {
            return "Eat for " + _curGameMode.necessaryPointsForRune * GameManager.Instance.ActivePlayersAtStart + " points.";
        }
        return "";
    }


}
