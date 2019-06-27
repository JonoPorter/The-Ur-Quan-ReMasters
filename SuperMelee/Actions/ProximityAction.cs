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
    public abstract class BaseProximityAction : BaseAction
    {
        protected float radius;
        protected int maxNumberofTargets;
        protected List<IControlable> closeones = new List<IControlable>();
        public BaseProximityAction(Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            ActionSounds actionSounds,
            float radius,
            int maxNumberofTargets)
            : base(delay, targetableTypes, costs, false, actionSounds)
        {
            this.radius = radius;
            this.maxNumberofTargets = maxNumberofTargets;
            this.aIInfo = new ProximityActionAIInfo(costs.ActivationCost);
        }
        protected BaseProximityAction(BaseProximityAction copy)
            : base(copy)
        {
            this.radius = copy.radius;
            this.maxNumberofTargets = copy.maxNumberofTargets;
        }
        public float Radius
        {
            get
            {
                return radius;
            }
        }
        public override void OnSourceCreation(GameResult gameResult, IShip source)
        {
            base.OnSourceCreation(gameResult,source);
            gameResult.AddCollidableArea(new CollidableBubble(radius, source, closeones));
        }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            int count = closeones.Count;
            if (count > 0)
            {
                int targetCount = 0;
                for (int pos = 0; pos < count && targetCount < maxNumberofTargets; ++pos)
                {
                    if (targetableTypes.MeetsRequirements(FactionInfo.GetTargetingType( source,closeones[pos])))
                    {
                        if (OnTargetAquired(actionResult, dt, closeones[pos]))
                        {
                            targetCount++;
                        }
                    }
                }
                if (targetCount > 0)
                {
                    return true;
                }
            }
            return OnNoTarget(actionResult, dt);
        }
        protected virtual bool OnNoTarget(GameResult gameResult, float dt)
        {
            return false;
        }
        protected abstract bool OnTargetAquired(GameResult gameResult, float dt, IControlable newTarget);
    }


    [Serializable]
    public class ProximityInstantAction : BaseProximityAction
    {
        IWeapon weapon;
        public ProximityInstantAction(Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            ActionSounds actionSounds,
            float radius,
            int maxNumberofTargets,
            IWeapon weapon)
            : base(delay, targetableTypes, costs, actionSounds,radius, maxNumberofTargets)
        {
            this.weapon = weapon;
        }
        protected ProximityInstantAction(ProximityInstantAction copy)
            : base(copy)
        {
            this.weapon = (IWeapon)copy.weapon.Clone();
        }
        protected override bool OnTargetAquired(GameResult gameResult, float dt, IControlable newTarget)
        {
            IWeapon newWeapon = (IWeapon)weapon.Clone();
            newWeapon.OnCreation(gameResult, source, this);
            newWeapon.WeaponInfo.Target = newTarget;
            newWeapon.OnCollision(gameResult, newTarget);
            return true;
        }
        public override object Clone()
        {
            return new ProximityInstantAction(this);
        }
    }
}
