using System;
using System.Collections;
using System.Collections.Generic;
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
        /// <summary>
        /// 何行目かを保存
        /// </summary>
        private int _lineNumber = 0;
        private int _questionIndentation = 0;
        private int _readPoint = 0;
        
        
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
            action?.Invoke();
            TextBox(true);
            _dataList = TextUpdate(textDataScriptable.TextDataList[selectionIndex].DataList); 
            Next();
        }

        void Next()
        {
            switch (_dataList[_lineNumber].DataType)
            {
                case TextDataType.Text:
                    break;
                case TextDataType.Question:
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

        void TextWrite()
        {
            
        }
        
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