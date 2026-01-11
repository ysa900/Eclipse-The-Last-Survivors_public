using Eclipse.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Splash.Splash2
{
    public class Client : Eclipse.Client
    {
        GUIManager guiManager;
        InputManager inputManager;
        AudioManager audioManager;

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
