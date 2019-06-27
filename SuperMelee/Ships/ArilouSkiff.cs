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
    public class ArilouSkiffShipLoader : ShipLoader
    {
        public ArilouSkiffShipLoader() : base("Arilou Skiff") { }
        protected override IShip CreateHardCodedShip()
        {
            return ArilouSkiff.Create(new PhysicsState(), null);
        }
    }

    /// <summary>
    /// The actaul ship class.
    /// </summary>
    [Serializable]
    public class ArilouSkiff : Ship // its a ship!
    {
        public static RigidBodyTemplate DefaultShape;
        public static LifeSpan DefaultLifeTime = new LifeSpan();
        public static BodyFlags DefaultBodyFlags = BodyFlags.IgnoreGravity;
        public static ShipMovementInfo DefaultMovementInfo;
        public static ShipState DefaultState;
        public static ActionList DefaultActions = new ActionList();
        public static ControlableSounds DefaultControlableSounds;
        public static ShipSounds DefaultShipSounds;

        /// <summary>
        /// The static constructor where most of the Default values are determined.
        /// </summary>
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            // this is the method call where the ships shape is set. 
            InitShape();
            // This is the movement info of the ship. 
            DefaultMovementInfo = new ShipMovementInfo(
                new Bounded<float>(TimeWarp.AngularAcceleration), // Angular Acceleration in Radians per second per second
                new Bounded<float>(TimeWarp.ScaleTurning(1)),   // Max Angular Velocity in Radians per second
                new Bounded<float>(TimeWarp.ScaleAcceleration(45, 0)), //Linear Acceleration in a Distance unit per second per second
                new Bounded<float>(TimeWarp.ScaleVelocity(45))); // Max Linear Velocity in a Distance unit per second
            DefaultState = new ShipState(
                new Bounded<float>(6), // Ships Health
                new Bounded<float>(20), //Ships Energy
                new Bounded<float>(0), //Health Recharge Rate
                new Bounded<float>(TimeWarp.RechargeRateToPerSeconds(6, 1))); //Energy Recharge Rate
            
            // Sets the actions of the ship (the weapons)
            // If you changed the order the Primary would become the Secondary
            DefaultActions.Add( ArilouSkiffPrimary.Create());
            DefaultActions.Add(ArilouSkiffSecondary.Create());

            //The sounds related to the Controlable since Ship inherits from Controlable. 
            //They are just the .oggs in the Sound directory without the file extension. 
            //The first is the sounds played when it is created the seconde is when it dies.
            DefaultControlableSounds = new ControlableSounds(null,"ShipDies");
            //Ships sounds this is actaully a music so its teh file in the music directory without the file extension.
            //The music played when the ship wins.
            DefaultShipSounds = new ShipSounds("ArilouSkiffDitty");
        }
        /// <summary>
        /// Initializes the ships shape.
        /// </summary>
        static void InitShape()
        {
            //This method is complicated and i dont fell like commenting it.
            // this one is actaully very simple compared to the rest but still anoying.

            Coefficients DefaultCoefficients = TimeWarp.Coefficients;
            DefaultShape = new RigidBodyTemplate(
                MassInertia.FromSolidCylinder(4, 20),
                new IGeometry2D[] { new Polygon2D(ALVector2D.Zero, Polygon2D.FromNumberofSidesAndRadius(10, 20)) },
                new Coefficients[] { DefaultCoefficients });
            //DefaultShape.BalanceBody();
            //////DefaultShape.CalcInertiaMultiplier(.1f);
        }
        /// <summary>
        /// A Method that all ship files must have in order to be part of the game. It creates a new ship.
        /// </summary>
        /// <param name="state">the physics state of the ship to be created.</param>
        /// <param name="factionInfo">the faction info of the ship to be created.</param>
        /// <returns>A new Ship</returns>
        public static IShip Create(PhysicsState state, FactionInfo factionInfo)
        {
            if (!initialized)
            {
                Initialize();
            }

            //Calls the constructor.
            ArilouSkiff returnvalue = new ArilouSkiff(state, factionInfo);
            //gives a InertialessControlHandler (Inertialess Drive)/
            returnvalue.ControlHandler = new InertialessControlHandler();
            return returnvalue;
        }
        /// <summary>
        /// A Method that all ship files must have in order to be part of the game. 
        /// It creates a object that can be used to create a new ship.
        /// </summary>
        /// <returns>A object that descibes the ship.</returns>
        /*public static ShipInfo GetShipInfo()
        {
            return new ShipInfo("Arilou Skiff", new CreateShipDelegate(Create));
        }*/
        /// <summary>
        /// Normal Constructor.
        /// </summary>
        /// <param name="state">the physics state of the ship to be created.</param>
        /// <param name="factionInfo">the faction info of the ship to be created.</param>
        protected ArilouSkiff(PhysicsState state, FactionInfo factionInfo)
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
    /// <summary>
    /// The Primary Action (Weapon). the tracking laser.
    /// </summary>
    [Serializable]
    public class ArilouSkiffPrimary : RayPointDefence //It acts like the earthing point defence.
    {
        public static LifeSpan DefaultLifeTime = new LifeSpan(0);
        public static Costs DefaultCost;
        public static EffectCollection DefaultEffectCollection = new EffectCollection();

        public static Bounded<float> DefaultDelay;

        public static float DefaultRadius = 500;
        public static int DefaultMaxNumberofTargets = 1;
        public static float DefaultImpulse = 20;
        public static TargetingInfo DefaultTargetingTypes;
        public static TargetingInfo DefaultEffectsWho;
        public static ActionSounds DefaultActionSounds;
        static bool initialized = false;
        static void Initialize()
        {
            initialized = true;

            //what the weapon can effect.
            DefaultEffectsWho = new TargetingInfo(
                TargetingTypes.None,    //must be ANY one of these.  
                TargetingTypes.None,    //must be ALL of these. 
                TargetingTypes.Ally     //must be NONE of these. 
                );
            //Note: about TargetingInfo. if a parameter is set to TargetingTypes.None then it is not applied.

            //what the weapon can target.
            DefaultTargetingTypes = new TargetingInfo(
                TargetingTypes.None, //must be ANY one of these.  
                TargetingTypes.Enemy,//must be ALL of these. 
                TargetingTypes.None  //must be NONE of these. 
                );
            //Note: about TargetingInfo. if the ANY or the ALL parameter has only one  
            //and the other one is set to TargetingTypes.None then they can be
            //swaped and still act the same.

            //what the action cost.
            DefaultCost = new Costs(
                new ShipStateChange( // the cost for it to activate.
                    0,  //the cost in health.
                    2,  //the cost in energy.
                    0,  //the cost in health recharge rate.
                    0), //the cost in energy recharge rate.
                null, // the cost in per seconds for it to run. If null then it has no running logic.
                null  // the cost for it to deactivate. If null then it has no deactivate logic.
                );

            //what the weapon does on contact.
            DefaultEffectCollection.Effects.Add(
                new ShipStateEffect(    //Its a ShipState effect so it can effect health or energy or the recharge of both. 
                    DefaultEffectsWho,  //what the weapon can effect.
                    EffectTypes.None,   //extra info about what effects the effect does. usualy is EffectTypes.None
                    new EffectSounds(), //the sounds assoiated with the effect. right now it has none.
                    new ShipStateChange(// the change it cuases.
                            -1, //change in health.
                            0,  //change in energy.
                            0,  //change in health's recharge rate.
                            0)  //change in energy's recharge rate.
                       )
                      ); 

            //The amount of time to wait before the action can be triggered again.
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(1));
            //The sounds associated with the Action.
            DefaultActionSounds = new ActionSounds(
                "Laser5", // the sound played on activation.
                null, // the sound played on running.
                null); // the sound played on deactivation.
        }
        /// <summary>
        /// Creates the weapon the action fires.
        /// </summary>
        /// <returns>the weapon the action fires</returns>
        static TargetedRayWeapon CreateWeapon()
        {
            TargetedRayWeapon rv = new TargetedRayWeapon(
                (LifeSpan)DefaultLifeTime.Clone(), 
                new WeaponsLogic(DefaultEffectsWho,
                new EffectCollection(DefaultEffectCollection)), 
                DefaultImpulse, 
                DefaultRadius);
            return rv;
        }
        public static ArilouSkiffPrimary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ArilouSkiffPrimary();
        }
        ArilouSkiffPrimary()
            : base(new Bounded<float>(DefaultDelay),
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            DefaultActionSounds,
            DefaultRadius,
            DefaultMaxNumberofTargets,
             CreateWeapon())
        {}
        public ArilouSkiffPrimary(ArilouSkiffPrimary copy)
            : base(copy)
        {}
        protected override bool OnNoTarget(GameResult gameResult, float dt)
        {
            if (source.Target == null || !source.Target.IsTargetable)
            {
                return false;
            }
            RayWeapon newWeapon = (RayWeapon)weapon.Clone();
            newWeapon.OnCreation(gameResult,source, this);
            gameResult.AddCollidableArea(newWeapon);
            newWeapon.WeaponInfo.Target = source.Target;
            return true;
        }
        public override object Clone()
        {
            return new ArilouSkiffPrimary(this);
        }
    }
    /// <summary>
    /// The teleport action.
    /// </summary>
    [Serializable]
    public class ArilouSkiffSecondary : InstantAction // it has a instant effect.
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

            //what it can target. 
            DefaultTargetingTypes = TargetingInfo.Self;
            //what it can effect.
            DefaultEffectsWho = DefaultTargetingTypes;
            //the cost.
            DefaultCost = new Costs(new ShipStateChange(0, 3, 0, 0),null,null);
            // teh delay before it can be activated again.
            DefaultDelay = new Bounded<float>(TimeWarp.RateToTime(2));
            
            //Adds a effect.
            DefaultEffectCollection.Effects.Add(
                new RandomTeleport( // its a random teleport!
                    DefaultTargetingTypes,  //what it can target.
                    EffectTypes.None, //extra effect types.
                    new EffectSounds(), // sounds played on applied.
                    3000)); // the maximum distance it can travel.
            //The sounds it plays!
            DefaultActionSounds = new ActionSounds("QuasiJump", null, null);
        }
        static IWeapon CreateWeapon()
        {
            return new InstantWeapon(
                new WeaponsLogic(DefaultEffectsWho,
            new EffectCollection(DefaultEffectCollection)));
        }
        public static ArilouSkiffSecondary Create()
        {
            if (!initialized)
            {
                Initialize();
            }
            return new ArilouSkiffSecondary();
        }

        ArilouSkiffSecondary()
            : base(new Bounded<float>(DefaultDelay), 
            DefaultTargetingTypes,
            new Costs(DefaultCost),
            DefaultActionSounds,
            CreateWeapon())
        {
            this.aIInfo = new ShieldActionAIInfo();
        }
    }
    /// <summary>
    /// The RandomTeleport effect.
    /// </summary>
    [Serializable]
    public class RandomTeleport : PhysicsStateEffect // it effects the physics state!
    {
        /// <summary>
        /// a Random number genorator.
        /// </summary>
        static Random rand = new Random();
        /// <summary>
        /// maximum distance it can change the state of.
        /// </summary>
        protected float maxDistance;
        /// <summary>
        /// The change it will hace on the Controlables PhysicsState.
        /// </summary>
        protected PhysicsState diff;
        public RandomTeleport(TargetingInfo effectWho,
            EffectTypes harmfulEffectTypes,
            EffectSounds effectSounds, float maxDistance)
            : base(effectWho, harmfulEffectTypes, effectSounds)
        {
            this.maxDistance = maxDistance;
        }
        public RandomTeleport(RandomTeleport copy)
            : base(copy)
        {
            this.maxDistance = copy.maxDistance;
        }
        public override PhysicsState PhysicsStateChange
        {
            get
            {
                return diff;
            }
            set
            {
                diff = value;
            }
        }
        /// <summary>
        /// Is called when the effect is created.
        /// </summary>
        /// <param name="weaponInfo"></param>
        public override void OnCreation(IWeaponsLogic weaponInfo)
        {
            diff = new PhysicsState();
            // sets a random Linear displacement.
            diff.Position.Linear = Vector2D.FromLengthAndAngle((float)rand.NextDouble() * maxDistance, (float)rand.NextDouble() * 2 * MathHelper.PI);
            base.OnCreation(weaponInfo);
        }
        /// <summary>
        /// Its not only for sheep! baa
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new RandomTeleport(this);
        }
    }
}
#endif