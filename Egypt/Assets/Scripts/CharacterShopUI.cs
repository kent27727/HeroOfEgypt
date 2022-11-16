using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class CharacterShopUI : MonoBehaviour
{
    [Header("Layout Settings")]
    [SerializeField] float itemSpacing = .5f;
    float itemHeight;

    [Header("UI Events")]
    [SerializeField] Image selectedCharacterIcon;
    [SerializeField] Transform shopMenu;
    [SerializeField] Transform shopItemsContainer;
    [SerializeField] GameObject itemPrefab;
    [Space(20f)]
    [SerializeField] CharacterShopDataBase characterDB;
    [Space(20f)]

    [Header("Shop Events")]
    [SerializeField] GameObject shopUI;
    [SerializeField] Button openShopButton;
    [SerializeField] Button closeShopButton;
    [SerializeField] Button scrollUpButton;

    [Space(20f)]
    [Header("Main Menu")]
    [SerializeField] Image mainMenuCharacterImage;
    [SerializeField] TMP_Text mainMenuCharacterName;

    [Space(20f)]
    [Header("Scroll View")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GameObject topFade;
    [SerializeField] GameObject bottomFade;





    [Space(20f)]
    [Header("Purchase Fx & Error Message")]
    [SerializeField] ParticleSystem purchaseFx;
    [SerializeField] Transform purchaseFxPos;
    [SerializeField] TMP_Text noEnoughCoinText;



    int newSelectedItemIndex = 0;
    int previouselectedItemIndex = 0;


    void Start()
    {
        purchaseFx.transform.position = purchaseFxPos.position;
        AddShopEvents();
        GenerateShopItemsUI();
        SetSelectedCharacter();
        SelectItemUI(GameDataManager.GetSelectedCharacterIndex());
        ChangePlayerSkin();
        AutoScrollShopList(GameDataManager.GetSelectedCharacterIndex());
    }

    void AutoScrollShopList(int itemIndex)
	{
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(1f - (itemIndex / (float)(characterDB.CharactersCount - 1)));
	}
    void SetSelectedCharacter()
	{
        int index = GameDataManager.GetSelectedCharacterIndex();

        GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);
	}

    void GenerateShopItemsUI()
	{
		for (int i = 0; i < GameDataManager.GetAllPurchasedCharacter().Count; i++)
		{
            int purchasedCharacterIndex = GameDataManager.GetPurchasedCharacter(i);
            characterDB.PurchaseCharacter(purchasedCharacterIndex);
		}
        itemHeight = shopItemsContainer.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
        Destroy(shopItemsContainer.GetChild(0).gameObject);
        shopItemsContainer.DetachChildren();

		for (int i = 0; i < characterDB.CharactersCount; i++)
		{
            Character character = characterDB.GetCharacter(i);
            CharacterItemUI uiItem = Instantiate(itemPrefab, shopItemsContainer).GetComponent<CharacterItemUI>();

			uiItem.SetItemPosition(Vector2.down * i * (itemHeight + itemSpacing));

			uiItem.gameObject.name = "Item+" + i + "-" + character.name;

			uiItem.SetCharacterName(character.name);
            uiItem.SetCharacterImage(character.image);
            uiItem.SetCharacterPrice(character.price);

			if (character.isPurchased)
			{
                uiItem.SetCharacterAsPurchased();
                uiItem.OnItemSelect(i,OnItemSelected);
			}
			else
			{
                uiItem.SetCharacterPrice(character.price);
                uiItem.OnItemPurchase(i ,OnItemPurchased );
			}

            shopItemsContainer.GetComponent<RectTransform>().sizeDelta =
                Vector2.up * (itemHeight + itemSpacing) * characterDB.CharactersCount;

        }
    }

    void ChangePlayerSkin()
	{
        Character character = GameDataManager.GetSelectedCharacter();
		if (character.image != null)
		{
            mainMenuCharacterImage.sprite = character.image;
            mainMenuCharacterName.text = character.name;

            selectedCharacterIcon.sprite = GameDataManager.GetSelectedCharacter().image;
        }
	}


    void OnItemSelected(int index)
	{
        SelectItemUI(index);
        
        GameDataManager.SetSelectedCharacter(characterDB.GetCharacter(index), index);

        ChangePlayerSkin();
	}

    void SelectItemUI(int itemIndex)
	{
        previouselectedItemIndex = newSelectedItemIndex;
        newSelectedItemIndex = itemIndex;

        CharacterItemUI prevUiItem = GetItemUI(previouselectedItemIndex);
        CharacterItemUI newUiItem = GetItemUI(newSelectedItemIndex);

        prevUiItem.DeselectItem();
        newUiItem.SelectItem();
	}

    CharacterItemUI GetItemUI(int index)
	{
        return shopItemsContainer.GetChild(index).GetComponent<CharacterItemUI>();
	}

    void OnItemPurchased(int index)
    {
        Character character = characterDB.GetCharacter(index);
        CharacterItemUI uiItem = GetItemUI(index);

		if (GameDataManager.CanSpendCoins(character.price))
		{
            GameDataManager.SpendCoins(character.price);
            purchaseFx.Play();

            GameSharedUI.Instance.UpdateCoinsUIText();
            characterDB.PurchaseCharacter(index);
            uiItem.SetCharacterAsPurchased();
            uiItem.OnItemSelect(index, OnItemSelected);
            GameDataManager.AddPurchasedCharacter(index);
		}
		else
		{
            AnimateNoMoreCoinsText();
            uiItem.AnimateShakeItem();

        }

    }

    void AnimateNoMoreCoinsText()
	{
        noEnoughCoinText.transform.DOComplete();
        noEnoughCoinText.DOComplete();

        noEnoughCoinText.transform.DOShakePosition(3f, new Vector3(5f, 0f, 0f), 10, 0);
        noEnoughCoinText.DOFade(1f, 3f).From(0f).OnComplete(() => { noEnoughCoinText.DOFade(0f, 1f); });
	}
    void AddShopEvents()
	{
        openShopButton.onClick.RemoveAllListeners();
        openShopButton.onClick.AddListener(OpenShop);
        
        closeShopButton.onClick.RemoveAllListeners();
        closeShopButton.onClick.AddListener(CloseShop);

        scrollRect.onValueChanged.RemoveAllListeners();
        scrollRect.onValueChanged.AddListener(OnShopListScroll);

        scrollUpButton.onClick.RemoveAllListeners();
        scrollUpButton.onClick.AddListener(OnScrollUpClicked);

    }

    void OnScrollUpClicked()
	{
        scrollRect.DOVerticalNormalizedPos(1f, .5f).SetEase(Ease.OutBack);
	}

    void OnShopListScroll(Vector2 value)
	{
        float scrollY = value.y;
		if (scrollY < 1f)
		{
            topFade.SetActive(true);
		}
		else
		{
            topFade.SetActive(false);
        }

        if (scrollY > 0f)
        {
            bottomFade.SetActive(true);
        }
        else
        {
            bottomFade.SetActive(false);
        }

		if (scrollY < .7f)
		{
            scrollUpButton.gameObject.SetActive(true);
		}
		else
		{
            scrollUpButton.gameObject.SetActive(false);
		}
    }

    void OpenShop()
	{
        shopUI.SetActive(true);
	}

    void CloseShop()
	{
        shopUI.SetActive(false);
	}


}
