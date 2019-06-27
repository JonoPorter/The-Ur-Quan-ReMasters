#region LGPL License
/*
 * The Ur-Quan ReMasters is a recreation of The Ur-Quan Masters in C#.
 * For the latest info, see http://sourceforge.net/projects/sc2-remake/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
using System.Collections.Generic;
using ReMasters.SuperMelee;
using System.Windows.Forms;


namespace ReMasters
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //MeleeMusic.InitMixer();
            Console.WriteLine("Loading Damage Sound Mappings");
            MeleeDamageSounds.Load();
            Console.WriteLine("Loading Music");
            MeleeMusic.Load();
            Console.WriteLine("Loading Sound");
            MeleeSound.Load();
            Console.WriteLine("Loading Game");


            ReMasters.SuperMelee.GUI.BattleSetup battleSetup = new ReMasters.SuperMelee.GUI.BattleSetup();
            battleSetup.ShowDialog();

            ReMasters.SuperMelee.GUI.ShipSelection p1 = new ReMasters.SuperMelee.GUI.ShipSelection();
            p1.Setup("Player 1", battleSetup.ImageList, battleSetup.Player1Ships);
            ReMasters.SuperMelee.GUI.ShipSelection p2 = new ReMasters.SuperMelee.GUI.ShipSelection();
            p2.Setup("Player 2", battleSetup.ImageList, battleSetup.Player2Ships);
            
            BaseDisplayDemo demo = new BaseDisplayDemo(
                p1,
                battleSetup.Player1IsAI,
                battleSetup.Player1WingmanCount,
                p2,
                battleSetup.Player2IsAI,
                battleSetup.Player2WingmanCount
                );

            Console.WriteLine("Creating Window");
            ReMasterSDL sdlr = new ReMasterSDL(demo);
            Console.WriteLine("Running Game");
            sdlr.Run();


        }
    }
}