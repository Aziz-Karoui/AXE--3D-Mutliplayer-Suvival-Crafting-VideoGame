using SimpleCraft.UI;
using UnityEngine;

namespace SimpleCraft.Core {
    
	[RequireComponent(typeof(Inventory))]
	public class Player : MonoBehaviour {
        
        [SerializeField]
        private ActionText _actionText;

        [SerializeField]
        private CraftUI _craftCostUI;

        [SerializeField]
        private GameObject _pauseMenu;

        [SerializeField]
        public GameObject inventBg;

        [SerializeField]
        private Material craftingMaterial;

        [SerializeField]
        private Material craftingMaterialsec;

        private MeshRenderer[] renderers_init;

        ////

        [SerializeField]
        private GameObject minimap;
        [SerializeField]
        private GameObject health;

        

        [Tooltip("The Transform used to position the items to be crafted.")]
        [SerializeField]
        private Transform _raycaster;

		[SerializeField]
        private ToolHandler _toolHandler;

        [Tooltip("How much an item devalues for the player")]
        [SerializeField]
        private float _tradeAdjustment = 0.5f;
        public float TradeAdjustment{
            get { return _tradeAdjustment; }
        }

        private LayerMask _focusLayers;
		private LayerMask _craftLayers;
        private GameObject _itemObj;
        private CraftableItem _currCraftItem;
        private Item _currItem = null;
        private Inventory.Type _invType;
        private int _craftTypeIdx = 0;
        private int _itemIdx = 0;
        private bool _craftingMode = false;
        public Transform _cam;
        private GameObject _interactionObj;
        private InventoryUI _inventoryUI;

        private bool _trading;
        public bool Trading{
            get { return _trading; }
        }

        private Inventory _inventory;
        public Inventory Inventory{
            get { return _inventory; }
            set { _inventory = value; }
        }

        private bool _showingMenu = false;

        [SerializeField]
        private QuickMessage _quickMessage;
        public QuickMessage QuickMessage{
            get { return _quickMessage; }
            set { _quickMessage = value; }
        }

        
        private enum Interaction{
			GrabTool, GrabItem, OpenContainer, None
		}
		private Interaction _interaction;
		
		
		UnityEngine.AI.NavMeshHit _hitTerrain;
		RaycastHit _hit;

		void Start () {
			_inventory = this.GetComponent<Inventory> ();

			_inventoryUI = this.GetComponent<InventoryUI> ();

			_cam =  Camera.main.transform;

			if (Manager.GetCraftableitemsLength(0) >= 1) {
				_itemIdx = 0;
				_itemObj = null;
			}

			_focusLayers = LayerMask.GetMask ("Default","CraftableItem");
			_craftLayers = LayerMask.GetMask ("Default");

			Time.timeScale = 1.0f;

			Cursor.visible = false;

			_pauseMenu.SetActive (_showingMenu);
		}
			
		void Update () {

            if (Input.GetKeyDown(KeyCode.I))
            {
                if(inventBg.activeSelf==true)
                {
                    inventBg.SetActive(false);
                }else{
                    inventBg.SetActive(true);
                }
            }

			if (Input.GetKeyDown (KeyCode.Escape) && !_inventoryUI.IsActive()) 
				ShowPauseMenu ();
                //minimap.SetActive(false);
                //health.SetActive(false);


            if ((Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab)) && !_showingMenu ){
                _inventoryUI.Toogle();
                _trading = false;
                //_inventoryUI.invbg.SetActive(false);
                minimap.SetActive(!_showingMenu);
                health.SetActive(!_showingMenu);
                
            }
                
            if (_showingMenu || _inventoryUI.IsActive())
				return;

           
			if (Input.GetKeyDown (KeyCode.B) || Input.GetKeyDown(KeyCode.C)) {
				_craftingMode = !_craftingMode;
				if (_itemObj == null)
					_itemObj = Manager.GetCraftableItemObj (_craftTypeIdx,_itemIdx);
				if (_itemObj != null) {
					_itemObj.SetActive (_craftingMode);
					_actionText.Text = "";
					_craftCostUI.gameObject.SetActive (_craftingMode);
					_currCraftItem = Manager.GetCraftableItem(_craftTypeIdx, _itemIdx);
                    _craftCostUI.DrawCostView (_currCraftItem,_inventory);
                    _craftCostUI.setTypeText(_craftTypeIdx);
				}
			}

            if (Input.GetKeyDown(KeyCode.G)){
                if (_toolHandler.CurrentTool != null){
                    _currItem = _toolHandler.CurrentTool;
                    Drop();
                    _currItem = null;
                }
            }

            if (_craftingMode)
				OnCraftingMode ();
			else{
				if (Input.GetButtonDown("Fire1") && _toolHandler.CurrentTool != null)
					_toolHandler.Attack ();

                
				if ((Input.GetKeyDown (KeyCode.E) || Input.GetButton("Fire2")) && _interactionObj != null) {
					if (_interaction == Interaction.GrabTool) {
						ItemReference tool = _interactionObj.GetComponent<ItemReference> () as ItemReference;
                        Tool item = _interactionObj.GetComponent<ItemReference>().Data as Tool;
                        _inventory.Add (item, tool.Amount,this);

                            


                        if (_toolHandler.CurrentTool == null){
                            _toolHandler.ChangeTool(item);
                            Destroy(_interactionObj);
                        }
                        else
                            Destroy(_interactionObj);
						_interactionObj = null;
					} else if(_interaction == Interaction.GrabItem){
						ItemReference item = _interactionObj.GetComponent<ItemReference> ();
                        float amountTaken = _inventory.Add(item.Data, item.Amount, this);

                        if (amountTaken == item.Amount)
							Destroy (_interactionObj);
                        else
                            item.Amount -= amountTaken;
					} else if(_interaction == Interaction.OpenContainer){
                        
						Inventory inventory = _interactionObj.GetComponent<Inventory> ();
                       
						_inventoryUI.Draw ( _inventory);

                        _trading = (inventory.InvType == Inventory.Type.Store);

                        
						_inventoryUI.Draw ( inventory);
					}
				}
				CheckPlayerFocus ();
			}
		}

		public void UseItem(){
           
            if (_currItem != null && _interactionObj != null){
                Interactable interactable = _interactionObj.GetComponent<Interactable>();

                if (interactable)
                    if (_interactionObj.GetComponent<Interactable>().UseItem(_currItem,_inventory)){
                        if (_inventoryUI.IsActive())
                            _inventoryUI.Toogle();
                        _quickMessage.ShowMessage(_interactionObj.GetComponent<Interactable>().SuccessMessage);
                        return;
                    }
            }
            _quickMessage.ShowMessage("Can't use that here!");
        }

		public void Equip(){
			if (_currItem == null)
				return;

            if (_invType != Inventory.Type.PlayerInventory)
                return;

			if (_currItem.GetType () == typeof(Tool))
                _toolHandler.ChangeTool(_currItem as Tool);
            else
                _quickMessage.ShowMessage("Can't equip this item!");	
		}

       
        void CheckPlayerFocus() {
            if (UnityEngine.Physics.Raycast(_cam.position, _cam.forward, out _hit, 5, _focusLayers.value)) {
                if (_hit.transform.gameObject.tag == "Resource") {
                    Resource resource = _hit.transform.gameObject.GetComponent<Resource>();
                    _actionText.Text = "Resource: " + resource.Item.name;
                } else if (_hit.transform.gameObject.tag == "Container" || _hit.transform.gameObject.tag == "Trader") {
                    if (_hit.transform.gameObject.tag == "Container")
                        _actionText.Text = "Press (E) to open Container";
                    else{
                        _actionText.Text = "Press (E) to trade";}

                    _interaction = Interaction.OpenContainer;
                    _interactionObj = _hit.transform.gameObject;
                } else if (_hit.transform.gameObject.tag == "Interactable") {
                    _actionText.Text = _hit.transform.gameObject.name;
                    _interaction = Interaction.None;
                    _interactionObj = _hit.transform.gameObject;
                } else if (_hit.transform.gameObject.GetComponent<ItemReference>() != null) {
                    ItemReference itemRef = _hit.transform.gameObject.GetComponent<ItemReference>();
                    if (itemRef != null) {
                        if (itemRef.Data.CanBePicked) {
                            _actionText.Text = "Press (E) to grab " + itemRef.Data.ItemName + " x " + itemRef.Amount;

                            if (itemRef.Data.GetType() == typeof(Tool))
                                _interaction = Interaction.GrabTool;
                            else
                                _interaction = Interaction.GrabItem;

                            _interactionObj = _hit.transform.gameObject;
                        }
                    }
                } else {
                    _interactionObj = null;
                    _actionText.Text = "";
                }
            } else {
                _interactionObj = null;
                _actionText.Text = "";
            }
        }

        void OnCraftingMode(){
			if(_currCraftItem == null)
				return;

            
			_raycaster.position =  transform.position + transform.forward * _currCraftItem.Offset;
			_raycaster.position = new Vector3(_raycaster.position.x, _raycaster.position.y+5, _raycaster.position.z);
            /**********************************************************************************************************************************/



            
            if (UnityEngine.Physics.Raycast(_raycaster.position, _raycaster.forward, out _hit, 20, _craftLayers.value)) {
                
                if (Vector3.Distance(_hit.point, this.gameObject.transform.position) >= _currCraftItem.Offset) {
                    if (_currCraftItem.OnlyOnGround) {
                        if (UnityEngine.AI.NavMesh.SamplePosition(_hit.point, out _hitTerrain, 100.0f, UnityEngine.AI.NavMesh.AllAreas)) {
                            if (Mathf.Abs(_hit.point.y - _hitTerrain.position.y) < 1)
                                _itemObj.transform.position = new Vector3(_hit.point.x, _hit.point.y +
                                    _currCraftItem.YCraftCorrection, _hit.point.z);
                                //_itemObj.transform.rotArrayY = 
                            else
                                _itemObj.transform.position = new Vector3(_hit.point.x, _hitTerrain.position.y +
                                    _currCraftItem.YCraftCorrection, _hit.point.z);
                        }
                    } else 
                        _itemObj.transform.position = new Vector3(_hit.point.x, _hit.point.y + _currCraftItem.YCraftCorrection, _hit.point.z);
                        
                        MeshRenderer[] renderers = _itemObj.GetComponentsInChildren<MeshRenderer>();
                        renderers_init = _itemObj.GetComponentsInChildren<MeshRenderer>();
                         /*foreach (MeshRenderer rendereri in renderers_init) {
                             Debug.Log(rendereri.material.name);
                        }*/
                        //Debug.Log(renderers_init.material.name);
                        foreach (MeshRenderer renderer in renderers) {
                            renderer.material = craftingMaterial;
                             Debug.Log(renderer.material.name);
                        }
                        //MeshRenderer[] renderers = _itemObj.GetComponentsInChildren<MeshRenderer>();
                        //renderers_init = _itemObj.GetComponentsInChildren<MeshRenderer>();
                        /*foreach (MeshRenderer renderer in renderers) {
                            renderer.material = craftingMaterial;
                        }*/

                        //make the item slightly transparent during crafting
                        /*Color itemColor = _itemObj.GetComponent<MeshRenderer>().material.color;*/
                        /*MeshRenderer[] renderers = _itemObj.GetComponentsInChildren<MeshRenderer>();
                        foreach (MeshRenderer renderer in renderers) {
                            Color itemColor = renderer.material.color;
                            itemColor.a = 0.5f;
                            renderer.material.color = itemColor;
                            _itemObj.GetComponent<MeshRenderer>().material.color = itemColor;*/

                        
                        
                }
            }

            if (_currCraftItem != null) {
				if (!_itemObj.GetComponent<ItemReference>().CanBuild()){

					_actionText.Text = "Can't build there!";
                    
                }

				else if (!HaveResources (_currCraftItem))
					_actionText.Text = "Lack of the required resources!";
				else
					_actionText.Text = "";
			}

            bool changeType = false;

            //Previous Category
            if (Input.GetKeyDown(KeyCode.Q)){
                if (_craftTypeIdx > 0)
                    _craftTypeIdx -= 1;
                else
                    _craftTypeIdx = Manager.GetCraftableTypeLength() - 1;
                _itemIdx = 0;
                changeType = true;
            }

            //Next Category
            if (Input.GetKeyDown(KeyCode.R)){
                if (_craftTypeIdx < Manager.GetCraftableTypeLength() - 1)
                    _craftTypeIdx += 1;
                else
                    _craftTypeIdx = 0;
                _itemIdx = 0;
                changeType = true;
            }

          
            if (changeType)
                _craftCostUI.setTypeText(_craftTypeIdx);

            
            if (Input.GetAxis ("Mouse ScrollWheel") != 0 || changeType) {
				if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
					if (_itemIdx <  Manager.GetCraftableitemsLength(_craftTypeIdx) - 1) 
						_itemIdx += 1;
					else
						_itemIdx = 0;
				} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
					if (_itemIdx > 0)
						_itemIdx -= 1;
					else
						_itemIdx = Manager.GetCraftableitemsLength(_craftTypeIdx) - 1;
				}

             
				_itemObj.SetActive (false);
				Destroy (_itemObj);
				_itemObj = null;
                _itemObj = Manager.GetCraftableItemObj(_craftTypeIdx, _itemIdx);
                _itemObj.SetActive (true);

				_currCraftItem = Manager.GetCraftableItem(_craftTypeIdx, _itemIdx);

                //update 
				_craftCostUI.DrawCostView (_currCraftItem,_inventory);

				if (_currCraftItem.HasRigidBody && _itemObj.GetComponent<Rigidbody> () != null)
					_itemObj.GetComponent<Rigidbody> ().detectCollisions = false;
			}

			
			if (Input.GetMouseButton (1)) {
				_itemObj.transform.Rotate (0, 30 * Time.deltaTime, 0);
			} else if (Input.GetMouseButton (0)) {
				_itemObj.transform.Rotate (0, -30 * Time.deltaTime, 0);
			}

			//try to place the item 
			if (Input.GetKeyDown (KeyCode.E)) {
				if (this.HaveResources (_currCraftItem) && _itemObj.GetComponent<ItemReference>().CanBuild()) {
                    Debug.Log("line 433");
                    /*foreach (MeshRenderer rendereri in renderers_init) {
                             Debug.Log("SAVEDDDDD"+rendereri.material.name);
                        }*/
                       // _itemObj.GetComponentsInChildren<MeshRenderer>().material=renderer_init.material;

                    MeshRenderer[] renderers_sec = _itemObj.GetComponentsInChildren<MeshRenderer>();
                     
                            //renderers_sec = renderers_init;
                            foreach (MeshRenderer renderer in renderers_sec) {
                            renderer.material = craftingMaterialsec;
                             Debug.Log(renderer.material.name);
                        }
                            
                          /* foreach (MeshRenderer renderer in renderers_sec) {
                            foreach (MeshRenderer renderer_init in renderers_init) {
                                Debug.Log("SAVED111111"+renderer_init.material.name);
                                Debug.Log("BLUEE22222"+renderer.material.name);
                            renderer.material = renderer_init.material;

                            }}*/
                             

					TakeResources (_currCraftItem);
					GameObject g = Instantiate (_itemObj);
                    
                    /////
                    

					if (Manager.CheckObjective (_currCraftItem))
						_quickMessage.ShowMessage("Objective Completed",5);

					if (_currCraftItem.HasRigidBody && g.GetComponent<Rigidbody> () != null)
						g.GetComponent<Rigidbody> ().detectCollisions = true;

					g.GetComponent<ItemReference> ().IsActive = true;
				}
			}
		}

		public void Drop(){
			if (_currItem == null)
				return;

            if (_invType != Inventory.Type.PlayerInventory)
                return;

			if (_toolHandler.CurrentTool != null) {
				if (_currItem == _toolHandler.CurrentTool && _inventory.Items(_currItem) == 1) {
					Destroy (_toolHandler.ToolObject);
					_toolHandler.CurrentTool = null;
				}
			}
				
			float amount =_inventoryUI.GetAmount();

            if (_inventory.DropItem(_currItem, amount)){
                if (!_inventory.HasItem(_currItem))
                    _currItem = null;
                if (_inventoryUI.IsActive())
                    _inventoryUI.Draw(_inventory);
            }
            else
                _quickMessage.ShowMessage("Can't drop it here!");
		}

	
		/// <returns><c>true</c>, if resources was hased, <c>false</c> otherwise.</returns>
		/// <param name="building">Building.</param>
		bool HaveResources(CraftableItem craftableItem){
			foreach (CraftableItem.CraftCost cost in craftableItem.GetCraftCost) {
                if (!_inventory.HasItem(cost.item, cost.amount))
					return false;
			}
			return true;
		}

	
		/// <param name="building">Building.</param>
		void TakeResources(CraftableItem craftableItem){
			foreach (CraftableItem.CraftCost craftCost in craftableItem.GetCraftCost) {
				_inventory.Add (craftCost.item, -craftCost.amount, this);
			}
			_craftCostUI.DrawCostView (craftableItem,_inventory);
		}

		void SetMenu(){
			_showingMenu = !_showingMenu;
			Cursor.visible = _showingMenu;
			if(_showingMenu)
				Time.timeScale = 0.0f;
			else
				Time.timeScale = 1.0f;

            //minimap.SetActive (!_showingMenu);
            //health.SetActive (!_showingMenu);
		}

		public void ShowPauseMenu(){
			SetMenu ();            
            minimap.SetActive(!_showingMenu);
            health.SetActive(!_showingMenu);
			_pauseMenu.SetActive (_showingMenu);
            
            
		}
 
		public void SelectItem(Item item,Inventory.Type type){
			_currItem = item;
			_invType = type;
			_inventoryUI.SelectItem (item,_invType);
		}

        /// <param name="taking"></param>
		public void TransferItem(bool taking){

			if (_currItem == null) {
				return;
			}

			Inventory otherInventory = _interactionObj.GetComponent<Inventory> (); 

			float amount = _inventoryUI.GetAmount();
            float price = 0;
            Item currency = Manager.Currency();

            if (taking) {
				if (_invType == Inventory.Type.PlayerInventory)
					return;

				if (amount > otherInventory.Items(_currItem))
					amount = otherInventory.Items(_currItem);

                if (_invType == Inventory.Type.Store){
                    price = amount * _currItem.Price;
                    
                    bool enoughCoins = _inventory.HasItem(currency, price);

                    if (!enoughCoins && price>0){
                        _quickMessage.ShowMessage("Not enough "+currency+"!");
                        return;
                    }
                    else
                        _quickMessage.ClearMessage();
                }

                if (_inventory.TryAdd(_currItem, amount)){
                    otherInventory.TryAdd(_currItem, -amount);
                    if (_invType == Inventory.Type.Store){
                        _inventory.Add(currency, -price);
                        otherInventory.Add(currency, price);
                    }
                }

				if(!otherInventory.HasItem(_currItem))
					_currItem = null;
			}else{
				if (_invType != Inventory.Type.PlayerInventory)
					return;

				if (amount > _inventory.Items(_currItem))
					amount = _inventory.Items(_currItem);

                if(_toolHandler.CurrentTool != null)
                    if(_toolHandler.CurrentTool == _currItem)
                        if(amount > _inventory.Items(_currItem) - 1){
                            Destroy(_toolHandler.ToolObject);
                            _toolHandler.CurrentTool = null;
                            /*
                                Destroy(_toolHandler);
                            _toolHandler= null;
                            */
                            
                        }

                if (otherInventory.InvType == Inventory.Type.Store){
                    price = amount * _currItem.Price;

                    if (price/amount > 1)
                        price = price * _tradeAdjustment;

                    bool enoughCoins = otherInventory.HasItem(currency, price);
                    if (!enoughCoins && price > 0){
                        _quickMessage.ShowMessage("Store can't Buy!");
                        return;
                    }
                    else
                        _quickMessage.ClearMessage();
                }

                if (otherInventory.TryAdd(_currItem, amount)){
                    _inventory.TryAdd(_currItem, -amount);
                    if (otherInventory.InvType == Inventory.Type.Store){
                        _inventory.Add(currency, price);
                        otherInventory.Add(currency, -price);
                    }
                }

				if (!_inventory.HasItem(_currItem)) 
					_currItem = null;
			}

			_inventoryUI.Draw(_inventory);
			_inventoryUI.Draw(otherInventory);
		}

	}
}