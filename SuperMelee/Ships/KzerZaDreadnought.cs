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
    public class KzerZaDreadnoughtShipLoader : ShipLoader
    {
        public KzerZaDreadnoughtShipLoader() : base("Kzer-Za Dreadnought") { }
        protected override IShip CreateHardCodedShip()
        {
            return KzerZaDreadnought.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class KzerZaDreadnought : Ship
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
                new Bounded<float>(TimeWarp.ScaleTurning(4)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(6, 6)),
                new Bounded<float>(TimeWarp.ScaleVelocity(30)));
            DefaultState = new ShipState(new Bounded<float>(42),
                new Bounded<float>(42),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(4, 1)));

            DefaultActions.Add(KzerZaDreadnoughtPrimary.Create());
            DefaultActions.Add(KzerZaDreadnoughtSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("KzerZaDreadnoughtDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            Vector2D[] Pod1p =
            new Vector2D[]{
                      new Vector2D(50,-15),
                      new Vector2D(80,15),
                      new Vector2D(-50,15),
                      new Vector2D(-50,-15)};
            Vector2D[] Pod2p =
                  new Vector2D[]{
                      new Vector2D(-50,-15),
                      new Vector2D(80,-15),
                      new Vector2D(50,15),
                      new Vector2D(-50,15)};
            Vector2D[] bridgep =
                new Vector2D[]{
                    new Vector2D(-35,20),
                    new Vector2D(-35,-20),
                    new Vector2D(-22,-44),
                    new Vector2D(-10,-50),
                    new Vector2D(20,-50),
                    new Vector2D(20,50),
                    new Vector2D(-10,50),
                    new Vector2D(-22,44)};


            Vector2D[] mainhullp = Polygon2D.FromRectangle(50, 150);
            Vector2D[] subhullp = Polygon2D.FromRectangle(100, 30);
            IGeometry2D[] geometry = new IGeometry2D[5];


            geometry[0] = new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-10, 0)), subhullp);
            geometry[1] = new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(0, 50)), Pod1p);
            geometry[2] = new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(0, -50)), Pod2p);
            geometry[3] = new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(0, 0)), mainhullp);
            geometry[4] = new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(90, 0)), bridgep);

            Coefficients[] coefficients = new Coefficients[5];
            for (int pos = 0; pos < 5; ++pos)
            {
                coefficients[pos] = DefaultCoefficients;
            }

            DefaultShape = new RigidBodyTemplate(20, 3907.8407737525167f, geometry, coefficients);
            DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            KzerZaDreadnought returnvalue = new KzerZaDreadnought(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Kzer-Za Dreadnought", new CreateShipDelegate(Create));
        }*/
        protected KzerZaDreadnought(PhysicsState state, FactionInfo factionInfo)
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
    public class KzerZaDreadnoughtPrimary : GunAction
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
        public static TargetingInfo DefaultEffectsWho;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(80)));
            DefaultState = new ShipState(new Bounded<float>(6), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultCost = new Costs(new ShipStateChange(0, 6, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(6));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(22, 80));

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-6, 0, 0, 0)));

            DefaultTargetingTypes = TargetingInfo.None;
            DefaultEffectsWho = new TargetingInfo(TargetingTypes.All, TargetingTypes.None, TargetingTypes.None);
            DefaultActionSounds = new ActionSounds("PlasmaBlast", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;

            Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(25,0),
                new Vector2D(20,10),
                new Vector2D(-20,5),
                new Vector2D(-20,-5),
                new Vector2D(20,-10)
            };
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, bulletverticies);
            DefaultShape = new RigidBodyTemplate(.5f, 166.12352450641404f, new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
            DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
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
               new WeaponsLogic(DefaultEffectsWho,
               new EffectCollection(DefaultEffectCollection)));
        }
        public static KzerZaDreadnoughtPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new KzerZaDreadnoughtPrimary();
        }

        KzerZaDreadnoughtPrimary()
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
    public class KzerZaDreadnoughtSecondary : GunAction
    {

        public static Costs DefaultCost;

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(2, 8, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(9));
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            DefaultEffectsWho = new TargetingInfo(TargetingTypes.All, TargetingTypes.None, TargetingTypes.None);
            DefaultActionSounds = new ActionSounds("launchFighters", null, null);
        }
        static ISolidWeapon CreateWeapon()
        {
            ISolidWeapon w = KzerZaFighter.Create();
            w.ControlHandler = new DefaultControlHandler();
            w.AddControler(null, new MannedOrbitControler(TargetingInfo.FromRequireAll(TargetingTypes.Enemy | TargetingTypes.Ship), TimeWarp.ScaleRange(2)));
            w.AddControler(null, new FireAllControler());
            return w;
        }

        public static KzerZaDreadnoughtSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new KzerZaDreadnoughtSecondary();
        }
        KzerZaDreadnoughtSecondary()
            : base(new Bounded<float>(DefaultDelay),
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            true,
            DefaultActionSounds,
            new float[] { MathHelper.PI + .1f, MathHelper.PI - .1f },
            new float[] { MathHelper.PI + .1f, MathHelper.PI - .1f },
            new ISolidWeapon[] { CreateWeapon(), CreateWeapon() }
            )
        { }
    }
    [Serializable]
    public class KzerZaFighter : Ship
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static TargetingInfo DefaultEffectsWho;
        public static ActionList DefaultActions = new ActionList();
        public static ControlableSounds DefaultControlableSounds;
        public static ShipSounds DefaultShipSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(400, 35));
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(20),
                new Bounded<float>(1000),
                new Bounded<float>(TimeWarp.ScaleVelocity(35)));
            DefaultState = new ShipState(
                new Bounded<float>(1),
                new Bounded<float>(1),
                new Bounded<float>(0),
                new Bounded<float>(1));

            DefaultEffectCollection.Effects.Add(new MedKitEffect(new TargetingInfo(TargetingTypes.None, TargetingTypes.Ally | TargetingTypes.Ship, TargetingTypes.None), EffectTypes.None, new EffectSounds(null, "FighterGet", null), 1));

            DefaultEffectsWho = new TargetingInfo(TargetingTypes.None, TargetingTypes.Ally | TargetingTypes.Ship, TargetingTypes.None);

            DefaultActions.Add(KzerZaFighterPrimary.Create());

            DefaultControlableSounds = new ControlableSounds();
            DefaultShipSounds = new ShipSounds();
        }
        static void InitShape()
        {
            DefaultShape = new RigidBodyTemplate(MassInertia.FromRectangle(.01f, 5, 10), new IGeometry2D[] { new Polygon2D(ALVector2D.Zero, Polygon2D.FromRectangle(5, 10)) }, new Coefficients[] { TimeWarp.Coefficients });
        }
        public static KzerZaFighter Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new KzerZaFighter();
        }

        KzerZaFighter()
            : base(
           (LifeSpan)DefaultLifeTime.Clone(),
           new PhysicsState(),
           DefaultBodyFlags,
           DefaultShape,
           new ShipMovementInfo(DefaultMovementInfo),
           DefaultState,
           DefaultControlableSounds,
           DefaultShipSounds,
           new WeaponsLogic(DefaultEffectsWho,
           new EffectCollection(DefaultEffectCollection)),
           new ActionList(DefaultActions),
           null)
        {
            this.controlableType = ControlableType.SubShip;
        }
    }
    [Serializable]
    public class KzerZaFighterPrimary : RayPointDefence
    {
        public static LifeSpan DefaultLifeTime = new LifeSpan(0);
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;

        public static float DefaultRadius;
        public static int DefaultMaxNumberofTargets;
        public static float DefaultImpulse = 20;
        public static ActionSounds DefaultActionSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(new TargetingInfo(TargetingTypes.Enemy | TargetingTypes.Debris), EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Enemy);
            DefaultCost = new Costs(new ShipStateChange(0, 0, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(8));
            DefaultRadius = TimeWarp.ScaleRange(3);
            DefaultMaxNumberofTargets = 1;
            DefaultActionSounds = new ActionSounds("Laser3", null, null);
        }
        static TargetedRayWeapon CreateWeapon()
        {
            return new TargetedRayWeapon(
                (LifeSpan)DefaultLifeTime.Clone(),
                new WeaponsLogic(DefaultTargetingTypes,
                new EffectCollection(DefaultEffectCollection)),
                DefaultImpulse,
                DefaultRadius);
        }
        public static KzerZaFighterPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new KzerZaFighterPrimary();
        }
        KzerZaFighterPrimary()
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
