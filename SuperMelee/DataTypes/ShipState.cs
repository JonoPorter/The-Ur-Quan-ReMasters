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
    public sealed class ShipStateChange
    {
        #region static methods
        public static ShipStateChange Merge(ShipStateChange first, ShipStateChange second)
        {
            ShipStateChange returnvalue = new ShipStateChange();
            returnvalue.Health = first.Health + second.Health;
            returnvalue.Energy = first.Energy + second.Energy;
            returnvalue.HealthChangeRate = first.HealthChangeRate + second.HealthChangeRate;
            returnvalue.EnergyChangeRate = first.EnergyChangeRate + second.EnergyChangeRate;
            return returnvalue;
        }
        public static ShipStateChange operator +(ShipStateChange first, ShipStateChange second)
        {
            ShipStateChange returnvalue = new ShipStateChange();
            returnvalue.Health = first.Health + second.Health;
            returnvalue.Energy = first.Energy + second.Energy;
            returnvalue.HealthChangeRate = first.HealthChangeRate + second.HealthChangeRate;
            returnvalue.EnergyChangeRate = first.EnergyChangeRate + second.EnergyChangeRate;
            return returnvalue;
        }
        public static ShipStateChange operator -(ShipStateChange source)
        {
            ShipStateChange returnvalue = new ShipStateChange();
            returnvalue.Health = - source.Health;
            returnvalue.Energy = - source.Energy;
            returnvalue.HealthChangeRate = - source.HealthChangeRate;
            returnvalue.EnergyChangeRate = - source.EnergyChangeRate;
            return returnvalue;
        }
        public static ShipStateChange operator -(ShipStateChange first, ShipStateChange second)
        {
            ShipStateChange returnvalue = new ShipStateChange();
            returnvalue.Health = first.Health - second.Health;
            returnvalue.Energy = first.Energy - second.Energy;
            returnvalue.HealthChangeRate = first.HealthChangeRate - second.HealthChangeRate;
            returnvalue.EnergyChangeRate = first.EnergyChangeRate - second.EnergyChangeRate;
            return returnvalue;
        }
        public static ShipStateChange operator *(ShipStateChange first, float s)
        {
            ShipStateChange returnvalue = new ShipStateChange();
            returnvalue.Health = first.Health * s;
            returnvalue.Energy = first.Energy * s;
            returnvalue.HealthChangeRate = first.HealthChangeRate * s;
            returnvalue.EnergyChangeRate = first.EnergyChangeRate * s;
            return returnvalue;
        }
        public static ShipStateChange operator *(float s, ShipStateChange first)
        {
            ShipStateChange returnvalue = new ShipStateChange();
            returnvalue.Health = first.Health * s;
            returnvalue.Energy = first.Energy * s;
            returnvalue.HealthChangeRate = first.HealthChangeRate * s;
            returnvalue.EnergyChangeRate = first.EnergyChangeRate * s;
            return returnvalue;
        }
        public static EffectTypes CalcHarmfulEffectTypes(ShipStateChange ssc)
        {
            EffectTypes returnvalue = EffectTypes.None;
            if (ssc.Energy < 0)
            {
                returnvalue = returnvalue | EffectTypes.Energy;
            }
            if (ssc.EnergyChangeRate < 0)
            {
                returnvalue = returnvalue | EffectTypes.EnergyChangeRate;
            }
            if (ssc.Health < 0)
            {
                returnvalue = returnvalue | EffectTypes.Health;
            }
            if (ssc.HealthChangeRate < 0)
            {
                returnvalue = returnvalue | EffectTypes.HealthChangeRate;
            }
            return returnvalue;
        }

        #endregion
        public float Health;
        public float Energy;
        public float HealthChangeRate;
        public float EnergyChangeRate;
        public ShipStateChange()
        {
        }
        public ShipStateChange(
                    float Health,
                    float Energy,
                    float HealthChangeRate,
                    float EnergyChangeRate)
        {
            this.Health = Health;
            this.Energy = Energy;
            this.HealthChangeRate = HealthChangeRate;
            this.EnergyChangeRate = EnergyChangeRate;
        }
        public ShipStateChange(ShipStateChange copy)
        {
            this.Health = copy.Health;
            this.Energy = copy.Energy;
            this.HealthChangeRate = copy.HealthChangeRate;
            this.EnergyChangeRate = copy.EnergyChangeRate;
        }
        public bool IsZero
        {
            get
            {
                return this.Energy == 0 && this.EnergyChangeRate == 0 && this.Health == 0 && this.HealthChangeRate == 0;
            }
        }
        public EffectTypes HarmfulEffectTypes
        {
            get
            {
                return CalcHarmfulEffectTypes(this);
            }
        }
        public ShipStateChange RemoveEffectTypes(EffectTypes types)
        {
            ShipStateChange removed = new ShipStateChange();
            if ((types & EffectTypes.Energy) == EffectTypes.Energy)
            {
                removed.Energy = Energy;
                this.Energy = 0;
            }
            if ((types & EffectTypes.EnergyChangeRate) == EffectTypes.EnergyChangeRate)
            {
                removed.EnergyChangeRate = EnergyChangeRate;
                this.EnergyChangeRate = 0;
            }
            if ((types & EffectTypes.Health) == EffectTypes.Health)
            {
                removed.Health = Health;
                this.Health = 0;
            }
            if ((types & EffectTypes.HealthChangeRate) == EffectTypes.HealthChangeRate)
            {
                removed.HealthChangeRate = HealthChangeRate;
                this.HealthChangeRate = 0;
            }
            return removed;
        }
        public void Merge(ShipStateChange other)
        {
            if (other != null)
            {
                this.Energy += other.Energy;
                this.EnergyChangeRate += other.EnergyChangeRate;
                this.Health += other.Health;
                this.HealthChangeRate += other.HealthChangeRate;
            }
        }
        public float SumValue
        {
            get
            {
                return Health + Energy + HealthChangeRate + EnergyChangeRate;
            }
        }
    }
    [Serializable]
    public sealed class ShipState
    {
        public Bounded<float> Health;
        public Bounded<float> Energy;
        public Bounded<float> HealthChangeRate;
        public Bounded<float> EnergyChangeRate;
        public ShipState()
        {
            this.Health = new Bounded<float>(0);
            this.Energy = new Bounded<float>(0);
            this.HealthChangeRate = new Bounded<float>(0);
            this.EnergyChangeRate = new Bounded<float>(0);
        }
        public ShipState(float health)
        {
            this.Health = new Bounded<float>(health);
            this.Energy = new Bounded<float>(0);
            this.HealthChangeRate = new Bounded<float>(0);
            this.EnergyChangeRate = new Bounded<float>(0);
        }
        public ShipState(
                    Bounded<float> Health,
                    Bounded<float> Energy,
                    Bounded<float> HealthChangeRate,
                    Bounded<float> EnergyChangeRate)
        {
            this.Health = Health;
            this.Energy = Energy;
            this.HealthChangeRate = HealthChangeRate;
            this.EnergyChangeRate = EnergyChangeRate;
        }
        public ShipState(ShipState copy)
        {
            this.Health = new Bounded<float>(copy.Health);
            this.Energy = new Bounded<float>(copy.Energy);
            this.HealthChangeRate = new Bounded<float>(copy.HealthChangeRate);
            this.EnergyChangeRate = new Bounded<float>(copy.EnergyChangeRate);
        }
        public void Change(ShipStateChange change)
        {
            this.Health.Value += change.Health;
            this.Energy.Value += change.Energy;
            this.HealthChangeRate.Value += change.HealthChangeRate;
            this.EnergyChangeRate.Value += change.EnergyChangeRate;
        }
        public void ChangeLeftOver(ShipStateChange change)
        {
            change.Health = this.Health.ChangeValue(change.Health);
            change.Energy = this.Energy.ChangeValue(change.Energy);
            change.HealthChangeRate = this.HealthChangeRate.ChangeValue(change.HealthChangeRate);
            change.EnergyChangeRate = this.EnergyChangeRate.ChangeValue(change.EnergyChangeRate);
        }
        public bool MeetsCost(ShipStateChange cost)
        {
            float health = cost.Health;
            if (health != 0)
            {
                health -= 1;
            }
            return this.Health.HasRoomFor(health) && 
                this.Energy.HasRoomFor(cost.Energy) && 
                this.HealthChangeRate.HasRoomFor(cost.HealthChangeRate) && 
                this.EnergyChangeRate.HasRoomFor(cost.EnergyChangeRate);
        }
        public void Update(float dt)
        {
            Health.Value += HealthChangeRate.Value * dt;
            Energy.Value += EnergyChangeRate.Value * dt;
        }
    }
}