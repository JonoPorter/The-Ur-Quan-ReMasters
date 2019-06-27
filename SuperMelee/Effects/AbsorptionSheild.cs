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
using AdvanceMath;
using System.Collections.Generic;
using System.Text;
using Physics2D;
namespace ReMasters.SuperMelee.Effects
{

    [Serializable]
    public class AbsorptionSheild : BaseFilterProlongedEffect
    {
        public static LifeSpan DefaultLifeTime = new LifeSpan(4);
        float absorptionRatio;

        public AbsorptionSheild(TargetingInfo effectWho, EffectTypes harmfulEffectTypes, EffectSounds effectSounds,
            LifeSpan lifeTime,EffectTypes filteredEffectTypes,
            float absorptionRatio)
            : base(effectWho, harmfulEffectTypes,effectSounds, lifeTime, filteredEffectTypes)
        {
            this.absorptionRatio = absorptionRatio;
        }
        protected AbsorptionSheild(AbsorptionSheild copy)
            : base(copy) 
        {
            this.absorptionRatio = copy.absorptionRatio;
        }
        public override void FilterEffects(float dt, List<IEffect> effects)
        {
            GeneralChange change = new GeneralChange();
            foreach (IEffect effect in effects)
            {
                change.Merge(effect.RemoveEffectTypes(filteredEffectTypes));
            }
            if (change.ShipStateChange != null && change.ShipStateChange.Health < 0)
            {
                ShipStateChange truechange = new ShipStateChange();
                truechange.Energy =  MathHelper.Floor(-change.ShipStateChange.Health * absorptionRatio);
                truechange.Health = MathHelper.Ceiling(change.ShipStateChange.Health * (1 - absorptionRatio));
                this.attachie.ShipState.Change(truechange);
            }
        }
        public override object Clone()
        {
            return new AbsorptionSheild(this);
        }
    }
}