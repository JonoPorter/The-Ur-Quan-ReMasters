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
namespace ReMasters.SuperMelee
{
    [Serializable]
    public class WeaponsLogic : IWeaponsLogic
    {
        protected EffectCollection effectCollection;
        protected IAction actionSource;
        protected IShip source;
        protected IWeaponsLogic parentWeapon;
        protected IControlable target;
        protected FactionInfo factionInfo;
        protected TargetingInfo effectsWho;
        protected IWeapon host;
        protected ISolidWeapon solidHost;
        public WeaponsLogic(
            TargetingInfo effectsWho,
            EffectCollection effectCollection)
        {
            this.effectsWho = effectsWho;
            this.effectCollection = effectCollection;
        }
        public WeaponsLogic(WeaponsLogic copy)
        {
            this.effectCollection = new EffectCollection(copy.effectCollection);
            this.actionSource = copy.actionSource;
            this.source = copy.source;
            this.parentWeapon = copy.parentWeapon;
            this.target = copy.target;
            this.factionInfo = copy.factionInfo;
            this.effectsWho = copy.effectsWho;
            this.host = copy.host;
            this.solidHost = copy.solidHost;
        }
        public EffectCollection EffectCollection
        {
            get { return effectCollection; }
        }
        public IAction ActionSource
        {
            get { return actionSource; }
        }
        public IWeapon Host
        {
            get { return host; }
        }
        public ISolidWeapon SolidHost
        {
            get { return solidHost; }
        }
        public IShip Source
        {
            get { return source; }
        }
        public FactionInfo FactionInfo
        {
            get
            {
                return factionInfo;
            }
            set
            {
                factionInfo = value;
            }
        }
        public IWeaponsLogic ParentWeapon
        {
            get { return parentWeapon; }
        }
        public IControlable LastBody
        {
            get
            {
                if (solidHost != null)
                {
                    return solidHost;
                }
                if (parentWeapon != null)
                {
                    IControlable lastBody = parentWeapon.LastBody;
                    if (lastBody != null)
                    {
                        return lastBody;
                    }
                }
                return source;

            }
        }
        public IControlable Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
        public virtual void OnCollision(GameResult gameResult, IControlable collider)
        {
            if (CanEffect(collider))
            {
                EffectAttachmentResult attachmentResult = new EffectAttachmentResult(gameResult);
                collider.AttachEffects(attachmentResult, effectCollection);
                if (attachmentResult.WeaponIsExpired && solidHost != null)
                {
                    solidHost.Kill(gameResult);
                }
            }
        }
        public virtual void OnCreation(GameResult gameResult, IWeapon host, IShip source, IAction actionSource)
        {
            this.host = host;
            this.source = source;
            this.actionSource = actionSource;
            this.target = actionSource.Target;
            this.parentWeapon = null;
            if (source != null)
            {
                this.factionInfo = new FactionInfo(source.FactionInfo);
            }
            else
            {
                this.factionInfo = new FactionInfo(solidHost.FactionInfo);
            }
            this.effectCollection.OnCreation(source, this);
            this.effectsWho = TargetingInfo.Merge(effectsWho, effectCollection.EffectsWho);
        }
        public virtual void OnCreation(GameResult gameResult, ISolidWeapon solidHost, IShip source, IAction actionSource)
        {
            this.solidHost = solidHost;
            this.solidHost.Target = actionSource.Target;
            OnCreation(gameResult, (IWeapon)solidHost, source, actionSource);
        }
        public virtual void OnCreation(GameResult gameResult, IWeapon host, IWeaponsLogic parentWeapon)
        {
            this.host = host;
            this.parentWeapon = parentWeapon;
            this.source = parentWeapon.Source;
            this.actionSource = parentWeapon.ActionSource;
            this.target = parentWeapon.ActionSource.Target;
            this.factionInfo = new FactionInfo(source.FactionInfo);
            this.effectCollection.OnCreation(source, this);
            //this.effectsWho = TargetingInfo.Merge(effectsWho, effectCollection.EffectsWho);
        }
        public virtual void OnCreation(GameResult gameResult, ISolidWeapon solidHost, IWeaponsLogic parentWeapon)
        {
            this.solidHost = solidHost;
            this.solidHost.Target = parentWeapon.Target;
            OnCreation(gameResult, (IWeapon)solidHost, parentWeapon);
        }
        public TargetingInfo EffectsWho
        {
            get { return effectsWho; }
        }
        public bool CanEffect(IControlable controlable)
        {
            return effectsWho.IsAll ||
                effectsWho.MeetsRequirements(FactionInfo.GetTargetingType(source, controlable));
        }
        public virtual void Update(float dt)
        {
            effectCollection.Update(dt);
        }
        public virtual object Clone()
        {
            return new WeaponsLogic(this);
        }

        public bool IsThreatTo(IControlable other)
        {

            if (solidHost.IgnoreInfo.CanCollideWith(other.IgnoreInfo) &&
                CanEffect(other) &&
                effectCollection.IsHarmful &&
                (other.Current.Velocity.Linear - solidHost.Current.Velocity.Linear) * (other.Current.Position.Linear - solidHost.Current.Position.Linear) < 0)
            {
                float time;
                if (Logic.TrySolveTimeToWaveIntercept(
                     other.Current.Position.Linear,
                    solidHost.Current.Position.Linear,
                    solidHost.Current.Velocity.Linear - other.Current.Velocity.Linear,
                    other.MovementInfo.MaxLinearVelocity,
                    out time))
                {
                    return ((solidHost.Current.Velocity.Linear * time + solidHost.Current.Position.Linear) -
                        (other.Current.Velocity.Linear * time + other.Current.Position.Linear)).Magnitude
                        < ((other.BoundingRadius + solidHost.BoundingRadius) * 3);
                }
            }



            return false;
        }
    }
    [Serializable]
    public class MultipleHitWeaponsLogic : WeaponsLogic
    {
        protected bool didCollision;
        protected Bounded<float> damageDelay;
        public MultipleHitWeaponsLogic(
            TargetingInfo effectsWho,
            EffectCollection effectCollection,
            Bounded<float> damageDelay):base(effectsWho,effectCollection)
        {
            this.damageDelay = damageDelay;
            this.didCollision = false;
        }
        public MultipleHitWeaponsLogic(MultipleHitWeaponsLogic copy)
            : base(copy)
        {
            this.damageDelay = new Bounded<float>(copy.damageDelay);
            this.didCollision = false;
        }
        public override void OnCollision(GameResult gameResult, IControlable collider)
        {
            if (damageDelay.IsFull)
            {
                didCollision = true;
                base.OnCollision(gameResult, collider);
            }
        }
        public override void Update(float dt)
        {
            if (didCollision)
            {
                didCollision = false;
                damageDelay.Empty();
            }
            damageDelay.Value += dt;
            base.Update(dt);
        }
        public override object Clone()
        {
            return new MultipleHitWeaponsLogic(this);
        }
    }
}
