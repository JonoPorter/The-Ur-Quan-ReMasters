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
    public class BlockingSheild : BaseProlongedEffect
    {
        public static LifeSpan DefaultLifeTime = new LifeSpan(4);
        EffectTypes blockedEffectTypes;
        public BlockingSheild(TargetingInfo effectWho, EffectTypes harmfulEffectTypes, EffectSounds effectSounds,
            LifeSpan lifeTime, EffectTypes blockedEffectTypes)
            : base(effectWho, harmfulEffectTypes, effectSounds, lifeTime)
        {
            this.blockedEffectTypes = blockedEffectTypes;
        }
        protected BlockingSheild(BlockingSheild copy)
            : base(copy)
        {
            this.blockedEffectTypes = copy.blockedEffectTypes;
        }
        public override bool BlockEffect(IEffect effect)
        {
            return ((effect.HarmfulEffectTypes & blockedEffectTypes) != EffectTypes.None);
        }
        public override void OnTargetDeath(GameResult gameResult)
        {}
        public override void ApplyEffect(GameResult gameResult, float dt)
        {}
        public override object Clone()
        {
            return new BlockingSheild(this);
        }
    }
}