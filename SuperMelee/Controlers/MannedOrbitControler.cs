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
using Physics2D;
using System.Collections;
using System.Collections.Generic;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using Color = System.Drawing.Color;
namespace ReMasters.SuperMelee.Controlers
{
    [Serializable]
    public class MannedOrbitControler : OrbitControler
    {
        public MannedOrbitControler(TargetingInfo targetingInfo, float orbitDistance)
            : base(targetingInfo, orbitDistance)
        {}
        protected MannedOrbitControler(MannedOrbitControler copy) : base(copy) { }
        protected override bool CheckTarget()
        {
            if (host.LifeTime.TimeLeft < host.LifeTime.Age || base.CheckTarget())
            {
                this.IsExpired = true;
                if (host is IWeapon)
                {
                    this.host.Target = ((IWeapon)host).WeaponInfo.Source;
                    this.target = this.host.Target;
                }
                else
                {
                    this.host.Target = null;
                    this.target = this.host.Target;
                }
                host.AddControler(null, new MissileControler(TargetingInfo.FromRequireAll(TargetingTypes.Ally | TargetingTypes.Ship)));
                return true;
            }
            return false;
        }
        public override object Clone()
        {
            return new MannedOrbitControler(this);
        }
    }
}