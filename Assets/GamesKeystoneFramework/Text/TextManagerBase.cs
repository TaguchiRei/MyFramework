using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GamesKeystoneFramework.Core.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamesKeystoneFramework.Text
{
    public abstract class TextManagerBase : MonoBehaviour
    {
        //使用者が設定に使用
        [SerializeField] private bool useBranch = true;
        [SerializeField] private bool displayCharOneByOne;
        [SerializeField] private bool resetForQuestion;
        [SerializeField] private int line = 3;
        [SerializeField] private float writeSpeed = 0.1f;
        [SerializeField] private string[] names;


        //必須
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private TextMeshProUGUI selectionText;
        [SerializeField] private Image mainTextImage;
        [SerializeField] private Image selectionTextImage;

        //処理に使用する
        private List<TextData> _dataList;
        private List<(string,int)> _choices = new();
        private int _lineNumber = 0;
        private int _questionIndentation = 0;
        private bool _movingCoroutin;
        private bool _selectMode = false;
        private Coroutine _typeTextCoroutine;
        
        public int SelectNumber;
        
        
        private void Start()
        {
            TextBox();
            if (useBranch)
            {
                SelectBox();
            }
        }

        public virtual void TextStart(TextDataScriptable textDataScriptable, int selectionIndex, Action action = null)
        {
            _lineNumber = 0;
            action?.Invoke();
            TextBox(true);
            _dataList = TextUpdate(textDataScriptable.TextDataList[selectionIndex].DataList); 
            Next();
        }

        /// <summary>
        /// 文章表示中に決定ボタンを押したときの処理を一括で管理
        /// </summary>
        void Next()
        {
            if (_movingCoroutin)
            {
                //一文字づつ表示を強制終了させてすべて表示
                StopCoroutine(_typeTextCoroutine);
                _typeTextCoroutine = null;
                mainText.maxVisibleCharacters = mainText.GetParsedText().Length;
            }
            else
            {
                if (_selectMode)
                {
                    //セレクトモード中に決定が押されたときの処理
                }
                else
                {
                    //通常時の処理
                    BranchCheck();
                }
            }
        }

        /// <summary>
        /// DataTypeから次の処理を決める
        /// </summary>
        void BranchCheck()
        {
            //次のテキストを読み込んで適切な処理をする
            switch (_dataList[_lineNumber].DataType)
            {
                case TextDataType.Text:
                     _typeTextCoroutine = StartCoroutine(TypeText(_dataList[_lineNumber].Text));
                    break;
                case TextDataType.Question:
                    //selectModeをonにする
                    _selectMode = true;
                    //何段目の質問化を記憶する
                    _questionIndentation++;
                    //選択肢とメインテキストの初期化を行う
                    selectionText.text = "";
                    if (resetForQuestion)
                        mainText.text = "";
                    SelectNumber = 0;
                    _typeTextCoroutine = StartCoroutine(TypeText(_dataList[_lineNumber].Text,(() =>
                    {
                        SelectorShow();
                    })));
                    break;
                case TextDataType.Branch:
                            
                    break;
                case TextDataType.QEnd:
                    break;
                case TextDataType.TextEnd:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// テキストを表示するコルーチン。
        /// </summary>
        /// <param name="text">表示したいテキスト</param>
        /// <param name="action">テキスト表示後に実行される</param>
        /// <returns></returns>
        IEnumerator TypeText(string text,Action action = null)
        {
            _movingCoroutin = true;
            //何行あるかを調べ、新規で行が追加されることを念頭に基底行数より多ければ一行目を削除して繰り上げする。
            var s = mainText.text.Split('\n').ToList();
            var sb = new StringBuilder();
            if (s.Count + 1 > line)
            {
                s.RemoveAt(0);
                sb.Append(string.Join("\n", s));
            }
            mainText.maxVisibleCharacters = mainText.GetParsedText().Length;
            sb.Append("\n");
            sb.Append(text);
            mainText.text = sb.ToString();
           
            if (displayCharOneByOne)
            {
                //一文字づつ表示
                for (int i = 0; i < text.Length; i++)
                {
                    mainText.maxVisibleCharacters++;
                    yield return new WaitForSeconds(writeSpeed);
                }
            }
            else
            {
                //一気に表示
                mainText.maxVisibleCharacters = mainText.GetParsedText().Length;
            }
            //完了後の処理
            _lineNumber++;
            action?.Invoke();
            _movingCoroutin = false;
            _typeTextCoroutine = null;
        }
        
        /// <summary>
        /// 選択肢を表示するのに使用
        /// </summary>
        void SelectorShow()
        {
            SelectBox(true);
            for (int i = _lineNumber; i < _dataList.Count; i++)
            {
                if (_dataList[i].DataType == TextDataType.Branch)
                {
                    _choices.Add((_dataList[i].Text, i));
                }
                else if (_dataList[i].DataType == TextDataType.QEnd)
                {
                    break;
                }
            }
        }
        
        /// <summary>
        /// テキストの中の名前を設定する
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        private List<TextData> TextUpdate(List<TextData> dataList)
        {
            const string pattern = @"/name(\d)";
            foreach (var t in dataList)
            {
                var matches = Regex.Matches(t.Text, pattern);
                var replacementText = t.Text;
                for (var j = 0; j < matches.Count; j++)
                {
                    replacementText =
                        replacementText.Replace(matches[j].ToString(), names[int.Parse(matches[j].Value)]);
                }
                t.Text = replacementText;
            }
            return dataList;
        }

        /// <summary>
        /// テキストボックスを表示非表示する
        /// </summary>
        /// <param name="show"></param>
        protected virtual void TextBox(bool show = false)
        {
            mainText.gameObject.SetActive(show);
            mainTextImage.gameObject.SetActive(show);
        }

        /// <summary>
        /// セレクトボックスを表示非表示する
        /// </summary>
        /// <param name="show"></param>
        protected virtual void SelectBox(bool show = false)
        {
            selectionText.gameObject.SetActive(show);
            selectionTextImage.gameObject.SetActive(show);
        }
    }
}