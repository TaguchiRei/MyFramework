using System;
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

        public virtual void Next()
        {
            if (!questionMode)
            {
                if (textDataList.Count - 1 < readPoint)
                {
                    TextBoxHide();
                    return;
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
                            SelectBoxShow();
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
                    var s = textBoxTMP.text.Split("\n");
                    textBoxTMP.text = "";
                    for (int i = 0; i < s.Length - 1; i++)
                    {
                        textBoxTMP.text += s[i] + "\n";
                    }
                    bool colorMode = false;
                    bool sizeMode = false;
                    for (int i = 0; i < text.Length; i++)
                    {
                        var textUpdate = TextUpdate(i, text, colorMode, sizeMode);
                        i += textUpdate.Item1;
                        colorMode = textUpdate.Item2;
                        sizeMode = textUpdate.Item3;
                    }
                    textBoxTMP.text += "\n";
                    readPoint++;
                }
            }
            else if (canSelect == true)
            {
                if (textCoroutine == null)
                {
                    Debug.Log("決定が押されました。");
                    SelectBoxHide();
                    readPoint = selectLine[selectNumber];
                    selectLine.Clear();
                    questionMode = false;
                    ansMode = true;
                    Next();
                }
            }
        }

        /// <summary>
        /// 会話分岐を制作するときに使います
        /// </summary>
        /// <param name="i"></param>
        public void Question(int i, string question)
        {
            selectTMP.text = "";
            textBoxTMP.text = "";
            textCoroutine = StartCoroutine(TypeText(question, (() => canSelect = true)));
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
                    selectNumber++;
            }
            else
            {
                if (selectNumber != selectLine.Count - 1)
                    selectNumber--;
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
        /// テキストを1文字ずつ表示するコルーチン
        /// </summary>
        /// <param name="text">表示したいテキストを入力</param>
        /// <param name="action">テキストを表示し終わった後に動かしたいプログラムを入れる</param>
        /// <returns></returns>
        IEnumerator TypeText(string text, Action action = null)
        {
            var s = textBoxTMP.text.Split("\n");
            if (s.Length == line + 1)
            {
                textBoxTMP.text = s[1];
                for (int i = 2; i < s.Length; i++)
                {
                    textBoxTMP.text += "\n" + s[i];
                }
            }
            //テキストを処理しつつ表示させる
            bool colorMode = false;
            bool sizeMode = false;
            for (int i = 0; i < text.Length; i++)
            {
                var textUpdate = TextUpdate(i, text, colorMode, sizeMode);
                i += textUpdate.Item1;
                colorMode = textUpdate.Item2;
                sizeMode = textUpdate.Item3;
                if(colorMode || sizeMode)
                    continue;
                yield return new WaitForSeconds(typingSpeed);
            }
            textBoxTMP.text += "\n";
            readPoint++;
            action?.Invoke();
            action = null;
            textCoroutine = null;
        }
        (int, bool, bool) TextUpdate(int i, string text, bool colorMode, bool sizeMode)
        {
            int returnNumber = 0;
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
                        textBoxTMP.text += $"<color=#{ColorUtility.ToHtmlStringRGB(colors[colorNumber])}>";
                        returnNumber = 2;
                    }
                    else
                    {
                        colorMode = false;
                        textBoxTMP.text += "</color>";
                    }
                    break;
                case '/':
                    if (!sizeMode)
                    {
                        int size = int.Parse(text[i + 1].ToString() + text[i + 2] + text[i + 3]);
                        textBoxTMP.text += $"<size={size}>";
                        returnNumber = 3;
                        sizeMode = true;
                    }
                    else
                    {
                        sizeMode = false;
                        textBoxTMP.text += "</size>";
                    }
                    break;
                default:
                    textBoxTMP.text += text[i];
                    break;
            }
            return (returnNumber, colorMode, sizeMode);
        }
    }
}