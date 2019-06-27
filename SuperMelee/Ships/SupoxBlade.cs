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
    public class SupoxBladeShipLoader : ShipLoader
    {
        public SupoxBladeShipLoader() : base("Supox Blade") { }
        protected override IShip CreateHardCodedShip()
        {
            return SupoxBlade.Create(new PhysicsState(), null);
        }
    }


    [Serializable]
    public class SupoxBlade : Ship
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
                new Bounded<float>(TimeWarp.ScaleAcceleration(16, 1)),
                new Bounded<float>(TimeWarp.ScaleVelocity(40)));
            DefaultState = new ShipState(new Bounded<float>(12),
                new Bounded<float>(12),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(4, 1)));

            DefaultActions.Add(SupoxBladePrimary.Create());
            DefaultActions.Add(SupoxBladeSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null, "ShipDies");
            DefaultShipSounds = new ShipSounds("SupoxBladeDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;


            Vector2D[] mainhullvertecies = new Vector2D[]
                {
                    new Vector2D(60,0),
                    new Vector2D(40,10),
                    new Vector2D(0,18),
                    new Vector2D(-40,13),
                    new Vector2D(-40,-13),
                    new Vector2D(0,-18),
                    new Vector2D(40,-10)
                };

            Vector2D[] mainhull2vertecies = new Vector2D[]
                {
                    new Vector2D(30,0),
                    new Vector2D(20,8),
                    new Vector2D(10,15),
                    new Vector2D(-20,20),
                    new Vector2D(-20,-20),
                    new Vector2D(10,-15),
                    new Vector2D(20,-8)
                };


            IGeometry2D mainhull = new Polygon2D(new ALVector2D(0, new Vector2D(-50, 0)), mainhullvertecies);
            IGeometry2D mainhull2 = new Polygon2D(ALVector2D.Zero, mainhull2vertecies);
            DefaultShape = new RigidBodyTemplate(14, 1026.1685677800115f, new IGeometry2D[] { mainhull2, mainhull, }, new Coefficients[] { DefaultCoefficients, DefaultCoefficients });
            DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            SupoxBlade returnvalue = new SupoxBlade(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo(" Supox Blade", new CreateShipDelegate(Create));
        }*/
        protected SupoxBlade(PhysicsState state, FactionInfo factionInfo)
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
    public class SupoxBladePrimary : GunAction
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
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.ScaleVelocity(120)));
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(1));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(15, 120));
            DefaultState = new ShipState(new Bounded<float>(1), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));

            DefaultCost = new Costs(new ShipStateChange(0, 1, 0, 0), null, null);

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds("Boom1", null, null), new ShipStateChange(-1, 0, 0, 0)));
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds("OrganicGun", null, null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(0,5),
                new Vector2D(-10,0),
                new Vector2D(0,-5),
                new Vector2D(10,0)
            };
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, bulletverticies);
            DefaultShape = new RigidBodyTemplate(.01f, 149.2709355885475f, new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
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
            new WeaponsLogic(TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static SupoxBladePrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new SupoxBladePrimary();
        }

        SupoxBladePrimary()
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
    public class SupoxBladeSecondary : BaseAction
    {
        public static Costs DefaultCost;
        public static Bounded<float> DefaultDelay;
        public static TargetingInfo DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(0, 0, 0, 0), null, null);
            DefaultDelay = new Bounded<float>(0);
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds();
        }
        public static SupoxBladeSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new SupoxBladeSecondary();
        }
        SupoxBladeSecondary()
            : base(new Bounded<float>(DefaultDelay),
           DefaultTargetingTypes,
           new Costs(DefaultCost),
           false,
           DefaultActionSounds)
        { }
        SupoxBladeSecondary(SupoxBladeSecondary copy) : base(copy) { }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            if (this.source.CurrentControlInput[InputAction.MoveForward])
            {
                this.source.CurrentControlInput[InputAction.MoveForward] = false;
                this.source.CurrentControlInput[InputAction.MoveBackwards] = true;
            }
            if (this.source.CurrentControlInput[InputAction.RotateLeft])
            {
                this.source.CurrentControlInput[InputAction.RotateLeft] = false;
                this.source.CurrentControlInput[InputAction.MoveRight] = true;
            }
            if (this.source.CurrentControlInput[InputAction.RotateRight])
            {
                this.source.CurrentControlInput[InputAction.RotateRight] = false;
                this.source.CurrentControlInput[InputAction.MoveLeft] = true;
            }
            this.source.CurrentControlInput.ThrustPercent = 1;
            return true;
        }
        public override object Clone()
        {
            return new SupoxBladeSecondary(this);
        }
    }
}
#endif
