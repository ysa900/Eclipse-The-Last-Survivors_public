using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Game
{
    public class Shield_Flame : PlayerAttachSkill
    {
        public delegate void OnShieldSkillDestroyed();
        public OnShieldSkillDestroyed onShieldSkillDestroyed;

        // Shield_Flame 컴포넌트들
        Transform absorberTransform;
        CircleCollider2D circleCollider2D;
        PolygonCollider2D polygonCollider2D;
        PointEffector2D pointEffector2D;

        // 자식 컴포넌트들
        List<Transform> childTransforms = new List<Transform>();

        bool oldisPlayerLookLeft = false;
        bool isAbsorberOn;
    
        float absorbTime = 1.47f;
        float absorbTimer = 0f;
    
        protected override void Awake()
        {
            base.Awake();
    
            absorberTransform = transform.Find("Absorber");
            circleCollider2D = absorberTransform.GetComponent<CircleCollider2D>();
            polygonCollider2D = absorberTransform.GetComponent<PolygonCollider2D>();
            pointEffector2D = absorberTransform.GetComponent<PointEffector2D>();

            for (int i = 0; i < transform.childCount; i++)
            {
                childTransforms.Add(transform.GetChild(i));
            }
        }
    
        public override void Init()
        {
            absorberTransform.gameObject.SetActive(false);
            absorbTimer = 0;
            isAbsorberOn = false;

            circleCollider2D.enabled = true;
            polygonCollider2D.enabled = false;
            pointEffector2D.forceMagnitude = -150f;
            pointEffector2D.forceMode = EffectorForceMode2D.Constant;
    
            base.Init();
        }
    
        
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime - 0.21f; // ������ 0.2�� ���� ���� (0.21�� �� ������ ���� ����)
    
            if (destroySkill)
            {
                StartCoroutine(Destroy()); // ������ 0.2�� ���� magnitude + 3000 �����ؼ� ���� ��ġ�� ��
    
                return;
            }
            else
            {
                AttachPlayer();
            }
    
            bool isAbsorbTime = absorbTimer > absorbTime;
            if (isAbsorbTime && !isAbsorberOn) 
            {
                absorberTransform.gameObject.SetActive(true);
                StartCoroutine(Active_Absorber());
                isAbsorberOn = true;
            }
    
            base.Update();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            absorbTimer += Time.deltaTime;
        }

        public new void AttachPlayer()
        {
            X = PlayerManager.player.transform.position.x;
            Y = PlayerManager.player.transform.position.y;
    
            spriteRenderer.flipX = PlayerManager.player.isPlayerLookLeft;
    
            if (PlayerManager.player.isPlayerLookLeft == oldisPlayerLookLeft) return; // �÷��̾ flip �������� return
    
            float xPosNum = 0;
            if (PlayerManager.player.isPlayerLookLeft) xPosNum = -xOffset;
            else xPosNum = xOffset;
    
            for (int i = 0; i < transform.childCount; i++)
            {
                Vector2 childPosition = childTransforms[i].position;
                childPosition.x += xPosNum;
                childTransforms[i].position = childPosition;
            }
    
            oldisPlayerLookLeft = PlayerManager.player.isPlayerLookLeft;
        }
    
        IEnumerator Destroy()
        {
            circleCollider2D.enabled = true;
            polygonCollider2D.enabled = false;
            pointEffector2D.forceMagnitude = 300;
    
            yield return new WaitForSeconds(0.2f);
    
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���
    
            onShieldSkillDestroyed();
    
            PoolManager.instance.ReturnSkill(this, returnIndex);
        }
    
        IEnumerator Active_Absorber()
        {
            yield return new WaitForSeconds(0.05f);
    
            circleCollider2D.enabled = false;
            polygonCollider2D.enabled = true;
        }
    }
}