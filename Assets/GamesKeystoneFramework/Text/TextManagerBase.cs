using System;
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
        [SerializeField] private string[] names;


        //必須
        [SerializeField] TextMeshProUGUI mainText;
        [SerializeField] TextMeshProUGUI selectionText;
        [SerializeField] Image mainTextImage;
        [SerializeField] Image selectionTextImage;

        //処理に使用する
        List<TextData> _dataList;

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
            
        }

        public void NextText()
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