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
using Physics2D;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee.Actions
{
    [Serializable]
    public class NullAction : IAction
    {
        IActionAIInfo aIInfo = null;
        public TargetingInfo TargetableTypes
        {
            get { return TargetingInfo.None; }
        }
        public Bounded<float> Delay
        {
            get { return null; }
        }
        public Costs Costs
        {
            get { return null; }
            set { }
        }
        public bool Ready
        {
            get { return false; }
        }
        public bool NeedsTarget
        {
            get { return false; }
        }
        public bool IsActive
        {
            get { return false; }
        }
        public ActionSounds ActionSounds
        {
            get { return null; }
            set { }
        }
        public void Update(float dt)
        { }
        public IControlable Target
        {
            get { return null; }
            set { }
        }
        public IShip Source
        {
            get { return null; }
        }
        public object Clone()
        {
            return this;
        }
        public IActionAIInfo AIInfo
        {
            get { return aIInfo; }
            set { aIInfo = value; }
        }
        public void OnAction(GameResult gameResult, float dt) { }
        public void OnAfterAction(GameResult gameResult, float dt) { }
        public void OnSourceCreation(GameResult gameResult, IShip source) { }
    }
}