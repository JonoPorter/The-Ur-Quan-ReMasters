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
namespace ReMasters.SuperMelee.Actions
{
    public interface IGunAction : IAction
    {
        ISolidWeapon Weapon { get;}
        float VelocityAngle { get;}
        float RandomVelocityAngle { get;}
    }

    public interface IMultiGunAction : IAction
    {
        ISolidWeapon[] Weapons { get;}
        float[] VelocityAngles { get;}
        float[] RandomVelocityAngles { get;}
    }



    public class SolidWeaponHardPoint
    {
        public Vector2D Position;
        public Vector2D Velocity;
        public float DirectionAngle;

        public SolidWeaponHardPoint() { }
        public SolidWeaponHardPoint(Vector2D Velocity, Vector2D Position, float DirectionAngle)
        {
            this.Velocity = Velocity;
            this.Position = Position;
            this.DirectionAngle = DirectionAngle;
        }
        public SolidWeaponHardPoint(float distance, float velocity, float angle) : this(distance, velocity, angle, angle) { }
        public SolidWeaponHardPoint(float distance, float velocity, float offsetAngle, float velocityAngle)
        {
            DirectionAngle = velocityAngle;
            Position = Vector2D.FromLengthAndAngle(distance, offsetAngle);
            Velocity = Vector2D.FromLengthAndAngle(velocity, velocityAngle);
        }
        public ISolidWeapon FireGun(ISolidWeapon weapon, IControlable source)
        {
            Vector2D pos = source.Matrix.NormalMatrix * this.Position;
            Vector2D vel = source.Matrix.NormalMatrix * this.Velocity;

            PhysicsState physicsState = new PhysicsState(source.Current);
            physicsState.Position.Linear += pos;
            physicsState.Position.Angular += this.DirectionAngle;
            physicsState.Velocity.Linear += vel;
            physicsState.Velocity.Angular = weapon.MovementInfo.MaxAngularVelocity.Binder.Lower;
            physicsState.ForceAccumulator = ALVector2D.Zero;
            physicsState.Acceleration = ALVector2D.Zero;
            source.ApplyImpulse(pos, vel * (-weapon.MassInfo.Mass));

            ISolidWeapon newWeapon = (ISolidWeapon)weapon.Clone();
            newWeapon.Current.Set(physicsState);
            newWeapon.SetAllPositions();
            return newWeapon;
        }
    }



    [Serializable]
    public class GunAction : BaseAction, IMultiGunAction, IGunAction
    {
        public static ISolidWeapon FireGun(ISolidWeapon weapon, IControlable source, float offsetAngle, float velocityAngle, float extraDistance)
        {

            Vector2D direction = Vector2D.FromLengthAndAngle(1, source.Current.Position.Angular + offsetAngle);
            Vector2D Velocity = Vector2D.FromLengthAndAngle(1, source.Current.Position.Angular + velocityAngle) * weapon.MovementInfo.MaxLinearVelocity.Value;
            float distance = source.BoundingRadius + weapon.BoundingRadius;
            Vector2D pos = direction * (distance + extraDistance);

            PhysicsState physicsState = new PhysicsState(source.Current);
            physicsState.Position.Linear += pos;
            physicsState.Position.Angular += velocityAngle;
            physicsState.Velocity.Linear += Velocity;
            physicsState.Velocity.Angular = weapon.MovementInfo.MaxAngularVelocity.Binder.Lower;
            physicsState.ForceAccumulator = ALVector2D.Zero;
            physicsState.Acceleration = ALVector2D.Zero;
            source.ApplyImpulse(pos, Velocity * (-weapon.MassInfo.Mass));

            ISolidWeapon newWeapon = (ISolidWeapon)weapon.Clone();
            newWeapon.Current.Set(physicsState);
            newWeapon.SetAllPositions();
            return newWeapon;
        }

        public static ISolidWeapon[] CreateWeapons(int count, ISolidWeapon weapon)
        {
            ISolidWeapon[] rv = new ISolidWeapon[count];
            for (int pos = 0; pos < count; ++pos)
            {
                rv[pos] = (ISolidWeapon)weapon.Clone();
            }
            return rv;
        }
        public static float[] CreateRange(int count, float start, float end)
        {
            float[] rv = new float[count];
            float inc = (start - end) / (count - 1);
            for (int pos = 0; pos < count; ++pos)
            {
                rv[pos] = start - inc * pos;
            }
            return rv;
        }
        public static float[] CreateArray(int count, float value)
        {
            float[] rv = new float[count];
            for (int pos = 0; pos < count; ++pos)
            {
                rv[pos] = value;
            }
            return rv;
        }

        static Random rand = new Random();
        float[] velocityAngles;
        float[] randomVelocityAngles;
        float[] offsetAngles;
        float[] randomOffsetAngles;
        float[] randomDistances;
        ISolidWeapon[] weapons;
        ISolidWeapon[] currentWeapons;


        #region constructors
        public GunAction(
          Bounded<float> delay,
          TargetingInfo targetableTypes,
          Costs costs,
          bool needsTarget,
          ActionSounds actionSounds,
          float velocityAngle,
          float offsetAngle,
          ISolidWeapon weapon)
            : this(delay, targetableTypes, costs, needsTarget, actionSounds, velocityAngle, offsetAngle, weapon, 0, 0, 0)
        { }

        public GunAction(
            Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            bool needsTarget,
            ActionSounds actionSounds,
            float velocityAngle,
            float offsetAngle,
            ISolidWeapon weapon,
            float randomVelocityAngle,
            float randomOffsetAngle,
            float randomDistance)
            : this(
                delay, targetableTypes, costs, needsTarget, actionSounds,
                new float[] { velocityAngle },
                new float[] { offsetAngle },
                new ISolidWeapon[] { weapon },
                new float[] { randomVelocityAngle },
                new float[] { randomOffsetAngle },
                new float[] { randomDistance })
        { }

        public GunAction(
            Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            bool needsTarget,
            ActionSounds actionSounds,
            float[] velocityAngles,
            float[] offsetAngles,
            ISolidWeapon[] weapons)
            : this(delay, targetableTypes, costs, needsTarget, actionSounds, velocityAngles, offsetAngles, weapons,
            new float[weapons.Length],
            new float[weapons.Length],
            new float[weapons.Length])
        {

        }


        public GunAction(
            Bounded<float> delay,
            TargetingInfo targetableTypes,
            Costs costs,
            bool needsTarget,
            ActionSounds actionSounds,
            float[] velocityAngles,
            float[] offsetAngles,
            ISolidWeapon[] weapons,
            float[] randomVelocityAngles,
            float[] randomOffsetAngles,
            float[] randomDistances)
            : base(delay, targetableTypes, costs, needsTarget, actionSounds)
        {
            this.velocityAngles = velocityAngles;
            this.randomVelocityAngles = randomVelocityAngles;
            this.offsetAngles = offsetAngles;
            this.randomOffsetAngles = randomOffsetAngles;
            this.randomDistances = randomDistances;
            this.weapons = weapons;
            this.aIInfo = new MultiGunActionAIInfo(costs.ActivationCost);
            this.currentWeapons = new ISolidWeapon[weapons.Length];
        }


        protected GunAction(GunAction copy)
            : base(copy)
        {
            this.velocityAngles = copy.velocityAngles;
            this.randomVelocityAngles = copy.randomVelocityAngles;
            this.offsetAngles = copy.offsetAngles;
            this.randomOffsetAngles = copy.randomOffsetAngles;
            this.randomDistances = copy.randomDistances;
            int length = copy.velocityAngles.Length;
            this.weapons = new ISolidWeapon[length];
            for (int pos = 0; pos < length; ++pos)
            {
                this.weapons[pos] = (ISolidWeapon)copy.weapons[pos].Clone();
            }
            this.currentWeapons = new ISolidWeapon[this.weapons.Length];
        }

        #endregion

        public float[] VelocityAngles
        {
            get
            {
                return velocityAngles;
            }
        }
        public float[] RandomVelocityAngles { get { return randomVelocityAngles; } }
        public float[] OffsetAngles
        {
            get
            {
                return offsetAngles;
            }
        }
        public ISolidWeapon[] Weapons
        {
            get
            {
                return weapons;
            }
        }
        public ISolidWeapon[] CurrentWeapons
        {
            get
            {
                return currentWeapons;
            }
        }


        public float VelocityAngle
        {
            get
            {
                return velocityAngles[0];
            }
        }
        public float RandomVelocityAngle { get { return randomVelocityAngles[0]; } }
        public float OffsetAngle
        {
            get
            {
                return offsetAngles[0];
            }
        }
        public ISolidWeapon Weapon
        {
            get
            {
                return weapons[0];
            }
        }
        public ISolidWeapon CurrentWeapon
        {
            get
            {
                return currentWeapons[0];
            }
            set
            {
                currentWeapons[0] = value;
            }
        }

        protected virtual void OnWeaponCreation(ActionResult actionResult, float dt, int index) { }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            float extradistance = .1f * source.MovementInfo.MaxLinearVelocity;
            int length = velocityAngles.Length;
            for (int index = 0; index < length; ++index)
            {
                currentWeapons[index] = FireGun(
                        weapons[index], source,
                        offsetAngles[index] + ((float)rand.NextDouble()-.5f) * randomOffsetAngles[index],
                        velocityAngles[index] + ((float)rand.NextDouble() - .5f) * randomVelocityAngles[index],
                        (float)rand.NextDouble() * randomDistances[index] + extradistance);
                currentWeapons[index].OnCreation(actionResult, source, this);
                OnWeaponCreation(actionResult, dt, index);
            }
            return true;
        }
        public override object Clone()
        {
            return new GunAction(this);
        }
    }
}