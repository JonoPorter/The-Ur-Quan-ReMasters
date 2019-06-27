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
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Actions
{
    [Serializable]
    public class RayPointDefence : BaseProximityAction
    {
        protected TargetedRayWeapon weapon;
        public RayPointDefence(Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            ActionSounds actionSounds,
            float radius,
            int maxNumberofTargets,
            TargetedRayWeapon weapon)
            : base(delay, targetableTypes, costs, actionSounds, radius, maxNumberofTargets)
        {
            this.weapon = weapon;
        }
        protected RayPointDefence(RayPointDefence copy)
            : base(copy)
        {
            this.weapon = (TargetedRayWeapon)copy.weapon.Clone();
        }
        protected override bool OnTargetAquired(GameResult gameResult, float dt, IControlable newTarget)
        {
            TargetedRayWeapon newWeapon = (TargetedRayWeapon)weapon.Clone();
            newWeapon.OnCreation(gameResult, source, this);
            newWeapon.WeaponInfo.Target = newTarget;
            return true;
        }
        public override object Clone()
        {
            return new RayPointDefence(this);
        }
    }

    [Serializable]
    public class GunPointDefence : BaseProximityAction
    {
        protected ISolidWeapon weapon;
        public GunPointDefence(Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            ActionSounds actionSounds,
            float radius,
            int maxNumberofTargets,
            ISolidWeapon weapon)
            : base(delay, targetableTypes, costs, actionSounds, radius, maxNumberofTargets)
        {
            this.weapon = weapon;
        }
        protected GunPointDefence(GunPointDefence copy)
            : base(copy)
        {
            this.weapon = (ISolidWeapon)copy.weapon.Clone();
        }
        protected override bool OnTargetAquired(GameResult gameResult, float dt, IControlable newTarget)
        {

            Vector2D sourcePosition = this.source.Current.Position.Linear;// +this.source.Current.Velocity.Linear * dt;
            Vector2D targetPosition = newTarget.Current.Position.Linear;// +newTarget.Current.Velocity.Linear * dt;
            Vector2D relativeVelocity = newTarget.Current.Velocity.Linear - this.source.Current.Velocity.Linear;
            float time;
            if (Logic.TrySolveTimeToWaveIntercept(
                sourcePosition,
                targetPosition,
                relativeVelocity,
                weapon.MovementInfo.MaxLinearVelocity.Value, out time))
            {
                if (time > 0 && time < weapon.LifeTime.TimeLeft && time < newTarget.LifeTime.TimeLeft)
                {
                    Vector2D point = targetPosition + relativeVelocity * time;
                    float angle = (point - sourcePosition).Angle - this.source.Current.Position.Angular;

                    SolidWeaponHardPoint hardpoint = new SolidWeaponHardPoint(0, weapon.MovementInfo.MaxLinearVelocity, angle);

                    ISolidWeapon newWeapon = hardpoint.FireGun((ISolidWeapon)weapon.Clone(), this.source);

                    newWeapon.IgnoreInfo.JoinGroupToIgnore(source.FactionInfo.FactionNumber);
                    newWeapon.OnCreation(gameResult, source, this);
                    return true;
                }
            }
            return false;
        }
        public override void OnSourceCreation(GameResult gameResult, IShip source)
        {
            source.IgnoreInfo.AddGroupToIgnore(source.FactionInfo.FactionNumber);
            base.OnSourceCreation(gameResult, source);
        }
        public override object Clone()
        {
            return new GunPointDefence(this);
        }
    }
}
