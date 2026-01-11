using UnityEngine;
using System.Collections;

namespace Eclipse.Splash {

    public class GUIManager : Eclipse.Manager
    {

        //==================================================
        [ReadOnly] public SkipButton skipButton;

        //==================================================
        [ReadOnly] public GuideText guideText;

        //==================================================
        [ReadOnly] public CustomImage[] arrowImage;

        //==================================================
        [SerializeField] public KoreanTyperSimple[] koreanTyper;
        int currentPoint;

        //==================================================

        [ReadOnly] public CharacterImage characterImage;

        //==================================================


        protected virtual void Awake()
        {
            skipButton = GetWidget<SkipButton>();
            guideText = GetWidget<GuideText>();
            characterImage = GetWidget<CharacterImage>();


            //==================================================
            guideText.Hide();

            for (int index = 0; index < koreanTyper.Length; index++)
            {
                koreanTyper[index].onTextTypeFinish = OnTextTypeFinish;
            }

            for (int index = 1; index < koreanTyper.Length; index++)
            {
                koreanTyper[index].gameObject.SetActive(false);
            }

        }

        public T GetWidget<T>() where T : UnityEngine.Component
        {
            return gameObject.GetComponentInChildren<T>();
        }

        void Start()
        {
            currentPoint = 0;
            koreanTyper[currentPoint].gameObject.SetActive(true);  // ù ���� �ÿ� ù ��° Ÿ���� ����
            StartCoroutine(TypingInterruptedInput());  // ù ��° Ÿ���� �߿� �Է� �����ϱ�
        }

        protected void OnTextTypeFinish()
        {
            if (currentPoint != koreanTyper.Length - 1)  // ������ �ؽ�Ʈ �ƴ� ��� (0, 1 .. )
            {
                StartCoroutine(TypingInterruptedInput());
            }
            else  // ������ �ؽ�Ʈ�� ���� ���
            {
                StartCoroutine(WaitForLastInputAndSceneChange());  // ������ �ؽ�Ʈ ó�� ���� ~
                return;
            }

            if (koreanTyper.Length > 1)
            {
                arrowImage[currentPoint].Show(); // 0��°����~
            }

            StartCoroutine(TypingNextText());
        }

        IEnumerator TypingInterruptedInput()
        {
            yield return new WaitForSeconds(0.8f);

            // �߰��� �Է��� ������ ������ �� ǥ��(Ÿ����)�ϰ� �������� �̵�
            yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return));
            koreanTyper[currentPoint].isInputOccured = true;  // ��� ��ü ���� ǥ��
        }

        IEnumerator TypingNextText()
        {
            yield return new WaitForSeconds(0.35f); // 0.35�� ������ ���� 

            // currentPoint�� �迭�� ������ ���� �ʵ��� üũ
            // ���� �ؽ�Ʈ(KoreanTyperSimple[] �迭 ����) Ÿ���� ����
            if (currentPoint < koreanTyper.Length - 1)
            {
                koreanTyper[++currentPoint].gameObject.SetActive(true); // ���⼭ currentPoint ������Ű�� ���� �� Ÿ���� ����
            }
        }

        IEnumerator WaitForLastInputAndSceneChange()
        {
            yield return new WaitForSeconds(0.15f);  // ������ �ȳ� �ؽ�Ʈ ǥ�� 0.15f ������ �ɱ�
            guideText.Show();  // �ȳ� �ؽ�Ʈ ǥ��
        }
    }

}

