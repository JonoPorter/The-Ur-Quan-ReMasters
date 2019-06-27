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
    public class MovementInfoEffect : BaseProlongedEffect
    {
        protected ShipMovementInfoChange smic;
        public MovementInfoEffect(TargetingInfo effectsWho,
            EffectTypes effectTypes,
            EffectSounds effectSounds,
            LifeSpan lifeTime,
            ShipMovementInfoChange smic)
            : base(effectsWho, effectTypes | smic.HarmfulEffectTypes, effectSounds, lifeTime)
        {
            this.smic = smic;
            this.isHarmful = smic.MaxAngularAcceleration < 0 || smic.MaxAngularVelocity < 0 || smic.MaxLinearAcceleration < 0 || smic.MaxLinearVelocity < 0;

        }
        protected MovementInfoEffect(MovementInfoEffect copy)
            : base(copy)
        {
            this.smic = new ShipMovementInfoChange(copy.smic);
        }
        public bool AppliesAll
        {
            get
            {
                return smic.IsZero;
            }
        }
        public override void ApplyEffect(GameResult gameResult, float dt)
        {
            ApplyAttributeChanges();
        }
        public override void ApplyAttributeChanges()
        {
            this.attachie.MovementInfo.Change(smic);
        }
        public override void RemoveAttributeChanges()
        {
            this.attachie.MovementInfo.Change(-smic);
        }
        public override GeneralChange RemoveEffectTypes(EffectTypes types)
        {
            GeneralChange returnvalue = new GeneralChange();
            returnvalue.MovementInfoChange = smic.RemoveEffectTypes(types);
            return returnvalue;
        }
        public override object Clone()
        {
            return new MovementInfoEffect(this);
        }
        public override void OnTargetDeath(GameResult gameResult) { }
    }
}