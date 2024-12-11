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

namespace WDTGG
{
    internal class Hooks
    {
        
        public static void Apply()
        {
            // Here so that we can keep track of if 2 or more "Player"'s have entered a new room
            int hasAnnounced = 0;
            On.Player.NewRoom += (orig, self, roomIN) =>
            {
                orig(self, roomIN);          

               
                //Gets current room name Example: GATE_SU_HI
                string roomName = self.room.abstractRoom.name;
                //Gets current region identifier Example: SU
                string region = self.room.world.name;

                //Gets slugcat specific regions Example instead of SS: Five Pebbles it will be RW: The Rot for Rivulet
                //Doesn't realy do anything if the slugcat isn't Rivulet or Saint
                region = Region.GetProperRegionAcronym(self.SlugCatClass, region);

                //Is the new room a gate?
                if (roomName.StartsWith("GATE"))
                {
                    
                    //Has a player already entered the gate?
                    if (hasAnnounced >= 1)
                    {
                        return;
                    }

                    //Increment counter
                    hasAnnounced++;

                    string nextRegion;
                    string[] splitRoomName = roomName.Split('_');
                    //Is the first element in the broken roomName is the region we're in
                    if (Region.GetProperRegionAcronym(self.SlugCatClass, splitRoomName[1]) == region)
                    {
                        //Set nextRegion to the desination region and get the correct acronym
                        nextRegion = Region.GetProperRegionAcronym(self.SlugCatClass, splitRoomName[2]);
                    }
                    //If the first element is not the region we're in
                    else
                    {
                        //Set nextRegion to the desination region and get the correct acronym
                        nextRegion = Region.GetProperRegionAcronym(self.SlugCatClass, splitRoomName[1]);
                    }


                    //Gets slugcat specific regions Example instead of SS: Five Pebbles it will be RW: The Rot for Rivulet
                    string correctRegion = Region.GetProperRegionAcronym(self.SlugCatClass, nextRegion);

                    //Gets the full region name of the destination region
                    string regionFullName = Region.GetRegionFullName(correctRegion, self.SlugCatClass);

                    //Send message
                    self.room.game.cameras[0].hud.textPrompt.AddMessage("Gate to " + regionFullName, 0, 100, false, true);
                } else
                {
                    //PLayer has exited gate reset counter
                    hasAnnounced = 0;
                }

                
                
                
            };
        }



    }
}
