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
using Physics2D;
using Physics2D.CollidableBodies;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using ReMasters.SuperMelee;

using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Ships
{
    [Serializable]
    class Planet : Controlable
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        public static BodyFlags DefaultBodyFlags = BodyFlags.InfiniteMass | BodyFlags.GravityWell;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static TargetingInfo DefaultTargetingTypes;

        public static Bounded<float> DefaultDamageDelay;

        public static float DefaultGravityStrength = 600;
        public static float DefaultGravityTolerance = 125 / (TimeWarp.DistanceRatio * TimeWarp.DistanceRatio);

        static Planet()
        {
            InitShape();
            DefaultDamageDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0));
            DefaultState = new ShipState(new Bounded<float>(float.MaxValue),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0));
            DefaultTargetingTypes = TargetingInfo.All;
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(new TargetingInfo(TargetingTypes.None, TargetingTypes.None, TargetingTypes.Ship), EffectTypes.None, new EffectSounds(), new ShipStateChange(-3, 0, 0, 0)));
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(new TargetingInfo(TargetingTypes.Ship, TargetingTypes.None, TargetingTypes.None), EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultEffectCollection.AttachmentFlags |= EffectAttachmentFlags.ClonedAttachment;
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;

            float radius = 200;
            //float mass = 9.9891e23f;
            float mass = 9.9891e24f;
            DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(mass, radius),
                new IGeometry2D[] { new Circle2D(radius,Vector2D.Zero) }, new Coefficients[] { DefaultCoefficients});
            //DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static Planet Create(GameResult gameResult, PhysicsState state)
        {
            Planet returnvalue = new Planet(state);
            returnvalue.OnCreation(gameResult, FactionInfo.DefaultFactionCollection.CreateNewOrGetFaction("Planets"));
            return returnvalue;
        }
        float gravityStrength = DefaultGravityStrength;
        float gravityTolerance = DefaultGravityTolerance;
        protected Planet(PhysicsState state)
            : base(
                (LifeSpan)DefaultLifeTime.Clone(), 
                state, 
                DefaultBodyFlags, DefaultShape, 
                 new ShipMovementInfo(DefaultMovementInfo), 
            new ShipState(DefaultState), 
            new ControlableSounds(), 
            new MultipleHitWeaponsLogic(TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection),
            new Bounded<float>(DefaultDamageDelay))) 
        {
            this.controlableType = ControlableType.Planet;
        }
        public override bool IsThreatTo(IControlable other)
        {
            Vector2D diff = other.Current.Position.Linear - this.current.Position.Linear;
            float MagnitudeSQ = diff.MagnitudeSq;
            float mindist = (this.boundingRadius * 3 + other.BoundingRadius);
            return (MagnitudeSQ < mindist * mindist);// ||
                //((MagnitudeSQ < (gravityTolerance * gravityTolerance) && MagnitudeSQ > 0) &&
                //((gravityStrength / MathHelper.Sqrt(MagnitudeSQ)) > other.MovementInfo.MaxLinearAcceleration * .5f));

        }
        public override void OnCollision(GameResult gameResult, IControlable collider)
        {
            collider.ShipState.Health.Value -= 1;
             base.OnCollision(gameResult,collider);
        }
        public override void OnCreation(GameResult gameResult, FactionInfo factionInfo)
        {
            base.OnCreation(gameResult,factionInfo);
            this.weaponInfo.OnCreation(gameResult,this, null, new NullAction());
        }
        public override Vector2D GetGravityPullAt(Vector2D position)
        {
            Vector2D diff = position - this.current.Position.Linear;
            float MagnitudeSQ = diff.MagnitudeSq;
            if (MagnitudeSQ < (gravityTolerance * gravityTolerance) && MagnitudeSQ > 0)
            {
                return diff * (-gravityStrength / MathHelper.Sqrt(MagnitudeSQ));
            }
            else 
            {
                return Vector2D.Zero;
            }
        }
    }
}