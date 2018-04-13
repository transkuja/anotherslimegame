
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
    MessageContainer[] messages;

    public PNJMessages(string _defaultMessages, string _questMessages = "", FaceEmotion[] _defaultMessagesEmotions = null, FaceEmotion[] _questMessagesEmotions = null)
    {
        messages = new MessageContainer[(_questMessages == "") ? 1 : 2];
        InitMessages(_defaultMessages, PNJMessagesType.Default, _defaultMessagesEmotions);
        InitMessages(_questMessages, PNJMessagesType.Quest, _questMessagesEmotions);
    }

    
    void InitMessages(string _messages, PNJMessagesType _type, FaceEmotion[] _emotions)
    {
        if (_messages == "")
            messages[(int)_type] = new MessageContainer();
        else
        {
            if (_emotions != null)
                messages[(int)_type] = new MessageContainer(_messages.Split('\n'), _emotions);
            else
                messages[(int)_type] = new MessageContainer(_messages.Split('\n'));
        }
    }

    MessageContainer GetDefaultMessages()
    {
        return messages[(int)PNJMessagesType.Default];
    }

    MessageContainer GetQuestMessages()
    {
        return messages[(int)PNJMessagesType.Quest];
    }
}

