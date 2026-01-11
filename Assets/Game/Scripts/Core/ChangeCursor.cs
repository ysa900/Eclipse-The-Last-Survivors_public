using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // Texture2D로 변환하고 싶은 Sprite(SpriteRenderer)

    void Start()
    {
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            // Sprite에서 Texture2D로 변환
            Texture2D cursorImg = TextureFromSprite(spriteRenderer.sprite);
            if (cursorImg != null)
            {
                // 확대된 커서 텍스처 생성
                Texture2D resizedCursorImg = ResizeTexture(cursorImg, cursorImg.width * 3, cursorImg.height * 3);

                // 확대된 커서를 설정
                Cursor.SetCursor(resizedCursorImg, Vector2.zero, CursorMode.Auto);
            }
        }
        else
        {
            Debug.LogError("SpriteRenderer 또는 Sprite가 null입니다.");
        }
    }

    // Sprite를 Texture2D로 변환
    public static Texture2D TextureFromSprite(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels(
                (int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height
            );
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
        {
            return sprite.texture;
        }
    }

    // 텍스처 크기를 변경하는 함수
    public static Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
    {
        Texture2D resizedTexture = new Texture2D(newWidth, newHeight, source.format, false);

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                Color color = source.GetPixelBilinear((float)x / newWidth, (float)y / newHeight);
                resizedTexture.SetPixel(x, y, color);
            }
        }
        resizedTexture.Apply();
        return resizedTexture;
    }
}