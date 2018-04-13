using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PNJDialogUtils {

    // Sneaky chief
    static string SneakyChiefDM = "Go away, I'm trying to focus.";
    static FaceEmotion[] SneakyChiefDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string SneakyChiefQM =
        "You found me!\n" +
        "I'm the Sneaky Chief.\n" +
        "I used to be a ninja and NO ONE was able to find me.\n" +
        "I'm sure it's a mere coincidence you arrived here.\n" +
        "No? Well, let's sort this out.\n" +
        "I'm gonna hide, let's see if you can find me when I'm serious.\n" +
        "See you!#" +

        "You little piece of slime.\n" +
        "You followed me, right?\n" +
        "Let's do it one more time.\n" +
        "If you find me this time, I'll give you this shiny rune.#" +

        "Right.Take it.\n" +
        "I don't know what it's used for anyway.\n" +
        "But I won't admit defeat yet.\n" +
        "The stars recently aligned so my stealth skills have improved.\n" +
        "If you want to claim you are that good at finding me,\n" +
        "the greatest ninja of all time,\n" +
        "you'll have to find me at my full potential.\n" +
        "See you dumby!"
    ;

    static FaceEmotion[] SneakyChiefQE = new FaceEmotion[19] {
        FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack,
        FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral,
        FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral
    };

    // Bob 

    // Roger

    // Mr Risotto


    public static string GetDefaultMessages(PNJName _pnjName)
    {
        switch (_pnjName)
        {
            case PNJName.SneakyChief:
                return SneakyChiefDM;
        }
        return "";
    }

    public static string GetQuestMessages(PNJName _pnjName)
    {
        switch (_pnjName)
        {
            case PNJName.SneakyChief:
                return SneakyChiefQM;
        }
        return "";
    }

    public static FaceEmotion[] GetDefaultEmotions(PNJName _pnjName)
    {
        switch (_pnjName)
        {
            case PNJName.SneakyChief:
                return SneakyChiefDE;
        }
        return null;
    }

    public static FaceEmotion[] GetQuestEmotions(PNJName _pnjName)
    {
        switch (_pnjName)
        {
            case PNJName.SneakyChief:
                return SneakyChiefQE;
        }
        return null;
    }
}
