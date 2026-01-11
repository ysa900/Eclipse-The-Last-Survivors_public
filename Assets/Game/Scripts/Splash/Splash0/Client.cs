using System.Collections;
using UnityEngine;

namespace Eclipse.Splash.Splash0
{
    public class Client : Eclipse.Client
    {
        GUIManager guiManager;
        InputManager inputManager;
        AudioManager audioManager;
        SavedataManager saveDataManager;

        protected override void Awake()
        {
            base.Awake();

            guiManager = GameObject.FindObjectOfType<GUIManager>();
            guiManager.SetClient(this);

            inputManager = GameObject.FindObjectOfType<InputManager>();
            inputManager.SetClient(this);
            inputManager.onChangeScene = ChangeScene;

            audioManager = GameObject.FindAnyObjectByType<AudioManager>();
            audioManager.SetClient(this);

            saveDataManager = GameObject.FindAnyObjectByType<SavedataManager>();
            saveDataManager.SetClient(this);
            saveDataManager.LoadServerData();
            saveDataManager.LoadSettingData();
        }


        IEnumerator Start()
        {
            yield return StartCoroutine(LoadingScreenOut());

            var managers = GetManagers();
            foreach (var manager in managers)
            {
                manager.Startup();
            }
        }

    }
}
