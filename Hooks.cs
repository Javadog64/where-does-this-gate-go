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
            On.Player.NewRoom += (orig, self, roomIN) =>
            {
                orig(self, roomIN);

                //Gets current room name Example: GATE_SU_HI
                string roomName = self.room.abstractRoom.name;
                //Gets current region identifier Example: SU
                string region = self.room.world.name;

                //Is the new room a gate?
                if (roomName.StartsWith("GATE"))
                {
                    string nextRegion;
                    string[] splitRoomName = roomName.Split('_');
                    //Is the first element in the broken roomName is the region we're in
                    if (splitRoomName[1] == region)
                    {
                        //Set nextRegion to the desination region
                        nextRegion = splitRoomName[2];
                    }
                    //If the first element is not the region we're in
                    else
                    {
                        //Set nextRegion to the desination region
                        nextRegion = splitRoomName[1];
                    }

                    //Gets slugcat specific regions Example instead of SS: Five Pebbles it will be RW: The Rot for Rivulet
                    string correctRegion = Region.GetProperRegionAcronym(self.SlugCatClass, nextRegion);

                    //Gets the full region name of the destination region
                    string regionFullName = Region.GetRegionFullName(correctRegion, self.SlugCatClass);


                    //Send message
                    self.room.game.cameras[0].hud.textPrompt.AddMessage("Gate to " + regionFullName, 0, 100, false, false);
                }

                
                
                
            };
        }



    }
}
