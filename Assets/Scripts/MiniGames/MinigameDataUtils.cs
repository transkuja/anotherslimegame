using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public static class MinigameDataUtils
{
    private static string ColorFloorTitle = "Floor Coloring";
    private static string ColorFloorTitleV3 = "Trapped Coloring";
    private static string ColorFloorTitleV5 = "Bouncy Coloring";
    private static string ColorFloorTitleV7 = "Trapped Bouncy Coloring";

    private static string ColorFloorTitleV2 = "Shape Coloring";
    private static string ColorFloorTitleV4 = "Trapped Shaper";
    private static string ColorFloorTitleV6 = "Bouncy Shaper";
    private static string ColorFloorTitleV8 = "Trapped Bouncy Shaper";

    private static string ClassicKartTitle = "Kart From Hell";
    private static string SnowKartTitle = "Snow Kart";
    private static string EasyKartTitle = "First Race";
    private static string RunnerTitle = "Classic Runner";
    private static string FruitTitle = "Classic Balls";
    private static string FruitTitle2 = "Super Balls";
    private static string ClashWithSpikesTitle = "Trapped Clash";
    private static string ClashWithPotsTitle = "Clash";
    private static string SuperRunnerTitle = "Super Runner";
    private static string TrappedRunnerTitle = "Trapped Runner";
    private static string FoodTitle = "Eat Them All";
    private static string FoodTitleV2 = "Eat Them Faster";
    private static string FoodTitleV3 = "Don't Eat Them All";
    private static string FoodTitleV4 = "Gluttony";

    private static string BreakingTitle = "Jar Pieces";
    private static string BreakingTrapsTitle = "Beware The Rabbits";
    private static string BreakingGroundTitle = "Slime Against Gravity";

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
            if (_version == 2)
                return EasyKartTitle;
            return ClassicKartTitle;
        }
        else if (curGameMode is Runner3DGameMode)
        {
            if (_version == 1)
                return SuperRunnerTitle;
            else if (_version == 2)
                return TrappedRunnerTitle;
            return RunnerTitle;
        }
        else if (curGameMode is PushGameMode)
        {
            if (_version == 1)
                return ClashWithPotsTitle;
            return ClashWithSpikesTitle;
        }
        else if (curGameMode is FoodGameMode)
        {
            if (_version == 1)
                return FoodTitleV2;
            if (_version == 2)
                return FoodTitleV3;
            if (_version == 3)
                return FoodTitleV4;
            return FoodTitle;
        }
        else if (curGameMode is BreakingGameMode)
        {
            if (_version == 3)
                return BreakingTrapsTitle;
            if (_version == 4)
                return BreakingGroundTitle;
            return BreakingTitle;
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
        else if (_minigameId == "MinigameKart 3")
        {
            return EasyKartTitle;
        }
        else if (_minigameId == "Minigame3dRunner")
        {
            return RunnerTitle;
        }
        else if (_minigameId == "Minigame3dRunner 1")
        {
            return SuperRunnerTitle;
        }
        else if(_minigameId == "Minigame3dRunner 2")
        {
            return TrappedRunnerTitle;
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
            return ClashWithSpikesTitle;
        }
        else if (_minigameId == "MinigamePush 1")
        {
            return ClashWithPotsTitle;
        }
        else if (_minigameId == "MinigameFood")
        {
            if (_version == 1)
                return FoodTitleV2;
            if (_version == 2)
                return FoodTitleV3;
            if (_version == 3)
                return FoodTitleV4;
            return FoodTitle;
        }
        else if (_minigameId == "MinigameAnthourloupe")
        {
            if (_version == 3)
                return BreakingTrapsTitle;
            if (_version == 4)
                return BreakingGroundTitle;
            return BreakingTitle;
        }
        return "";
    }

    public static string GetDescription(GameMode _curGameMode, int _version = 0)
    {
        GameMode curGameMode = _curGameMode;
        bool fromHub = (SlimeDataContainer.instance != null && !SlimeDataContainer.instance.launchedFromMinigameScreen);

        if (curGameMode is ColorFloorGameMode)
        {
            if (_version % 2 == 1)
                if (fromHub)
                    return "Encircle areas with your color to capture the center and convert your squares into points!";
                else
                    return "Encircle areas with your color to capture the center and convert your squares into points. The one with the most points win!";

            if (fromHub)
                return "Grab golden crystals to convert your squares into points!";
            else
                return "Grab golden crystals to convert your squares into points! Be the one with the most points to win!";
        }
        else if (curGameMode is KartGameMode)
        {
            if (fromHub)
                return "Beat the clock!";
            else
                return "Win the race.";
        }
        else if (curGameMode is Runner3DGameMode)
        {
            return "RUN !";
        }
        else if (curGameMode is PushGameMode)
        {
            if (curGameMode.minigameVersion == 0)
                return "Steal other players' coins!";
            else
                return "Break the vases and steal other players' coins!";
        }
        else if (curGameMode is FoodGameMode)
        {
            return "Eat as much as you can by pressing the correct buttons. But beware not eating too much too fast!";
        }
        else if (curGameMode is BreakingGameMode)
        {
            if (_version == 4)
                return "Survive!";
            string desc = "Break pots as fast as you can! ";
            if (_version == 3)
                desc += "Be careful, rabbits may be hiding inside.";

            return desc;
        }
        return "";
    }

    public static List<ControlDetails> GetControls(GameMode _curGameMode)
    {
        GameMode curGameMode = _curGameMode;
        List<ControlDetails> controls = new List<ControlDetails>();

        if (curGameMode is ColorFloorGameMode)
        {
            controls.Add(new ControlDetails(ControlType.LeftThumbstick));
            if (_curGameMode.minigameVersion < 4)
                controls.Add(new ControlDetails(ControlType.X, "Dash forward with X"));
        }
        else if (curGameMode is KartGameMode)
        {
            controls.Add(new ControlDetails(ControlType.RightTrigger, "Accelerate with RT"));
            controls.Add(new ControlDetails(ControlType.LeftTrigger, "Reverse with LT"));
            controls.Add(new ControlDetails(ControlType.LeftThumbstick, "Steer with L stick"));
            controls.Add(new ControlDetails(ControlType.X, "Boost with X"));
        }
        else if (curGameMode is Runner3DGameMode)
        {
            controls.Add(new ControlDetails(ControlType.LeftThumbstick));
            controls.Add(new ControlDetails(ControlType.A));
            controls.Add(new ControlDetails(ControlType.X, "Dash forward with X"));
            if (_curGameMode.minigameVersion == 1)
            {
                controls.Add(new ControlDetails(ControlType.RightTrigger, "Create platforms with RT"));
            }

        }
        else if (curGameMode is PushGameMode)
        {
            controls.Add(new ControlDetails(ControlType.LeftThumbstick));
            controls.Add(new ControlDetails(ControlType.A));
            controls.Add(new ControlDetails(ControlType.X, "Dash forward with X"));
            controls.Add(new ControlDetails(ControlType.Y, "Stomp the ground with Y"));
        }
        else if (curGameMode is BreakingGameMode)
        {
            controls.Add(new ControlDetails(ControlType.LeftThumbstick));
            controls.Add(new ControlDetails(ControlType.A));
            controls.Add(new ControlDetails(ControlType.X, "Dash forward with X"));
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
            if (!((ColorFloorGameMode)curGameMode).withBadSpawns)
                possiblePickups.Add(new PossiblePickup(PickUpType.ColorAround, "Color nearby stones"));
            possiblePickups.Add(new PossiblePickup(PickUpType.ColorArrow, "Color stones in a direction"));
            possiblePickups.Add(new PossiblePickup(PickUpType.SpeedUp, "Speeds you up"));
            if (((ColorFloorGameMode)curGameMode).withBadSpawns)
                possiblePickups.Add(new PossiblePickup(PickUpType.BadOne, "Decrease your score!"));

        }
        else if (curGameMode is KartGameMode)
        {
        }
        else if (curGameMode is Runner3DGameMode)
        {
        }
        else if (curGameMode is BreakingGameMode)
        {
            if (_version != 4)
            {
                possiblePickups.Add(new PossiblePickup(PickUpType.BallPickup, "Hit it multiple times to get a power!"));
            }
        }
        return possiblePickups;
    }

    public static string GetRuneInformation(GameMode _curGameMode, int _version = 0)
    {
        GameMode curGameMode = _curGameMode;
        if (curGameMode is ColorFloorGameMode)
        {
            if (_version%2 == 1)
                return "Score " + _curGameMode.necessaryPointsForRune * GameManager.Instance.ActivePlayersAtStart + " points.";

            return "Score " + _curGameMode.necessaryPointsForRune * GameManager.Instance.ActivePlayersAtStart + " points.";
        }
        else if (curGameMode is KartGameMode)
        {
            return "Finish the race in less than " + TimeFormatUtils.GetFormattedTime(((KartGameMode)_curGameMode).necessaryTimeForRune, TimeFormat.Sec) + "s";
        }
        else if (curGameMode is Runner3DGameMode)
        {
            return "Run for " + _curGameMode.necessaryPointsForRune * GameManager.Instance.ActivePlayersAtStart + "m!";
        }
        else if (curGameMode is FoodGameMode)
        {
            return "Eat for " + _curGameMode.necessaryPointsForRune * GameManager.Instance.ActivePlayersAtStart + " points.";
        }
        else if (curGameMode is BreakingGameMode)
        {
            if (_version < 4)
                return "Break " + _curGameMode.necessaryPointsForRune * GameManager.Instance.ActivePlayersAtStart + " pots.";

            return "Survive during " + _curGameMode.necessaryPointsForRune + " seconds.";
        }
        return "";
    }

    public static int[] GetMinMaxGoldTargetValues(MinigameType _curGameType, int _version = 0)
    {
        int[] result = new int[2];
        if (_curGameType == MinigameType.Floor)
        {
            result[0] = 0;
            if (_version % 2 == 1)
            {
                result[1] = 300;
                return result;
            }

            result[1] = 400;
            return result;
        }
        else if (_curGameType == MinigameType.Kart)
        {
            result[0] = 120;
            result[1] = 30;
            return result;
        }
        else if (_curGameType == MinigameType.Runner)
        {
            result[0] = 0;
            result[1] = 500;
            return result;
        }
        else if (_curGameType == MinigameType.Food)
        {
            result[0] = 0;
            result[1] = 2500;
            return result;
        }
        else if (_curGameType == MinigameType.Clash)
        {
            if (_version < 4)
            {
                result[0] = 0;
                result[1] = 250;
            }
            else
            {
                result[0] = 0;
                result[1] = 450;
            }
            return result;
        }
        return result;
    }

    public static int GetRuneScoreObjective(MinigameType minigameType, int minigameVersion)
    {
        if (minigameType == MinigameType.Floor)
        {
            if (minigameVersion >= 4)
            {
                return 80;
            }
            return 125;
        }
        else if (minigameType == MinigameType.Kart)
        {
            return 30;
        }
        else if (minigameType == MinigameType.Runner)
        {
            if (minigameVersion == 0)
            {
                return 500;
            }
            return 450;
        }
        else if (minigameType == MinigameType.Food)
        {
            if (minigameVersion == 0 || minigameVersion == 1)
                return 1750;
            if (minigameVersion == 2)
                return 1500;
            if (minigameVersion == 3)
                return 1250;
        }
        else if (minigameType == MinigameType.Clash)
        {
            if (minigameVersion == 2 || minigameVersion == 3)
            {
                return 150;
            }
            else
            {
                return 60;
            }
        }
        return 0;
    }

    public static int GetRuneScoreObjective(GameMode minigameMode, int minigameVersion)
    {
        if (minigameMode is ColorFloorGameMode)
        {
            return GetRuneScoreObjective(MinigameType.Floor, minigameVersion);
        }
        else if (minigameMode is KartGameMode)
        {
            return GetRuneScoreObjective(MinigameType.Kart, minigameVersion);
        }
        else if (minigameMode is Runner3DGameMode)
        {
            return GetRuneScoreObjective(MinigameType.Runner, minigameVersion);
        }
        else if (minigameMode is FoodGameMode)
        {
            return GetRuneScoreObjective(MinigameType.Food, minigameVersion);
        }
        else if (minigameMode is BreakingGameMode)
        {
            return GetRuneScoreObjective(MinigameType.Clash, minigameVersion);
        }
        return 0;
    }
}
