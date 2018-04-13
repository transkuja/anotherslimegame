using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PNJDialogUtils {

    // Sneaky chief
    static string SneakyChiefDM = "";
    static FaceEmotion[] SneakyChiefDE = new FaceEmotion[1] { FaceEmotion.Neutral };
    static string SneakyChiefQM = "";
    static FaceEmotion[] SneakyChiefQE = new FaceEmotion[1] { FaceEmotion.Neutral };
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
