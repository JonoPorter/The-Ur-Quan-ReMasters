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


    public class IlwrathAvengerShipLoader : ShipLoader
    {
        public IlwrathAvengerShipLoader() : base("Ilwrath Avenger") { }
        protected override IShip CreateHardCodedShip()
        {
            return IlwrathAvenger.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class IlwrathAvenger : Ship 
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
                new Bounded<float>(TimeWarp.ScaleTurning(2)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(5, 0)),
                new Bounded<float>(TimeWarp.ScaleVelocity(25)));
            DefaultState = new ShipState(new Bounded<float>(22),
                new Bounded<float>(16), 
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(4, 4)));

            DefaultActions.Add( IlwrathAvengerPrimary.Create());
            DefaultActions.Add( IlwrathAvengerSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null,"ShipDies");
            DefaultShipSounds = new ShipSounds("IlwrathAvengerDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();

            Vector2D[] mainhullvertecies = new Vector2D[]
                {
                    new Vector2D(40,0),
                    //new Vector2D(20,10),
                    //new Vector2D(20,15),
                    new Vector2D(-8,20),
                    new Vector2D(-8,-20),
                    //new Vector2D(20,-15),
                    //new Vector2D(20,-10)
                };
            Vector2D[] leftWingvertecies = new Vector2D[]
                {
                    new Vector2D(10,20),
                    new Vector2D(-30,10),
                    new Vector2D(0,-50),
                    new Vector2D(40,-80)
                };
            int length = leftWingvertecies.Length;
            Vector2D[] RightWingvertecies = new Vector2D[]
                {
                    new Vector2D(40,80),
                    new Vector2D(0,50),
                    new Vector2D(-30,-10),
                    new Vector2D(10,-20)
                };

            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-10, -20)), leftWingvertecies));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-10, 20)), RightWingvertecies));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(30, 0)), Polygon2D.FromRectangle(20, 120)));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(80, 0)), mainhullvertecies));
            coes.Add(DefaultCoefficients);
            DefaultShape = new RigidBodyTemplate(11, 1129.1553811562187f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            IlwrathAvenger returnvalue = new IlwrathAvenger(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }

        protected IlwrathAvenger(PhysicsState state, FactionInfo factionInfo)
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
        {}
    }
    [Serializable]
    public class IlwrathAvengerPrimary : GunAction
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

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration),
                new Bounded<float>(TimeWarp.ScaleTurning(4)),
                new Bounded<float>(TimeWarp.ScaleAcceleration(2, 1)),
                new Bounded<float>(TimeWarp.ScaleVelocity(28)));
            DefaultState = new ShipState(new Bounded<float>(1),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(2.8f, 28));


            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.None, TargetingTypes.Enemy | TargetingTypes.Ship, TargetingTypes.None);

            DefaultInitialRadius = 3;
            DefaultExpansionRate = 5 + TimeWarp.ScaleRange(5);
            DefaultMass = .1f;
            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultActionSounds = new ActionSounds("FlameThrower", null, "Uncloak");

        }
         static ISolidWeapon CreateWeapon()
        {
            return new ControlableWave(
            (LifeSpan)DefaultLifeTime.Clone(), 
            DefaultMass, 
            new PhysicsState(), DefaultInitialRadius, 
            DefaultExpansionRate, 
            TimeWarp.DefaultExposionColors,
            TimeWarp.DefaultExplosionPrimaryColor, 
            DefaultMovementInfo, 
            DefaultState, 
            new ControlableSounds(), 
            new WeaponsLogic(
            TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static IlwrathAvengerPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new IlwrathAvengerPrimary();
        }

         IlwrathAvengerPrimary()
            : base(new Bounded<float>(DefaultDelay), 
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            false,
            DefaultActionSounds,
            0, 
            0,
            CreateWeapon())
        {}
        IlwrathAvengerPrimary(IlwrathAvengerPrimary copy) : base(copy) { }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            bool invisible = source.IsInvisible;
            source.IsInvisible = false;
            if (invisible)
            {
                this.actionSounds.DeActivated.Play();
                this.target = source.TargetRetriever.NextClosest(this.TargetableTypes, source.Current.Position.Linear);
                if (this.target != null)
                {
                    Vector2D diff = target.Current.Position.Linear - source.Current.Position.Linear;
                    source.Current.Position.Angular = diff.Angle;
                    source.SetAllPositions();
                }
            }
            return base.OnActivated(actionResult, dt);
        }
        public override object Clone()
        {
            return new IlwrathAvengerPrimary(this);
        }
    }
    [Serializable]
    public class IlwrathAvengerSecondary : BaseAction
    {
        public static Costs DefaultCost;
        public static Bounded<float>  DefaultDelay;
        public static TargetingInfo  DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(0, 3, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(7));
            DefaultTargetingTypes =  TargetingInfo.None;
            DefaultActionSounds = new ActionSounds("Cloak", null, "Uncloak");
        }
        public static IlwrathAvengerSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new IlwrathAvengerSecondary();
        }

         IlwrathAvengerSecondary()
            : base(new Bounded<float>(DefaultDelay), 
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            false,
            DefaultActionSounds)
        {
            this.aIInfo = new CloakingActionAIInfo(DefaultCost.ActivationCost);
        }
        IlwrathAvengerSecondary(IlwrathAvengerSecondary copy) : base(copy) { }


        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            bool invisible = !source.IsInvisible;
            source.IsInvisible = invisible;
            actionResult.PlaySound = invisible;
            actionResult.ApplyCosts = invisible;
            if (invisible)
            {
                this.actionSounds.DeActivated.Play();
            }
            return true;
        }
        public override object Clone()
        {
            return new IlwrathAvengerSecondary(this);
        }
    }
}
#endif
