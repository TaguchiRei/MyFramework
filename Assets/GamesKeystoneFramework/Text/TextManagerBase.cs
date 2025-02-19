using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace GamesKeystoneFramework.TextSystem
{
    public abstract class TextManagerBase : MonoBehaviour
    {
        [SerializeField] bool _useBranch = true;
        [SerializeField] int line = 2;
        [SerializeField] Color[] colors;
        [SerializeField] TextDataReader reader;
        [SerializeField] TextMeshProUGUI textBoxTMP;
        [SerializeField] Image textBox;
        [SerializeField] TextMeshProUGUI selectTMP;
        [SerializeField] Image selectBox;
        [SerializeField, Range(0, 1)] float typingSpeed = 0.1f;
        List<TextData> textDataList = new();
        List<int> selectLine = new();
        int readPoint = 0;
        int selectNumber = 0;
        bool questionMode = false;
        bool ansMode = false;
        bool canSelect = false;
        private Coroutine textCoroutine;

        private void Start()
        {
            TextBoxHide();
            SelectBoxHide();
            selectNumber = 0;
            textBoxTMP.lineSpacing = 0f;
        }
        public virtual void StartText(string fileName, bool read = false)
        {
            if (read)
            {
                reader.LoadTextData(fileName);
            }
            textDataList = reader.textData;
            TextBoxShow();
            textBoxTMP.text = "";
            readPoint = 0;
            textCoroutine = null;
            Next();
        }

        public virtual bool Next()
        {
            if (!questionMode)
            {
                if (textDataList.Count - 1 < readPoint)
                {
                    TextBoxHide();
                    if (_useBranch)
                        SelectBoxHide();
                    return false;
                }
                string text = textDataList[readPoint].text;
                if (textCoroutine == null)
                {
                    switch (textDataList[readPoint].dataType)
                    {
                        case TextDataType.Text:
                            textCoroutine = StartCoroutine(TypeText(text));
                            break;
                        case TextDataType.Question:
                            questionMode = true;
                            canSelect = false;
                            Question(readPoint, text);
                            break;
                        case TextDataType.Branch:
                            if (ansMode == true)
                            {
                                for (int i = readPoint; i < textDataList.Count; i++)
                                {
                                    if (textDataList[i].dataType == TextDataType.QEnd)
                                    {
                                        readPoint = i + 1;
                                        Next();
                                        break;
                                    }
                                }
                            }
                            break;
                        case TextDataType.QEnd:
                            readPoint++;
                            Next();
                            break;
                        case TextDataType.TextEnd:
                            SelectBoxHide();
                            TextBoxHide();
                            break;
                    }
                }
                else
                {
                    StopCoroutine(textCoroutine);
                    textCoroutine = null;
                    textBoxTMP.maxVisibleCharacters = textBoxTMP.GetParsedText().Length;
                    textBoxTMP.text += "\n";
                    readPoint++;
                }
            }
            else if (canSelect == true)
            {
                SelectBoxHide();
                readPoint = selectLine[selectNumber];
                selectLine.Clear();
                questionMode = false;
                ansMode = true;
                Next();
            }
            else
            {
                StopCoroutine(textCoroutine);
                SelectorShow(readPoint);
                textCoroutine = null;
                textBoxTMP.maxVisibleCharacters = textBoxTMP.GetParsedText().Length;
                textBoxTMP.text += "\n";
            }
            return true;
        }

        /// <summary>
        /// 会話分岐を制作するときに使います
        /// </summary>
        /// <param name="readPoint"></param>
        public void Question(int readPoint, string question)
        {
            selectTMP.text = "";
            textBoxTMP.text = "";
            selectNumber = 0;
            Debug.Log(textBoxTMP.text.Length);
            textCoroutine = StartCoroutine(TypeText(question, (() => SelectorShow(readPoint))));
        }

        /// <summary>
        /// 別の選択肢を選ぶときに呼び出す。
        /// </summary>
        /// <param name="up">falseにすると下の選択肢になる</param>
        public virtual void SelectChange(bool up = true)
        {
            if (!questionMode)
            {
                Debug.Log("非QuestionMode状態でSelectChangeメソッドが呼ばれました");
                return;
            }
            if (up)
            {
                if (selectNumber != 0)
                    selectNumber--;
            }
            else
            {
                if (selectNumber != selectLine.Count - 1)
                    selectNumber++;
            }
        }
        public virtual void SelectBoxShow()
        {
            selectBox.gameObject.SetActive(true);
            selectTMP.gameObject.SetActive(true);
        }
        public virtual void SelectBoxHide()
        {
            selectBox.gameObject.SetActive(false);
            selectTMP.gameObject.SetActive(false);
        }
        public virtual void TextBoxShow()
        {
            textBox.gameObject.SetActive(true);
            textBoxTMP.gameObject.SetActive(true);
        }
        public virtual void TextBoxHide()
        {
            textBox.gameObject.SetActive(false);
            textBoxTMP.gameObject.SetActive(false);
        }
        /// <summary>
        /// 選択肢を表示するためのメソッド
        /// </summary>
        /// <param name="i"></param>
        void SelectorShow(int i)
        {
            SelectBoxShow();
            for (; i < textDataList.Count; i++)
            {
                if (textDataList[i].dataType == TextDataType.Branch)
                {
                    selectLine.Add(i + 1);
                    selectTMP.text += textDataList[i].text + "\n";
                }
                else if (textDataList[i].dataType == TextDataType.QEnd)
                {
                    break;
                }
            }
            canSelect = true;
        }
        /// <summary>
        /// テキストを1文字ずつ表示するコルーチン
        /// </summary>
        /// <param name="text">表示したいテキストを入力</param>
        /// <param name="action">テキストを表示し終わった後に動かしたいプログラムを入れる</param>
        /// <returns></returns>
        IEnumerator TypeText(string text, Action action = null)
        {
            var s = textBoxTMP.text.Split('\n').ToList();
            if (s.Count > line)
            {
                s.RemoveAt(0);
                textBoxTMP.text = string.Join("\n", s);
            }
            textBoxTMP.ForceMeshUpdate();
            textBoxTMP.maxVisibleCharacters = textBoxTMP.GetParsedText().Length;
            textBoxTMP.text += TextUpdate(text);
            var wait = new WaitForSeconds(typingSpeed);
            for (int i = 0; i < text.Length; i++)
            {
                textBoxTMP.maxVisibleCharacters++;
                yield return wait;
            }
            textBoxTMP.text += "\n";
            readPoint++;
            action?.Invoke();
            textCoroutine = null;
        }
        string TextUpdate(string text)
        {
            bool colorMode = false;
            bool sizeMode = false;
            string returnText = "";
            int returnInt = 0;
            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '*':
                        if (!colorMode)
                        {
                            colorMode = true;
                            var colorNumber = int.Parse(text[i + 1].ToString() + text[i + 2]);
                            if (colors.Length < colorNumber)
                            {
                                Debug.LogError("そのインデックスに対応する色が設定されていません");
                                StopCoroutine(textCoroutine);
                                textCoroutine = null;
                            }
                            returnText += $"<color=#{ColorUtility.ToHtmlStringRGB(colors[colorNumber])}>";
                            i += 2;
                        }
                        else
                        {
                            colorMode = false;
                            returnText += "</color>";
                        }
                        break;
                    case '/':
                        if (!sizeMode)
                        {
                            int size = int.Parse(text[i + 1].ToString() + text[i + 2] + text[i + 3]);
                            returnText += $"<size={size}>";
                            sizeMode = true;
                            i += 3;
                        }
                        else
                        {
                            sizeMode = false;
                            returnText += "</size>";
                        }
                        break;
                    default:
                        returnText += text[i];
                        returnInt++;
                        break;
                }
            }
            return returnText;
        }
    }
}