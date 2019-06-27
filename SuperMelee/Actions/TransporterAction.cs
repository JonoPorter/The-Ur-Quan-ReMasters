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
    public class TransporterAction : BaseAction
    {
        ISolidWeapon weapon;
        public TransporterAction(
            Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            bool needsTarget,
            ActionSounds actionSounds,
            ISolidWeapon weapon)
            : base(delay, targetableTypes, costs, needsTarget, actionSounds)
        {
            this.weapon = weapon;
        }
        protected TransporterAction(TransporterAction copy)
            : base(copy)
        {
            this.weapon = (ISolidWeapon)copy.weapon.Clone();
        }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            ISolidWeapon newweapon = (ISolidWeapon)weapon.Clone();
            if (target == null || !needsTarget)
            {
                newweapon.Current.Set(source.Current);
            }
            else
            {
                newweapon.Current.Set(target.Current);
            }
            newweapon.SetAllPositions();
            newweapon.OnCreation(actionResult,source, this);
            return true;
        }
        public override object Clone()
        {
            return new TransporterAction(this);
        }
    }
}