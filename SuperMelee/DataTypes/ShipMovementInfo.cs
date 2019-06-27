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
    public sealed class ShipMovementInfoChange
    {
        public static EffectTypes CalcHarmfulEffectTypes(ShipMovementInfoChange ssc)
        {
            EffectTypes returnvalue = EffectTypes.None;
            if (ssc.MaxAngularAcceleration < 0)
            {
                returnvalue |= EffectTypes.MaxAngularAcceleration;
            }
            if (ssc.MaxAngularVelocity < 0)
            {
                returnvalue |= EffectTypes.MaxAngularVelocity;
            }
            if (ssc.MaxLinearAcceleration < 0)
            {
                returnvalue |= EffectTypes.MaxLinearAcceleration;
            }
            if (ssc.MaxLinearVelocity < 0)
            {
                returnvalue |= EffectTypes.MaxLinearVelocity;
            }
            return returnvalue;
        }

        public float MaxAngularAcceleration;
        public float MaxAngularVelocity;
        public float MaxLinearAcceleration;
        public float MaxLinearVelocity;
        public ShipMovementInfoChange()
        {
            this.MaxAngularAcceleration = 0;
            this.MaxAngularVelocity = 0;
            this.MaxLinearAcceleration = 0;
            this.MaxLinearVelocity = 0;
        }
        public ShipMovementInfoChange(
                    float MaxAngularAcceleration,
                    float MaxAngularVelocity,
                    float MaxLinearAcceleration,
                    float MaxLinearVelocity)
        {
            this.MaxAngularAcceleration = MaxAngularAcceleration;
            this.MaxAngularVelocity = MaxAngularVelocity;
            this.MaxLinearAcceleration = MaxLinearAcceleration;
            this.MaxLinearVelocity = MaxLinearVelocity;
        }
        public ShipMovementInfoChange(ShipMovementInfoChange copy)
        {
            this.MaxAngularAcceleration = copy.MaxAngularAcceleration;
            this.MaxAngularVelocity = copy.MaxAngularVelocity;
            this.MaxLinearAcceleration = copy.MaxLinearAcceleration;
            this.MaxLinearVelocity = copy.MaxLinearVelocity;
        }

        public static ShipMovementInfoChange Merge(ShipMovementInfoChange first, ShipMovementInfoChange second)
        {
            ShipMovementInfoChange returnvalue = new ShipMovementInfoChange();
            returnvalue.MaxAngularAcceleration = first.MaxAngularAcceleration + second.MaxAngularAcceleration;
            returnvalue.MaxAngularVelocity = first.MaxAngularVelocity + second.MaxAngularVelocity;
            returnvalue.MaxLinearAcceleration = first.MaxLinearAcceleration + second.MaxLinearAcceleration;
            returnvalue.MaxLinearVelocity = first.MaxLinearVelocity + second.MaxLinearVelocity;
            return returnvalue;
        }
        public void Merge( ShipMovementInfoChange other)
        {
            this.MaxAngularAcceleration = this.MaxAngularAcceleration + other.MaxAngularAcceleration;
            this.MaxAngularVelocity = this.MaxAngularVelocity + other.MaxAngularVelocity;
            this.MaxLinearAcceleration = this.MaxLinearAcceleration + other.MaxLinearAcceleration;
            this.MaxLinearVelocity = this.MaxLinearVelocity + other.MaxLinearVelocity;
        }
        public bool IsZero
        {
            get
            {
                return MaxAngularAcceleration == 0 && MaxAngularVelocity == 0 && MaxLinearAcceleration == 0 && MaxLinearVelocity == 0;
            }
        }
        public EffectTypes HarmfulEffectTypes
        {
            get
            {
                return CalcHarmfulEffectTypes(this);
            }
        }
        public ShipMovementInfoChange RemoveEffectTypes(EffectTypes types)
        {
            ShipMovementInfoChange removed = new ShipMovementInfoChange();
            if ((types & EffectTypes.MaxAngularAcceleration) == EffectTypes.MaxAngularAcceleration)
            {
                removed.MaxAngularAcceleration = MaxAngularAcceleration;
                this.MaxAngularAcceleration = 0;
            }
            if ((types & EffectTypes.MaxAngularVelocity) == EffectTypes.MaxAngularVelocity)
            {
                removed.MaxAngularVelocity = MaxAngularVelocity;
                this.MaxAngularVelocity = 0;
            }
            if ((types & EffectTypes.MaxLinearAcceleration) == EffectTypes.MaxLinearAcceleration)
            {
                removed.MaxLinearAcceleration = MaxLinearAcceleration;
                this.MaxLinearAcceleration = 0;
            }
            if ((types & EffectTypes.MaxLinearVelocity) == EffectTypes.MaxLinearVelocity)
            {
                removed.MaxLinearVelocity = MaxLinearVelocity;
                this.MaxLinearVelocity = 0;
            }
            return removed;
        }
        public static ShipMovementInfoChange operator -(ShipMovementInfoChange value)
        {
            return new ShipMovementInfoChange(-value.MaxAngularAcceleration,
                    -value.MaxAngularVelocity,
                    -value.MaxLinearAcceleration,
                    -value.MaxLinearVelocity);
        }
    }
    [Serializable]
    public sealed class ShipMovementInfo
    {
        public Bounded<float> MaxAngularAcceleration;
        public Bounded<float> MaxAngularVelocity;
        public Bounded<float> MaxLinearAcceleration;
        public Bounded<float> MaxLinearVelocity;
        public ShipMovementInfo()
        {
            MaxAngularAcceleration = new Bounded<float>(0);
            MaxAngularVelocity = new Bounded<float>(0);
            MaxLinearAcceleration = new Bounded<float>(0);
            MaxLinearVelocity = new Bounded<float>(0);
        }
        public ShipMovementInfo(
                    Bounded<float> MaxAngularAcceleration,
                    Bounded<float> MaxAngularVelocity,
                    Bounded<float> MaxLinearAcceleration,
                    Bounded<float> MaxLinearVelocity)
        {
            this.MaxAngularAcceleration = MaxAngularAcceleration;
            this.MaxAngularVelocity = MaxAngularVelocity;
            this.MaxLinearAcceleration = MaxLinearAcceleration;
            this.MaxLinearVelocity = MaxLinearVelocity;
        }
        public ShipMovementInfo(ShipMovementInfo copy)
        {
            this.MaxAngularAcceleration = new Bounded<float>(copy.MaxAngularAcceleration);
            this.MaxAngularVelocity = new Bounded<float>(copy.MaxAngularVelocity);
            this.MaxLinearAcceleration = new Bounded<float>(copy.MaxLinearAcceleration);
            this.MaxLinearVelocity = new Bounded<float>(copy.MaxLinearVelocity);
        }
        public void Change(ShipMovementInfoChange change)
        {
            this.MaxAngularAcceleration.Value += change.MaxAngularAcceleration;
            this.MaxAngularVelocity.Value += change.MaxAngularVelocity;
            this.MaxLinearAcceleration.Value += change.MaxLinearAcceleration;
            this.MaxLinearVelocity.Value += change.MaxLinearVelocity;
        }
        public void ChangeLeftOver(ShipMovementInfoChange change)
        {
            change.MaxAngularAcceleration = this.MaxAngularAcceleration.ChangeValue(change.MaxAngularAcceleration);
            change.MaxAngularVelocity = this.MaxAngularVelocity.ChangeValue(change.MaxAngularVelocity);
            change.MaxLinearAcceleration = this.MaxLinearAcceleration.ChangeValue(change.MaxLinearAcceleration);
            change.MaxLinearVelocity = this.MaxLinearVelocity.ChangeValue(change.MaxLinearVelocity);
        }

    }
}