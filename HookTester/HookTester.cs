using Pluton.Core;
using Pluton.Rust.Events;
using Pluton.Rust.Objects;
using Pluton.Rust.PluginLoaders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Rust;
using Logger = Pluton.Core.Logger;

namespace HookTester
{
	public class HookTester : CSharpPlugin
	{
		private BaseNPC TestChicken;
		private BasePlayer TestBot;

		#region helper functions

		void Broadcast(string msg)
		{
			Console.WriteLine(msg);
			Server.Broadcast(msg);
		}

		bool GetHookWorking(string hook)
		{
			return (bool)DataStore[GetType().ToString(), hook] == true;
		}

		void SetHookWorking(string hook)
		{
			DataStore[GetType().ToString(), hook] = true;
		}

		void SetHookNotWorking(string hook)
		{
			DataStore[GetType().ToString(), hook] = false;
		}

		void OutputResults()
		{
			Broadcast("Hook(s) that was/were not called during the tests: ");

			string[] nottested = (from hook in Pluton.Rust.Hooks.GetInstance().HookNames
								  where !GetHookWorking(hook)
								  select hook).ToArray();

			Broadcast(String.Join(", ", nottested));
		}

		#endregion

		#region Plugin Initialization

		public void On_PluginInit()
		{
			// Creating a copy of the full list of hook names
			if (DataStore.Get(GetType().ToString(), "Initialized") != null) {
				foreach (var hook in Hooks.GetInstance().HookNames) {
					DataStore[GetType().ToString(), hook] = false;
				}
			}

			DataStore.Add(GetType().ToString(), "Initialized", true);

			// Remove the ones that we can't programmatically test, from a command
			SetHookWorking("On_PluginInit");

			/*NotCalledHooks.Remove("On_ServerInit");
            NotCalledHooks.Remove("On_ServerShutdown");

            NotCalledHooks.Remove("On_PlayerConnected");
            NotCalledHooks.Remove("On_PlayerDisconnected");

            NotCalledHooks.Remove("On_PlayerLoaded"); // ?
            */
			Server.Broadcast("HookTester Initialized");
		}

		public void On_AllPluginLoaded()
		{
			SetHookWorking("On_AllPluginLoaded");

			Server.Broadcast("All plugins loaded");
		}

		public void On_PluginDeinit()
		{
			SetHookWorking("On_PluginDeinit");

			Server.Broadcast("HookTester deinitialized");
		}

		#endregion

		#region Hook Testing

		public void On_Command(CommandEvent ce)
		{
			SetHookWorking("On_Command");

			if (ce.Cmd == "nch" && ce.User.Admin) {
				OutputResults();
			}

			if (ce.Cmd == "test" && ce.User.Admin) {
				Server.BroadcastFrom("Pluton Tester", "Initiating funcionality test.");

				//Server.Broadcast("#" + NotCalledHooks.Count + " hooks to be tested...");

				try {
					TestChicken = World.SpawnAnimal("chicken", ce.User.X, ce.User.Z) as BaseNPC;
					TestChicken.Hurt(new HitInfo(TestChicken, TestChicken, DamageType.Bite, 1));
					TestChicken.Die(new HitInfo(TestChicken, TestChicken, DamageType.Bite, 999));

					TestBot = World.SpawnMapEntity("assets/prefabs/player/player.prefab", ce.User.X, ce.User.Z) as BasePlayer;
					TestBot.StartSleeping();
					TestBot.EndSleeping();
					TestBot.Hurt(new HitInfo(TestBot, TestBot, DamageType.Bite, 1));
					TestBot.UpdateRadiation(1);
					TestBot.UpdateRadiation(0);
					TestBot.RespawnAt(ce.User.Location, default(Quaternion));

					ce.User.Inventory.Add(new InvItem("Wood", 2000));
					ce.User.Inventory.Add(new InvItem("Wooden Door", 1));
					ce.User.Inventory.Add(new InvItem("Hammer", 1));
					ce.User.Inventory.Add(new InvItem("Lock", 1));
					ce.User.Inventory.Add(new InvItem("Code Lock", 1));
					ce.User.Inventory.Add(new InvItem("Tool Cupboard", 1));

					ce.User.SendConsoleCommand("testing");
				} catch (Exception ex) {
					Logger.LogException(ex);
				}
			}
		}

		private void OutputResults()
		{
			//Console.WriteLine(DataStore.GetTable(GetType().ToString()).Co + " hook(s) was/were not called during the tests: ");
			Server.Broadcast("Hook(s) that was/were not called during the tests: ");

			string[] nottested = (from hook in Pluton.Rust.Hooks.GetInstance().HookNames
			                      where !GetHookWorking(hook)
			                      select hook).ToArray();

			//List<string> nottested = new List<string>();

			/*foreach (DictionaryEntry hook in DataStore.GetTable(GetType().ToString()))
            {
				if (((bool)hook.Value) == false)
					nottested.Add((string)hook.Key);
            }*/
			Server.Broadcast(String.Join(", ", nottested));
		}

		#endregion

		#region Test Hooks

		public void On_BeingHammered(HammerEvent he)
		{
			SetHookWorking("On_BeingHammered");
			Server.Broadcast(he.Victim.Name + " got hammered by " + he.Player.Name);
		}

		public void On_BuildingComplete(BuildingPart bp)
		{
			SetHookWorking("On_BuildingComplete");
			Server.Broadcast("Building completed " + bp.Name);
		}

		public void On_BuildingPartDemolished(BuildingPartDemolishedEvent bpde)
		{
			SetHookWorking("On_BuildingPartDemolished");
			Server.Broadcast(bpde.BuildingPart.Name + " got demolished by " + bpde.Player.Name);
		}

		public void On_BuildingPartDestroyed(BuildingPartDestroyedEvent bpde)
		{
			SetHookWorking("On_BuildingPartDestroyed");
			Server.Broadcast(bpde.BuildingPart.Name + " got destroyed by " + bpde.Attacker.Name);
		}

		public void On_Chat(ChatEvent ce)
		{
			SetHookWorking("On_Chat");
		}

		public void On_ClientAuth(AuthEvent ae)
		{
			SetHookWorking("On_ClientAuth");
			Server.Broadcast(ae.Name + " authenticated on the server with IP " + ae.IP + " and ID " + ae.GameID);
		}

		public void On_ClientConsole(ClientConsoleEvent cce)
		{
			SetHookWorking("On_ClientConsole");
			Server.Broadcast(cce.User.Name + " used the command " + cce.Cmd + " on his client console");
		}

		public void On_CombatEntityHurt(CombatEntityHurtEvent cehe)
		{
			SetHookWorking("On_CombatEntityHurt");
			Server.Broadcast(cehe.Attacker.Name + " attacked " + cehe.Victim.Name);
		}

		public void On_CommandPermission(CommandPermissionEvent cpe)
		{
			SetHookWorking("On_CommandPermission");
			Server.Broadcast(cpe.User.Name + " is trying to use the command " + cpe.Cmd);
		}

		public void On_ConsumeFuel(ConsumeFuelEvent cfe)
		{
			SetHookWorking("On_ConsumeFuel");
			Server.Broadcast(cfe.Item.Name + " consumed for fuel");
		}

		public void On_CorpseHurt(HurtEvent he)
		{
			SetHookWorking("On_CorpseHurt");
			Server.Broadcast(he.Attacker.Name + " attacked a corpse with " + he.Weapon.Name);
		}

		public void On_DoorCode(DoorCodeEvent dce)
		{
			SetHookWorking("On_DoorCode");
			Server.Broadcast(dce.Player.Name + " used code " + dce.Entered + " in the door with code " + dce.Code);
		}

		public void On_DoorUse(DoorUseEvent due)
		{
			SetHookWorking("On_DoorUse");
			Server.Broadcast(due.Player.Name + " used a door");
		}

		public void On_ItemLoseCondition(ItemConditionEvent ice)
		{
			SetHookWorking("On_ItemLoseCondition");
			Server.Broadcast(ice.Item.Name + " lost condition");
		}

		public void On_ItemPickup(ItemPickupEvent ipe)
		{
			SetHookWorking("On_ItemPickup");
			Server.Broadcast(ipe.Item.Name + " was picked up by " + ipe.Player.Name);
		}

		public void On_ItemRepaired(ItemRepairEvent ire)
		{
			SetHookWorking("On_ItemRepaired");
			Server.Broadcast(ire.Player.Name + " repaired his " + ire.Item.Name);
		}

		public void On_ItemUsed(ItemUsedEvent iue)
		{
			SetHookWorking("On_ItemUsed");
			Server.Broadcast(iue.Item.Name + " was used " + iue.Amount);
		}

		public void On_LandmineArmed(Landmine l)
		{
			SetHookWorking("On_LandmineArmed");
			Server.Broadcast("Landmine has been armed");
		}

		public void On_LandmineExploded(Landmine l)
		{
			SetHookWorking("On_LandmineExploded");
			Server.Broadcast("Landmine has exploded");
		}

		public void On_LandmineTriggered(LandmineTriggerEvent lte)
		{
			SetHookWorking("On_LandmineTriggered");
			Server.Broadcast("Landmine has been triggered by " + lte.Player.Name);
		}

		public void On_LootingEntity(EntityLootEvent ele)
		{
			SetHookWorking("On_LootingEntity");
			Server.Broadcast(ele.Target.Name + " is being looted by " + ele.Looter.Name);
		}

		public void On_LootingItem(ItemLootEvent ile)
		{
			SetHookWorking("On_LootingItem");
			Server.Broadcast(ile.Target + " is being looted by " + ile.Looter.Name);
		}

		public void On_LootingPlayer(PlayerLootEvent ple)
		{
			SetHookWorking("On_LootingPlayer");
			Server.Broadcast(ple.Target.Name + " is being looted by " + ple.Looter.Name);
		}

		public void On_NPCHurt(NPCHurtEvent he)
		{
			try {
				if (he.Attacker?.baseEntity == TestChicken && he.Victim?.baseEntity == TestChicken) {
					SetHookWorking("On_NPCHurt");
					// Server.BroadcastFrom(SUCCESS, "The chicken hurt itself. (On_NPCHurt)");
				}
			} catch (Exception ex) {
				Logger.LogException(ex);
			}
		}

		public void On_NPCKilled(NPCDeathEvent de)
		{
			if (de.Attacker?.baseEntity == TestChicken && de.Victim?.baseEntity == TestChicken) {
				SetHookWorking("On_NPCKilled");
				//Server.BroadcastFrom(SUCCESS, "The chicken died. (On_NPCDied)");
			}
		}

		public void On_Placement(BuildingEvent be)
		{
			SetHookWorking("On_Placement");
			Server.Broadcast(be.BuildingPart.Name + " has been placed by " + be.Builder.Name);
		}

		public void On_PlayerAssisted(Player player)
		{
			if (player.basePlayer == TestBot) {
				SetHookWorking("On_PlayerAssisted");
			}
			player.Message("Somebody cured your wounds");
		}

		public void On_PlayerClothingChanged(PlayerClothingEvent pce)
		{
			SetHookWorking("On_PlayerClothingChanged");
			pce.Player.Message("Your clothing has been changed");
		}

		public void On_PlayerConnected(Player player)
		{
			SetHookWorking("On_PlayerConnected");
			Server.Broadcast(player.Name + " has joined the Server!");
		}

		public void On_PlayerDied(PlayerDeathEvent pde)
		{
			if (pde.Attacker?.baseEntity == TestBot && pde.Victim?.basePlayer == TestBot) {
				SetHookWorking("On_PlayerDied");
				/*Player attacker = pde.Attacker.ToPlayer();
                Server.Broadcast(pde.Victim.Name + " was killed by " + attacker.Name);
                World.SpawnAnimal("wolf", pde.Victim.Location);*/
			}
		}

		public void On_PlayerDisconnected(Player player)
		{
			SetHookWorking("On_PlayerDisconnected");
			Server.Broadcast(player.Name + " has left the Server!");
		}

		public void On_PlayerGathering(GatherEvent ge)
		{
			SetHookWorking("On_PlayerGathering");
			ge.Gatherer.Message("You collected " + ge.Amount + " " + ge.Resource.Name);
		}

		public void On_PlayerHealthChange(PlayerHealthChangeEvent phce)
		{
			if (phce.Player.basePlayer == TestBot) {
				SetHookWorking("On_PlayerHealthChange");
			}
		}

		public void On_PlayerHurt(PlayerHurtEvent phe)
		{
			if (phe.Victim?.basePlayer == TestBot && phe.Victim?.basePlayer == phe.Attacker?.baseEntity) {
				SetHookWorking("On_PlayerHurt");
				TestBot.StartWounded();
				TestBot.StopWounded();
				TestBot.Kill();
				// Server.BroadcastFrom(SUCCESS, "On_PlayerHurt");
			}
			if (phe.Attacker != null) {
				phe.Victim.Message("You got hit by " + phe.Attacker.Name);
			} else {
				phe.Victim.Message("You got hit by something");
			}
		}

		public void On_PlayerLoaded(Player player)
		{
			SetHookWorking("On_PlayerLoaded");
			player.Message("You're loaded.");
		}

		public void On_PlayerSleep(Player player)
		{
			if (player.basePlayer == TestBot) {
				SetHookWorking("On_PlayerSleep");
				Server.Broadcast(player.Name + " is going back to sleep!");
			}
		}

		public void On_PlayerStartCrafting(CraftEvent ce)
		{
			SetHookWorking("On_PlayerStartCrafting");
			Server.Broadcast(ce.Crafter.Name + " is crafting " + ce.Target);
		}

		public void On_PlayerSyringeOther(SyringeUseEvent sue)
		{
			SetHookWorking("On_PlayerSyringeOther");
			Server.Broadcast(sue.User.Name + " syringed " + sue.Receiver.Name);
		}

		public void On_PlayerSyringeSelf(SyringeUseEvent sue)
		{
			SetHookWorking("On_PlayerSyringeSelf");
			Server.Broadcast(sue.User.Name + " syringed himself");
		}

		public void On_PlayerTakeRadiation(PlayerTakeRadsEvent ptre)
		{
			if (ptre.Victim?.basePlayer == TestBot) {
				SetHookWorking("On_PlayerTakeRadiation");
			}
			Server.Broadcast(ptre.Victim.Name + " has taken " + ptre.RadAmount + " radiation");
		}

		public void On_PlayerWakeUp(Player player)
		{
			if (player.basePlayer == TestBot) {
				SetHookWorking("On_PlayerWakeUp");
				player.Message(player.Name + " just woke up");
			}
		}

		public void On_PlayerWounded(Player player)
		{
			if (player.basePlayer == TestBot) {
				SetHookWorking("On_PlayerWounded");
				player.Message(player.Name + " is wounded !");
			}
		}

		public void On_QuarryMining(MiningQuarry mq)
		{
			SetHookWorking("On_QuarryMining");
			Server.Broadcast(mq.ShortPrefabName + " mined");
		}

		public void On_Respawn(RespawnEvent re)
		{
			if (re.Player.basePlayer == TestBot) {
				SetHookWorking("On_Respawn");
				re.GiveDefault = false;
			}
			Server.Broadcast(re.Player.Name + " respawned on " + re.SpawnPos);
		}

		public void On_RocketShooting(RocketShootEvent rse)
		{
			SetHookWorking("On_RocketShooting");
			Server.Broadcast(rse.Player.Name + " shot a rocket");
		}

		public void On_ServerConsole(ServerConsoleEvent sce)
		{
			SetHookWorking("On_ServerConsole");
			Server.Broadcast(sce.Cmd + " command used in server console");
		}

		public void On_ServerInit()
		{
			SetHookWorking("On_ServerInit");
			Pluton.Core.Logger.Log("There are " + Server.SleepingPlayers.Count + " sleepers on the Server");
		}

		public void On_ServerSaved()
		{
			SetHookWorking("On_ServerSaved");
			Server.Broadcast("Server data saved");
		}

		public void On_ServerShutdown()
		{
			SetHookWorking("On_ServerShutdown");
			Pluton.Core.Logger.Log("There are " + Server.SleepingPlayers.Count + " sleepers on the Server");
		}

		public void On_Shooting(ShootEvent se)
		{
			SetHookWorking("On_Shooting");
			Server.Broadcast(se.Player.Name + " just shot " + se.BaseProjectile.ShortPrefabName);
		}

		public void On_WeaponThrow(WeaponThrowEvent wte)
		{
			SetHookWorking("On_WeaponThrow");
			Server.Broadcast(wte.Player.Name + " threw a " + wte.Weapon.ShortPrefabName);
		}
	}

	#endregion
}
