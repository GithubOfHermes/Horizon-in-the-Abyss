using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 상품 슬롯 한 개를 제어하는 헬퍼 클래스
[System.Serializable]
public class ShopProductSlot
{
    public GameObject gameObject;  // Shop_Product_N (Empty Object)
    public Image      iconImage;   // Shop_Product_N_Image
    public Text       nameText;    // Shop_Product_N_Text
    public Button     buyButton;   // Shop_Product_N_Button
    public Text       priceText;   // Shop_Product_N_Button_Text

	[Header("할인 UI (선택적)")]
    public Image saleImage;         // Shop_Product_N_SaleImage
    public Text  saleText;          // Shop_Product_N_SaleText

    [HideInInspector] public GadgetItem gadget;
    [HideInInspector] public int        price;

	// 슬롯에 새 아이템·가격 세팅
	public void Setup(GadgetItem g, int p)
	{
		gadget = g;
		price = p;
		iconImage.sprite = g.icon;
		nameText.text = g.itemName;
		priceText.text = p.ToString();
		gameObject.SetActive(true);
		
		if (saleImage != null) saleImage.gameObject.SetActive(false);
        if (saleText  != null) saleText.text = string.Empty;
    }
}
