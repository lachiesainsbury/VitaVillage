﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

    [SerializeField]
    private Image icon;

    private Food food;

    public void AddFood(Food newFood) {
        food = newFood;
        UpdateIcon();
    }

    public void ClearSlot() {
        // Remove the current item and icon, and reset the alpha to 0
        food = null;
        icon.sprite = null;
        icon.color += new Color(0, 0, 0, -255);
    }

    private void UpdateIcon() {
        // If item has a sprite, set that sprite to the icon and maximise the alpha.
        icon.sprite = Resources.Load<Sprite>("Foods/" + food.seedIcon);
        icon.color += new Color(0, 0, 0, 255);
    }

    public Food GetFood() {
        return food;
    }
}
