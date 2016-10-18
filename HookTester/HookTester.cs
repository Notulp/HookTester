using Pluton.Core;
using Pluton.Rust.Events;
using Pluton.Rust.Objects;
using Pluton.Rust.PluginLoaders;

namespace HookTester
{
    public class HookTester : CSharpPlugin
    {
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
            Server.Broadcast(ce.User.Name + " used the command " + ce.Cmd + " with arguments " + ce.Args);
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
            Server.Broadcast(he.Attacker + " attacked a corpse with " + he.Weapon);
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
            Server.Broadcast(l.name + " has been armed");
        }

        public void On_LandmineExploded(Landmine l)
        {
            Server.Broadcast(l.name + " has exploded");
        }

        public void On_LandmineTriggered(LandmineTriggerEvent lte)
        {
            Server.Broadcast(lte.Landmine + " has been triggered by " + lte.Player.Name);
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
            if (he.Attacker != null)
            {
                Server.Broadcast(he.Victim.Name + " has been hurt by " + he.Attacker.Name);
            }
            else
            {
                Server.Broadcast(he.Victim.Name + " has been hurt");
            }
        }

        public void On_NPCKilled(NPCDeathEvent de)
        {
            Server.Broadcast(de.Victim.Name + " has been killed by " + de.Attacker.Name);
        }

        public void On_Placement(BuildingEvent be)
        {
            Server.Broadcast(be.BuildingPart.Name + " has been placed by " + be.Builder.Name);
        }

        public void On_PlayerAssisted(Player player)
        {
            player.Message("Lucky you ! Somebody cured your wounds");
        }

        public void On_PlayerClothingChanged(PlayerClothingEvent pce)
        {
            pce.Player.Message("You look so sexy now !");
        }

        public void On_PlayerConnected(Player player)
        {
            player.Message("Welcome to our Server!");
            Server.Broadcast(player.Name + " has joined the Server!");
        }

        public void On_PlayerDied(PlayerDeathEvent pde)
        {
            if (pde.Attacker.IsPlayer())
            {
                Player attacker = pde.Attacker.ToPlayer();
                Server.Broadcast(pde.Victim.Name + " was killed by " + attacker.Name);
                World.SpawnAnimal("wolf", pde.Victim.Location);
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
            Player player = phce.Player;

            if (phce.OldHealth < phce.NewHealth)
            {
                player.Message("You have gained some health :)");
            }
            else
            {
                player.Message("You lost some health :(");
            }
        }

        public void On_PlayerHurt(PlayerHurtEvent phe)
        {
            if (phe.Attacker.IsPlayer())
            {
                Player attacker = phe.Attacker.ToPlayer();
                Player victim = phe.Victim;
                attacker.Message("You hit the player " + victim.Name);
                victim.Message("You got hit by the player " + attacker.Name);
            }
            else
            {
                phe.Victim.Message("You got hit by a " + phe.Attacker.Name);
            }
        }

        public void On_PlayerLoaded(Player player)
        {
            player.Message("You're loaded.");
        }

        public void On_PlayerSleep(Player player)
        {
            Server.Broadcast(player.Name + " is going back to sleep!");
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
            Server.Broadcast(ptre.Victim.Name + " has taken " + ptre.RadAmount + " radiation");
        }

        public void On_PlayerWakeUp(Player player)
        {
            player.Message(player.Name + " just woke up");
        }

        public void On_PlayerWounded(Player player)
        {
            player.Message(player.Name + " is wounded !");
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
            Logger.Log("There are " + Server.SleepingPlayers.Count + " sleepers on the Server");
        }

        public void On_ServerSaved()
        {
            Server.Broadcast("Server data saved");
        }

        public void On_ServerShutdown()
        {
            Logger.Log("There are " + Server.SleepingPlayers.Count + " sleepers on the Server");
        }

        public void On_Shooting(ShootEvent se)
        {
            Server.Broadcast(se.Player.Name + " just shot ");
        }

        public void On_WeaponThrow(WeaponThrowEvent wte)
        {
            Server.Broadcast(wte.Player.Name + " threw a " + wte.Weapon.ShortPrefabName);
        }
    }
}
