﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    private static PlayerManager instance = null;

    public static PlayerManager GetInstance()
    {
        if (instance == null)
        {
            instance = GameManager.GetInstance().gameObject.AddComponent<PlayerManager>();
        }
        return instance;
    }
    #endregion


    private void Awake()
    {
        //instance = this;
        CreatePlayerList();
    }

    //Variables
    public List<Player> listOfPlayers = new List<Player>();

    //References for Cashing
    public Player playerWithFocus;
    //public CameraController cameraLocker;

    //private CameraController _cameraController;

    #region Unity API Methods
    // Start is called before the first frame update
    //void Start()
    //{
    //    //cameraLocker = Camera.main.GetComponent<CameraController>();
    //}

    // Update is called once per frame
    public void Update()
    {
        ChangePlayerFocusWithButton();
    }
    #endregion

    #region Class Methods
    /// <summary>
    /// Populates the list of Player's Objects (Towers and Food)
    /// </summary>
    private void CreatePlayerList()
    {
        foreach (Player p in GameObject.FindObjectsOfType<Player>())
        {
            listOfPlayers.Add(p);

            playerWithFocus = p;
        }        
    }

    /// <summary>
    /// Changes the Focus of a Tower or Food by pressing one Button
    /// </summary>
    private void ChangePlayerFocusWithButton()
    {
        if (Input.GetButtonDown("SwitchPlayer"))
        {
            if (listOfPlayers.Count == 0)
            {
                return;
            }
            ClearEnemyFocusOnListAndCamera();

            int index = listOfPlayers.IndexOf(playerWithFocus);
            index++;
            if (index >= listOfPlayers.Count)
            {
                index = 0;
            }
            playerWithFocus = listOfPlayers[index];
            CameraManager.GetInstance().isLocked = true;
        }
    }

    public void ClearEnemyFocusOnListAndCamera()
    {
        EnemyManager.GetInstance().ClearEnemyFocus();
        CameraManager.GetInstance().enemyWithFocus = null;
    }

    private void ChangePlayerFocusWithMouse()
    {
        EnemyManager.GetInstance().ClearEnemyFocus();
        CameraManager.GetInstance().enemyWithFocus = null;

        int index = listOfPlayers.IndexOf(playerWithFocus);
        index++;
        if (index >= listOfPlayers.Count)
        {
            index = 0;
        }
        playerWithFocus = listOfPlayers[index];
        CameraManager.GetInstance().isLocked = true;
    }
    #endregion

    public void AddPlayer(Player _player)
    {
        //Debug.Log($"PLayer added {_player}");
        if (playerWithFocus == null)
        {
            playerWithFocus = _player;
        }
        listOfPlayers.Add(_player);
    }

    public void RemovePlayer(Player _player)
    {
        listOfPlayers.Remove(_player);
    }
}
