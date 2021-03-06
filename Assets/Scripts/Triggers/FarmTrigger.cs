﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FarmTrigger : MonoBehaviour {
    [SerializeField]
    private GameObject inventory, quizBox;
    [SerializeField]
    private Tilemap object1, foreground;

    private ActionButton actionButton;
    private GameObject player;
    
    private bool playerWithinZone;

    private FarmTile[] farmTiles;

	// Use this for initialization
	void Start () {
        actionButton = GameObject.FindGameObjectWithTag("ActionButton").GetComponent<ActionButton>();
        player = GameObject.FindGameObjectWithTag("Player");

        initializeFarmTiles();
    }

    private void initializeFarmTiles() {
        farmTiles = new FarmTile[5];

        int xCoordinate = 14;

        for (int i=0; i < farmTiles.Length; i++) {
            FarmTile newTile = new FarmTile(new Vector3Int(xCoordinate, -8, 0));
            farmTiles[i] = newTile;

            xCoordinate++;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (playerWithinZone && actionButton.GetClicked()) {
            /* HARVEST CROPS */
            if (!inventory.GetComponent<Inventory>().IsFull()) {
                for (int i = 0; i < farmTiles.Length; i++) {
                    if (farmTiles[i].IsCropFullyGrown()) {
                        Food harvest = farmTiles[i].HarvestCrop();
                        AddHarvestToInventory(harvest);

                        return;
                    }
                }
            }

            /* PLANT CROPS */
            for (int i=0; i < farmTiles.Length; i++) {
                if (!farmTiles[i].HasCrop()) {
                    // Plant crop if player has seeds
                    Food food = GetSeedsFromInventory();

                    if (food != null) {
                        PlantSeeds(farmTiles[i], food, object1);

                        return;
                    } else {
                        Debug.Log("No seeds in inventory.");
                    }
                }
            }
        }
    }

    private Food GetSeedsFromInventory() {
        foreach (GameObject slot in inventory.GetComponent<Inventory>().GetInventorySlots()) {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();

            if (inventorySlot.HasItem()) {
                Food food = inventorySlot.GetItem();

                if (food.itemType == ItemType.Seeds) {
                    inventorySlot.ClearSlot();

                    return food;
                }
            }
        }

        return null;
    }

    private void AddHarvestToInventory(Food harvest) {     
        foreach (GameObject slot in inventory.GetComponent<Inventory>().GetInventorySlots()) {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();

            if (!inventorySlot.HasItem()) {
                inventorySlot.AddItem(harvest);

                return;
            }
        }
    }

    private void PlantSeeds(FarmTile farmTile, Food food, Tilemap tilemap) {
        Tile tile = Resources.Load<Tile>("Foods/Plants/" + food.growthStageOneTile);

        farmTile.PlantCrop(food, tile, object1);
    }

    public bool HasCropsToGrow() {
        foreach (FarmTile farmTile in farmTiles) {
            if (farmTile.HasCrop() && !farmTile.IsCropFullyGrown()) {
                return true;
            }
        }

        return false;
    }

    public void GrowCrops() {
        StartCoroutine(Grow());
    }

    public IEnumerator Grow() {
        ScreenFader screenFader = GameObject.FindGameObjectWithTag("Fader").GetComponent<ScreenFader>();

        // Call FadeToBlack method and don't move on to the next line of code until method is finished
        yield return StartCoroutine(screenFader.FadeToBlack());

        for (int i = 0; i < farmTiles.Length; i++) {
            if (farmTiles[i].HasCrop()) {
                farmTiles[i].GrowCrop();
            }
        }

        // Call FadeToClear method and don't move on to the next line of code until method is finished
        yield return StartCoroutine(screenFader.FadeToClear());
    }

    public void ShowQuiz() {
        if (HasCropsToGrow()) {
            quizBox.GetComponent<UIWindow>().OpenWindow();
            quizBox.GetComponent<QuizBox>().UpdateQuizBox(GetFarmTileCategories());
        } else {
            StartCoroutine(Grow());
        }
    }

    private string[] GetFarmTileCategories() {
        List<string> categories = new List<string>();

        foreach (FarmTile farmTile in farmTiles) {
            if (farmTile.HasCrop()) {
                foreach (string category in farmTile.GetCropCategories()) {
                    if (!categories.Contains(category)) {
                        categories.Add(category);
                    }
                }
            }
        }

        return categories.ToArray();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        playerWithinZone = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        playerWithinZone = false;
    }

    public bool IsPlayerWithinZone() {
        return playerWithinZone;
    }
}
