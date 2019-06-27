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
namespace ReMasters.SuperMelee
{
    [Flags, Serializable]
    public enum EffectAttachmentFlags : int
    {
        None = 0,
        WeaponExpires = 1 << 0,
        ClonedAttachment = 1 << 1,
    }

    [Serializable]
    public sealed class EffectCollection :  ICleanable, IEffecter
    {
        #region feilds
        IWeaponsLogic weaponInfo;
        TargetingInfo effectsWho = TargetingInfo.None;
        EffectAttachmentFlags attachmentFlags = EffectAttachmentFlags.None;
        List<IEffect> effects;
        List<IProlongedEffect> prolongedEffects;
        #endregion
        #region constructors
        public EffectCollection()
        {
            this.effects = new List<IEffect>();
            this.prolongedEffects = new List<IProlongedEffect>();
        }
        public EffectCollection(params IEffect[] effects)
        {
            this.effects = new List<IEffect>();
            this.prolongedEffects = new List<IProlongedEffect>();
            foreach (IEffect effect in effects)
            {
                if (effect is IProlongedEffect)
                {
                    this.prolongedEffects.Add((IProlongedEffect)effect);
                }
                else 
                {
                    this.effects.Add(effect);
                }
            }
        }

        public EffectCollection(EffectCollection copy)
        {
            this.effects = new List<IEffect>(copy.effects.Count);
            this.prolongedEffects = new List<IProlongedEffect>(copy.prolongedEffects.Count);
            this.effectsWho = copy.effectsWho;
            this.weaponInfo = copy.weaponInfo;
            this.attachmentFlags = copy.attachmentFlags;
            foreach (IEffect effect in copy.effects)
            {
                this.effects.Add((IEffect)effect.Clone());
            }
            foreach (IProlongedEffect prolongedEffect in copy.prolongedEffects)
            {
                this.prolongedEffects.Add((IProlongedEffect)prolongedEffect.Clone());
            }
        }

        #endregion
        #region properties
        public EffectAttachmentFlags AttachmentFlags
        {
            get { return attachmentFlags; }
            set { attachmentFlags = value; }
        }
        public IShip Source
        {
            get
            {
                if (weaponInfo != null)
                {
                    return weaponInfo.Source;
                }
                return null;
            }

        }
        public List<IEffect> Effects
        {
            get
            {
                return effects;
            }
        }
        public List<IProlongedEffect> ProlongedEffects
        {
            get
            {
                return prolongedEffects;
            }
        }
        public bool AllExhausted
        {
            get
            {
                bool empty = effects.Count > 0 || prolongedEffects.Count > 0;
                if (empty)
                {
                    foreach (IEffect effect in effects)
                    {
                        if (!(empty = (empty && effect.Exhausted)))
                        {
                            break;
                        }
                    }
                    foreach (IProlongedEffect prolongedEffect in prolongedEffects)
                    {
                        if (!(empty = (empty && prolongedEffect.Exhausted)))
                        {
                            break;
                        }
                    }
                }
                return empty;
            }

        }
        public TargetingInfo EffectsWho
        {
            get { return effectsWho; }
        }
        public bool IsHarmful
        {
            get
            {
                foreach (IEffect effect in effects)
                {
                    if (effect.IsHarmful)
                    {
                        return true;
                    }
                }
                foreach (IProlongedEffect prolongedEffect in prolongedEffects)
                {
                    if (prolongedEffect.IsHarmful)
                    {
                        return true;
                    }
                }
                return false;
            }

        }
        #endregion
        #region methods
        public void UpdateEffects(GameResult gameResult, float dt)
        {
            foreach (IEffect effect in effects)
            {
                effect.ApplyEffect(gameResult, dt);
            }
            foreach (IProlongedEffect prolongedEffect in prolongedEffects)
            {
                prolongedEffect.ApplyEffect(gameResult, dt);
            }
        }
        bool BlockEffect(IEffect effect)
        {
            foreach (IProlongedEffect prolongedEffect in prolongedEffects)
            {
                if (prolongedEffect.BlockEffect(effect))
                {
                    return true;
                }
            }
            return false;
        }
        public void AttachEffects(EffectAttachmentResult attachmentResult, IControlable attachie, EffectCollection effectCollection)
        {
            if (effectCollection.CanEffect(attachie))
            {

                attachmentResult.WeaponIsExpired = (effectCollection.attachmentFlags & EffectAttachmentFlags.WeaponExpires) == EffectAttachmentFlags.WeaponExpires;
                if ((effectCollection.attachmentFlags & EffectAttachmentFlags.ClonedAttachment) == EffectAttachmentFlags.ClonedAttachment)
                {
                    foreach (IEffect effect in effectCollection.effects)
                    {
                        if (effect.CanEffect(attachie) && (!BlockEffect(effect)))
                        {
                            IEffect clone = (IEffect)effect.Clone();
                            clone.OnTargetAttachment(attachmentResult,attachie);
                            effects.Add(clone);
                            attachmentResult.EffectAttached = true;
                        }
                    }
                    foreach (IProlongedEffect prolongedEffect in effectCollection.prolongedEffects)
                    {
                        if (prolongedEffect.CanEffect(attachie) && (!BlockEffect(prolongedEffect)))
                        {
                            IProlongedEffect clone = (IProlongedEffect)prolongedEffect.Clone();
                            clone.OnTargetAttachment(attachmentResult,attachie);
                            prolongedEffects.Add(clone);
                            attachmentResult.EffectAttached = true;
                        }
                    }
                }
                else
                {
                    foreach (IEffect effect in effectCollection.effects)
                    {
                        if (effect.CanEffect(attachie) && (!BlockEffect(effect)))
                        {
                            effect.OnTargetAttachment(attachmentResult, attachie);
                            effects.Add(effect);
                            attachmentResult.EffectAttached = true;
                        }
                    }
                    foreach (IProlongedEffect prolongedEffect in effectCollection.prolongedEffects)
                    {
                        if (prolongedEffect.CanEffect(attachie) && (!BlockEffect(prolongedEffect)))
                        {
                            prolongedEffect.OnTargetAttachment(attachmentResult,attachie);
                            prolongedEffects.Add(prolongedEffect);
                            attachmentResult.EffectAttached = true;
                        }
                    }
                }

            }
        }
        public void OnCreation(IShip source, IWeaponsLogic weaponInfo)
        {
            this.weaponInfo = weaponInfo;
            foreach (IEffect effect in effects)
            {
                effect.OnCreation(weaponInfo);
                effectsWho = TargetingInfo.Merge(effectsWho,effect.EffectsWho);
            }
            foreach (IProlongedEffect prolongedEffect in prolongedEffects)
            {
                prolongedEffect.OnCreation(weaponInfo);
                effectsWho = TargetingInfo.Merge(effectsWho, prolongedEffect.EffectsWho);
            }
        }
        public void OnCreation(GameResult gameResult, IControlable host)
        {
            EffectAttachmentResult attachmentResult = new EffectAttachmentResult(gameResult);
            foreach (IEffect effect in effects)
            {
                effect.OnTargetAttachment(attachmentResult, host);
            }
            foreach (IProlongedEffect prolongedEffect in prolongedEffects)
            {
                prolongedEffect.OnTargetAttachment(attachmentResult, host);
            }
        }
        public void FilterEffects(float dt)
        {
            foreach (IProlongedEffect prolongedEffect in prolongedEffects)
            {
                prolongedEffect.FilterEffects(dt, effects);
            }
        }
        public void RemoveAttributeChanges()
        {
            foreach (IProlongedEffect prolongedEffect in prolongedEffects)
            {
                prolongedEffect.RemoveAttributeChanges();
            }
        }
        public void ApplyAttributeChanges()
        {
            foreach (IProlongedEffect prolongedEffect in prolongedEffects)
            {
                prolongedEffect.ApplyAttributeChanges();
            }
        }
        public void OnTargetDeath(GameResult gameResult)
        {
            foreach (IProlongedEffect prolongedEffect in prolongedEffects)
            {
                prolongedEffect.OnTargetDeath(gameResult);
            }
        }
        public bool RemoveExpired()
        {
            effects.Clear();
            prolongedEffects.RemoveAll(delegate(IProlongedEffect item)
            {
                if (item.IsExpired)
                {
                    item.OnExpired();
                    return true;
                }
                return false;
            });
            return false;
        }
        public bool CanEffect(IControlable controlable)
        {
            return effectsWho.IsAll ||
                effectsWho.MeetsRequirements(FactionInfo.GetTargetingType(Source, controlable));
        }
        public void AttachedUpdate(float dt)
        {
            foreach (IProlongedEffect prolongedEffect in prolongedEffects)
            {
                prolongedEffect.AttachedUpdate(dt);
            }
        }
        public void Update(float dt)
        {
            foreach (IEffect effect in effects)
            {
                effect.Update(dt);
            }
            foreach (IProlongedEffect prolongedEffect in prolongedEffects)
            {
                prolongedEffect.Update(dt);
            }
        }
        public void Clear()
        {
            effects.Clear();
            prolongedEffects.ForEach(delegate(IProlongedEffect item) { item.RemoveAttributeChanges(); });
            prolongedEffects.Clear();
        }
        #endregion
    }
}
