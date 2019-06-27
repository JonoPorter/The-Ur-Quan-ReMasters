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
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Ships
{
    public class SyreenPenetratorShipLoader : ShipLoader
    {
        public SyreenPenetratorShipLoader() : base("Syreen Penetrator") { }
        protected override IShip CreateHardCodedShip()
        {
            return SyreenPenetrator.Create(new PhysicsState(), null);
        }
    }



    [Serializable]
    public class SyreenPenetrator : Ship
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
                new Bounded<float>(TimeWarp.ScaleAcceleration(9, 1)),
                new Bounded<float>(TimeWarp.ScaleVelocity(36)));
            DefaultState = new ShipState(new Bounded<float>(0, 12, 42, false),
                new Bounded<float>(16),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(6, 1)));

            DefaultActions.Add(SyreenPenetratorPrimary.Create());
            DefaultActions.Add(SyreenPenetratorSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("SyreenPenetratorDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;

            Vector2D[] mainhullvertecies = new Vector2D[]
                {
                    new Vector2D(90,0),
                    new Vector2D(70,15),
                    new Vector2D(-20,15),
                    new Vector2D(-40,10),
                    new Vector2D(-40,-10),
                    new Vector2D(-20,-15),
                    new Vector2D(70,-15)
                };
            Vector2D[] leftWingvertecies = new Vector2D[]
                {
                    new Vector2D(10,10),
                    new Vector2D(-30,10),
                    new Vector2D(-50,-30),
                    new Vector2D(-20,-30)
                };
            int length = leftWingvertecies.Length;
            Vector2D[] RightWingvertecies = new Vector2D[]
                {
                    new Vector2D(-20,30),
                    new Vector2D(-50,30),
                    new Vector2D(-30,-10),
                    new Vector2D(10,-10)
                };
            IGeometry2D mainhull = new Polygon2D(ALVector2D.Zero, mainhullvertecies);
            IGeometry2D leftWing = new Polygon2D(new ALVector2D(0, new Vector2D(10, -20)), leftWingvertecies);
            IGeometry2D RightWing = new Polygon2D(new ALVector2D(0, new Vector2D(10, 20)), RightWingvertecies);
            DefaultShape = new RigidBodyTemplate(10, 1468.2144894018809f, new IGeometry2D[] { leftWing, RightWing, mainhull }, new Coefficients[] { DefaultCoefficients, DefaultCoefficients, DefaultCoefficients });
            DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {

            if (!initialized)
            {
                Initialize();
            }

            SyreenPenetrator returnvalue = new SyreenPenetrator(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Syreen Penetrator", new CreateShipDelegate(Create));
        }*/
        protected SyreenPenetrator(PhysicsState state, FactionInfo factionInfo)
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
    public class SyreenPenetratorPrimary : GunAction
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static Costs DefaultCost;

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(TimeWarp.ScaleVelocity(120)));
            DefaultState = new ShipState(new Bounded<float>(2), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(17, 120));
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-2, 0, 0, 0)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(8));
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds("Gun4", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;

            Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(5,2),
                new Vector2D(-5,3),
                new Vector2D(-5,-3),
                new Vector2D(5,-2)
            };
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, bulletverticies);
            DefaultShape = new RigidBodyTemplate(.001f, 10.549201161945275f, new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
            DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
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

        public static SyreenPenetratorPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new SyreenPenetratorPrimary();
        }

        SyreenPenetratorPrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           0,
           0,
           CreateWeapon())
        { }
    }
    [Serializable]
    public class SyreenPenetratorSecondary : ProximityInstantAction
    {
        public static LifeSpan DefaultLifeTime = new LifeSpan(0);
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static Bounded<float> DefaultDelay;

        public static float DefaultRadius = 500;
        public static int DefaultMaxNumberofTargets = 10;
        public static float DefaultImpulse = 200;
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultEffectsWho = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Ship | TargetingTypes.Enemy, TargetingTypes.None);
            DefaultCost = new Costs(new ShipStateChange(0, 5, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(20));
            DefaultActionSounds = new ActionSounds("SirensCall", null, null);
            DefaultEffectCollection.Effects.Add(new JumpShipEffect(DefaultEffectsWho, 5, new ActionSounds()));
        }
        static IWeapon CreateWeapon()
        {
            return new InstantWeapon(new WeaponsLogic(DefaultEffectsWho,
                new EffectCollection(DefaultEffectCollection)));
        }

        public static SyreenPenetratorSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new SyreenPenetratorSecondary();
        }
        SyreenPenetratorSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           DefaultActionSounds,
           DefaultRadius,
           DefaultMaxNumberofTargets,
            CreateWeapon())
        { }
    }

}
#endif
