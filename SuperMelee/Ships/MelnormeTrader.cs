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
using Physics2D;
using Physics2D.CollidableBodies;
using System.Collections.Generic;
using AdvanceMath.Geometry2D;
using AdvanceMath; using AdvanceSystem;
using ReMasters.SuperMelee.Weapons; using ReMasters.SuperMelee.Controlers;using ReMasters.SuperMelee.Effects;using ReMasters.SuperMelee.Actions;
namespace ReMasters.SuperMelee.Ships
{

    public class MelnormeTraderShipLoader : ShipLoader
    {
        public MelnormeTraderShipLoader() : base("Melnorme Trader") { }
        protected override IShip CreateHardCodedShip()
        {
            return MelnormeTrader.Create(new PhysicsState(), null);
        }
    }

    [Serializable]
    public class MelnormeTrader : Ship
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
                new Bounded<float>(TimeWarp.ScaleAcceleration(6, 1)),
                new Bounded<float>(TimeWarp.ScaleVelocity(36)));
            DefaultState = new ShipState(new Bounded<float>(20),
                new Bounded<float>(42),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(4, 1)));



            DefaultActions.Add(MelnormeTraderPrimary.Create());
            DefaultActions.Add(MelnormeTraderSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("MelnormeTraderDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;

            Vector2D[] engineconevertecies = new Vector2D[]
            {
                new Vector2D(30,10),
                new Vector2D(-30,40),
                new Vector2D(-30,-40),
                new Vector2D(30,-10)
            };
            Vector2D offset = Polygon2D.CalcCentroid(engineconevertecies);
            OperationHelper.ArrayRefOp<Vector2D, Vector2D, Vector2D>(
                engineconevertecies,
                ref offset,
                engineconevertecies,
                Vector2D.Subtract);

            //engineconevertecies = Vector2D.Translate(-offset, engineconevertecies);

            IGeometry2D mainhull = new Polygon2D(ALVector2D.Zero, Polygon2D.FromRectangle(80, 10));
            IGeometry2D engine = new Polygon2D(new ALVector2D(0, new Vector2D(-30, 0)), Polygon2D.FromNumberofSidesAndRadius(10, 40));
            IGeometry2D enginecone = new Polygon2D(new ALVector2D(0, offset), engineconevertecies);
            DefaultShape = new RigidBodyTemplate(12, 1231.9384791047398f, new IGeometry2D[] { engine, mainhull, enginecone }, new Coefficients[] { DefaultCoefficients, DefaultCoefficients, DefaultCoefficients });
            DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            MelnormeTrader returnvalue = new MelnormeTrader(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Melnorme Trader", new CreateShipDelegate(Create));
        }*/
        protected MelnormeTrader(PhysicsState state, FactionInfo factionInfo)
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
    public class MelnormeTraderPrimary : BuildUpGunAction
    {
        public static LifeSpan[] DefaultLifeTimes;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState[] DefaultState;
        public static Costs DefaultCost;
        public static EffectCollection[] DefaultEffectCollections;

        public static Bounded<float> DefaultDelay;
        public static Bounded<float> DefaultBuildUpDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(120)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(1));
            DefaultBuildUpDelay = new Bounded<float>(2);
            int length = 4;
            DefaultCost = new Costs(new ShipStateChange(0, 5, 0, 0), new ShipStateChange(0, TimeWarp.RechargeRateToPerSeconds(4, 1), 0, 0), new ShipStateChange(0, 0, 0, 0));
            DefaultState = new ShipState[length];
            DefaultLifeTimes = new LifeSpan[length];
            DefaultEffectCollections = new EffectCollection[length];
            EffectSounds sounds = null;
            for (int pos = 0; pos < length; ++pos)
            {
                switch (pos)
                {
                    case 1:
                        sounds = new EffectSounds();
                        break;
                    case 2:
                        sounds = new EffectSounds("Boom35", null, null);
                        break;
                    case 3:
                        sounds = new EffectSounds();
                        break;
                    default:
                        sounds = new EffectSounds();
                        break;
                }
                DefaultEffectCollections[pos] = new EffectCollection();
                DefaultEffectCollections[pos].Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, sounds, new ShipStateChange(-(float)Math.Pow(2, pos + 1), 0, 0, 0)));
                DefaultState[pos] = new ShipState(new Bounded<float>((float)Math.Pow(2, pos + 1)), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
                DefaultLifeTimes[pos] = new LifeSpan(TimeWarp.RangeToTime(21 + 3 * pos, 112));
            }
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds(null, null, "EnergyGun2");
        }
        public static MelnormeTraderPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new MelnormeTraderPrimary();
        }

        static ISolidWeapon[] CreateWeapons()
        {
            Coefficients coe = TimeWarp.Coefficients;
            int count = 4;
            ISolidWeapon[] weapons = new ISolidWeapon[count];
            for (int pos = 0; pos < count; ++pos)
            {
                float radius = 5 + 5 * pos;
                weapons[pos] = new Controlable(
                        DefaultLifeTimes[pos],
                        MassInertia.FromSolidCylinder(.1f, radius),
                        new PhysicsState(),
                        DefaultBodyFlags,
                        new ICollidableBodyPart[] { new RigidBodyPart(ALVector2D.Zero, (Polygon2D)Polygon2D.FromNumberofSidesAndRadius(8, radius), coe) },
                        new ShipMovementInfo(DefaultMovementInfo),
                        new ShipState(DefaultState[pos]),
                        new ControlableSounds("EnergyChargeUp", null),
                        new WeaponsLogic(TargetingInfo.All,
                        new EffectCollection(DefaultEffectCollections[pos])));
            }
            return weapons;
        }
        MelnormeTraderPrimary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds,
           new Bounded<float>(DefaultBuildUpDelay),
           0,
           0,
           CreateWeapons())
        { }
    }
    [Serializable]
    public class MelnormeTraderSecondary : GunAction
    {
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static LifeSpan ConfusedLifeTime;

        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(120)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(20));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(45, 120));
            DefaultState = new ShipState(new Bounded<float>(99), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            DefaultCost = new Costs(new ShipStateChange(0, 20, 0, 0), null, null);


            DefaultEffectCollection.ProlongedEffects.Add(
                new ControlerEffect(
                TargetingInfo.All,
                EffectTypes.Controls,
                new EffectSounds(),
                new LifeSpan(12),
                new ConfusionControler(new LifeSpan(12))
                ));
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds("EnergyGun2", null, null);
        }
        public static MelnormeTraderSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new MelnormeTraderSecondary();
        }

        static ISolidWeapon CreateWeapon()
        {
            Coefficients coe = TimeWarp.Coefficients;
            float radius = 20;
            return new Controlable(
                DefaultLifeTime,
                MassInertia.FromSolidCylinder(1, radius),
                new PhysicsState(),
                DefaultBodyFlags,
                new ICollidableBodyPart[] { new RigidBodyPart(ALVector2D.Zero, (Polygon2D)Polygon2D.FromNumberofSidesAndRadius(8, radius), coe) },
                new ShipMovementInfo(DefaultMovementInfo),
                new ShipState(DefaultState),
                new ControlableSounds(),
                new WeaponsLogic(TargetingInfo.All,
                new EffectCollection(DefaultEffectCollection)));
        }
        MelnormeTraderSecondary()
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
}
#endif
