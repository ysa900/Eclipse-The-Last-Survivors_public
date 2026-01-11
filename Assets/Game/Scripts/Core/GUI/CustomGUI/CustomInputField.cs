using TMPro;
using System;

namespace Eclipse
{
    public class CustomInputField : CustomGUI
    {
        protected TMP_InputField _inputField;
        public TMP_InputField InputField
        {
            get { return _inputField; }
        }

        public Action<string> onValueChanged; // 사용자가 텍스트를 입력하거나 수정할 때
        public Action<string> onEndEdit; // 사용자가 입력을 완료하고 포커스를 다른 곳으로 이동할 때
        public Action<string> onSubmit; // 사용자가 Enter 키를 눌렀을 때
        public Action<string> onSelect; // 사용자가 필드를 클릭하여 포커스를 맞출 때
        public Action<string> onDeselect; // 사용자가 필드를 떠나 다른 곳으로 포커스를 옮길 때    

        protected virtual void OnEnable()
        {
            _inputField = gameObject.GetComponent<TMP_InputField>();

            InputField.onValueChanged.RemoveAllListeners();
            InputField.onEndEdit.RemoveAllListeners();
            InputField.onSubmit.RemoveAllListeners();
            InputField.onSelect.RemoveAllListeners();
            InputField.onDeselect.RemoveAllListeners();

            InputField.onValueChanged.AddListener(OnValueChanged);
            InputField.onEndEdit.AddListener(OnEndEdit);
            InputField.onSubmit.AddListener(OnSubmit);
            InputField.onSelect.AddListener(OnSelect);
            InputField.onDeselect.AddListener(OnDeselect);
        }

        private void OnValueChanged(string text)
        {
            onValueChanged?.Invoke(text);
        }
        private void OnEndEdit(string text)
        {
            onEndEdit?.Invoke(text);
        }
        private void OnSubmit(string text)
        {
            onSubmit?.Invoke(text);
        }
        private void OnSelect(string text)
        {
            onSelect?.Invoke(text);
        }
        private void OnDeselect(string text)
        {
            onDeselect?.Invoke(text);
        }

        public void MakeInteractable()
        {
            InputField.interactable = true;
        }

        public void MakeUnInteractable()
        {
            InputField.interactable = false;
        }
    }
}