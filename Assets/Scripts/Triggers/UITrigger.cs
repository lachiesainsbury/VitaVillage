﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrigger : MonoBehaviour {

    [SerializeField]
    private GameObject UIElement, NPC, Shopkeeper;

    private ActionButton actionButton;

    private bool playerWithinZone;

	// Use this for initialization
	void Start () {
        actionButton = GameObject.FindGameObjectWithTag("ActionButton").GetComponent<ActionButton>();
	}
	
	// Update is called once per frame
	void Update () {
        if (playerWithinZone && actionButton.GetClicked()) {
            UIElement.GetComponent<UIWindow>().OpenWindow();

            if (NPC != null) {
                UIElement.GetComponent<DialogueBox>().UpdateDialogueBox(NPC.GetComponent<NPC>());
            }

            if (Shopkeeper != null) {
                UIElement.GetComponent<DialogueBox>().UpdateDialogueBoxShopkeeper(Shopkeeper.GetComponent<Shopkeeper>());
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision) {
        playerWithinZone = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        playerWithinZone = false;
    }

    public bool IsPlayerWithinZone() {
        return this.playerWithinZone;
    }
}
