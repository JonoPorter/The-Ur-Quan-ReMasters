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
using AdvanceMath; 
using AdvanceSystem;
using AdvanceMath.Geometry2D;
namespace ReMasters.SuperMelee.Effects
{
    [Serializable]
    public class EffectBlockEffect<T> : BaseProlongedEffect
        where T : IEffect
    {
        public EffectBlockEffect(EffectSounds effectSounds, LifeSpan lifeTime) : base(TargetingInfo.All, EffectTypes.None, effectSounds, lifeTime) { }
        protected EffectBlockEffect(EffectBlockEffect<T> copy) : base(copy) { }
        public override object Clone()
        {
            return new EffectBlockEffect<T>(this);
        }
        public override void OnTargetDeath(GameResult gameResult) { }
        public override void ApplyEffect(GameResult gameResult, float dt) { }
        public override bool BlockEffect(IEffect effect)
        {
            return effect is T;
        }
    }
    [Serializable]
    public class JumpShipEffect : RemoteActionEffect
    {
        public JumpShipEffect(TargetingInfo effectsWho, int count, ActionSounds actionSounds)
            : base(
            effectsWho,
            EffectTypes.HealthSteal,
            new EffectSounds(),
            new JumpShipAction(count, actionSounds))
        { }
        protected JumpShipEffect(JumpShipEffect copy) : base(copy) { }
        public override object Clone()
        {
            return new JumpShipEffect(this);
        }
    }
    [Serializable]
    public class JumpShipAction : ReMasters.SuperMelee.Actions.GunAction
    {
        public JumpShipAction(int count, ActionSounds actionSounds)
            : base(
            new Bounded<float>(0),
            new TargetingInfo(TargetingTypes.Ship),
            new Costs(new ShipStateChange(count, 0, 0, 0), null, null),
            false,
            actionSounds,
            CreateArray(count, MathHelper.PI),
            CreateArray(count, MathHelper.PI),
            CreateWeapons(count, new ReMasters.SuperMelee.Ships.LifePod()),
            CreateArray(count, MathHelper.PI),
            CreateArray(count, MathHelper.PI),
            CreateArray(count, 200))
        { }
        protected JumpShipAction(JumpShipAction copy) : base(copy) { }
        public override object Clone()
        {
            return new JumpShipAction(this);
        }
    }
}