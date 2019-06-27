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
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
namespace ReMasters.SuperMelee.Effects
{
    [Serializable]
    public abstract class PhysicsStateEffect : BaseEffect
    {
        public PhysicsStateEffect(TargetingInfo effectWho,
            EffectTypes harmfulEffectTypes,
            EffectSounds effectSounds)
            : base(effectWho, harmfulEffectTypes, effectSounds)
        {}
        protected PhysicsStateEffect(PhysicsStateEffect copy)
            : base(copy)
        {}
        public static EffectTypes GetEffectTypes(PhysicsState state)
        {
            EffectTypes rv = EffectTypes.None;
            if (state.Position != ALVector2D.Zero)
            {
                rv = rv | EffectTypes.Position;
            }
            if (state.Velocity != ALVector2D.Zero)
            {
                rv = rv | EffectTypes.Velocity;
            }
            return rv;
        }
        public override void ApplyEffect(GameResult gameResult, float dt)
        {
            PhysicsState psc = PhysicsStateChange;
            this.attachie.Current.Position += psc.Position;
            this.attachie.Current.Velocity += psc.Velocity;
            Exhausted = true;
        }
        public override GeneralChange RemoveEffectTypes(EffectTypes types)
        {
            GeneralChange returnvalue = new GeneralChange();
            PhysicsState psc = PhysicsStateChange;
            bool filtered = false;
            if (  ((int)types*(int)EffectTypes.Position) != 0)
            {
                if(returnvalue.PhysicsChange==null)
                {
                    returnvalue.PhysicsChange = new PhysicsState();
                }
                returnvalue.PhysicsChange.Position += psc.Position;
                psc.Position = ALVector2D.Zero;
                filtered = true;
            }
            if (((int)types * (int)EffectTypes.Velocity) != 0)
            {
                if(returnvalue.PhysicsChange==null)
                {
                    returnvalue.PhysicsChange = new PhysicsState();
                }
                returnvalue.PhysicsChange.Velocity += psc.Velocity;
                psc.Velocity = ALVector2D.Zero;
                filtered = true;
            }
            if (filtered)
            {
                PhysicsStateChange = psc;
            }
            return returnvalue;
        }
        public abstract PhysicsState PhysicsStateChange { get;set;}
    }
}