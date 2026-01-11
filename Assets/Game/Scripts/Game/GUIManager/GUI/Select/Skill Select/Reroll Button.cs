using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Eclipse.Game.SkillSelect
{
    public class RerollButton : CustomButton
    {
        public Server_PlayerData server_PlayerData;

        // 리롤 가능 횟수
        private int _rerollNum;
        public int RerollNum
        {
            get { return _rerollNum; }
            set
            {
                _rerollNum = value;
                PlayerPrefs.SetInt("RerollNum", value);
                HandleRerollNumUI();
            }
        }

        TextMeshProUGUI rerollText;

        protected override void Awake()
        {
            base.Awake();
            rerollText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetRerollNum()
        {
            if (SceneManager.GetActiveScene().name == "Stage1")
            {
                PlayerPrefs.SetInt("RerollNum", 2 + server_PlayerData.specialPassiveLevels[6]);
            }
            _rerollNum = PlayerPrefs.GetInt("RerollNum");
            HandleRerollNumUI();
        }

        // _rerollNum 변경 시 처리
        private void HandleRerollNumUI()
        {
            bool isRerollNumMoreThanZero = _rerollNum > 0;

            // 리롤 횟수가 0인 경우
            if (!isRerollNumMoreThanZero)
            {
                // 상호작용도 안되게 하고, 깜빡이지도 않게 하기
                GetComponent<Button>().interactable = false;
                GetComponentInChildren<UnscaledAnimationController>().StopAnimation();
            }
            // 잔여 횟수가 남아있는 경우
            else
            {
                GetComponent<Button>().interactable = isRerollNumMoreThanZero;
            }

            rerollText.text = _rerollNum.ToString();
        }

        private IEnumerator WaitForRerollButtonAndStartAnimation(GameObject rerollButton)
        {
            // 버튼 활성화를 기다림
            while (!rerollButton.activeSelf)
            {
                yield return null;
            }

            // 버튼이 활성화된 뒤 애니메이션 시작
            rerollButton.GetComponentInChildren<UnscaledAnimationController>().StartAnimation();
        }
    }
}