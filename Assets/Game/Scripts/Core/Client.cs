using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse
{
    public class Client : MonoBehaviour
    {
        [SerializeField] private LoadingScreen loadingScreen;

        [ReadOnly][SerializeField] private List<Manager> managers = new List<Manager>();
        [ReadOnly][SerializeField] private bool isStartup;

        // 씬 변경 시 처리용
        public bool isChangingScene = false; // clinet를 싱글톤으로 바꾼다면, 추가 처리 필요

        protected virtual void Awake()
        {
            loadingScreen = FindAnyObjectByType<LoadingScreen>();
        }

        public T GetManager<T>() where T : Manager
        {
            //var item = managers.Find(e => e.GetType() == typeof(T)); // 정확히 해당 클래스만 찾을 수 있음
            var item = managers.Find(e => e is T); // 자식 클래스까지 찾아 반환할 수 있도록 is 키워드 사용
            if (item != null)
            {
                return item as T;
            }

            throw new System.Exception();
        }

        public List<Manager> GetManagers()
        {
            return managers;
        }

        public void AddManager(Manager manager)
        {
            managers.Add(manager);
        }

        protected void ChangeScene(string sceneName)
        {
            IEnumerator Process()
            {
                yield return StartCoroutine(LoadingScreenIn());

                // 비동기로 씬 로드 시작
                var ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
                ao.allowSceneActivation = false;

                // 씬이 로딩될 동안 대기
                while (ao.progress < 0.9f)
                {
                    yield return null;
                }

                ao.allowSceneActivation = true;
            }

            isChangingScene = true;
            StartCoroutine(Process());
        }

        protected IEnumerator LoadingScreenIn()
        {
            loadingScreen.Init();
            yield return StartCoroutine(loadingScreen.In());
        }

        protected IEnumerator LoadingScreenOut()
        {
            loadingScreen.Init();
            yield return StartCoroutine(loadingScreen.Out());
        }

    }
}