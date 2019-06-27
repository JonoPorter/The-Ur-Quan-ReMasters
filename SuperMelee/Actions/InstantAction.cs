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
using System.Collections;
using System.Collections.Generic;
using Physics2D;
using Physics2D.CollisionDetection;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee.Actions
{
    [Serializable]
    public class InstantAction : BaseAction
    {
        protected IWeapon weapon;
       
        public InstantAction(
            Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            ActionSounds actionSounds,
            IWeapon weapon)
            : base(delay, targetableTypes, costs, true, actionSounds)
        {
            this.weapon = weapon;
        }
        protected InstantAction(InstantAction copy)
            : base(copy)
        {
            this.weapon = (IWeapon)copy.weapon.Clone();
        }
        bool OnAny(ActionResult actionResult, float dt)
        {
            IWeapon newWeapon = (IWeapon)weapon.Clone();
            newWeapon.OnCreation(actionResult, source, this);
            newWeapon.OnCollision(actionResult, target);
            return true;
        }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            return OnAny(actionResult, dt);
        }
        protected override bool OnRunning(ActionResult actionResult, float dt)
        {
            return OnAny(actionResult, dt);
        }
        protected override bool OnDeActivated(ActionResult actionResult, float dt)
        {
            return OnAny(actionResult, dt);
        }
        public override object Clone()
        {
            return new InstantAction(this);
        }

    }
}