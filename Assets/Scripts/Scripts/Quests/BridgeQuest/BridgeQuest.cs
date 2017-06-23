using UnityEngine;
using UnityEngine.UI;
using Items;
using System;
using Utils;
using Quests;

namespace Assets.Resources.Scripts.Quests
{
    /// <summary>
    /// A quest where you have to reconstruct a destroyed bridge.
    /// </summary>
    class BridgeQuest : Quest
    {
        private const float NOTIFICATION_RATE = 20f;
        Vector3 mainCameraStartPosition;
        Camera bridgeCamera, mainCamera;
        GameObject hero, solveQuestUI, requiredMaterialPrefab;
        NotificationManager notificationManager;
        Hero heroScript;
        float nextNotificationShowRequest = 0f;
        public Bridge bridge;
        bool hasEnoughtMatsToSolve;
		AudioClip _bridgeBroken;

        MovementManager _moventManager;

        public GameObject _VisibleQuestGameObjects;

        bool _hasBeenConfigured = false;

        public override void Setup()
        {
            bridgeCamera = GameObject.Find("QuestCamera").GetComponent<Camera>();
            bridgeCamera.enabled = false;

            if (!_hasBeenConfigured)
            {
                _VisibleQuestGameObjects.SetActive(false);
            }
        }


        public override void Interact()
        {
            if (!_hasBeenConfigured) return;
            if (isPlayerSolvingTheQuest) LevelController.instance.Hero.movementManager.StopMovement();
            if (Vector3.Distance(hero.transform.position, gameObject.transform.position) < 2f)
            {
                if (!isPlayerSolvingTheQuest)
                {
                    if (nextNotificationShowRequest < Time.time)
                    {
                        notificationManager.showNotification(NotificationManager.NotificationType.STAR, "Challenge", "Press E to continue", 5f);
                        nextNotificationShowRequest = Time.time + NOTIFICATION_RATE;
                    }
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    isPlayerSolvingTheQuest = true;
                    goToBridgeCamera();
                    checkRequiredItems();
                    solveQuestUI.GetComponent<UIElement>().show();
                    LevelController.instance.Hero.movementManager.StopMovement();
                }
            }
        }

        void checkRequiredItems()
        {
            bool hasMats = true;
            foreach (RequiredItem item in requiredItems)
            {
                Material mat = originalMaterial;
                if (!QuestManager.instance.Inventory.hasEnoughtOfItem(item.item, item.quantity))
                {
                    mat = notEnoughtMaterial;
                    hasMats = false;
                }
                try
                {
                    item.UIRepresentation.GetComponent<Image>().material = mat;
                    item.UIRepresentation.transform.Find("MaterialQuantity").GetComponent<Text>().text = item.quantity.ToString();
                }
                catch (MissingReferenceException ex){
                }
            }
            hasEnoughtMatsToSolve = hasMats;
        }

        public override void Solve(Fraction fraction)
        {
            if (!isPlayerSolvingTheQuest) return;

            if (!hasEnoughtMatsToSolve)
            {
                NotificationManager.instance.addNotification(NotificationManager.NotificationType.ALERT, 
                    TextFactory.GetText("game_solve_not_enought_mats_title"), 
                    TextFactory.GetText("game_solve_not_enought_mats_text"), 5f);
                return;
            }

            goToMainCamera();
            LevelController.instance.Hero.isControlledByPlayer = false;

			if (fraction==Configuration.solution)
            {
                Debug.Log("Success");
                Success();
            }
            else
            {
				int numFails = PlayerPrefs.GetInt (Constants.STORAGE_NUM_FAILS);
				numFails++;
				PlayerPrefs.SetInt (Constants.STORAGE_NUM_FAILS, numFails);
                Fail();
                Debug.Log("Fail");
            }
			SendTry (fraction, DataManager.instance.ActualQuest.questConfigurationID);
        }



        public override void Fail()
        {
            isPlayerSolvingTheQuest = false;
            JukeBox.instance.LoadAndPlaySound("bridge_destroyed", 1);
            bridge.GetComponent<BoxCollider>().enabled = false;
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("BridgeWood"))
            {
                Rigidbody rb = g.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.AddForce(new Vector3(UnityEngine.Random.RandomRange(1, 1000), UnityEngine.Random.RandomRange(1, 1000), UnityEngine.Random.RandomRange(1, 1000)));
                }
            }
            LevelController.instance.Hero.DieDrowned();
        }

        public override void Success()
        {
            //Clear all the notifications
            NotificationManager.instance.ClearAndHideNotifications();
            isPlayerSolvingTheQuest = false;
            JukeBox.instance.LoadAndPlaySound("objective_done", 1);
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("BridgeWood"))
            {
                BridgeWood bw = g.GetComponent<BridgeWood>();
                try
                {
                    bw.setTransparent(false);
                }
                catch (Exception ex)
                {
                    Debug.Log(g.name);
                }
            }
			MoveHeroToEndPoint ();
            QuestManager.instance.ChageActualQuestState(QuestManager.ActualQuestStates.PENDING_TO_REWARD);
        }


        /// <summary>
        /// Switches between main camera and bridge camera
        /// </summary>
        void goToBridgeCamera()
        {
            mainCamera.enabled = false;
            bridgeCamera.enabled = true;
        }

        /// <summary>
        /// Switches between bridge camera and main camera
        /// </summary>
        public void goToMainCamera()
        {
            bridgeCamera.enabled = false;
            GameObject.Find("Camera").GetComponent<Camera>().enabled = true;
            solveQuestUI.GetComponent<UIElement>().hide();
            isPlayerSolvingTheQuest = false;
        }


        /// <summary>
        /// Shows in the user interface of the solve quest the required materials
        /// </summary>
        private void setupRequiredMaterials()
        {
            //Destroy last required items
            solveQuestUI = GameObject.Find("SolveQuestUI");
            GameObject[] requiredItemsToDestroy = GameObject.FindGameObjectsWithTag("RequiredItem");
            for (int i = 0; i < requiredItemsToDestroy.Length; i++)
            {
                Destroy(requiredItemsToDestroy[i]);
            }

            requiredMaterialPrefab = (GameObject)UnityEngine.Resources.Load(Constants.PATH_RESOURCES + "UIQuests/ReqItem");

            Transform reqMatTrans = GameObject.Find("RequiredMaterials").transform;

            GameObject wood = (GameObject)Instantiate(requiredMaterialPrefab);
            wood.transform.SetParent(reqMatTrans);
            wood.GetComponent<Image>().sprite = Items.Item.Wood.InventoryRepresentation;

            requiredItems.Add(new RequiredItem(Item.Wood, Configuration.solution.top, wood));

            originalMaterial = wood.GetComponent<Image>().material;
            originalMaterial.color = Color.white;
            notEnoughtMaterial = new Material(originalMaterial);
            notEnoughtMaterial.color = Color.red;

            solveQuestUI.GetComponent<UIElement>().hide();
        }

        public override void Restart()
        {
            bridge.GetComponent<BoxCollider>().enabled = true;
            isPlayerSolvingTheQuest = false;
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("BridgeWood"))
            {
                g.GetComponent<BridgeWood>().resetPosition();
            }
        }

		public void MoveHeroToEndPoint()
		{
            //Disable the colliders in order to make the hero move without problems
            bridge.DisableColliders();
			GameObject endQuestPoint = GameObject.Find ("EndQuestPoint");
            heroScript.PlayAnimation(HumanNPC.NPC_ANIMATION.RUN);
			hero.transform.LookAt (endQuestPoint.transform.position);
			heroScript.movementManager.SetNewDestination (endQuestPoint.transform.position, true);
			heroScript.SetHeroUpdate (null);
		}

        public override void Configure(ConfiguredQuest configuration)
        {
            _hasBeenConfigured = true;

            Configuration = configuration;
            bridge = GameObject.Find("Bridge").GetComponent<Bridge>();
            mainCameraStartPosition = Camera.main.transform.localPosition;
            hero = GameObject.Find("Hero(Clone)");
            heroScript = hero.GetComponent<Hero>();

            notificationManager = GameObject.Find("UI").GetComponent<NotificationManager>();
            mainCamera = Camera.main;
            isPlayerSolvingTheQuest = false;
            hasEnoughtMatsToSolve = false;
            _bridgeBroken = UnityEngine.Resources.Load("Media/Sounds/bridge_destroyed") as AudioClip;

            setupRequiredMaterials();

            UnityEngine.Events.UnityAction nextCallback = () => { goToMainCamera(); };
            GameObject.Find("GoToGame").GetComponent<Button>().onClick.AddListener(nextCallback);
            Setup();
            bridge.constructBridge(configuration.solution.top, configuration.solution.down);
            GUIController.instance.ShowGoal(UIGoal.GOALTYPE.NON_VIOLENT, Description, gameObject, this);
        }
    }
}
