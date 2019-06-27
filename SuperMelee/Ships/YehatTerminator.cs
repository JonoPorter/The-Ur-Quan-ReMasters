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
    public class YehatTerminatorShipLoader : ShipLoader
    {
        public YehatTerminatorShipLoader() : base("Yehat Terminator") { }
        protected override IShip CreateHardCodedShip()
        {
            return YehatTerminator.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class YehatTerminator : Ship 
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
                new Bounded<float>(TimeWarp.ScaleAcceleration(6, 2)),
                new Bounded<float>(TimeWarp.ScaleVelocity(30)));
            DefaultState = new ShipState(new Bounded<float>(20),
                new Bounded<float>(10), 
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(6,2)));

            DefaultActions.Add( YehatTerminatorPrimary.Create());
            DefaultActions.Add( YehatTerminatorSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null,"ShipDies");
            DefaultShipSounds = new ShipSounds("YehatTerminatorDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();

            Vector2D[] mainhullvertecies = new Vector2D[]
                {
                    new Vector2D(40,0),
                    new Vector2D(20,10),
                    new Vector2D(20,15),
                    new Vector2D(-38,20),
                    new Vector2D(-38,-20),
                    new Vector2D(20,-15),
                    new Vector2D(20,-10)
                };
            Vector2D[] leftWingvertecies = new Vector2D[]
                {
                    new Vector2D(40,-70),
                    new Vector2D(10,20),
                    new Vector2D(-30,10),
                    new Vector2D(0,-50)
                };
            int length = leftWingvertecies.Length;
            Vector2D[] RightWingvertecies = new Vector2D[]
                {
                    new Vector2D(40,70),
                    new Vector2D(0,50),
                    new Vector2D(-30,-10),
                    new Vector2D(10,-20)
                };
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-10, -20)), leftWingvertecies));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-10, 20)), RightWingvertecies));
            coes.Add(DefaultCoefficients);
            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(0, 0)), mainhullvertecies));
            coes.Add(DefaultCoefficients);
            DefaultShape = new RigidBodyTemplate(18, 1807.1936f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();
            //DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }
            YehatTerminator returnvalue = new YehatTerminator(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Yehat Terminator", new CreateShipDelegate(Create));
        }*/
        protected YehatTerminator(PhysicsState state, FactionInfo factionInfo)
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
    public class YehatTerminatorPrimary : GunAction
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

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            InitShape();
            DefaultState = new ShipState(new Bounded<float>(1), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultMovementInfo = new ShipMovementInfo(new Bounded<float>(0), 
                new Bounded<float>(0), 
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(80)));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(12, 80));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultEffectsWho = TargetingInfo.All;
            DefaultActionSounds = new ActionSounds("EnergyGun3", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(6,6));
            DefaultShape = new RigidBodyTemplate(.001f, 9.012392754810497f, new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
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
        public static YehatTerminatorPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new YehatTerminatorPrimary();
        }

         YehatTerminatorPrimary()
            : base(new Bounded<float>(DefaultDelay),
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            false,
            DefaultActionSounds,
            new float[] { 0,  0 },
            new float[] { .9f, -.9f },
            new ISolidWeapon[] { CreateWeapon(), CreateWeapon() })
        { }
    }
    [Serializable]
    public class YehatTerminatorSecondary : InstantAction
    {
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


            DefaultTargetingTypes = TargetingInfo.None;
            DefaultEffectsWho = DefaultTargetingTypes;
            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), new ShipStateChange(0, TimeWarp.RechargeRateToPerSeconds(2, 3), 0, 0), null);
            DefaultEffectCollection.ProlongedEffects.Add(new BlockingSheild(DefaultEffectsWho, EffectTypes.None, new EffectSounds(), new LifeSpan(TimeWarp.RateToTime(2)), EffectTypes.Health));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(2));
            DefaultActionSounds = new ActionSounds(new MeleeSound("Shield2"), new MeleeSound("Shield2", new Bounded<float>(.3f)), null);
        }
         static IWeapon CreateWeapon()
        {
            return new InstantWeapon(
                new WeaponsLogic(DefaultEffectsWho,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static YehatTerminatorSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new YehatTerminatorSecondary();
        }
         YehatTerminatorSecondary()
            : base(new Bounded<float>(DefaultDelay), 
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            DefaultActionSounds,
            CreateWeapon())
        {
            this.aIInfo = new ShieldActionAIInfo();
         }
    }
}
#endif
