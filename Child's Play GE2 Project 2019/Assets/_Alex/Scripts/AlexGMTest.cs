﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is to be place in the GameManager script
/// </summary>
public class AlexGMTest : MonoBehaviour
{
    //[SerializeField] private float currentcash = 5;
    [SerializeField] private MenuInteraction _menuInteractionRef;
    [SerializeField] private GameObject tileSelectionCursor;
    [SerializeField] private GameObject tileSelectedCursor;
    [SerializeField] private GameObject[] listOfTower;
    [SerializeField] private GameObject[] listOfTowerPlaceHolder;
    [SerializeField] private GameObject[] listOfBarrier;
    [SerializeField] private GameObject[] listOfBarrierPlaceHolder;
    [SerializeField] private int selectedTowerIndex = 0;
    [SerializeField] private int selectedBarrierIndex = 0;

    //Daniel Temporary
    [SerializeField] private HudManager _hudManagerRef;
    private int legoTowerIndex = 1;
    private int soldierTowerIndex = 0;

    [SerializeField] private int initialMoney = 100;
    private Money myMoney;

    //-//
    // To be placed in UI management script
    [SerializeField] private Text UITextSelectedTile;
    [SerializeField] private Text UITextCash;
    //-//

    private ItemTile selectedTile;
    public ItemTile SelectedTile { get => selectedTile; }
    public GameObject TileSelectionCursor { get => tileSelectionCursor; }

    public int SelectedTowerIndex { get => selectedTowerIndex; set => selectedTowerIndex = value; }
    public int SelectedBarrierIndex { get => selectedBarrierIndex; set => selectedBarrierIndex = value; }

    //All Managers
    //private PlayerManager PlayerManager.GetInstance();

    // Start is called before the first frame update
    void Start()
    {
        ItemSelectionReset(); // for testing
        UpdateSelectedTileText();

        myMoney = gameObject.AddComponent<Money>();
        myMoney.CurrentMoney = initialMoney; // This value changes at the beginning of new level.
        UpdateCashText(); // to be place in UI management script
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            DeselectTile();
        }
    }

    /// <summary>
    /// Deselect the current selected tile,
    /// hide all placeholder items
    /// and call the method to update the selected tile text.
    /// </summary>
    public void DeselectTile()
    {
        tileSelectedCursor.SetActive(false);

        HidePlaceHolders();
        selectedTile = null;
        UpdateSelectedTileText();

        PanelSelection(_menuInteractionRef.defaultIndex);
    }

    void ItemSelectionReset()
    {
        for (int i = 0; i < listOfBarrierPlaceHolder.Length; i++)
        {
            listOfBarrierPlaceHolder[i] = Instantiate(listOfBarrierPlaceHolder[i]);
        }

        for (int i = 0; i < listOfTowerPlaceHolder.Length; i++)
        {
            listOfTowerPlaceHolder[i] = Instantiate(listOfTowerPlaceHolder[i]);
        }

        HidePlaceHolders();
        PanelSelection(_menuInteractionRef.defaultIndex); //Daniel temporary testing
        tileSelectionCursor.SetActive(false);
        tileSelectedCursor.SetActive(false);
    }

    /// <summary>
    /// Select a tile and sets it as the tile selected.
    /// </summary>
    /// <param name="tile">Tile to select</param>
    public void TileSelection(ItemTile tile)
    {
        HidePlaceHolders();
        ShowCursorOnTile(tileSelectedCursor, tile);
        selectedTile = tile;
        UpdateSelectedTileText();
        if (tile.CurrentItem != null)
        {
             PanelSelection(_menuInteractionRef.storeIndex);  //Daniel temporary testing
            _hudManagerRef.Display(listOfTower[SelectedTowerIndex]);  //Daniel Temporary testing
            return;
        }

        switch (tile.TileType)
        {
            case TileType.Tower:
                 PanelSelection(_menuInteractionRef.storeIndex);  //Daniel temporary testing
                _hudManagerRef.Display(listOfTower[selectedTowerIndex]);  //Daniel Temporary testing
                ShowItemOnTile(listOfTowerPlaceHolder[selectedTowerIndex], tile);
                break;
            case TileType.Barrier:
                PanelSelection(_menuInteractionRef.storeIndex); //Daniel temporary testing
                _hudManagerRef.Display(listOfBarrier[selectedBarrierIndex]);
                ShowItemOnTile(listOfBarrierPlaceHolder[selectedBarrierIndex], tile);
                break;
            default:
                break;
        }
    }

    private void HidePlaceHolders()
    {
        foreach (var item in listOfBarrierPlaceHolder)
        {
            item.SetActive(false);
        }
        foreach (var item in listOfTowerPlaceHolder)
        {
            item.SetActive(false);
        }
    }

    /// <summary>
    /// Show cursor on tile.
    /// </summary>
    /// <param name="cursor">Cursor to show</param>
    /// <param name="tile">Tile to show cursor on</param>
    public void ShowCursorOnTile(GameObject cursor, ItemTile tile)
    {
        cursor.SetActive(true);
        cursor.transform.position = tile.transform.position;
        cursor.transform.rotation = tile.transform.rotation;
        cursor.transform.position += Vector3.up * 3.01f;
    }

    /// <summary>
    /// Show the selected item on the selected tile as preview.
    /// </summary>
    /// <param name="item">Item to show</param>
    /// <param name="tile">Selected Tile</param>
    private void ShowItemOnTile(GameObject item, ItemTile tile)
    {
        item.SetActive(true);
        item.transform.position = tile.transform.position;
        item.transform.rotation = tile.transform.rotation;
        item.transform.position += Vector3.up * 3.0f;
    }

    /// <summary>
    /// Place an item on the selected tile if it is empty.
    /// </summary>
    public void PlaceItem()
    {
        if (selectedTile == null)
        {
            Debug.Log("No Tile Selected.");
            return;
        }
        if (selectedTile.CurrentItem != null)
        {
            Debug.Log("Already an Item, remove current Item before.");
            return;
        }

        switch (selectedTile.TileType)
        {
            case TileType.Tower:
                if (!myMoney.TryToBuy(listOfTower[selectedTowerIndex].GetComponent<Item>().Value))
                {
                    return;
                }
                InstantiateItemOnTile(listOfTower[selectedTowerIndex]);
                break;
            case TileType.Barrier:
                if (!myMoney.TryToBuy(listOfTower[selectedTowerIndex].GetComponent<Item>().Value))
                {
                    return;
                }
                InstantiateItemOnTile(listOfBarrier[selectedBarrierIndex]);
                break;
            default:
                break;
        }
        selectedTile.CurrentItem.name = selectedTile.CurrentItem.GetComponent<Item>().ItemName;
        TileSelection(selectedTile);
    }
    
    /// <summary>
    /// Remove and destroy the Item on the current selected tile, and call the sell method.
    /// </summary>
    public void RemoveItem()
    {
        if (selectedTile == null)
        {
            Debug.Log("No Tile Selected.");
            return;
        }
        if (selectedTile.CurrentItem == null)
        {
            Debug.Log("No Item on the current selected Tile.");
            return;
        }
        myMoney.MoneyChange(selectedTile.CurrentItem.GetComponent<Item>().Value); //Sell item
        PlayerManager.GetInstance().RemovePlayer(selectedTile.CurrentItem.GetComponent<Item>());
        Destroy(selectedTile.CurrentItem.gameObject);
        selectedTile.CurrentItem = null;
        TileSelection(selectedTile);
    }

    // to be placed in UI management script
    public void UpdateCashText()
    {
        UITextCash.text = $"Money: {myMoney.CurrentMoney}";
    }

    // to be placed in UI management script
    private void UpdateSelectedTileText()
    {
        if (UITextSelectedTile != null)
        {
            UITextSelectedTile.text = $"Selected Tile: " + (selectedTile != null ? selectedTile.name : "");
            if (selectedTile != null)
            {
                UITextSelectedTile.text += $"\nCurrent Item: " + (selectedTile.CurrentItem != null ? selectedTile.CurrentItem.name : "");
            }
            else
            {
                UITextSelectedTile.text += $"\nCurrent Item: ";
            }
        }
    }
    
    public void InstantiateItemOnTile(GameObject item)
    {
        selectedTile.CurrentItem =
                    Instantiate(
                        item,
                        selectedTile.transform.position + Vector3.up * 3.0f,
                        selectedTile.transform.rotation,
                        null
                        );
        PlayerManager.GetInstance().AddPlayer(selectedTile.CurrentItem.GetComponent<Item>());
        HidePlaceHolders();
    }

    public void StoreButtonPressed()
    {
        HidePlaceHolders();
        ShowItemOnTile(listOfTowerPlaceHolder[selectedTowerIndex], selectedTile);
        TileSelection(selectedTile);
    }

    public void SetTowerSelectionIndex(int index)
    {
        selectedTowerIndex = index;
    }

    public void PanelSelection(int index)
    {
        _menuInteractionRef.PanelToggle(index);
    }
}
