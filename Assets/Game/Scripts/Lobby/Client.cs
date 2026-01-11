using System;
using System.Collections;
using UnityEngine;


namespace Eclipse.Lobby
{
    public class Client : Eclipse.Client
    {
        GUIManager guiManager;
        InputManager inputManager;
        AudioManager audioManager;
        SavedataManager saveDataManager;

        Server_PlayerData server_PlayerData;

        bool isUserLoggedin;

        protected override void Awake()
        {
            base.Awake();

            server_PlayerData = Resources.Load<Server_PlayerData>("Datas/Server_PlayerData");

            guiManager = GameObject.FindObjectOfType<GUIManager>();
            guiManager.SetClient(this);

            inputManager = GameObject.FindObjectOfType<InputManager>();
            inputManager.SetClient(this);
            inputManager.onChangeScene = ChangeScene;

            audioManager = GameObject.FindAnyObjectByType<AudioManager>();
            audioManager.SetClient(this);

            saveDataManager = GameObject.FindAnyObjectByType<SavedataManager>();
            saveDataManager.SetClient(this);
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