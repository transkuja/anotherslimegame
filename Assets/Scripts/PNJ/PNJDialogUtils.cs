using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PNJDialogUtils {

    // Vendor Eric
    static string EricDM = "";
    static FaceEmotion[] EricDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string EricQM = "Do you want to change your outfits ?"
    ;

    static FaceEmotion[] EricQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };

    // Vincent Tuto
    static string VincentDM = "You're new here, right?!\n"
       + "I heard that if you can collect 2 runes\n" /* going through this portal in order to grab a rune which will unlock\n"*/
       + "the door may open!\n"
       + "This world is so UNREAL, I hope you'll be able\n"
       + "to open it!";
    static FaceEmotion[] VincentDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string VincentQM = ""
    ;

    static FaceEmotion[] VincentQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };

    // Alex Tuto
    static string AlexDM = "Wonder how I bought this car?\n"
        + "I used to press X to break all the pots I could.\n"
        + "They hide a lot of money that you can use to\n"
        + "customize yourself. As for the car,\n"
        + "I used my influence to have the only one left!";
    static FaceEmotion[] AlexDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string AlexQM = ""
    ;

    static FaceEmotion[] AlexQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };


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

        // 3
        "Right.Take it.\n" +
        "I don't know what it's used for anyway.\n" +
        "But I won't admit defeat yet.\n" +
        "The stars recently aligned so my stealth skills have improved.\n" +
        "If you want to claim you are that good at finding me,\n" +
        "the greatest ninja of all time,\n" +
        "you'll have to find me at my full potential.\n" +
        "See you dumby!#" +

        "It does not count, I was practicing.\n" +
        "...#" +

        "I was not hiding up here,\n" +
        "I was looking for fresh air but it seems it ran out \n" +
        "the second you arrived.\n" +
        "See ya never!#" +

        "Why are you breaking all these pots?!\n" +
        "Only to find me?!\n" +
        "How barbaric.#" +

        "FINE!\n" +
        "Will you let go of me if I give you \n" +
        "this crappy thing I found in the sewer?!\n" +
        "Youngsters these days,\n" +
        "you can't hide in pots without being disturbed!"
    ;

    static FaceEmotion[] SneakyChiefQE = new FaceEmotion[19] {
        FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack,
        FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral,
        FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Neutral
    };

    // Gwen
    // Bob 
    static string BobDM = "I already gave you my hat";
    static FaceEmotion[] BobDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string BobQM =
        "If you reach the goal in time\n" +
        "I will give you my hat.#" +

        "Well done, take this !"
    ;
    static FaceEmotion[] BobQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };

    // Gwen
    static string GwenDM = "Shoo, I need space.\n" +
        "My life sucks anyway.\n" +
        "Hopefully I've a nice view on the beach from here.\n" +
        "Have you seen how beautiful the sand is?";

    static FaceEmotion[] GwenDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string GwenQM =
        "I've been watching you and let me say...\n" +
        "You suck.\n" +
        "I bet you won't be able to run over there in time.#" +

        "Well, eat my watermelon. You can have my hat." 
    ;
    static FaceEmotion[] GwenQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };


    // Mickey
    static string MickeyDM = "Nothing to say.";
    static FaceEmotion[] MickeyDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string MickeyQM =
        "Fight#" +

        "Well done, take this !"
    ;
    static FaceEmotion[] MickeyQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };

    // Roger
    static string RogerDM = "My child is messing with me.\n" 
        + "Could you spank him for me.";
    static FaceEmotion[] RogerDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string RogerQM = ""
    ;

    static FaceEmotion[] RogerQE = new FaceEmotion[2] {
        FaceEmotion.Neutral, FaceEmotion.Neutral
    };

    // Mr Risotto
    static string RisottoDM = "Hi, I'm Mr Risotto!\n"
       + "I often go to this island over there to get ingredients for my infamous recipe\n"
       + "It's too bad you're too short on your feet to check these islands out,\n"
       + "even if you don't really have feet.";
    static FaceEmotion[] RisottoDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string RisottoQM = ""
    ;

    static FaceEmotion[] RisottoQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };


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
        "1What do you call a blind dinosaur?\n" +
        "2Doyouthinkhesaurus!\n" +
        "...\n" + 
        "Do you think he saur us?!#" +

        "Ok next!\n" +
        "1What did one hat say to another?\n" + 
        "2You stay here, I'll go on a head!#" +

        "Here's another one\n" +
        "1How do think the unthinkable?\n" +
        "2With an itheberg!\n" +
        "You know, like the Titanic! The unSINKable!\n" +
        "How good was this one?!#" +

        "Okay, a better one\n" +
        "1What do you call a belt made out of watches?\n" +
        "2A waist of time!#" +

        "1I sold my vacuum the other day...\n" +
        "2all it was doing was collecting dust!#" +

        "1How much does a skeleton weigh?\n" +
        "2A skeleTON!#" +

        "1What kind of shoes do ninjas wear?\n" +
        "2Sneakers!#" +

        // 7
        "1I'm really starting to hate these stupid little Russian Dolls.\n" +
        "2They're so full of themselves.\n" + 
        "Here, take this wonderful artefact,\n" +
        "as a present for supporting me so much!#" +

        "Oh you're still here? Then I guess I'll have to entertain you a bit more!\n" +
        "Let's have fun!\n" + 
        "...\n" +
        "1What's the difference between a piano and a fish?!\n" +
        "2You can tune a piano, but you can't tuna fish!#" +

        "One more!\n" +
        "1What do you call a magician dog?\n" +
        "2A labracadabrador!#" +

        "One more!\n" +      
        "1Why did the vet turn away the injured sea animal?\n" +
        "2Do not accept if seal is broken!#" +

        "1Did you hear about the restaurant on the moon?\n" +
        "2Great food, but no atmosphere.#" +

        "1Why don't dinosaurs talk?\n" +
        "2Because they're extinct.#" +

        "1Why did the scarecrow get promoted?\n" +
        "2He was outstanding in his field!#" +

        "Woosh! I'm getting really good at this!\n" +
        "A few more and if you're still there I may give you something else!\n" +
        "1Why do gorillas have big nostrils?!\n" +
        "2Because gorillas have big fingers!#" +

        "1I used to be a banker,\n" +
        "2but I lost interest.#" +

        "1I'm reading a book about anti-gravity,\n" +
        "2it's impossible to put down!#" +

        // 17
        "1You're such a good person!\n" +
        "...\n" +
        "...\n" +
        "...\n" +
        "...\n" +
        "...\n" +
        "...\n" +
        "No, really, I mean it, it wasn't one of my jokes!\n" +
        "2There!\n" +
        "Take this shiny thing I found the other day while I was fleeing from angry rabbits!"
    ;



    // Remi
    static string RemiDM = "I exiled myself here because nobody wanted to\n"
       + "hear my jokes. They called them inappropriate for\n"
       + "this world.\n"
       + "I think I'll just live in the clouds for a while.\n"
       + "You may hear me sing if you pass by,\n"
       + "and if you're willing to hit me, I'll be grateful.\n"
       + "There's no one around here to play with me.";

    static FaceEmotion[] RemiDE = new FaceEmotion[4] { FaceEmotion.Neutral, FaceEmotion.Winner, FaceEmotion.Attack, FaceEmotion.Neutral };
    /*static string RemiQM = ""
    ;

    static FaceEmotion[] RemiQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };*/



    // Antho
    static string AnthoDM = "One day I'll have some cool quest for you.\n"
       + "For now, I'm just standing there with my cool glasses.\n"
       + "And I may have left enough money in the pots around\n"
       + "for you to buy them.";
       
    static FaceEmotion[] AnthoDE = new FaceEmotion[1] { FaceEmotion.Attack };
    /*static string AnthoQM = ""
    ;

    static FaceEmotion[] AnthoQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };*/


    // Olivier
    static string OlivierDM = "I'm a resourceful architect,\n"
       + "I designed a place that builds by itself endlessly.\n"
       + "Everytime you go there, new surprises arise!\n"
       + "Now if you'll excuse me, I have some complicated maths to do.";
    static FaceEmotion[] OlivierDE = new FaceEmotion[3] { FaceEmotion.Hit, FaceEmotion.Neutral, FaceEmotion.Loser };
    /*static string OlivierQM = ""
    ;

    static FaceEmotion[] OlivierQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };*/


    // Anais
    static string AnaisDM = "I could beat the slime out of anyone,\n"
       + "But people think I'm cute.\n"
       + "I wonder why."; 
    static FaceEmotion[] AnaisDE = new FaceEmotion[1] { FaceEmotion.Winner };
    /*static string AnaisQM = ""
    ;

    static FaceEmotion[] AnaisQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };*/



    // Seb
    static string SebDM = "See my wonderful color?\n"
       + "I won't give it for free.\n"
       + "...\n"
       + "It doesn't mean I'd give it for money";
    static FaceEmotion[] SebDE = new FaceEmotion[3] { FaceEmotion.Loser, FaceEmotion.Neutral, FaceEmotion.Winner };
    /*static string SebQM = ""
    ;

    static FaceEmotion[] SebQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };*/


    // Mathieu
    static string MathieuDM = "Do you know what is my favorite animal ?\n"
       + "No ? I'ill give you a hint.\n"
       + "Owooooo !!\n"
       + "You found it, don't you ?\n"
       + "That was the wolf, if it wasn't obvious.";
    static FaceEmotion[] MathieuDE = new FaceEmotion[5] { FaceEmotion.Neutral, FaceEmotion.Loser, FaceEmotion.Neutral, FaceEmotion.Attack, FaceEmotion.Winner };
    /*static string MathieuQM = ""
    ;

    static FaceEmotion[] MathieuQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };*/


    // Theo
    static string TheoDM = "I used to tell jokes but\n"
        + "I've been replaced.\n"
        + "Now I just stare at the sea,\n"
        + "listen to metal and drink beers.";
    static FaceEmotion[] TheoDE = new FaceEmotion[3] { FaceEmotion.Neutral, FaceEmotion.Hit, FaceEmotion.Winner };
    /*static string TheoQM = ""
    ;

    static FaceEmotion[] TheoQE = new FaceEmotion[3] {
        FaceEmotion.Neutral, FaceEmotion.Neutral, FaceEmotion.Winner
    };*/





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
            case PNJName.Mickey:
                return MickeyDM;
            case PNJName.Gwen:
                return GwenDM;
            case PNJName.Bob:
                return BobDM;
            case PNJName.Eric:
                return EricDM;
            case PNJName.Roger:
                return RogerDM;
            case PNJName.Risotto:
                return RisottoDM;
            case PNJName.Remi:
                return RemiDM;
            case PNJName.Antho:
                return AnthoDM;
            case PNJName.Olivier:
                return OlivierDM;
            case PNJName.Anais:
                return AnaisDM;
            case PNJName.Seb:
                return SebDM;
            case PNJName.Mathieu:
                return MathieuDM;
            case PNJName.Theo:
                return TheoDM;
            case PNJName.Vincent:
                return VincentDM;
            case PNJName.Alex:
                return AlexDM;
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
            case PNJName.Mickey:
                return MickeyQM;
            case PNJName.Gwen:
                return GwenQM;
            case PNJName.Bob:
                return BobQM;
            case PNJName.Roger:
                return RogerQM;
            case PNJName.Risotto:
                return RisottoQM;
            case PNJName.Eric:
                return EricQM;
                /*case PNJName.Remi:
                    return RemiQM;
                case PNJName.Remi:
                    return RemiQM;
                case PNJName.Olivier:
                    return OlivierQM;
                case PNJName.Anais:
                    return AnaisQM;
                case PNJName.Seb:
                    return SebQM;
                case PNJName.Mathieu:
                    return MathieuQM;
                case PNJName.Theo:
                    return TheoQM;*/
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
            case PNJName.Mickey:
                return MickeyDE;
            case PNJName.Gwen:
            case PNJName.Bob:
                return BobDE;
            case PNJName.Roger:
                return RogerDE;
            case PNJName.Risotto:
                return RisottoDE;
            case PNJName.Remi:
                return RemiDE;
            case PNJName.Antho:
                return AnthoDE;
            case PNJName.Olivier:
                return OlivierDE;
            case PNJName.Anais:
                return AnaisDE;
            case PNJName.Seb:
                return SebDE;
            case PNJName.Mathieu:
                return MathieuDE;
            case PNJName.Theo:
                return TheoDE;
            case PNJName.Vincent:
                return VincentDE;
            case PNJName.Alex:
                return AlexDE;
            case PNJName.Eric:
                return EricDE;
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
            case PNJName.Mickey:
                return MickeyQE;
            case PNJName.Gwen:
            case PNJName.Bob:
                return BobQE;
            case PNJName.Roger:
                return RogerQE;
            case PNJName.Risotto:
                return RisottoQE;
            case PNJName.Eric:
                return EricQE;
                /*case PNJName.Remi:
                    return RemiQE;
                case PNJName.Antho:
                    return AnthoQE;
                case PNJName.Olivier:
                    return OlivierQE;
                case PNJName.Anais:
                    return AnaisQE;
                case PNJName.Seb:
                    return SebQE;
                case PNJName.Mathieu:
                    return MathieuQE;
                case PNJName.Theo:
                    return TheoQE;*/
        }
        return null;
    }

    public static void EndDialog(PlayerCharacterHub pnj, int playerIndex)
    {
        pnj.dialogState = DialogState.Normal;

        GameManager.Instance.PlayerStart.PlayersReference[playerIndex].GetComponent<PlayerCharacterHub>().dialogState = DialogState.Normal;
        pnj.GetComponent<PNJMessage>().currentMessage = 0;

        pnj.GetComponent<PNJMessage>().Message[playerIndex].SetActive(false);
    }
}
