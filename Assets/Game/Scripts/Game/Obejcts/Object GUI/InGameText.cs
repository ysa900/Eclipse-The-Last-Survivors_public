using Eclipse.Game;
using TMPro;
using UnityEngine;

public class InGameText : MonoBehaviour, IPoolingObject
{
    public TextMeshPro textMesh;
    private GameObject criticalEffect;
    private SpriteRenderer criticalSprite;

    // 외부 데이터
    public string showText;
    public bool isCritical;

    private float lifetime;
    private float normalDuration = 1f;
    private float criticalDuration = 1.5f;

    private float spawnTime;
    private float moveSpeed = 0.15f;
    private float alphaSpeed = 3f;
    private float scaleLerpSpeed = 3f;

    private float delayTime = 0.5f;
    private float delayTimer = 0f;

    private Vector3 defaultScale = new Vector3(0.3f, 0.3f, 0.3f);
    private Vector3 criticalScale => defaultScale * 2f;

    private Color alphaColor;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        criticalEffect = GetComponentInChildren<CriticalEffect>().gameObject;
        criticalSprite = criticalEffect.GetComponentInChildren<SpriteRenderer>(true);
    }

    public void Init()
    {
        spawnTime = Time.time;
        delayTimer = 0f;

        textMesh.text = showText;
        transform.localScale = isCritical ? criticalScale : defaultScale;
        lifetime = isCritical ? criticalDuration : normalDuration;

        alphaColor = textMesh.color;
        alphaColor.a = 1f;
        textMesh.color = alphaColor;

        if (isCritical)
        {
            criticalEffect.SetActive(true);
            SetCriticalAlpha(1f);
        }
        else
        {
            criticalEffect.SetActive(false);
        }
    }

    public bool Tick()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        if (delayTimer > delayTime)
        {
            alphaColor.a = Mathf.Lerp(alphaColor.a, 0f, Time.deltaTime * alphaSpeed);
            textMesh.color = alphaColor;

            if (isCritical)
                SetCriticalAlpha(alphaColor.a);
        }

        if (isCritical)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, Time.deltaTime * scaleLerpSpeed);
        }

        delayTimer += Time.deltaTime;
        return Time.time - spawnTime < lifetime;
    }

    private void SetCriticalAlpha(float alpha)
    {
        Color color = criticalSprite.color;
        color.a = alpha;
        criticalSprite.color = color;
    }
}
