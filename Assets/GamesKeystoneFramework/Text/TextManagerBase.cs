using System;
using GamesKeystoneFramework.Core.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamesKeystoneFramework.Text
{
    public class TextManagerBase : MonoBehaviour
    {
        [SerializeField] bool _useBranch = true;

        [SerializeField] TextMeshProUGUI _mainText;
        [SerializeField] TextMeshProUGUI _selectionText;
        [SerializeField] Image _mainTextImage;
        [SerializeField] Image _selectionTextImage;

        private void Start()
        {
            TextBoxHide();
            if (_useBranch)
            {
                SelectBoxHide();
            }
        }

        public virtual void StartText(TextDataScriptable textData)
        {
        }

        void TextBoxHide()
        {
            _mainText.gameObject.SetActive(false);
        }

        void SelectBoxHide()
        {
            _selectionText.gameObject.SetActive(true);
        }
    }
}