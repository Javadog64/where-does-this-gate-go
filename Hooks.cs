using HUD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using IL.Smoke;
using Steamworks;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using RWCustom;

namespace WDTGG
{
    internal class Hooks
    {
        // Here so that we can keep track of if 2 or more "Player"'s have entered a new room
        private static int hasAnnounced = 0;
        public static void Apply()
        {

            On.Player.NewRoom += Player_NewRoom;

        }

        private static void Player_NewRoom(On.Player.orig_NewRoom orig, Player self, Room roomIn)
        {
            orig(self, roomIn);


            //Gets current room name Example: GATE_SU_HI
            string roomName = self.room.abstractRoom.name;
            //Gets current region identifier Example: SU
            string region = Region.GetVanillaEquivalentRegionAcronym(self.room.world.name);


            //Is the new room a gate?
            if (self.room.IsGateRoom())
            {

                //Has a player already entered the gate?
                if (hasAnnounced >= 1)
                {
                    return;
                }

                //Increment counter
                hasAnnounced++;

                string nextRegion = "";
                string[] splitRoomName = roomName.Split('_');
                //Is the first element in the broken roomName is the region we're in
                if (splitRoomName[1] == region)
                {
                    //Set nextRegion to the desination region and get the correct acronym
                    nextRegion = splitRoomName[2];
                }
                //If the first element is not the region we're in
                else
                {
                    //Set nextRegion to the desination region and get the correct acronym
                    nextRegion = splitRoomName[1];
                }


                //Gets slugcat specific regions Example instead of SS: Five Pebbles it will be RM: The Rot for Rivulet
                string correctRegion = Region.GetProperRegionAcronym(self.room.game.TimelinePoint, nextRegion);


                //Gets the full region name of the destination region
                string regionFullName = Region.GetRegionFullName(correctRegion, self.SlugCatClass);

                //Just in case a room is named something like GATE_HI
                if(splitRoomName.Length < 3)
                {
                    return;
                }

                //Don't show the message if it isn't a gate(yeah i know its weird but its for stuff like in GHTS) or both parts are equal
                //Example: Q0_TOFP returns false, DM_FAKEGATE_UNDER returns false, GATE_MK_MK returns false 
                if (splitRoomName[0].ToUpper() != "GATE" || (splitRoomName.Length >= 3 && splitRoomName[1] == splitRoomName[2]))
                {
                    return;
                }

                //Send message
                self.room.game.cameras[0].hud.textPrompt.AddMessage(Custom.rainWorld.inGameTranslator.Translate("Gate to ") + Custom.rainWorld.inGameTranslator.Translate(regionFullName), 0, 100, false, true);
            }
            else
            {
                //PLayer has exited gate reset counter
                hasAnnounced = 0;
            }
        }

    }
}
