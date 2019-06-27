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


    public class DruugeMaulerShipLoader : ShipLoader
    {
        public DruugeMaulerShipLoader() : base("Druuge Mauler") { }
        protected override IShip CreateHardCodedShip()
        {
            return DruugeMauler.Create(new PhysicsState(), null);
        }
    }



    [Serializable]
    public class DruugeMauler : Ship 
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
                new Bounded<float>(TimeWarp.ScaleAcceleration(2,1)),
                new Bounded<float>(TimeWarp.ScaleVelocity(20)));
            DefaultState = new ShipState(new Bounded<float>(14), 
                new Bounded<float>(32), 
                new Bounded<float>(0),
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(50,1)));

            DefaultActions.Add( DruugeMaulerPrimary.Create());
            DefaultActions.Add( DruugeMaulerSecondary.Create());

            DefaultControlableSounds = new ControlableSounds(null,"ShipDies");
            DefaultShipSounds = new ShipSounds("DruugeMaulerDitty");
        }
        static void InitShape()
        {
            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            List<IGeometry2D> goes = new List<IGeometry2D>();
            List<Coefficients> coes = new List<Coefficients>();
            Vector2D[] engineconevertecies = new Vector2D[]
            {
                new Vector2D(-50,10),
                new Vector2D(-100,20),
                new Vector2D(-100,-20),
                new Vector2D(-50,-10)
            };
            Vector2D offset = Polygon2D.CalcCentroid(engineconevertecies);
            //engineconevertecies = Vector2D.Translate( -offset,engineconevertecies);

            OperationHelper.ArrayRefOp<Vector2D, Vector2D, Vector2D>(
                    engineconevertecies,
                    ref offset,
                    engineconevertecies,
                    Vector2D.Subtract);

            goes.Add(new Polygon2D(ALVector2D.Zero, Polygon2D.FromRectangle(20, 150)));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(0, new Vector2D(-100, 0)), Polygon2D.FromNumberofSidesAndRadius(10, 20)));
            coes.Add(DefaultCoefficients);

            goes.Add(new Polygon2D(new ALVector2D(0, offset), engineconevertecies));
            coes.Add(DefaultCoefficients);
            DefaultShape = new RigidBodyTemplate(18, 3383.9114375372737f, goes.ToArray(), coes.ToArray());
            DefaultShape.BalanceBody();
            ////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }
            DruugeMauler returnvalue = new DruugeMauler(state, factionInfo);
            returnvalue.ControlHandler = new DefaultControlHandler();
            return returnvalue;
        }
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Druuge Mauler", new CreateShipDelegate(Create));
        }*/
        protected DruugeMauler(PhysicsState state, FactionInfo factionInfo)
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
    public class DruugeMaulerPrimary : GunAction
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime;
        public static BodyFlags DefaultBodyFlags = BodyFlags.None;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static Bounded<float>  DefaultDelay;
        public static TargetingInfo  DefaultTargetingTypes;
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
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(10));
            DefaultLifeTime = new LifeSpan(TimeWarp.RangeToTime(40,120));
            DefaultState = new ShipState(new Bounded<float>(4), new Bounded<float>(0), new Bounded<float>(0), new Bounded<float>(0));
            
            DefaultCost = new Costs(new ShipStateChange(0, 4, 0, 0),null,null);

            DefaultEffectCollection.Effects.Add(new ShipStateEffect(TargetingInfo.All, EffectTypes.None, new EffectSounds(), new ShipStateChange(-6, 0, 0, 0)));
            DefaultTargetingTypes = TargetingInfo.None;
        
            DefaultActionSounds = new ActionSounds("Gun2",null,null);
        }
        static void InitShape()
        {
            Coefficients coe = TimeWarp.Coefficients;
            Vector2D[] bulletverticies = new Vector2D[]
            {
                new Vector2D(25,0),
                new Vector2D(20,5),
                new Vector2D(-20,10),
                new Vector2D(-20,-10),
                new Vector2D(20,-5)
            };
            IGeometry2D enginecone = new Polygon2D(ALVector2D.Zero, bulletverticies);
            DefaultShape = new RigidBodyTemplate(4, 165.272552f, new IGeometry2D[] { enginecone }, new Coefficients[] { coe });
            DefaultShape.BalanceBody();
            //DefaultShape.CalcInertiaMultiplier(.1f);
        }
        static ISolidWeapon CreateWeapon()
        {
            return new Controlable(
                DefaultLifeTime, 
                new PhysicsState(), 
                DefaultBodyFlags, 
                DefaultShape, 
                 new ShipMovementInfo( DefaultMovementInfo), 
            new ShipState( DefaultState), 
            new ControlableSounds(), 
            new WeaponsLogic(TargetingInfo.All,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static DruugeMaulerPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new DruugeMaulerPrimary();
        }

        DruugeMaulerPrimary()
            : base(new Bounded<float>(DefaultDelay), 
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            false,
            DefaultActionSounds,
            0, 
            0,
            CreateWeapon())
        {}
    }
    [Serializable]
    public class DruugeMaulerSecondary : BaseAction
    {
        public static Costs DefaultCost;
        public static Bounded<float>  DefaultDelay;
        public static TargetingInfo  DefaultTargetingTypes;
        public static ActionSounds DefaultActionSounds;

        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            DefaultCost = new Costs(new ShipStateChange(1, -16, 0, 0),null,null);
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(30));
            DefaultTargetingTypes = TargetingInfo.None;
            DefaultActionSounds = new ActionSounds("FireryDeath", null, null);
        }
        public static DruugeMaulerSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new DruugeMaulerSecondary();
        }

        DruugeMaulerSecondary()
            : base(new Bounded<float>(DefaultDelay), 
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            false,
            DefaultActionSounds)
        {
            this.aIInfo = new RechargeActionAIInfo(DefaultCost.ActivationCost);
        }
        DruugeMaulerSecondary(DruugeMaulerSecondary copy) : base(copy) { }
        protected override bool OnActivated(ActionResult actionResult, float dt)
        {
            return true;
        }
        public override object Clone()
        {
            return new DruugeMaulerSecondary(this);
        }
    }
}
#endif
