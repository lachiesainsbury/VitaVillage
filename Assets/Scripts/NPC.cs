﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour {

    [SerializeField]
    private GameObject gameController, inventory, questNPCBox, questNameBox;

    private Quest quest;

    private Dialogue dialogue;

    private void Start() {
        quest = gameController.GetComponent<GameController>().FindQuestByNPC(this.gameObject.name);
        quest.questStatus = QuestStatus.NotStarted;

        dialogue = gameController.GetComponent<GameController>().FindDialogueByNPC(this.gameObject.name);
    }

    public void StartQuest(GameObject player) {
        if (quest != null) {
            if (!player.GetComponent<Player>().HasQuest() && quest.questStatus == QuestStatus.NotStarted) {
                quest.questStatus = QuestStatus.InProgress;

                player.GetComponent<Player>().AddNewQuest(quest);
                questNPCBox.GetComponent<Text>().text = quest.questNPC + ":";
                questNameBox.GetComponent<Text>().text = quest.name;
            } else {
                Debug.Log("Player has a quest already or has completed this quest.");
            }
        } else {
            Debug.Log("NPC does not have a quest assigned.");
        }
    }

    private void FinishQuest() {
        quest.questStatus = QuestStatus.Completed;

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.FinishQuest();

        questNPCBox.GetComponent<Text>().text = "-";
        questNameBox.GetComponent<Text>().text = "-";

        GameObject.FindGameObjectWithTag("TownHealthBar").GetComponent<TownHealthBar>().QuestCompleted();
    }

    public string GetNPCName() {
        return dialogue.NPC;
    }

    public string GetDialogueLine() {
        switch (quest.questStatus) {
            case QuestStatus.NotStarted:
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().HasQuest()) {
                    return dialogue.questConflict;
                } else {
                    return dialogue.questOffer;
                }

            case QuestStatus.InProgress:
                if (HasPlayerFinishedQuest()) {
                    return dialogue.questHandIn;
                } else {
                    return dialogue.questInProgress;
                }

            case QuestStatus.Completed:
                return dialogue.questCompleted;

            default:
                return dialogue.questConflict;
        }
    }

    private bool HasPlayerFinishedQuest() {
        if (quest != null) {
            GameObject[] inventorySlots = inventory.GetComponent<Inventory>().GetInventorySlots();
            InventorySlot[] validInventorySlots = new InventorySlot[6];
            int validItemCount = 0;

            foreach (GameObject slot in inventorySlots) {
                InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();

                foreach (string questItem in quest.questItems) {
                    if (inventorySlot.HasItem()) {
                        if ((inventorySlot.GetItem().name).Equals(questItem) && (inventorySlot.GetItem().itemType == ItemType.Food)) {
                            for (int i = 0; i < validInventorySlots.Length; i++) {
                                if (validInventorySlots[i] == null) {
                                    validInventorySlots[i] = inventorySlot;
                                    validItemCount++;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (validItemCount >= quest.itemAmount) {
                for (int i = 0; i < quest.itemAmount; i++) {
                    validInventorySlots[i].ClearSlot();
                }

                FinishQuest();
                return true;
            }
        }

        return false;
    }

    public Quest GetQuest() {
        return this.quest;
    }
}
