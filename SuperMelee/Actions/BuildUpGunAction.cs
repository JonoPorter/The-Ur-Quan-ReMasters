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
using Physics2D.Joints;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee.Actions
{
    [Serializable]
    public class BuildUpGunAction : BaseAction
    {
        float velocityAngle;
        float offsetAngle;
        ISolidWeapon[] weapons;
        Bounded<float> buildUpDelay;
        ISolidWeapon currentWeapon;
        IJoint currentJoint;
        float maxBoundingRadius;
        int stage;
        int stageCount;
        public BuildUpGunAction(
            Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            bool needsTarget,
            ActionSounds actionSounds,
            Bounded<float> buildUpDelay,
            float velocityAngle,
            float offsetAngle,
            ISolidWeapon[] weapons)
            : base(delay, targetableTypes, costs, needsTarget, actionSounds)
        {
            this.buildUpDelay = buildUpDelay;
            this.velocityAngle = velocityAngle;
            this.offsetAngle = offsetAngle;
            this.weapons = weapons;
            this.maxBoundingRadius = 0;
            this.stage = 0;
            this.stageCount = weapons.Length;
            foreach (ISolidWeapon weapon in weapons)
            {
                maxBoundingRadius = MathHelper.Max(maxBoundingRadius, weapon.BoundingRadius);
            }
            this.aIInfo = new BuildUpGunActionAIInfo(null);
        }
        public ISolidWeapon CurrentWeapon
        {
            get
            {
                return currentWeapon;
            }
        }
        public ISolidWeapon Weapon
        {
            get
            {
                ISolidWeapon rv = currentWeapon;
                if (rv == null)
                {
                    rv = weapons[0];
                }
                return rv;
            }
        }
        public float VelocityAngle
        {
            get
            {
                return velocityAngle;
            }
        }


        protected BuildUpGunAction(BuildUpGunAction copy)
            : base(copy)
        {
            this.maxBoundingRadius = copy.maxBoundingRadius;
            this.velocityAngle = copy.velocityAngle;
            this.offsetAngle = copy.offsetAngle;
            this.stageCount = copy.stageCount;
            this.stage = copy.stage;
            this.buildUpDelay = new Bounded<float>(copy.buildUpDelay);
            this.weapons = new ISolidWeapon[copy.stageCount];
            for (int pos = 0; pos < stageCount; pos++)
            {
                this.weapons[pos] = (ISolidWeapon)copy.weapons[pos].Clone();
            }
        }
        protected virtual void ReleaseWeapon()
        {
            currentJoint.IsExpired = true;
            float distance = source.BoundingRadius + maxBoundingRadius + 5;
            Vector2D direction = Vector2D.FromLengthAndAngle(1, source.Current.Position.Angular + offsetAngle);
            Vector2D Velocity = Vector2D.FromLengthAndAngle(1, source.Current.Position.Angular + velocityAngle) * weapons[stage].MovementInfo.MaxLinearVelocity.Value;
            currentWeapon.Current.Velocity.Linear += Velocity;
            source.ApplyImpulse(distance * direction, Velocity * (-weapons[stage].MassInfo.Mass));
            this.currentWeapon = null;
            this.currentJoint = null;
        }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            buildUpDelay.Empty();
            buildUpDelay.Value+=dt;
            stage = 0;
            SetStage(actionResult);
            return true;
        }
        protected override bool OnRunning(ActionResult actionResult, float dt)
        {
            if (currentWeapon.IsExpired)
            {
                this.activated = false;
                this.currentWeapon = null;
                this.currentJoint = null;
                return false;
            }
            buildUpDelay.Value += dt;
            currentWeapon.LifeTime = (LifeSpan)weapons[stage].LifeTime.Clone();
            if (buildUpDelay.IsFull)
            {
                buildUpDelay.Empty();
                stage++;
                if (stage < stageCount)
                {
                    currentWeapon.IsExpired = true;
                    currentJoint.IsExpired = true;
                    SetStage(actionResult);
                }
                else
                {
                    stage--;
                }
            }
            return true;
        }
        protected override bool OnDeActivated(ActionResult actionResult, float dt)
        {
            if (!currentWeapon.IsExpired)
            {
                currentWeapon.LifeTime = (LifeSpan)weapons[stage].LifeTime.Clone();
                ReleaseWeapon();
            }
            return true;
        }
        protected virtual void SetStage(GameResult gameResult)
        {
            Vector2D direction = Vector2D.FromLengthAndAngle(1, source.Current.Position.Angular + offsetAngle);
            PhysicsState physicsState = new PhysicsState(source.Current);
            float distance = source.BoundingRadius + maxBoundingRadius + 5;
            physicsState.Position.Linear += direction * distance;
            physicsState.Position.Angular += velocityAngle;
            physicsState.Velocity.Angular = weapons[stage].MovementInfo.MaxAngularVelocity.Value;
            currentWeapon = (ISolidWeapon)weapons[stage].Clone();
            currentWeapon.Current.Set(physicsState);
            currentWeapon.SetAllPositions();

            currentWeapon.OnCreation(gameResult,source, this);
            currentJoint = new PinJoint(new CollidablePair(currentWeapon, source), currentWeapon.Current.Position.Linear, 0, .1f);
            gameResult.AddJoint(currentJoint);
        }

        public override object Clone()
        {
            return new BuildUpGunAction(this);
        }
    }
}