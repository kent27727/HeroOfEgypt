using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCharacter : MonoBehaviour
{
    [SerializeField] GameObject[] players;
    void Start()
    {
        ChangePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangePlayer()
    {
        Character character = GameDataManager.GetSelectedCharacter();
        if (character.image != null)
        {
            int selectedPlayer = GameDataManager.GetSelectedCharacterIndex();
            players[selectedPlayer].SetActive(true);
            for (int i = 0; i < players.Length; i++)
            {
                if (i != selectedPlayer)
                {
                    players[i].SetActive(false);
                }
            }
        }
    }
}
