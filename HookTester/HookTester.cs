using Pluton.Core;
using Pluton.Rust.Events;
using Pluton.Rust.Objects;
using Pluton.Rust.PluginLoaders;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HookTester
{
    public class HookTester : CSharpPlugin
    {
		BaseNPC testchicken;
		BasePlayer testbot;

		List<string> NotWorkingHooks = new List<string>();

		string success = "[Success]";
		string fail = 	 "[Fail]   ";

        public void On_AllPluginsLoaded()
        {
            Server.Broadcast("All plugins loaded");
        }

        public void On_BeingHammered(HammerEvent he)
        {
            Server.Broadcast(he.Victim.Name + " got hammered by " + he.Player.Name);
        }

        public void On_BuildingComplete(BuildingPart bp)
        {
            Server.Broadcast("Building completed " + bp.Name);
        }

        public void On_BuildingPartDemolished(BuildingPartDemolishedEvent bpde)
        {
            Server.Broadcast(bpde.BuildingPart.Name + " got demolished by " + bpde.Player.Name);
        }

        public void On_BuildingPartDestroyed(BuildingPartDestroyedEvent bpde)
        {
            Server.Broadcast(bpde.BuildingPart.Name + " got destroyed by " + bpde.Attacker.Name);
        }

        public void On_Chat(ChatEvent ce)
        {
            Server.Broadcast("Received message from " + ce.User.Name);
        }

        public void On_ClientAuth(AuthEvent ae)
        {
            Server.Broadcast(ae.Name + " authenticated on the server with IP " + ae.IP + " and ID " + ae.GameID);
        }

        public void On_ClientConsole(ClientConsoleEvent cce)
        {
            Server.Broadcast(cce.User.Name + " used the command " + cce.Cmd + " on his client console");
        }

        public void On_CombatEntityHurt(CombatEntityHurtEvent cehe)
        {
            Server.Broadcast(cehe.Attacker.Name + " attacked " + cehe.Victim.Name);
        }

        public void On_Command(CommandEvent ce)
        {
			if (ce.Cmd == "test" && ce.User.Admin) {
				// put all hook names 1st, remove them if they turn out to be working
				NotWorkingHooks = Hooks.GetInstance().HookNames;

				// since this hook works, remove it in advance
				NotWorkingHooks.Remove("On_Command");

				// remove the ones that we can't programmatically test, from a command
				NotWorkingHooks.Remove("On_PlayerConnected");
				NotWorkingHooks.Remove("On_PlayerDisconnected");
				NotWorkingHooks.Remove("On_ServerInit");
				NotWorkingHooks.Remove("On_ServerShutdown");
				NotWorkingHooks.Remove("On_PluginInit");
				NotWorkingHooks.Remove("On_PluginDeinit");
				NotWorkingHooks.Remove("On_AllPluginsLoaded");
				NotWorkingHooks.Remove("On_PlayerLoaded"); // ?

				Server.BroadcastFrom("Pluton Tester", "Initiating funcionality test.");

				Server.Broadcast("#" + NotWorkingHooks.Count + " hooks to be tested...");

				try
				{
					testchicken = World.SpawnAnimal("chicken", ce.User.X, ce.User.Z) as BaseNPC;
					testchicken.Hurt(new HitInfo(testchicken, testchicken, Rust.DamageType.Bite, 1));

					testbot = World.SpawnMapEntity("assets/prefabs/player/player.prefab", ce.User.X, ce.User.Z) as BasePlayer;
					testbot.StartSleeping();
					testbot.EndSleeping();
					testbot.Hurt(new HitInfo(testbot, testbot, Rust.DamageType.Bite, 1));
					testbot.UpdateRadiation(1);
					testbot.UpdateRadiation(0);
					testbot.RespawnAt(ce.User.Location, default(Quaternion));

					ce.User.SendConsoleCommand("testing");
				}
				catch (System.Exception ex)
				{
					Pluton.Core.Logger.LogException(ex);
				}
				Plugin.CreateTimer("ProbablyTestsFinished", 3000, (a) => {
					System.Console.WriteLine("The not working/tested hooks are(" + NotWorkingHooks.Count + "): " + string.Join(", ", NotWorkingHooks.ToArray()));
					a.Kill();
				}).Start();
			}
        }

        public void On_CommandPermission(CommandPermissionEvent cpe)
        {
            Server.Broadcast(cpe.User.Name + " is trying to use the command " + cpe.Cmd);
        }

        public void On_ConsumeFuel(ConsumeFuelEvent cfe)
        {
            Server.Broadcast(cfe.Item.Name + " consumed for fuel");
        }

        public void On_CorpseHurt(HurtEvent he)
        {
            Server.Broadcast(he.Attacker.Name + " attacked a corpse with " + he.Weapon.Name);
        }

        public void On_DoorCode(DoorCodeEvent dce)
        {
            Server.Broadcast(dce.Player.Name + " used code " + dce.Entered + " in the door with code " + dce.Code);
        }

        public void On_DoorUse(DoorUseEvent due)
        {
            Server.Broadcast(due.Player.Name + " used a door");
        }

        public void On_ItemLoseCondition(ItemConditionEvent ice)
        {
            Server.Broadcast(ice.Item.Name + " lost condition");
        }

        public void On_ItemPickup(ItemPickupEvent ipe)
        {
            Server.Broadcast(ipe.Item.Name + " was picked up by " + ipe.Player.Name);
        }

        public void On_ItemRepaired(ItemRepairEvent ire)
        {
            Server.Broadcast(ire.Player.Name + " repaired his " + ire.Item.Name);
        }

        public void On_ItemUsed(ItemUsedEvent iue)
        {
            Server.Broadcast(iue.Item.Name + " was used " + iue.Amount);
        }

        public void On_LandmineArmed(Landmine l)
        {
            Server.Broadcast("Landmine has been armed");
        }

        public void On_LandmineExploded(Landmine l)
        {
            Server.Broadcast("Landmine has exploded");
        }

        public void On_LandmineTriggered(LandmineTriggerEvent lte)
        {
            Server.Broadcast("Landmine has been triggered by " + lte.Player.Name);
        }

        public void On_LootingEntity(EntityLootEvent ele)
        {
            Server.Broadcast(ele.Target.Name + " is being looted by " + ele.Looter.Name);
        }

        public void On_LootingItem(ItemLootEvent ile)
        {
            Server.Broadcast(ile.Target + " is being looted by " + ile.Looter.Name);
        }

        public void On_LootingPlayer(PlayerLootEvent ple)
        {
            Server.Broadcast(ple.Target.Name + " is being looted by " + ple.Looter.Name);
        }

        public void On_NPCHurt(NPCHurtEvent he)
        {
			try
			{
				if (he.Attacker?.baseEntity == testchicken && he.Victim?.baseEntity == testchicken)
				{
					// Server.BroadcastFrom(success, "The chicken hurt itself. (On_NPCHurt)");
					NotWorkingHooks.Remove("On_NPCHurt");
					he.Victim.Kill();
				}
			}
			catch (System.Exception ex)
			{
				Pluton.Core.Logger.LogException(ex);
			}
        }

        public void On_NPCKilled(NPCDeathEvent de)
        {
			if (de.Attacker?.baseEntity == testchicken && de.Victim?.baseEntity == testchicken)
			{
				NotWorkingHooks.Remove("On_NPCKilled");
				//Server.BroadcastFrom(success, "The chicken died. (On_NPCDied)");
			}
        }

        public void On_Placement(BuildingEvent be)
        {
            Server.Broadcast(be.BuildingPart.Name + " has been placed by " + be.Builder.Name);
        }

        public void On_PlayerAssisted(Player player)
        {
			if (player.basePlayer == testbot)
			{
				NotWorkingHooks.Remove("On_PlayerAssisted");
			}
            // player.Message("Lucky you ! Somebody cured your wounds");
            player.Message("Somebody cured your wounds");
        }

        public void On_PlayerClothingChanged(PlayerClothingEvent pce)
        {
            pce.Player.Message("Your clothing has been changed");
        }

        public void On_PlayerConnected(Player player)
        {
            Server.Broadcast(player.Name + " has joined the Server!");
        }

        public void On_PlayerDied(PlayerDeathEvent pde)
        {
			if (pde.Attacker?.baseEntity == testbot && pde.Victim?.basePlayer == testbot)
			{
				NotWorkingHooks.Remove("On_PlayerDied");
                /*Player attacker = pde.Attacker.ToPlayer();
                Server.Broadcast(pde.Victim.Name + " was killed by " + attacker.Name);
                World.SpawnAnimal("wolf", pde.Victim.Location);*/
            }
        }

        public void On_PlayerDisconnected(Player player)
        {
            Server.Broadcast(player.Name + " has left the Server!");
        }

        public void On_PlayerGathering(GatherEvent ge)
        {
            ge.Gatherer.Message("You collected " + ge.Amount + " " + ge.Resource.Name);
        }

        public void On_PlayerHealthChange(PlayerHealthChangeEvent phce)
        {
			if (phce.Player.basePlayer == testbot)
			{
				NotWorkingHooks.Remove("On_PlayerHealthChange");
			}
			/*
            Player player = phce.Player;

            if (phce.OldHealth < phce.NewHealth)
            {
                player.Message("You have gained some health :)");
            }
            else
            {
                player.Message("You lost some health :(");
            }*/
        }

        public void On_PlayerHurt(PlayerHurtEvent phe)
        {
			if (phe.Victim?.basePlayer == testbot && phe.Victim?.basePlayer == phe.Attacker?.baseEntity)
            {
				NotWorkingHooks.Remove("On_PlayerHurt");
				testbot.StartWounded();
				testbot.StopWounded();
				testbot.Kill();
				// Server.BroadcastFrom(success, "On_PlayerHurt");
            if (phe.Attacker != null)
            {
                phe.Victim.Message("You got hit by " + phe.Attacker.Name);
            }
            else
            {
                phe.Victim.Message("You got hit by something");
            }
        }

        public void On_PlayerLoaded(Player player)
        {
            player.Message("You're loaded.");
        }

        public void On_PlayerSleep(Player player)
        {
			if (player.basePlayer == testbot)
			{
				NotWorkingHooks.Remove("On_PlayerSleep");
				Server.Broadcast(player.Name + " is going back to sleep!");
			}
        }

        public void On_PlayerStartCrafting(CraftEvent ce)
        {
            Server.Broadcast(ce.Crafter.Name + " is crafting " + ce.Target);
        }

        public void On_PlayerSyringeOther(SyringeUseEvent sue)
        {
            Server.Broadcast(sue.User.Name + " syringed " + sue.Receiver.Name);
        }

        public void On_PlayerSyringeSelf(SyringeUseEvent sue)
        {
            Server.Broadcast(sue.User.Name + " syringed himself");
        }

        public void On_PlayerTakeRadiation(PlayerTakeRadsEvent ptre)
        {
			if (ptre.Victim?.basePlayer == testbot)
			{
				if (NotWorkingHooks.Contains("On_PlayerTakeRadiation"))
					NotWorkingHooks.Remove("On_PlayerTakeRadiation");
			}
            Server.Broadcast(ptre.Victim.Name + " has taken " + ptre.RadAmount + " radiation");
        }

        public void On_PlayerWakeUp(Player player)
        {
			if (player.basePlayer == testbot)
			{
				NotWorkingHooks.Remove("On_PlayerWakeUp");
				player.Message(player.Name + " just woke up");
			}
        }

        public void On_PlayerWounded(Player player)
        {
			if (player.basePlayer == testbot)
			{
				NotWorkingHooks.Remove("On_PlayerWounded");
				player.Message(player.Name + " is wounded !");
			}
        }

        public void On_PluginDeinit()
        {
            Server.Broadcast("HookTester deinitialized");
        }

        public void On_PluginInit()
        {
            Server.Broadcast("HookTester initialized");
        }

        public void On_QuarryMining(MiningQuarry mq)
        {
            Server.Broadcast(mq.ShortPrefabName + " mined");
        }

        public void On_Respawn(RespawnEvent re)
        {
			if (re.Player.basePlayer == testbot)
			{
				NotWorkingHooks.Remove("On_Respawn");
				re.GiveDefault = false;
			}
            Server.Broadcast(re.Player.Name + " respawned on " + re.SpawnPos);
        }

        public void On_RocketShooting(RocketShootEvent rse)
        {
            Server.Broadcast(rse.Player.Name + " shot a rocket");
        }

        public void On_ServerConsole(ServerConsoleEvent sce)
        {
            Server.Broadcast(sce.Cmd + " command used in server console");
        }

        public void On_ServerInit()
        {
            Pluton.Core.Logger.Log("There are " + Server.SleepingPlayers.Count + " sleepers on the Server");
        }

        public void On_ServerSaved()
        {
            Server.Broadcast("Server data saved");
        }

        public void On_ServerShutdown()
        {
			Pluton.Core.Logger.Log("There are " + Server.SleepingPlayers.Count + " sleepers on the Server");
        }

        public void On_Shooting(ShootEvent se)
        {
            Server.Broadcast(se.Player.Name + " just shot " + se.BaseProjectile.ShortPrefabName);
        }

        public void On_WeaponThrow(WeaponThrowEvent wte)
        {
            Server.Broadcast(wte.Player.Name + " threw a " + wte.Weapon.ShortPrefabName);
        }
    }
}
