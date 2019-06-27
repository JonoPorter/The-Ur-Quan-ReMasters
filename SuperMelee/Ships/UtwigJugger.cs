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

    public class UtwigJuggerShipLoader : ShipLoader
    {
        public UtwigJuggerShipLoader() : base("Utwig Jugger") { }
        protected override IShip CreateHardCodedShip()
        {
            return UtwigJugger.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class UtwigJugger : Ship
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
                new Bounded<float>(TimeWarp.ScaleAcceleration(6, 6)),
                new Bounded<float>(TimeWarp.ScaleVelocity(36)));
            DefaultState = new ShipState(new Bounded<float>(20),
                new Bounded<float>(0, 10, 20, false),
                new Bounded<float>(0),
                new Bounded<float>(0));
            DefaultActions.Add(UtwigJuggerPrimary.Create());
            DefaultActions.Add(UtwigJuggerSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("UtwigJuggerDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;

            Vector2D[] noseconevertecies = new Vector2D[]
            {
                new Vector2D(10,10),
                new Vector2D(0,20),
                new Vector2D(-10,25),
                new Vector2D(-10,-25),
                new Vector2D(0,-20),
                new Vector2D(10,-10),
            };
            Vector2D[] mainhullvertecies = new Vector2D[]
            {
                new Vector2D(10,25),
                new Vector2D(0,40),

                new Vector2D(-10,40),
                new Vector2D(-40,20),
                new Vector2D(-40,-20),
                new Vector2D(-10,-40),

                new Vector2D(0,-40),
                new Vector2D(10,-25)
            };
            IGeometry2D mainhull = new Polygon2D(ALVector2D.Zero, mainhullvertecies);
            IGeometry2D nosecone = new Polygon2D(new ALVector2D(0, new Vector2D(20.5f, 0)), noseconevertecies);
            DefaultShape = new RigidBodyTemplate(20, 654.58713987691476f, new IGeometry2D[] { mainhull, nosecone }, new Coefficients[] { DefaultCoefficients, DefaultCoefficients });
            DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            UtwigJugger returnvalue = new UtwigJugger(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Utwig Jugger", new CreateShipDelegate(Create));
        }*/

        protected UtwigJugger(PhysicsState state, FactionInfo factionInfo)
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
    public class UtwigJuggerPrimary : GunAction
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
                new Bounded<float>(TimeWarp.ScaleVelocity(120)));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(14, 120));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(7));
            DefaultCost = new Costs(new ShipStateChange(0, 0, 0, 0), null, null);

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-1, 0, 0, 0)));
            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Enemy);
            DefaultEffectsWho = TargetingInfo.All;
            DefaultActionSounds = new ActionSounds("EnergyGun1", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(5,1),
                new Vector2D(-5,2),
                new Vector2D(-5,-2),
                new Vector2D(5,-1)
            };
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, bulletverticies);
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
        public static UtwigJuggerPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new UtwigJuggerPrimary();
        }

        UtwigJuggerPrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           new float[] { 0, 0, 0, 0, 0, 0 },
           new float[] { .8f, .4f, .2f, -.2f, -.4f, -.8f },
           new ISolidWeapon[] { CreateWeapon(), CreateWeapon(), CreateWeapon(), CreateWeapon(), CreateWeapon(), CreateWeapon() })
        { }
    }
    [Serializable]
    public class UtwigJuggerSecondary : InstantAction
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

            DefaultTargetingTypes = new TargetingInfo(TargetingTypes.Self);
            DefaultEffectsWho = DefaultTargetingTypes;
            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), new ShipStateChange(0, 1, 0, 0), null);
            DefaultEffectCollection.ProlongedEffects.Add(new AbsorptionSheild(DefaultEffectsWho, EffectTypes.None, new EffectSounds(), new LifeSpan(TimeWarp.RateToTime(7)), EffectTypes.Health, 1));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(0));
            DefaultActionSounds = new ActionSounds(new MeleeSound("Shield1"), new MeleeSound("Shield1", new Bounded<float>(.3f)), null);
        }
        static IWeapon CreateWeapon()
        {

            return new InstantWeapon(
                new WeaponsLogic(DefaultEffectsWho,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static UtwigJuggerSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new UtwigJuggerSecondary();
        }
        UtwigJuggerSecondary()
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
