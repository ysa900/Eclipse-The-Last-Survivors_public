using UnityEditor;
using UnityEngine;
using TMPro;

public class TMPMaterialInspector : EditorWindow
{
    [MenuItem("Tools/TMP 머티리얼 → 연결된 폰트 찾기")]
    public static void ShowWindow()
    {
        GetWindow<TMPMaterialInspector>("TMP Font 추적기");
    }

    private Material targetMaterial;

    void OnGUI()
    {
        GUILayout.Label("TextMeshPro 머티리얼 분석", EditorStyles.boldLabel);

        targetMaterial = (Material)EditorGUILayout.ObjectField("TMP Material", targetMaterial, typeof(Material), false);

        if (targetMaterial != null && GUILayout.Button("폰트 찾기"))
        {
            FindFontAssetLinkedToMaterial(targetMaterial);
        }
    }

    private void FindFontAssetLinkedToMaterial(Material mat)
    {
        Texture mainTex = mat.GetTexture("_MainTex");
        if (mainTex == null) mainTex = mat.GetTexture("_FaceTex");

        if (mainTex == null)
        {
            Debug.LogWarning("이 머티리얼은 _MainTex 또는 _FaceTex를 가지고 있지 않음.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        bool found = false;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);

            if (font != null && font.atlasTexture == mainTex)
            {
                Debug.Log($"연결된 폰트: <b>{font.name}</b> @ {path}", font);
                EditorGUIUtility.PingObject(font);
                found = true;
            }
        }

        if (!found)
        {
            Debug.LogWarning("이 텍스처를 사용하는 TMP_FontAsset을 찾을 수 없음.");
        }
    }
}
