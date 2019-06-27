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
using Color = System.Drawing.Color;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Ships
{


    public class MyconPodshipShipLoader : ShipLoader
    {
        public MyconPodshipShipLoader() : base("Mycon Podship") { }
        protected override IShip CreateHardCodedShip()
        {
            return MyconPodship.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class MyconPodship : Ship
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
                new Bounded<float>(TimeWarp.ScaleAcceleration(9, 6)),
                new Bounded<float>(TimeWarp.ScaleVelocity(27)));
            DefaultState = new ShipState(new Bounded<float>(20),
                new Bounded<float>(40),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(4, 1)));

            DefaultActions.Add( MyconPodshipPrimary.Create());
            DefaultActions.Add( MyconPodshipSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null,"ShipDies");
            DefaultShipSounds = new ShipSounds("MyconPodshipDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            IGeometry2D mainhull = new Polygon2D(new ALVector2D(0, new Vector2D(35, 0)),  Polygon2D.FromRectangle(50, 20));
            IGeometry2D engine = new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(10, 40));
            DefaultShape = new RigidBodyTemplate(18, 1239.4170508254688f, new IGeometry2D[] { mainhull, engine, }, new Coefficients[] { DefaultCoefficients, DefaultCoefficients });
            DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            MyconPodship returnvalue = new MyconPodship(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Mycon Podship", new CreateShipDelegate(Create));
        }*/
        protected MyconPodship(PhysicsState state, FactionInfo factionInfo)
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
    public class MyconPodshipPrimary : GunAction
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


        public static float DefaultInitialRadius;
        public static float DefaultExpansionRate;
        public static float DefaultMass;

        public static int[] DefaultPodColors = new int[] { Color.White.ToArgb(), Color.White.ToArgb() };
        public static int DefaultPodPrimaryColor = Color.LightBlue.ToArgb();

        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultMass = .5f;
            DefaultInitialRadius = 20;
            DefaultExpansionRate = 7;

            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(1)),
                new Bounded<float>(1000),
                new Bounded<float>(TimeWarp.ScaleVelocity(35)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(5));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(60, 35));
            DefaultState = new ShipState(new Bounded<float>(4), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultCost = new Costs(new ShipStateChange(0, 20, 0, 0), null, null);
            DefaultEffectCollection.Effects.Add(new DiminishingShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-10, 0, 0, 0)));
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy|TargetingTypes.Ship, TargetingTypes.None);
            DefaultActionSounds = new ActionSounds("DeepEcho",null,null);
        }

        public static MyconPodshipPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new MyconPodshipPrimary();
        }

        static ISolidWeapon CreateWeapon()
        {
            ISolidWeapon w = new ControlableWave(
                (LifeSpan)DefaultLifeTime.Clone(),
                DefaultMass,
                new PhysicsState(), DefaultInitialRadius,
                DefaultExpansionRate,
                DefaultPodColors,
                DefaultPodPrimaryColor,
                new ShipMovementInfo(DefaultMovementInfo),
                new ShipState(DefaultState),
                new ControlableSounds(),
                new WeaponsLogic(TargetingInfo.All,
                new EffectCollection(DefaultEffectCollection)));
            w.ControlHandler = new DefaultControlHandler();
            w.AddControler(null,new MissileControler(TargetingInfo.FromRequireAll(TargetingTypes.Enemy | TargetingTypes.Ship)));
            w.Flags = w.Flags & ~BodyFlags.NoImpulse;
            return w;
        }
         MyconPodshipPrimary()
            : base(new Bounded<float>(DefaultDelay),
            TargetingInfo.All,
            new Costs(DefaultCost),
            false,
            DefaultActionSounds,
            0,
            0,
            CreateWeapon())
        { }

    }
    [Serializable]
    public class MyconPodshipSecondary : BaseAction
    {
        public static Costs DefaultCost;
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(-4, 40, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(30));
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds("DeepBirdCall", null, null);
        }
        public static MyconPodshipSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new MyconPodshipSecondary();
        }

        MyconPodshipSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds)
        {
            this.aIInfo = new RechargeActionAIInfo(DefaultCost.ActivationCost);
        }
        MyconPodshipSecondary(MyconPodshipSecondary copy) : base(copy) { }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            return true;
        }

        public override object Clone()
        {
            return new MyconPodshipSecondary(this);
        }
    }
    public class MyconShipStateEffect : DiminishingShipStateEffect
    {
        ControlableWave wave;
        public MyconShipStateEffect(TargetingInfo effectsWho, EffectTypes harmfulEffectTypes, EffectSounds effectSounds, ShipStateChange ssc)
            : base(effectsWho, harmfulEffectTypes, effectSounds, ssc)
        {}
        public MyconShipStateEffect(DiminishingShipStateEffect copy)
            : base(copy)
        {}
        public override void OnCreation(IWeaponsLogic weaponInfo)
        {
            wave = (ControlableWave)weaponInfo.SolidHost;
            base.OnCreation(weaponInfo);
        }
        public override void ApplyEffect(GameResult gameResult, float dt)
        {
            float originalsum = ssc.SumValue;
            if (originalsum == 0)
            {
                 base.ApplyEffect(gameResult, dt);
                 return;
            }
            base.ApplyEffect(gameResult, dt);
            float timediff = (ssc.SumValue / originalsum) * currentTime;
            wave.UpdateRadius(timediff);
            currentTime -= timediff;
        }
        public override void Update(float dt)
        {
            if (!Exhausted)
            {
                float inv = (dt / currentTime); ;
                ssc -= ssc * inv;
                currentTime -= dt;
                Exhausted = currentTime <= 0;
                if (wave.ShipState.Health.Value < ssc.Health && ssc.Health > 0)
                {
                    float timediff = (wave.ShipState.Health.Value / ssc.Health) * currentTime;
                    wave.UpdateRadius(timediff);
                    currentTime -= timediff;
                    ssc.Health = wave.ShipState.Health.Value;
                }
            }
            base.Update(dt);
        }
        public override object Clone()
        {
            return new MyconShipStateEffect(this);
        }
    }
}
#endif
