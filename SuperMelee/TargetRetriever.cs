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
using AdvanceMath;
using System;
using System.Collections.Generic;
namespace ReMasters.SuperMelee
{
    [Serializable]
    public class CircularIndexEnumerator 
    {
        int currentIndex = 0;
        int stopIndex = -1;
        public int Current
        {
            get { return currentIndex; }
            set
            {
                currentIndex = value;
                stopIndex = value - 1;
            }
        }
        public bool MoveNext(int length)
        {
            if (length == 0) { return false; }
            if (stopIndex >= length || stopIndex < 0)
            {
                stopIndex = length - 1;
            }
            if (currentIndex == stopIndex)
            {
                return false;
            }
            currentIndex++;
            if (currentIndex >= length)
            {
                currentIndex = 0;
            }
            return true;
        }
        public void Reset()
        {
            stopIndex = currentIndex-1;
        }
    }


    [Serializable]
    public class TargetRetriever
    {
        static List<IControlable> possibleTargets;
        static internal void SetPossibleTargets(List<IControlable> PossibleTargets)
        {
            possibleTargets = PossibleTargets;
        }


        IControlable host;
        CircularIndexEnumerator index = new CircularIndexEnumerator();

        public TargetRetriever() { }
        public void OnSourceCreation(IControlable host)
        {
            if (host is IWeapon && host.ControlableType == ControlableType.Weapon)
            {
                this.host = ((IWeapon)host).WeaponInfo.Source ?? host;
            }
            else
            {
                this.host = host;
            }
        }

        public IControlable NextClosest(Predicate<IControlable> match, Vector2D location)
        {
            if (match(host))
            {
                return host;
            }
            float gooddistance = 0;
            int goodIndex = -1;
            for (int pos = 0; pos < possibleTargets.Count; pos++)
            {
                IControlable possibleTarget = possibleTargets[pos];

                if (match(possibleTarget))
                {
                    float distance = (possibleTarget.Current.Position.Linear - location).MagnitudeSq;
                    if (goodIndex < 0 || distance < gooddistance)
                    {
                        goodIndex = pos;
                        gooddistance = distance;
                    }
                }
            }
            if (goodIndex >= 0)
            {
                index.Current = goodIndex;
                return possibleTargets[goodIndex];
            }
            return null;
        }
        public IControlable NextClosest(TargetingInfo targetingInfo, Vector2D location)
        {
            return NextClosest(
                CreatePredicate(targetingInfo),
                location);
        }

        public IControlable Next(Predicate<IControlable> match)
        {
            if (match(host))
            {
                return host;
            }
            index.Reset();
            while (index.MoveNext(possibleTargets.Count))
            {
                IControlable possibleTarget = possibleTargets[index.Current];

                if (match(possibleTarget))
                {
                    return possibleTarget;
                }
            }
            return null;
        }
        public IControlable Next(TargetingInfo targetingInfo)
        {
            return Next(CreatePredicate(targetingInfo));
        }

        private Predicate<IControlable> CreatePredicate(TargetingInfo targetingInfo)
        {
            return
                delegate(IControlable possibleTarget)
                {
                    return
                        possibleTarget.IsTargetable &&
                        targetingInfo.MeetsRequirements(FactionInfo.GetTargetingType(   host,possibleTarget));
                    //  GetTargetingType(possibleTarget));
                };
        }
    }
}