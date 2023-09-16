using UnityEngine;
using UnityEngine.UI;
using SimpleCraft.Core;

namespace SimpleCraft.UI{
	public class InventoryUI : MonoBehaviour {

		[SerializeField]
        private  GameObject _invScrollView;

		[SerializeField]
        private  GameObject _secondInvScrollView;

		[SerializeField]
        private  Button _inventoryButton;

		[SerializeField]
        private  Button _secondInvButton;

		[SerializeField]
        private  InputField _inputAmount;

		[SerializeField]
        private  Text _descriptionText;

		[SerializeField]
        private  Text _itemNameText;

		[SerializeField]
        private  Text _inventoryWeight;

        [SerializeField]
        private float _buttonHeight = 90;

		[SerializeField]
        private Image _Imagee;

		[SerializeField]
        private Image _Icon;



        Player _player;
		Button[] _but;
		Button[] _secondBut;

		void Start(){
			_player = this.GetComponent<Player> ();
			_invScrollView.SetActive (false);
			_secondInvScrollView.SetActive (false);
		}
        /// <param name="inventory"></param>
		public void Draw(Inventory inventory){
			if (inventory.InvType == Inventory.Type.PlayerInventory) {
				DrawInventoryItem (_invScrollView, _inventoryButton, inventory, ref _but);
				_inventoryWeight.text = "Weight: "+inventory.Weight + "/" + inventory.MaxWeight;
			}else
				DrawInventoryItem (_secondInvScrollView,_secondInvButton,inventory, ref _secondBut);
		}


        /// <param name="item"></param>
		public void SelectItem(Item item,Inventory.Type type){
			_Imagee.sprite = item.Image;
			_itemNameText.text = item.ItemName;
			_descriptionText.text = item.Description;

			_descriptionText.text += "\n\n" + "Weigth: " + item.Weight;

            float price = item.Price;

            if (_player.Trading && type != Inventory.Type.Store)
                if(price > 1)
                    price = price * _player.TradeAdjustment;

            _descriptionText.text += "   Price: " + price;
		}

        /// <returns></returns>
		public float GetAmount(){
			float amount = 1;

			if(_inputAmount.text != "")
				amount = float.Parse(_inputAmount.text);

			return amount;
		}

        /// <param name="inventoryScrollView"></param>
        /// <param name="inventoryButton"></param>
        /// <param name="inventory"></param>
        /// <param name="but"></param>
		void DrawInventoryItem (GameObject inventoryScrollView,
			Button inventoryButton,
			Inventory inventory,
			ref Button[] but){

			RectTransform Content;
			inventoryScrollView.SetActive(true);
			Inventory.Type buttonType = inventory.InvType;
			Content = inventoryScrollView.GetComponent<ScrollRect> ().content; 

			inventoryButton.gameObject.SetActive(true);

			Cursor.visible = true;
			Time.timeScale = 0;
			DestroyButtons (but);
			but = new Button[inventory.ItemCount()];
			int i = 0;
			foreach (Item item in inventory.ItemKeys()) {
				but[i] = Instantiate (inventoryButton) as Button;

				but[i].image.rectTransform.sizeDelta = new Vector2 (465, _buttonHeight);
				but[i].transform.GetChild(0).GetComponent<Image>().sprite=item.Image;

				//but[i].GetChild(1).GetComponent<Image>().sprite=item.Image;
				/*Image imageIcon = but[i].GetComponentInChildren<Image>();
				imageIcon.sprite=item.Image;*/
				//but[i].GetComponentInChildren<Image>().sprite=item.Image;
				/*Text number = but[i].GetComponentInChildren<Text>(transform.GetChild(1));*/
				//number.text = "" + inventory.Items(item) + "";
                //position the button bellow the previous
                but[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (-(i + 1) * _buttonHeight));
                but[i].transform.localScale = new Vector3 (1,1,1);

				but[i].GetComponentInChildren <Text> ().text = item.ItemName;
				

				if (inventory.Items(item) > 1){
					//but[i].GetComponentInChildren <Text> ().text += "  X " + inventory.Items(item) + "";
					but[i].transform.GetChild(3).GetComponent<Text>().text += "" + inventory.Items(item) + "";
					but[i].transform.GetChild(2).gameObject.SetActive(true);

					/*number.text += "" + inventory.Items(item) + "";*/
					}

				but[i].transform.SetParent (inventoryButton.transform.parent, false);
				but[i].enabled = true;

				ItemButton itemButton = but [i].gameObject.GetComponent<ItemButton> ();

				itemButton.Player = this.gameObject.GetComponent<Player>();
				itemButton.Item = item;
				itemButton.InventoryType = buttonType;

				i++;
			}

            Content.GetComponent<RectTransform>().sizeDelta = new 
                Vector2(0, (inventory.ItemCount() + 1) * _buttonHeight);

            inventoryButton.gameObject.SetActive(false);
		}

		public void Toogle(){
			_invScrollView.SetActive (!_invScrollView.activeSelf);
			_secondInvScrollView.SetActive (!_invScrollView.activeSelf);
            Cursor.visible = _invScrollView.activeSelf;
            if (_invScrollView.activeSelf){
                this.DrawInventoryItem(_invScrollView, _inventoryButton, _player.Inventory, ref _but);
                _inventoryWeight.text = "Weight: " + _player.Inventory.Weight + "/" + _player.Inventory.MaxWeight;
            }
            else
                Time.timeScale = 1.0f;
        }

        public bool IsActive(){
            return _invScrollView.activeSelf;
        }

        void DestroyButtons(Button[] but){
			if (but != null)
				for (int i = 0; i < but.Length; i++)
					Destroy (but [i].gameObject);
		}
	}
}