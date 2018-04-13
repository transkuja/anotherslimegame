
[System.Serializable]
public class MessageContainer
{
    public string[] messages;
    public FaceEmotion[] emotions;

    public MessageContainer(string[] _messages, FaceEmotion[] _emotions)
    {
        messages = _messages;
        emotions = _emotions;
    }

    public MessageContainer(string[] _messages)
    {
        messages = _messages;
        emotions = new FaceEmotion[messages.Length];
        for (int i = 0; i < emotions.Length; i++)
            emotions[i] = FaceEmotion.Neutral;
    }

    public MessageContainer()
    {
        messages = new string[1] { "I don't want to talk to you." };
        emotions = new FaceEmotion[1] { FaceEmotion.Neutral };
    }
}

public class PNJMessages
{
    enum PNJMessagesType { Default, Quest, Size }
    MessageContainer[][] messages;

    public PNJMessages(string _defaultMessages, string _questMessages = "", FaceEmotion[] _defaultMessagesEmotions = null, FaceEmotion[] _questMessagesEmotions = null)
    {
        messages = new MessageContainer[(int)PNJMessagesType.Size][];
        InitMessages(_defaultMessages, PNJMessagesType.Default, _defaultMessagesEmotions);
        InitMessages(_questMessages, PNJMessagesType.Quest, _questMessagesEmotions);
    }

    
    void InitMessages(string _messages, PNJMessagesType _type, FaceEmotion[] _emotions)
    {
        if (_messages == "")
        {
            messages[(int)_type] = new MessageContainer[1] { new MessageContainer() };
        }
        else
        {
            string[] containers = _messages.Split('#');
            messages[(int)_type] = new MessageContainer[containers.Length];
            int emotionIndex = 0;
            FaceEmotion[] faceEmotions;
            for (int i = 0; i < containers.Length; i++)
            {
                //if (_emotions != null)
                //{
                //    faceEmotions = new FaceEmotion[containers[i].Length];
                //    for (int j = emotionIndex; j < emotionIndex + containers[i].Length; j++)
                //    {
                //        faceEmotions[j - emotionIndex] = _emotions[j];
                //    }
                //    emotionIndex += containers[i].Length;

                //    messages[(int)_type][i] = new MessageContainer(containers[i].Split('\n'), faceEmotions);
                //}
                //else
                    messages[(int)_type][i] = new MessageContainer(containers[i].Split('\n'));
            }
        }
    }

    public MessageContainer GetDefaultMessages(int _index = 0)
    {
        return messages[(int)PNJMessagesType.Default][_index];
    }

    public MessageContainer GetQuestMessages(int _index = 0)
    {
        return messages[(int)PNJMessagesType.Quest][_index];
    }

    public int QuestMessagesNbr()
    {
        return messages[(int)PNJMessagesType.Quest].Length;
    }
}

