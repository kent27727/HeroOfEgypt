using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CharacterShopData
{
	public List<int> purchasedCharacterIndexes = new List<int>();

}

[System.Serializable]
public class PlayerData
{
	public int coins = 0;
	public int selectedCharacterIndex = 0;

}

public static class GameDataManager 
{
	static PlayerData playerData = new PlayerData();
	static CharacterShopData characterShopData = new CharacterShopData();

	static Character selectedCharacter;


	static GameDataManager()
	{
		LoadPlayerData();
		LoadCharactersShopData();
	}
	public static Character GetSelectedCharacter()
	{
		return selectedCharacter;
	}

	public static void SetSelectedCharacter(Character character , int index)
	{
		selectedCharacter = character;
		playerData.selectedCharacterIndex = index;
		SavePlayerData();
	}
	public static int GetSelectedCharacterIndex()
	{
		return playerData.selectedCharacterIndex;
	}

	

	public static int GetCoins()
	{
		return playerData.coins;

	}

	public static void AddCoins(int amount)
	{
		playerData.coins += amount;
		SavePlayerData();
	}

	public static bool CanSpendCoins(int amount)
	{
		return (playerData.coins >= amount);
	}

	public static void SpendCoins(int amount)
	{
		playerData.coins -= amount;
		SavePlayerData();
	}

	static void LoadPlayerData()
	{
		playerData = BinarySerializer.Load<PlayerData>("player-data.txt");
		UnityEngine.Debug.Log("<color=green>[PlayerData] Saved.</color>");
	}
	static void SavePlayerData()
	{
		BinarySerializer.Save(playerData, "player-data.txt");
		UnityEngine.Debug.Log("<color=magenta>[PlayerData] Saved.</color>");

	}

	public static void AddPurchasedCharacter(int characterIndex)
	{
		characterShopData.purchasedCharacterIndexes.Add(characterIndex);
		SaveCharactersShopData();
	}

	public static List<int> GetAllPurchasedCharacter()
	{
		return characterShopData.purchasedCharacterIndexes;
	}

	public static int GetPurchasedCharacter(int index)
	{
		return characterShopData.purchasedCharacterIndexes[index];
	}
	

	static void LoadCharactersShopData()
	{
		characterShopData = BinarySerializer.Load<CharacterShopData>("characters-shop-data.txt");
		UnityEngine.Debug.Log("<color=green>[CharactersShopData] Saved.</color>");
	}
	static void SaveCharactersShopData()
	{
		BinarySerializer.Save(characterShopData, "characters-shop-data.txt");
		UnityEngine.Debug.Log("<color=magenta>[CharactersShopData] Saved.</color>");

	}
}
