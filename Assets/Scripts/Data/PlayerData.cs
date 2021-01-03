using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[System.Serializable]
public class PlayerData : ScriptableObject, ICloneable {

    public string Name;
    public Sprite Image;
    public int MaxHealth;
    public int Health;
    public int Shield;
    public bool Dodge;
    public Card[] Cards;

    public List<Card> Inventory;

    public void ResetShield() {
        Shield = 0;
    }

    public void ResetDodge() {
        Dodge = false;
    }

    public bool ContainsCard(Card card) {
        for(int i = 0; i < Cards.Length; i++) {
            if (Cards[i] == null) continue;
            if (Cards[i].Equals(card)) return true;
        }
        return false;
    }

    public void AddCard(Card card, int pos) {
        Cards[pos] = card;
    }

    public void RemoveCard(Card card) {
        for (int i = 0; i < Cards.Length; i++) {
            if (Cards[i] == null) continue;
            if (Cards[i].Equals(card)) {
                Cards[i] = null;
            }
        }
    }

    public void SwitchCards(Card card1, Card card2) {
        if (card1 == null) return;
        if (card2 == null) return;
        int index1 = 0;
        int index2 = 0;
        for(int i = 0; i < Cards.Length; i++) {
            if (Cards[i] == null) continue;
            if (Cards[i].Equals(card1)) index1 = i;
            if (Cards[i].Equals(card2)) index2 = i;
        }

        Card aux = Cards[index1];
        Cards[index1] = Cards[index2];
        Cards[index2] = aux;
    }

    public object Clone() {
        return this.MemberwiseClone();
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Debug/Player", false, 1022)]
    static void CreatePlayerAsset() {
        const string Folder = "Assets/Database";
        const string AssetPath = Folder + "/Player.asset";
        if (File.Exists(AssetPath))
            return;

        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        var playerAsset = CreateInstance<PlayerData>();
        AssetDatabase.CreateAsset(playerAsset, AssetPath);
    }

#endif
}