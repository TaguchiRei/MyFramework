using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace GamesKeystoneFramework.TextSystem
{
    public abstract class TextManagerBace : MonoBehaviour
    {
        [SerializeField] int line = 2;
        [SerializeField] TextDataReader reader;
        [SerializeField] TextMeshProUGUI tmp;
        [SerializeField] Image image;
        [SerializeField, Range(0, 1)] float typingSpeed = 0.1f;
        List<TextData> textDataList = new();
        int readPoint = 0;
        private Coroutine textCoroutine;
        public virtual void StartText(string fileName, bool read = false)
        {
            if (read)
            {
                reader.LoadTextData(fileName);
            }
            textDataList = reader.textData;
            TextBoxShow();
            tmp.text = "";
            readPoint = 0;
            textCoroutine = null;
            Next();
        }

        public virtual void Next()
        {
            if (textCoroutine == null)
            {
                if (textDataList.Count - 1 < readPoint)
                {
                    TextBoxHide();
                    return;
                }
                switch (textDataList[readPoint].dataType)
                {
                    case TextDataType.Text:
                        textCoroutine = StartCoroutine(TypeText(textDataList[readPoint].text));
                        break;
                    case TextDataType.Selection:
                        break;
                    case TextDataType.Question:
                        break;
                    case TextDataType.Ghost:
                        break;
                }
            }
            else
            {
                StopCoroutine(textCoroutine);
                textCoroutine = null;
                var s = tmp.text.Split("\n");
                tmp.text = "";
                for (int i = 0; i < s.Length - 1; i++)
                {
                    tmp.text += s[i] + "\n";
                }
                tmp.text += textDataList[readPoint].text + "\n";
                readPoint++;
            }
        }

        public virtual void TextBoxShow()
        {
            image.gameObject.SetActive(true);
            tmp.gameObject.SetActive(true);
        }
        public virtual void TextBoxHide()
        {
            image.gameObject.SetActive(false);
            tmp.gameObject.SetActive(false);
        }

        IEnumerator TypeText(string text)
        {
            var s = tmp.text.Split("\n");
            if (s.Length == line + 1)
            {
                tmp.text = s[1];
                for (int i = 2; i < s.Length; i++)
                {
                    tmp.text += "\n" + s[i];
                }
            }

            foreach (char c in text)
            {
                tmp.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
            tmp.text += "\n";
            readPoint++;
            textCoroutine = null;
        }

    }
}