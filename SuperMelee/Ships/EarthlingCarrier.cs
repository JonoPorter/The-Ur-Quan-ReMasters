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
using AdvanceMath; 
using AdvanceSystem;
using ReMasters.SuperMelee.Weapons; 
using ReMasters.SuperMelee.Controlers;
using ReMasters.SuperMelee.Effects;
using ReMasters.SuperMelee.Actions;

namespace ReMasters.SuperMelee.Ships
{


    public class EarthlingCarrierShipLoader : ShipLoader
    {
        public EarthlingCarrierShipLoader() : base("Earthling Carrier") { }
        protected override IShip CreateHardCodedShip()
        {
            return EarthlingCarrier.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class EarthlingCarrier : Ship
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
                new Bounded<float>(TimeWarp.ScaleTurning(6)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(3, 4)),
                new Bounded<float>(TimeWarp.ScaleVelocity(26)));

            DefaultState = new ShipState(new Bounded<float>(18),
                new Bounded<float>(18),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(8, 1)));
            DefaultActions.Add(EarthlingCarrierPrimary.Create());
            DefaultActions.Add(EarthlingCarrierSecondary.Create());
            DefaultActions.Add(EarthlingCarrierTrinary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("EarthlingCruiserDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();

            Vector2D[] pods = Polygon2D.FromRectangle(20, 70);
            Vector2D[] mainhullp = Polygon2D.FromRectangle(30, 140);
            Vector2D[] subhullp = Polygon2D.FromRectangle(40, 20);

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI + .7f, new Vector2D(-35, 22)), subhullp));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI - .7f, new Vector2D(-35, -22)), subhullp));
            coes.Add(DefaultCoefficients);



            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-50, 35)), pods));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-50, -35)), pods));
            coes.Add(DefaultCoefficients);



            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(95, 0)), Polygon2D.FromNumberofSidesAndRadius(4, 25)));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(15f, 0)), mainhullp));
            coes.Add(DefaultCoefficients);





            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(30, 0)), Polygon2D.FromRectangle(80, 50)));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(120, 0)), Polygon2D.FromRectangle(50, 50)));
            coes.Add(DefaultCoefficients);


            DefaultShape = new RigidBodyTemplate(18, 4077.711f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();

            //DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            EarthlingCarrier returnvalue = new EarthlingCarrier(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Earthling Carrier", new CreateShipDelegate(Create));
        }*/
        protected EarthlingCarrier(PhysicsState state, FactionInfo factionInfo)
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
    public class EarthlingCarrierPrimary : RayAction
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

            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Other);
            DefaultCost = new Costs(new ShipStateChange(0, 2, 0, 0), null, null);
            DefaultRange = TimeWarp.ScaleRange(20);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(
                    DefaultTargetingTypes,
                    EffectTypes.None,
                    new EffectSounds(),
                    new ShipStateChange(-.25f, 0, 0, 0)
                    ));
            DefaultLifeTime = new LifeSpan(TimeWarp.RateToTime(0));
            DefaultActionSounds = new ActionSounds("Laser2", null, null);
            DefaultEffectCollection.AttachmentFlags |= EffectAttachmentFlags.ClonedAttachment;
        }
        static DirectedRayWeapon CreateWeapon()
        {
            return new DirectedRayWeapon(
                (LifeSpan)DefaultLifeTime.Clone(),
                new WeaponsLogic(DefaultTargetingTypes,
                new EffectCollection(DefaultEffectCollection)),
                new float[] { 100, 100, 100, 100 },
                new float[] { .1f, .125f, .15f, .175f },
                new float[] { 0, 0, 0, 0 },
                new float[] { DefaultRange, DefaultRange, DefaultRange, DefaultRange });
        }
        public static EarthlingCarrierPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new EarthlingCarrierPrimary();
        }

        EarthlingCarrierPrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           CreateWeapon())
        { }

    }
    [Serializable]
    public class EarthlingCarrierSecondary : GunPointDefence
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static Costs DefaultCost;

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static ActionSounds DefaultActionSounds;


        public static float DefaultRadius = 900;
        public static int DefaultMaxNumberofTargets = 10;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultState = new ShipState(
                new Bounded<float>(2),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0));
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(96)));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(10.5f, 96));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(2));
            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Enemy);
            DefaultEffectsWho = TargetingInfo.All;
            DefaultActionSounds = new ActionSounds("Gun3", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(.01f, 7),
                new IGeometry2D[] { new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(10, 7)) },
                new Coefficients[] { coe });
            //DefaultShape.BalanceBody();
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
        public static EarthlingCarrierSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new EarthlingCarrierSecondary();
        }

        EarthlingCarrierSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           DefaultActionSounds,
           DefaultRadius,
           DefaultMaxNumberofTargets,
            CreateWeapon())
        { }
    }
    [Serializable]
    public class EarthlingCarrierTrinary : GunAction
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

            DefaultCost = new Costs(new ShipStateChange(1, 4, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(9));
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);
            DefaultEffectsWho = new TargetingInfo(TargetingTypes.All, TargetingTypes.None, TargetingTypes.None);
            DefaultActionSounds = new ActionSounds("launchFighters", null, null);
        }
        static ISolidWeapon CreateWeapon()
        {
            ISolidWeapon w = EarthlingFighter.Create();
            w.ControlHandler = new DefaultControlHandler();
            w.AddControler(null, new MannedOrbitControler(TargetingInfo.FromRequireAll(TargetingTypes.Enemy | TargetingTypes.Ship), TimeWarp.ScaleRange(2)));
            w.AddControler(null, new FireAllControler());
            return w;
        }
        public static EarthlingCarrierTrinary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new EarthlingCarrierTrinary();
        }

        EarthlingCarrierTrinary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           true,
           DefaultActionSounds,
           0,
           0,
           CreateWeapon()
           )
        { }
    }
    [Serializable]
    public class EarthlingFighter : Ship
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

            DefaultActions.Add(EarthlingFighterPrimary.Create());

            DefaultControlableSounds = new ControlableSounds();
            DefaultShipSounds = new ShipSounds();
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();

            Vector2D[] pods = Polygon2D.FromRectangle(2, 5);
            Vector2D[] mainhullp = Polygon2D.FromRectangle(3, 7);
            Vector2D[] subhullp = Polygon2D.FromRectangle(4, 2);

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI + .7f, new Vector2D(-3, 2)), subhullp));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI - .7f, new Vector2D(-3, -2)), subhullp));
            coes.Add(DefaultCoefficients);



            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-3, 3)), pods));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(-3, -3)), pods));
            coes.Add(DefaultCoefficients);





            goes.Add(new Polygon2D(new ALVector2D(MathHelper.PI, new Vector2D(0, 0)), mainhullp));
            coes.Add(DefaultCoefficients);




            DefaultShape = new RigidBodyTemplate(.01f, 9.609863f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();

            DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static EarthlingFighter Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new EarthlingFighter();
        }

        EarthlingFighter()
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
    public class EarthlingFighterPrimary : GunPointDefence
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static Costs DefaultCost;

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static ActionSounds DefaultActionSounds;


        public static float DefaultRadius = 400;
        public static int DefaultMaxNumberofTargets = 1;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultState = new ShipState(new Bounded<float>(1), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultMovementInfo = new ShipMovementInfo(new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(96)));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(10.5f, 96));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Enemy);
            DefaultEffectsWho = TargetingInfo.All;
            DefaultActionSounds = new ActionSounds("Gun3", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            DefaultShape = new RigidBodyTemplate(MassInertia.FromSolidCylinder(.005f, 4),
                new IGeometry2D[] { new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(10, 4)) },
                new Coefficients[] { coe });
            //DefaultShape.BalanceBody();
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
        public static EarthlingFighterPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new EarthlingFighterPrimary();
        }

        EarthlingFighterPrimary()
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
