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

    // Joker
    static string JokerDM = "My jokes are the best!";
    static FaceEmotion[] JokerDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string JokerQM =
        "Hi,\n" +
        "Do you have a minute?\n" +
        "My name's Joker,\n" +
        "I need to practice my jokes for the joke contest.\n" +
        "Can I tell you some and ask you what you think of them?\n" +
        "Here's the first one,\n" +
        "What do you call a blind dinosaur?\n" +
        "Doyouthinkhesaurus!#" +

        "Ok next!\n" +
        "What did one hat say to another?\n" + 
        "You stay here, I'll go on a head!#" +

        "Here's another one\n" +
        "How do think the unthinkable?\n" +
        "With an itheberg!\n" +
        "You know, like the Titanic!\n" +
        "How good was this one?!#" +

        "Okay, a better one\n" +
        "What do you call a belt made out of watches?\n" +
        "A waist of time!#" +

        "I sold my vacuum the other day…\n" +
        "all it was doing was collecting dust!#" +

        "What do you call a guy with a rubber toe?\n" +
        "Roberto!#" + 

        "What is invisible and smells like carrots?\n" +
        "Rabbit farts!#" +

        "I'm really starting to hate these stupid little Russian Dolls.\n" +
        "They're so full of themselves.\n" + 
        "Here, take this wonderful artefact,\n" +
        "as a present for supporting me so much!#"

    ;

    static FaceEmotion[] JokerQE = new FaceEmotion[19] {
        FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack,
        FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral,
        FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral
    };

    public static string GetDefaultMessages(PNJName _pnjName)
    {
        switch (_pnjName)
        {
            case PNJName.SneakyChief:
                return SneakyChiefDM;
            case PNJName.Joker:
                return JokerDM;
        }
        return "";
    }

    public static string GetQuestMessages(PNJName _pnjName)
    {
        switch (_pnjName)
        {
            case PNJName.SneakyChief:
                return SneakyChiefQM;
            case PNJName.Joker:
                return JokerQM;
        }
        return "";
    }

    public static FaceEmotion[] GetDefaultEmotions(PNJName _pnjName)
    {
        switch (_pnjName)
        {
            case PNJName.SneakyChief:
                return SneakyChiefDE;
            case PNJName.Joker:
                return JokerDE;
        }
        return null;
    }

    public static FaceEmotion[] GetQuestEmotions(PNJName _pnjName)
    {
        switch (_pnjName)
        {
            case PNJName.SneakyChief:
                return SneakyChiefQE;
            case PNJName.Joker:
                return JokerQE;
        }
        return null;
    }
}
