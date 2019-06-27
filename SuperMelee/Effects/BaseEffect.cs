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

namespace ReMasters.SuperMelee.Effects
{
    [Serializable]
    public abstract class BaseEffect : IEffect
    {
        private bool exhausted;
        protected bool isTargetAttached;
        protected bool isHarmful = true;
        protected IControlable attachie;
        protected EffectTypes harmfulEffectTypes;
        protected TargetingInfo effectsWho;
        protected IWeaponsLogic weaponInfo;
        protected EffectSounds effectSounds;
        public BaseEffect(TargetingInfo effectsWho, EffectTypes harmfulEffectTypes, EffectSounds effectSounds)
        {
            this.effectsWho = effectsWho;
            this.harmfulEffectTypes = harmfulEffectTypes;
            this.effectSounds = effectSounds;
        }
        protected BaseEffect(BaseEffect copy)
        {
            this.exhausted = copy.exhausted;
            this.harmfulEffectTypes = copy.harmfulEffectTypes;
            this.weaponInfo = copy.weaponInfo;
            this.attachie = copy.attachie;
            this.effectsWho = copy.effectsWho;
            this.effectSounds = copy.effectSounds;
            this.isHarmful = copy.isHarmful;
        }
        public EffectSounds EffectSounds
        {
            get { return effectSounds; }
            set { effectSounds = value; }
        }
        public IControlable Host
        {
            get
            {
                if (weaponInfo != null)
                {
                    return weaponInfo.LastBody;
                }
                return null;
            }
        }
        public bool IsHarmful
        {
            get { return isHarmful; }
        }
        public bool IsTargetAttached
        {
            get { return isTargetAttached; }
        }
        public virtual IWeaponsLogic WeaponInfo
        {
            get { return weaponInfo; }
        }
        public virtual EffectTypes HarmfulEffectTypes
        {
            get { return harmfulEffectTypes; }
        }
        public virtual GeneralChange RemoveEffectTypes(EffectTypes types)
        {
            return null;
        }
        public virtual void OnCreation(IWeaponsLogic weaponInfo)
        {
            this.weaponInfo = weaponInfo;
        }
        public virtual bool Exhausted
        {
            get { return exhausted; }
            set
            {
                if (value && !exhausted)
                {
                    effectSounds.Exhausted.Play();
                }
                exhausted = value;
            }
        }
        public TargetingInfo EffectsWho
        {
            get { return effectsWho; }
        }
        public bool CanEffect(IControlable controlable)
        {
            return (!exhausted) && (effectsWho.IsAll ||
                effectsWho.MeetsRequirements(FactionInfo.GetTargetingType(Host, controlable)));
        }
        public virtual void Update(float dt)
        {

        }
        public abstract object Clone();

        public virtual void OnTargetAttachment(EffectAttachmentResult attachmentResult, IControlable attachie)
        {
            effectSounds.Attached.Play();
            this.attachie = attachie;
            this.isTargetAttached = true;
        }
        public abstract void ApplyEffect(GameResult gameResult, float dt);

    }
}
