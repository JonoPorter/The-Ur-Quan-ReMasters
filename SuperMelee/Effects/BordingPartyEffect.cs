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
using System.Text;
using Physics2D;
using AdvanceMath; using AdvanceSystem;
using AdvanceMath.Geometry2D;
namespace ReMasters.SuperMelee.Effects
{
    [Serializable]
    public class BordingPartyEffect : BaseProlongedEffect
    {
        static Random rand = new Random();
        protected ISolidWeapon returnWeapon;
        protected Bounded<float> damageDelay;
        protected float damageAmount;
        public BordingPartyEffect(TargetingInfo effectsWho, EffectTypes harmfulEffectTypes, EffectSounds effectSounds, LifeSpan lifeTime, Bounded<float> damageDelay, float damageAmount)
            : base(effectsWho, harmfulEffectTypes | EffectTypes.Health,effectSounds, lifeTime)
        {
            this.damageDelay = damageDelay;
            this.damageAmount = damageAmount;
        }
        protected BordingPartyEffect(BordingPartyEffect copy)
            : base(copy)
        {
            this.damageDelay = new Bounded<float>(copy.damageDelay);
            this.damageAmount = copy.damageAmount;
        }
        public override void ApplyEffect(GameResult gameResult, float dt)
        {
            damageDelay.Value += dt;
            if(damageDelay.IsFull)
            {
                damageDelay.Empty();
                float number = rand.Next(0, 256);
                if (16 > number)
                {
                    this.IsExpired = true;
                    Exhausted = true;
                }
                else if (144 > number)
                {
                    effectSounds.Applied.Play();
                    attachie.ShipState.Health.Value -= damageAmount;
                }
            }
        }
        public override void OnTargetAttachment(EffectAttachmentResult attachmentResult, IControlable attachie)
        {
            base.OnTargetAttachment(attachmentResult, attachie);
            attachmentResult.WeaponIsExpired = true;
            this.isTargetAttached = true;
        }
        public override void OnTargetDeath(GameResult gameResult)
        {
            IControlable host = Host;
            float departureAngle = (float)(rand.NextDouble() * MathHelper.PI * 2);
            Vector2D direction = Vector2D.FromLengthAndAngle(1, attachie.Current.Position.Angular + departureAngle);
            Vector2D Velocity = Vector2D.FromLengthAndAngle(1, attachie.Current.Position.Angular + departureAngle) * returnWeapon.MovementInfo.MaxLinearVelocity.Value;
            PhysicsState physicsState = new PhysicsState(attachie.Current);
            float distance = host.BoundingRadius + returnWeapon.BoundingRadius + 2;
            physicsState.Position.Linear += direction * distance;
            physicsState.Position.Angular += departureAngle;
            physicsState.Velocity.Linear += Velocity;
            physicsState.Velocity.Angular = returnWeapon.MovementInfo.MaxAngularVelocity.Value;
            attachie.ApplyImpulse(distance * direction, Velocity * (-returnWeapon.MassInfo.Mass));
            ISolidWeapon newWeapon = (ISolidWeapon)returnWeapon.Clone();
            newWeapon.Current.Set(physicsState);
            newWeapon.SetAllPositions();
            newWeapon.OnCreation(gameResult,weaponInfo.Source, weaponInfo.ActionSource);
            newWeapon.Target = host;
        }
        public override object Clone()
        {
            return new BordingPartyEffect(this);
        }
        public override void OnExpired()
        { }
        public override void OnCreation(IWeaponsLogic weaponInfo)
        {
           base.OnCreation(weaponInfo);
           this.returnWeapon = (ISolidWeapon)weaponInfo.SolidHost.Clone();
        }
    }
}