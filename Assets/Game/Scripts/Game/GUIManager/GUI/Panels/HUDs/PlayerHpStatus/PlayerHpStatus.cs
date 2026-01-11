using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.Panels
{
    public class PlayerHpStatus : MonoBehaviour
    {
        private Image hpBar;

        private GameObject shatterFx;
        private GameObject bloodFx;
        private GameObject characterborder;

        private Animator shatterFxAnime;
        private Animator characterPanelAnime;
        private Animator hpBorderAnime;

        TextMeshProUGUI hpText;
        float curHp = 0f;
        float startHp = 0f;

        bool isPlayerDead_old = false;

        private PlayerData _playerData;
        public PlayerData PlayerData
        {
            get { return _playerData; }
            set { _playerData = value; }
        }

        private void Awake()
        {
            hpBar = transform.Find("HpBar1").GetComponent<Image>();
            shatterFx = GameObject.Find("ShatterFx");
            bloodFx = GameObject.Find("BloodFx");
            characterborder = GameObject.Find("Character Border");

            shatterFxAnime = shatterFx.GetComponent<Animator>();
            characterPanelAnime = characterborder.GetComponent<Animator>();
            hpBorderAnime = transform.Find("HpBorder").GetComponent<Animator>();

            hpBar.fillAmount = startHp;
            hpText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start()
        {
            shatterFx.SetActive(false);
            bloodFx.SetActive(false);
        }

        private void Update()
        {
            curHp = _playerData.hp;

            hpBar.fillAmount = HpNormalized();
            hpText.text = curHp < 1f ? string.Format("{0}", Mathf.CeilToInt(curHp)) : string.Format("{0}", Mathf.FloorToInt(curHp));

            bool isPlayerDead = PlayerManager.player.isPlayerDead;
            if (isPlayerDead == isPlayerDead_old) return;
            if (isPlayerDead)
            {
                shatterFx.SetActive(true);
                bloodFx.SetActive(true);

                shatterFxAnime.SetTrigger("Break");
                characterPanelAnime.SetTrigger("Break");
                hpBorderAnime.SetTrigger("Break");
            }
            else
            {
                shatterFx.SetActive(false);
                bloodFx.SetActive(false);

                shatterFxAnime.Rebind();
                characterPanelAnime.Rebind();
                hpBorderAnime.Rebind();
            }
            isPlayerDead_old = isPlayerDead;
        }

        private float HpNormalized()
        {
            return curHp / PlayerData.maxHp;
        }
    }
}