using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Eclipse.Game
{
    public class InGameTextManager : MonoBehaviour
    {
        public static InGameTextManager Instance { get; private set; }

        [Header("폰트 & 머티리얼 설정")]
        [SerializeField] private TMP_FontAsset[] fonts = new TMP_FontAsset[2];
        [SerializeField] private Vector3 fontScale = new Vector3(0.3f, 0.3f, 0.3f);

        private List<InGameText> activeTexts = new();

        private Dictionary<string, Material> normalMats = new();
        private Dictionary<string, Material> critMats = new();

        [Header("머티리얼 매핑")]
        [Header("마법사")]
        [SerializeField] private Material fireNormalMat;
        [SerializeField] private Material electricNormalMat;
        [SerializeField] private Material waterNormalMat;

        [Header("전사")]
        [SerializeField] private Material holyNormalMat;
        [SerializeField] private Material bloodNormalMat;
        [SerializeField] private Material darkNormalMat;

        [Header("도적")]
        [SerializeField] private Material amgiNormalMat;
        [SerializeField] private Material ninshuNormalMat;
        [SerializeField] private Material trapsNormalMat;
        [SerializeField] private Material mijiNormalMat;
        [SerializeField] private Material legendaryNormalMat;

        [Header("노말 기본")]
        [SerializeField] private Material defaultNormalMat;

        [Header("크리티컬 도적")]
        [SerializeField] private Material amgiCritMat;
        [SerializeField] private Material ninshuCritMat;
        [SerializeField] private Material trapsCritMat;
        [SerializeField] private Material mijiCritMat;
        [SerializeField] private Material legendaryCritMat;

        [Header("크리티컬 기본")]
        [SerializeField] private Material defaultCritMat;

        private void Awake()
        {
            // 인스턴스 중복 방지
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // 머티리얼 매핑
            normalMats["Fire"] = fireNormalMat;
            normalMats["Electric"] = electricNormalMat;
            normalMats["Water"] = waterNormalMat;

            normalMats["Holy"] = holyNormalMat;
            normalMats["Blood"] = bloodNormalMat;
            normalMats["Dark"] = darkNormalMat;

            normalMats["Amgi"] = amgiNormalMat;
            normalMats["Ninshu"] = ninshuNormalMat;
            normalMats["Traps"] = trapsNormalMat;
            normalMats["Miji"] = mijiNormalMat;
            normalMats["Legendary"] = legendaryNormalMat;

            normalMats["Default"] = defaultNormalMat;

            critMats["Amgi"] = amgiCritMat;
            critMats["Ninshu"] = ninshuCritMat;
            critMats["Traps"] = trapsCritMat;
            critMats["Miji"] = mijiCritMat;
            critMats["Legendary"] = legendaryCritMat;

            critMats["Default"] = defaultCritMat;
        }

        public void ShowText(string text, string skillTag, bool isCritical, Vector3 targetPos)
        {
            InGameText txt = PoolManager.instance.GetInGameText(text, isCritical);
            if (txt == null) return;

            TextMeshPro tmp = txt.GetComponent<TextMeshPro>();

            // 폰트 설정
            tmp.font = isCritical ? fonts[1] : fonts[0];

            if (!normalMats.ContainsKey(skillTag)) skillTag = "Default";
            tmp.fontMaterial = isCritical ? critMats[skillTag] : normalMats[skillTag];

            // 랜덤 오프셋 추가
            Vector2 offset = UnityEngine.Random.insideUnitCircle * 0.3f;
            Vector3 spawnPos = targetPos + new Vector3(offset.x, offset.y + 0.2f, 0f); // 타겟 위쪽으로 약간 보정
            txt.transform.position = spawnPos;

            // InGameText 메테리얼 설정 적용
            txt.textMesh = tmp;

            // 리스트에 추가
            activeTexts.Add(txt);
        }

        private void Update()
        {
            for (int i = activeTexts.Count - 1; i >= 0; i--)
            {
                if (!activeTexts[i].Tick())
                {
                    PoolManager.instance.ReturnText(activeTexts[i].gameObject);
                    activeTexts.RemoveAt(i);
                }
            }
        }
    }
}
