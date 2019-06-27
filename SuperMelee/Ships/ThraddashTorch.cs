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


    public class ThraddashTorchShipLoader : ShipLoader
    {
        public ThraddashTorchShipLoader() : base("Thraddash Torch") { }
        protected override IShip CreateHardCodedShip()
        {
            return ThraddashTorch.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class ThraddashTorch : Ship
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
                new Bounded<float>(TimeWarp.ScaleAcceleration(7, 3)),
                new Bounded<float>(TimeWarp.ScaleVelocity(18)));
            DefaultState = new ShipState(new Bounded<float>(8),
                new Bounded<float>(28),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(6, 1)));
            DefaultActions.Add(ThraddashTorchPrimary.Create());
            DefaultActions.Add(ThraddashTorchSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("ThraddashTorchDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();
            Vector2D[] hullvertecies = new Vector2D[]
            {
                new Vector2D(30,13),
                new Vector2D(0,18),
                new Vector2D(-30,20),
                new Vector2D(-30,-20),
                new Vector2D(0,-18),
                new Vector2D(30,-13)
            };
            goes.Add(new Polygon2D(new ALVector2D(0, hullvertecies[0]), Polygon2D.FromNumberofSidesAndRadius(8, 12)));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, hullvertecies[5]), Polygon2D.FromNumberofSidesAndRadius(8, 12)));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, hullvertecies[2]), Polygon2D.FromNumberofSidesAndRadius(8, 12)));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, hullvertecies[3]), Polygon2D.FromNumberofSidesAndRadius(8, 12)));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(ALVector2D.Zero, hullvertecies));
            coes.Add(DefaultCoefficients);
            DefaultShape = new RigidBodyTemplate(7, 771.453f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();
            //DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            ThraddashTorch returnvalue = new ThraddashTorch(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Thraddash Torch", new CreateShipDelegate(Create));
        }*/
        protected ThraddashTorch(PhysicsState state, FactionInfo factionInfo)
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
        {
        }

    }
    [Serializable]
    public class ThraddashTorchPrimary : GunAction
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
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultMovementInfo = new ShipMovementInfo(new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(TimeWarp.ScaleVelocity(120)));
            DefaultState = new ShipState(new Bounded<float>(2), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultCost = new Costs(new ShipStateChange(0, 2, 0, 0), null, null);
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(25, 120));
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(
                    TargetingInfo.All,
                    EffectTypes.None,
                    new EffectSounds(),
                    new ShipStateChange(-1, 0, 0, 0)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(12));
            DefaultActionSounds = new ActionSounds("Gun1", null, null);
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
        public static ThraddashTorchPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ThraddashTorchPrimary();
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
                new WeaponsLogic(
                    TargetingInfo.All,
                    new EffectCollection(DefaultEffectCollection)));
        }
        ThraddashTorchPrimary()
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
    public class ThraddashTorchSecondary : GunAction
    {
        public static Costs DefaultCost;
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static float DefaultInitialRadius;
        public static float DefaultExpansionRate;
        public static float DefaultMass;
        public static LifeSpan DefaultLifeTime;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;

        public static EffectCollection DefaultEffectCollection = new EffectCollection();
        public static ActionSounds DefaultActionSounds;

        public static int[] DefaultExposionColors = new int[] { Color.Red.ToArgb(), Color.Red.ToArgb(), Color.Red.ToArgb(), Color.Yellow.ToArgb() };
        public static int DefaultExplosionPrimaryColor = Color.Orange.ToArgb();
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleAcceleration(30, 0)),
                new Bounded<float>(TimeWarp.ScaleVelocity(80)));
            DefaultState = new ShipState(new Bounded<float>(2),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0));
            DefaultLifeTime = new LifeSpan(3);
            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));

            DefaultTargetingTypes = TargetingInfo.None;
            DefaultInitialRadius = 20;
            DefaultExpansionRate = 6;
            DefaultMass = .8f;
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(
                    TargetingInfo.All,
                    EffectTypes.None,
                    new EffectSounds("Boom23", null, null),
                    new ShipStateChange(-2, 0, 0, 0)));
            DefaultActionSounds = new ActionSounds("FlameThrower3", null, null);
        }
        static ISolidWeapon CreateWeapon()
        {
            return new ControlableWave(
            (LifeSpan)DefaultLifeTime.Clone(),
            DefaultMass,
            new PhysicsState(), DefaultInitialRadius,
            DefaultExpansionRate,
            DefaultExposionColors,
            DefaultExplosionPrimaryColor,
            DefaultMovementInfo,
            DefaultState,
            new ControlableSounds(),
            new WeaponsLogic(
            TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static ThraddashTorchSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ThraddashTorchSecondary();
        }
        ThraddashTorchSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           MathHelper.PI,
           MathHelper.PI,
           CreateWeapon())
        {
            //this.aIInfo = new ShieldActionAIInfo();
            this.aIInfo = new OffensiveShieldActionAIInfo(DefaultCost.ActivationCost, TimeWarp.ScaleRange(25) / 2);
            //this.aIInfo = new SpecificRangeActionAIInfo(DefaultCost.ActivationCost,0,TimeWarp.ScaleRange( 25));//, 3000);
        
        }
        ThraddashTorchSecondary(ThraddashTorchSecondary copy) : base(copy) { }
        protected override void OnWeaponCreation(ActionResult actionResult, float dt, int index)
        {
            this.CurrentWeapon.ControlHandler = new InertialessControlHandler();
        }
        public override object Clone()
        {
            return new ThraddashTorchSecondary(this);
        }
    }
}
#endif
