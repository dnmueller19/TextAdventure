﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextAdeventure.Properties;
using ClassLibrary1;
using ClassLibrary1.Locations;
using ClassLibrary1.Monsters.Quest;
using ClassLibrary1.Potions.Items.Weapons;


namespace TextAdeventure
{
	public partial class TextAdventure : Form
	{
		private Player player;
		private Monster currentMonster;

		public TextAdventure()
		{
			InitializeComponent();
			//add player things here
			player = new Player(10, 10, 20, 0, 1);
			MoveTo(World.LocationbyID(World.idHome));
			player.Inventory.Add(new InventoryItem(World.ItemByID(World.knife), 1));




			// making the states go to the labels
			lblHitPoints.Text = player.CurrentHP.ToString();
			lblGold.Text = player.Gold.ToString();
			lblLevel.Text = player.Level.ToString();
			lblExperience.Text = player.ExperiencePoints.ToString();

			//Location 
			//Location test1 = new Location(1, "Home", "This is your house");
			//Location location = new Location(1, "Home", "This is your house", null, null, null);
		}
		private void btnNorth_Click(object sender, EventArgs e)
		{
			MoveTo(player.CurrentLocation.LocationNorth);
		}

		private void btnWest_Click(object sender, EventArgs e)
		{
			MoveTo(player.CurrentLocation.LocationWest);
		}

		private void btnEast_Click(object sender, EventArgs e)
		{
			MoveTo(player.CurrentLocation.LocationEast);
		}

		private void btnSouth_Click(object sender, EventArgs e)
		{
			MoveTo(player.CurrentLocation.LocationSouth);
		}

		private void btnUsePotion_Click_1(object sender, EventArgs e)
		{

		}

		private void MoveTo(Location newLocation)
		{

			// item needed
			if (!player.HasRequiredItemToEnterLocation(newLocation))
			{
				rtbMessages.Text += "You need a" + newLocation.ItemRequiredToEnter.Name + "to enter this location." + Environment.NewLine;
				return;
			}

			//update players location
			player.CurrentLocation = newLocation;

			//show/hide availble movement buttons
			btnNorth.Visible = (newLocation.LocationNorth != null);
			btnEast.Visible = (newLocation.LocationEast != null);
			btnSouth.Visible = (newLocation.LocationSouth != null);
			btnWest.Visible = (newLocation.LocationWest != null);

			//display current location name and discription 
			rtbLocation.Text = newLocation.Name + Environment.NewLine;
			rtbLocation.Text = newLocation.Description + Environment.NewLine;

			//completely heal the player
			player.CurrentHP = player.MaxHP;

			//update player HP in UI
			lblHitPoints.Text = player.CurrentHP.ToString();

			//does location have a quest
			if (newLocation.QuestAvailableHere != null)
			{
				bool playerAlreadyHasQuest = player.HasThisQuest(newLocation.QuestAvailableHere);
				bool playerAlreadyCompletedQuest = player.CompletedThisQuest(newLocation.QuestAvailableHere);


				//see if player already has quest
				if (playerAlreadyHasQuest)
				{
					if (!playerAlreadyCompletedQuest)
					{
						bool playerHasAllItemsToCompleteQuest = player.HasAllQuestCompletionItems(newLocation.QuestAvailableHere);
						// The player has all items required to complete the quest
						if (playerHasAllItemsToCompleteQuest)
						{
							// Display message
							rtbMessages.Text += Environment.NewLine;
							rtbMessages.Text += "you completed the" + newLocation.QuestAvailableHere.Name + " quest" + Environment.NewLine;

							//remove quest item 
							player.RemoveQuestCompletionItem(newLocation.QuestAvailableHere);

							// Give quest rewards
							rtbMessages.Text = "You recieve:" + Environment.NewLine;
							rtbMessages.Text = newLocation.QuestAvailableHere.RewardEXP.ToString() + "EXP" + Environment.NewLine;
							rtbMessages.Text = newLocation.QuestAvailableHere.RewardGold.ToString() + " pieces of gold" + Environment.NewLine;
							rtbMessages.Text = newLocation.QuestAvailableHere.RewardItem.ToString() + Environment.NewLine;
							rtbMessages.Text = Environment.NewLine;

							player.ExperiencePoints += newLocation.QuestAvailableHere.RewardEXP;
							player.Gold += newLocation.QuestAvailableHere.RewardGold;

							// Add the reward item to the player's inventory
							player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);

							// Mark the quest as completed
							player.MarkQuestCompleted(newLocation.QuestAvailableHere);

						}
					}

				}
				else
				{
					// The player does not already have the quest
					// Display the messages
					rtbMessages.Text += "Quest " + newLocation.QuestAvailableHere.Name + " has been added to your adventure" + Environment.NewLine;
					rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
					rtbMessages.Text += "To complete quest, return with " + Environment.NewLine;

					//foreach loop to run through questcompletionitems in newlocation
					foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
					{
						//if qci quanity = 1, rtbmessage(qci quanity and details)
						if (qci.Quanity == 1)
						{
							rtbMessages.Text += qci.Quanity.ToString() + " " + qci.Details.Name + Environment.NewLine;
						}
						else
						{
							//else (qci quanity + plural)
							rtbMessages.Text += qci.Quanity.ToString() + " " + qci.Details.PluralName + Environment.NewLine;
						}
					}
					rtbMessages.Text += Environment.NewLine;
					// Add the quest to the player's quest list
					player.Quest.Add(new PlayerQuest(newLocation.QuestAvailableHere));
				}

			}

			// Is there a monster at the location?
			if (newLocation.MonsterLivingHere != null)
			{
				rtbMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

				//If so,
				//Display message
				//Spawn new monster to fight
				// Make a new monster, using the values from the standard monster in the World.Monster list
				Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

				currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaxDamage, standardMonster.RewardEXP, standardMonster.RewardGold, standardMonster.CurrentHP, standardMonster.MaxHP);

				//foreaach loop to go through lootitem/table
				foreach (LootItem li in standardMonster.LootTable)
				{
					//add li to loottable
					currentMonster.LootTable.Add(li);
				}
				//Display combat comboboxes and buttons
				cboPotions.Visible = true;
				cboWeapons.Visible = true;
				btnUsePotion.Visible = true;
				btnUseWeapon.Visible = true;
			}
			//	If not
			//		Hide combat comboboxes and buttons
			else
			{
				currentMonster = null;

				cboWeapons.Visible = false;
				cboPotions.Visible = false;
				btnUseWeapon.Visible = false;
				btnUsePotion.Visible = false;

			}

			//Repopulate comboboxes, in case inventory changed

			//refrsh IVENTORY ITems
			UpdateInventoryListInUI();


			//refresh Quest list
			UpdateQuestListInUI();

			//refreash weapon combo box
			UpdateWeaponsListInUI();


			//Refresh potion combo box
			UpdatePotionsListInUI();
		}

		private void UpdatePotionsListInUI()
		{
			// 1a. create new healing potion list
			List<HealingPotion> healingPotions = new List<HealingPotion>();

			//1b. foreach: inventitems in player(iventory)
			foreach (InventoryItem healingInv in player.Inventory)
			{
				//1c. if items is a HEALINGPOTION
				if (healingInv.Details is HealingPotion)
				{
					//1c. if: quanity id > 0
					if (healingInv.Quantity > 0)
					{
						//1d. add to potion list
						healingPotions.Add((HealingPotion)healingInv.Details);
					}
				}
			}

			//2a. if potion count is 0
			if (healingPotions.Count == 0)
			{
				//2b. hide the potion cbo and btn
				cboPotions.Visible = false;
				btnUseWeapon.Visible = false;
			}
			//2c. else: cbo(dataspurce: hp(list))...all smae ad weapons above
			else
			{
				cboPotions.DataSource = healingPotions;
				cboPotions.DisplayMember = "Name";
				cboPotions.ValueMember = "ID";
				cboPotions.SelectedIndex = 0;
			}
		}

		private void UpdateWeaponsListInUI()
		{
			// 1. create a new weapons list
			List<Weapon> weapons = new List<Weapon>();

			// 2.foreach through inventory items
			foreach (InventoryItem weaponInv in player.Inventory)
			{
				// 2a.if: item is weapon 
				if (weaponInv.Details is Weapon)
				{
					//2b. if: quanity is > 0
					if (weaponInv.Quantity > 0)
					{
						// 2c. add to (weapon) list
						weapons.Add((Weapon)weaponInv.Details);
					}
				}
			}

			// 3. if" player has no weapopns then hide cboweapon and btn button, use count weapons is a list
			if (weapons.Count == 0)
			{
				cboWeapons.Visible = false;
				btnUseWeapon.Visible = false;
			}
			//else: cboweapon(datasource:weapons)(displaymember = name)(valuemember = id)(selectedindex = 0)
			else
			{
				cboWeapons.DataSource = weapons;
				cboWeapons.DisplayMember = "Name";
				cboWeapons.ValueMember = "ID";

				cboWeapons.SelectedIndex = 0;

			}
		}

		private void UpdateQuestListInUI()
		{
			dgvQuests.RowHeadersVisible = false;

			dgvQuests.ColumnCount = 2;
			dgvQuests.Columns[0].Name = "Name";
			dgvQuests.Columns[0].Width = 197;
			dgvQuests.Columns[1].Name = "Done?";

			dgvQuests.Rows.Clear();

			//loop through quest using playerquest data type
			foreach (PlayerQuest playerQuest in player.Quest)
			{
				//new arrya that adds name and iscompleted to it using dgvquest
				dgvQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.QuestCompleted.ToString() });
			}

		}

		//put weapons here
		private void UpdateInventoryListInUI()
		{
			dgvInventory.RowHeadersVisible = false;

			dgvInventory.ColumnCount = 2;
			dgvInventory.Columns[0].Name = "Name";
			dgvInventory.Columns[0].Width = 197;
			dgvInventory.Columns[1].Name = "Quanity";

			dgvInventory.Rows.Clear();

			//loop through inventoryitem in player inventory
			foreach (InventoryItem inventoryItem in player.Inventory)
			{
				//if statement: invetory quanity > 0
				if (inventoryItem.Quantity > 0)
				{
					//add a new array using dgvInvent(row)(add)
					//adding Inventitem name and quanity(tostring)
					dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
				}
			}
		}

		//here accidents here
		private void TextAdventure_Load(object sender, EventArgs e)
		{

		}

		private void label5_Click(object sender, EventArgs e)
		{

		}


		//put weapon button here
		private void btnUseWeapon_Click(object sender, EventArgs e)
		{
			//get the currently selectged weapon from teh cbo weapn
			Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

			//determin the amount of damamage
			int damageMonster = RandomNumber.NumbeBetween(currentWeapon.MinDamage, currentWeapon.MaxDamage);

			//applu the damage to teh monsters current HItPoints
			currentMonster.CurrentHP -= damageMonster;

			//display message
			rtbMessages.Text += "Yout hit " + currentMonster.Name + "for" + damageMonster.ToString() + " damage" + Environment.NewLine;

			//check if monster dead
			if (currentMonster.CurrentHP <= 0)
			{
				//monster is dead
				rtbMessages.Text += Environment.NewLine;
				rtbMessages.Text += "You defeated " + currentMonster.Name + Environment.NewLine;

				//give player EXP
				player.ExperiencePoints += currentMonster.RewardEXP;
				rtbMessages.Text += "you recieved " + currentMonster.RewardEXP.ToString() + " EXP" + Environment.NewLine;

				//give player gold
				player.Gold += currentMonster.RewardGold;
				rtbMessages.Text += "you recieved " + currentMonster.RewardGold.ToString() + " pieces of gold" + Environment.NewLine;

				//get random loot 
				//create a new list
				List<InventoryItem> lootedItems = new List<InventoryItem>();

				//add loot to looted items list (foreace)/if
				foreach (LootItem lootItem in currentMonster.LootTable)
				{
					if (RandomNumber.NumbeBetween(1, 100) <= lootItem.DropPercentage)
					{
						lootedItems.Add(new InventoryItem(lootItem.Details, 1));
					}
				}
				//if no items were andomly selected add default look item
				if (lootedItems.Count == 0)
				{
					foreach (LootItem lootItem in currentMonster.LootTable)
					{
						if (lootItem.DefaultItem)
						{
							lootedItems.Add(new InventoryItem(lootItem.Details, 1));
						}
					}
				}

			}

		}
	}

	


	

	



	
}

	


		
	



