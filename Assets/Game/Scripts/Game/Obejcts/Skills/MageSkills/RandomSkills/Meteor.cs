using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Meteor : RandomSkill
    {
        float impactPonitX;
        float impactPonitY;
    
        Vector2 direction; // ���ư� ����
    
        bool isShadowAlive; // �׸��ڰ� ��������� ���İ��� �����ϱ� ����
        float alpha = 0;

        float aliveTime_explode = 0.5f;
        float aliveTime_shadow = 0.6f;
    
        Meteor_Explode skillObject; // 메테오 그림자
        SpriteRenderer shadowSpriteRenderer;
    
        public override void Init()
        {
            float tmpX = PlayerManager.player.transform.position.x;
            float tmpY = PlayerManager.player.transform.position.y;
    
            float ranNum = UnityEngine.Random.Range(-3f, 2f);
            float ranNum2 = UnityEngine.Random.Range(-1.6f, 0.6f);
    
            tmpX += ranNum;
            tmpY += ranNum2;
    
            impactPonitX = tmpX;
            impactPonitY = tmpY;
    
            // ���׿� ���� ���� (�浹 ���� �ٶ󺸰�)
            Vector2 direction = new Vector2(-1.8f, -2.8f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
            Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
            transform.rotation = rotation;
    
            // ���׿� ��ġ ����
            X = tmpX + 1.8f;
            Y = tmpY + 2.8f;
    
            SetDirection();
    
            // �׸��� ��Ÿ���� �����, ��ġ ���� ���� ��
            StartCoroutine(DisplayShadowNDestroy(tmpX + 2.2f / 5, tmpY + 2.4f / 5));
    
            alpha = 0;
    
            base.Init();
        }
    
        protected void Start()
        {
            speed = 5f;
        }
    
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;
    
            if (destroySkill)
            {
                Meteor_Explode explode;
                explode = PoolManager.instance.GetSkill(3) as Meteor_Explode;
    
                explode.X = X;
                explode.Y = Y;
    
                explode.AliveTime = aliveTime_explode;
                explode.Damage = damage;
    
                Transform parent = explode.transform.parent;
    
                explode.transform.parent = null;
                explode.transform.localScale = new Vector3(scale, scale, 0);
                explode.transform.parent = parent;
    
                explode.skillIndex = skillIndex;
                explode.onSkillAttack = onSkillAttack;
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���
    
                PoolManager.instance.ReturnSkill(this, returnIndex);
                
                return;
            }
            else
            {
                MoveToimpactPonit();
            }
    
            // meteor �׸��� ����
            if (isShadowAlive)
            {
                if (alpha < 1f)
                    alpha += 0.02f;
            }
            else
                alpha = 0f;

            shadowSpriteRenderer.color = new Color(1f, 1f, 1f, alpha);
    
            base.Update();
        }
    
        // ���ư� ������ ���ϴ� �Լ�
        private void SetDirection()
        {
            Vector2 impactVector = new Vector2(impactPonitX, impactPonitY);
            Vector2 nowVector = transform.position;
    
            direction = impactVector - nowVector;
    
            direction = direction.normalized;
        }
    
        // ���� �������� �̵��ϴ� �Լ�
        private void MoveToimpactPonit()
        {
            rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);
    
            X = transform.position.x;
            Y = transform.position.y;
        }
    
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // �ƹ��͵� ���� �ʱ� (����� �ȵ�, ����� virtual�� ���� �� RandomSkill�� TriggerEnter�� �۵� ��)
        }
    
        // ���׿� ������ �� �׸��� ������Ʈ ���� �� ����
        IEnumerator DisplayShadowNDestroy(float x, float y)
        {
            skillObject = PoolManager.instance.GetSkill(5) as Meteor_Explode;

            skillObject.transform.position = new Vector2(x, y);
    
            // �׸��� sacle ����
            Transform parent = skillObject.transform.parent;
    
            skillObject.transform.parent = null;
    
            float myScale = transform.localScale.x;

            // * 6 / 1.5�� ���׿��� ���׿� �׸��� ������ ������ ����
            skillObject.transform.parent = null;
            skillObject.transform.localScale = new Vector3(myScale * 6 / 1.5f, myScale * 6 / 1.5f, 0);
            skillObject.transform.parent = parent;

            shadowSpriteRenderer = skillObject.GetComponent<SpriteRenderer>();
            shadowSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
    
            isShadowAlive = true;
    
            skillObject.AliveTime = aliveTime_shadow;
    
            yield return new WaitForSeconds(aliveTime_shadow); // ������ �� ��ŭ ����
    
            isShadowAlive = false;
    
            PoolManager.instance.ReturnSkill(skillObject, 5);
        }
    }
}