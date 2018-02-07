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
            return "";
        }
        else if (curGameMode is Runner3DGameMode)
        {
            return "";
        }
        else if (curGameMode is FruitGameMode)
        {
            return "";
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
            return "";
        }
        else if (curGameMode is Runner3DGameMode)
        {
            return "";
        }
        else if (curGameMode is FruitGameMode)
        {
            return "";
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
        }
        else if (curGameMode is Runner3DGameMode)
        {
        }
        else if (curGameMode is FruitGameMode)
        {
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
            //possiblePickups.Add(new PossiblePickup(PickUpType.Bomb, "Collect to do smthg"));
            //possiblePickups.Add(new PossiblePickup(PickUpType.Missile, "Collect to do smthg else"));
            possiblePickups.Add(new PossiblePickup(PickUpType.ColorAround, "Color near stones"));
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
            return "";
        }
        else if (curGameMode is Runner3DGameMode)
        {
            return "";
        }
        else if (curGameMode is FruitGameMode)
        {
            return "";
        }
        return "";
    }


}
