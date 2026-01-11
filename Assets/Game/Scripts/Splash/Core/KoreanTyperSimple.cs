using System.Collections;
using UnityEngine;
using KoreanTyper;
using TMPro;

public class KoreanTyperSimple : MonoBehaviour
{
    public bool isInputOccured = false;

    public string message;

    TextMeshProUGUI myText;
    WaitForSeconds typingWait;

    public delegate void OnTextTypeFinish();
    public OnTextTypeFinish onTextTypeFinish;

    void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        message = myText.text;
        myText.text = "";
    }

    void Start()
    {
        StartCoroutine(TypingMsg());
    }


    IEnumerator TypingMsg()
    { 
        int typingLength = message.GetTypingLength();

        for (int index = 0; index <= typingLength; index++)
        {
            yield return new WaitForSeconds(0.05f);

            myText.text = message.Typing(index);

            if(isInputOccured)
            {
                myText.text = message; 
                yield return new WaitForSeconds(0.5f);
                break;
            }
        }

        onTextTypeFinish();
    }
}
