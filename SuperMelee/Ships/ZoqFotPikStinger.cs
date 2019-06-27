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
#if !Release
using System;
using System.Collections.Generic;
using Physics2D;
using Physics2D.CollidableBodies;
using Physics2D.Joints;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;

namespace ReMasters.SuperMelee.Ships
{
    public class ZoqFotPikStingerShipLoader : ShipLoader
    {
        public ZoqFotPikStingerShipLoader() : base("Zoq-Fot-Pik Stinger") { }
        protected override IShip CreateHardCodedShip()
        {
            return ZoqFotPikStinger.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class ZoqFotPikStinger : Ship
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static ActionList DefaultActions = new ActionList();
        public static ControlableSounds DefaultControlableSounds;
        public static ShipSounds DefaultShipSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(1)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(10, 1)),
                new Bounded<float>(TimeWarp.ScaleVelocity(40)));
            DefaultState = new ShipState(new Bounded<float>(10),
                new Bounded<float>(10),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(4, 1)));

            DefaultActions.Add( ZoqFotPikStingerPrimary.Create());
            DefaultActions.Add( ZoqFotPikStingerSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("ZoqFotPikStingerDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();


            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(0, 0)), Polygon2D.FromNumberofSidesAndRadius(8, 30)));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-30, 0)), Polygon2D.FromNumberofSidesAndRadius(8, 12)));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-15, 25)), Polygon2D.FromNumberofSidesAndRadius(8, 12)));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-15, -25)), Polygon2D.FromNumberofSidesAndRadius(8, 12)));
            coes.Add(DefaultCoefficients);
            DefaultShape = new RigidBodyTemplate(5, 3383.9114375372737f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            ZoqFotPikStinger returnvalue = new ZoqFotPikStinger(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Zoq-Fot-Pik Stinger", new CreateShipDelegate(Create));
        }*/
        protected ZoqFotPikStinger(PhysicsState state, FactionInfo factionInfo)
            : base(
            (LifeSpan)DefaultLifeTime.Clone(), 
            state, 
            DefaultBodyFlags, DefaultShape, 
            new ShipMovementInfo(DefaultMovementInfo), 
            new ShipState(DefaultState), 
            DefaultControlableSounds, 
            DefaultShipSounds, 
            new ActionList(DefaultActions), 
            null)
        { }
    }
    [Serializable]
    public class ZoqFotPikStingerPrimary : GunAction
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;

        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;
            InitShape();

            DefaultTargetingTypes = TargetingInfo.None;
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(120)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(11, 120));
            DefaultState = new ShipState(new Bounded<float>(1), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));

            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultActionSounds = new ActionSounds("Gun3", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(.01f, 7),
                new IGeometry2D[] { new Polygon2D(Polygon2D.FromNumberofSidesAndRadius(10, 7)) },
                new Coefficients[] { coe });
            DefaultShape.BalanceBody();
        }
        static ISolidWeapon CreateWeapon()
        {
            return new Controlable(
                DefaultLifeTime,
                new PhysicsState(),
                DefaultBodyFlags,
                DefaultShape,
                 new ShipMovementInfo(DefaultMovementInfo),
            new ShipState(DefaultState),
            new ControlableSounds(),
            new WeaponsLogic(TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static ZoqFotPikStingerPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ZoqFotPikStingerPrimary();
        }

        ZoqFotPikStingerPrimary()
            : base(new Bounded<float>(DefaultDelay), 
           DefaultTargetingTypes, 
           new Costs(DefaultCost), 
           false, 
           DefaultActionSounds, 
           0, 
           0, 
           CreateWeapon(), 
           .1f, 
           0, 
           0)
        { }

    }
    [Serializable]
    public class ZoqFotPikStingerSecondary : RayAction
    {
        public static LifeSpan DefaultLifeTime;
        public static Costs DefaultCost;
        public static float DefaultRange;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Enemy | TargetingTypes.Debris);
            DefaultCost = new Costs(new ShipStateChange(0, 7, 0, 0), null, null);
            DefaultRange = TimeWarp.ScaleRange(2);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(6));
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(
                    DefaultTargetingTypes,
                    EffectTypes.None,
                    new EffectSounds("Boom67", null, null),
                    new ShipStateChange(-12, 0, 0, 0)));
            DefaultLifeTime = new LifeSpan(.4f);
            DefaultActionSounds = new ActionSounds("OrganicGun3", null, null);
        }
        static MultiStageDirectedRayWeapon CreateWeapon()
        {
            return new MultiStageDirectedRayWeapon(
                new WeaponsLogic(DefaultTargetingTypes,
                new EffectCollection(DefaultEffectCollection)),
                new float[] { 100, 100, 100, 100 },
                new float[] { 0, 0, 0, 0 },
                new float[] { 0, 0, 0, 0 },
                new float[] { DefaultRange / 4, DefaultRange / 2, DefaultRange, DefaultRange / 2 },
                new float[] { 0, .05f, .1f, .15f },
                new float[] { .05f, .05f, .05f, .05f });
        }
        public static ZoqFotPikStingerSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ZoqFotPikStingerSecondary();
        }
        ZoqFotPikStingerSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           CreateWeapon())
        { }
    }
}
#endif
