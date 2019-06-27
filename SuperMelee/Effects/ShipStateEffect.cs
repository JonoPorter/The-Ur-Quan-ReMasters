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
namespace ReMasters.SuperMelee.Effects
{
    [Serializable]
    public class ShipStateEffect : BaseEffect
    {
        protected ShipStateChange ssc;
            
        public ShipStateEffect(TargetingInfo effectsWho,
            EffectTypes effectTypes,
            EffectSounds effectSounds,
            ShipStateChange ssc)
            : base(effectsWho, effectTypes | ssc.HarmfulEffectTypes, effectSounds)
        {
            this.ssc = ssc;
            this.isHarmful = ssc.Energy < 0 || ssc.EnergyChangeRate < 0 || ssc.Health < 0 || ssc.HealthChangeRate < 0;
        }
        protected ShipStateEffect(ShipStateEffect copy)
            : base(copy)
        {
            this.ssc = new ShipStateChange(copy.ssc);
        }
        public bool AppliesAll
        {
            get
            {
                return ssc.IsZero;
            }
        }
        public override void ApplyEffect(GameResult gameResult, float dt)
        {
            if (!ssc.IsZero)
            {
                float healthbefore = this.attachie.ShipState.Health.Value;
                this.attachie.ShipState.ChangeLeftOver(ssc);
                if (effectSounds.Applied.Name == null)
                {
                    float healthafter = this.attachie.ShipState.Health.Value;
                    float diff = healthbefore - healthafter;
                    MeleeDamageSounds.PlayDamage(diff);
                }
                else
                {
                    effectSounds.Applied.Play();
                }
                if (ssc.IsZero)
                {
                    Exhausted = true;
                }
            }
            else
            {
                Exhausted = true;
            }
        }
        public override GeneralChange RemoveEffectTypes(EffectTypes types)
        {
            GeneralChange returnvalue = new GeneralChange();
            returnvalue.ShipStateChange = ssc.RemoveEffectTypes(types);
            return returnvalue;
        }
        public override object Clone()
        {
            return new ShipStateEffect(this);
        }
    }
}