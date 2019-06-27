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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
/*using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;*/
//using Microsoft.DirectX.DirectInput;
using SdlDotNet;
using Physics2D;
using AdvanceMath; using AdvanceSystem;
using AdvanceMath.Geometry2D;
using ReMasters.SuperMelee.Ships;
namespace ReMasters.SuperMelee.Controlers
{
    [Serializable]
    public class PlayerControlerConfigInfo
    {
        public Key MoveForward;
        public Key RotateRight;
        public Key RotateLeft;
        public Key PrimaryAction;
        public Key SecondaryAction;
        public Key TargetSelection;
    }
    [Serializable]
    public class PlayerControler : BaseControler,IPlayerControler
    {
        [NonSerialized]
        KeyboardState keys;
        IShip shipHost;
        PlayerControlerConfigInfo configInfo;
        public PlayerControler(PlayerControlerConfigInfo configInfo)
        {
            this.configInfo = configInfo;
        }
        protected PlayerControler(PlayerControler copy)
            : base(copy)
        {
            this.configInfo = copy.configInfo;
        }
        public void SetKeyboardState(KeyboardState keys)
        {
            this.keys = keys;
        }
        public override void OnCreation(GameResult gameResult, IControlable host)
        {
            this.lifeTime = new LifeSpan();
            this.host = host;
            this.shipHost = host as IShip;
        }
        float timepass = 0;
        public override ControlInput GetControlInput(float dt, ControlInput original)
        {
            timepass += dt;
            if (keys != null)
            {
                if (keys[configInfo.TargetSelection])
                {
                    if (timepass > .25f)
                    {
                        if (shipHost.Actions.Count > 0 && 
                            shipHost.Actions[0].TargetableTypes != TargetingInfo.None &&
                            shipHost.Actions[0].TargetableTypes != TargetingInfo.Self)
                        {
                            if (keys[configInfo.RotateRight])
                            {
                                this.host.Target = this.host.TargetRetriever.Next(shipHost.Actions[0].TargetableTypes);
                                timepass = 0;
                            }
                            if (keys[configInfo.PrimaryAction])
                            {
                                this.host.Target = this.host.TargetRetriever.NextClosest(shipHost.Actions[0].TargetableTypes, this.host.Current.Position.Linear);
                                timepass = 0;
                            }
                        }
                        if (shipHost.Actions.Count > 1 && 
                            shipHost.Actions[1].TargetableTypes != TargetingInfo.None &&
                            shipHost.Actions[1].TargetableTypes != TargetingInfo.Self)
                        {
                            if (keys[configInfo.SecondaryAction])
                            {
                                this.host.Target = this.host.TargetRetriever.NextClosest(shipHost.Actions[1].TargetableTypes, this.host.Current.Position.Linear);
                                timepass = 0;
                            }
                            if (keys[configInfo.RotateLeft])
                            {
                                this.host.Target = this.host.TargetRetriever.Next(shipHost.Actions[1].TargetableTypes);
                                timepass = 0;
                            }
                        }
                        if (keys[configInfo.MoveForward])
                        {
                            foreach (IAction action in shipHost.Actions)
                            {
                                if (action.TargetableTypes != TargetingInfo.None &&
                                    action.TargetableTypes != TargetingInfo.Self)
                                {
                                    this.host.Target = this.host.TargetRetriever.NextClosest(action.TargetableTypes, this.host.Current.Position.Linear);
                                    timepass = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (keys[configInfo.MoveForward])
                    {
                        original.ThrustPercent = 1;
                        original[InputAction.MoveForward] = true;
                    }
                    if (keys[configInfo.RotateRight])
                    {
                        original.TorquePercent = 1;
                        original[InputAction.RotateRight] = true;
                    }
                    if (keys[configInfo.RotateLeft])
                    {
                        original.TorquePercent = 1;
                        original[InputAction.RotateLeft] = true;
                    }
                    if (keys[configInfo.PrimaryAction] && keys[configInfo.SecondaryAction] && original.ActiveActions.Length > 2)
                    {
                        original[InputAction.Action] = true;
                        original.ActiveActions[2] = true;
                    }
                    else
                    {
                        if (keys[configInfo.PrimaryAction])
                        {
                            original[InputAction.Action] = true;
                            if (original.ActiveActions.Length > 0)
                            {
                                original.ActiveActions[0] = true;
                            }
                        }
                        if (keys[configInfo.SecondaryAction])
                        {
                            original[InputAction.Action] = true;
                            if (original.ActiveActions.Length > 1)
                            {
                                original.ActiveActions[1] = true;
                            }
                        }
                    }
                }
            }
            return original;
        }
        public override object Clone()
        {
            return new PlayerControler(this);
        }
    }                 
}