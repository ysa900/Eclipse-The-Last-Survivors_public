using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Magnet : Item
    {
        PlayerData playerData;
        protected override void Awake()
        {
            base.Awake();
            playerData = PlayerManager.instance.playerData;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IPlayer iPlayer = collision.GetComponent<IPlayer>();
    
            if (iPlayer == null)
            {
                return;
            }

            StartCoroutine(SetMagnetRangeMax(3f)); // 자석 범위 늘려주기

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Pickup);
        }

        private IEnumerator SetMagnetRangeMax(float time)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            col.enabled = false;

            if (Mathf.Approximately(playerData.magnetRange_Additional, 999f))
            {
                // 이미 자석 범위가 늘어나 있는 경우 대기
                yield return new WaitWhile(() => Mathf.Approximately(playerData.magnetRange_Additional, 999f));
            }
            playerData.magnetRange_Additional = 999f; // 플레이어 자석 범위 늘려주기

            StartCoroutine(MagnetRangeRestore(time));
        }

        IEnumerator MagnetRangeRestore(float time)
        {
            yield return new WaitForSeconds(time);

            if (!StageManager.instance.isStageClear)
            {
                playerData.magnetRange_Additional = 0; // 플레이어 자석 범위 정상화
            }
            Destroy(gameObject); // 자석 오브젝트 삭제
        }
    }
}