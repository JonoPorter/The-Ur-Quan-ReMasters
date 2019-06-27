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
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace ReMasters.SuperMelee
{


    [Serializable]
    public sealed class FactionInfo : IDeserializationCallback
    {
        
        public static FactionCollection DefaultFactionCollection = new FactionCollection();
        internal int factionNumber;
        FactionCollection factionCollection;
        public FactionInfo(int factionNumber)
        {
            this.factionNumber = factionNumber;
            this.factionCollection = DefaultFactionCollection;
        }
        public FactionInfo(int factionNumber, FactionCollection factionCollection)
        {
            this.factionNumber = factionNumber;
            this.factionCollection = factionCollection;
        }
        public FactionInfo(FactionInfo info)
        {
            this.factionNumber = info.factionNumber;
            this.factionCollection = info.factionCollection;
        }
        public int FactionNumber
        {
            get
            {
                return factionNumber;
            }
        }
        public FactionCollection FactionCollection
        {
            get
            {
                return factionCollection;
            }
        }
        public string FactionName
        {
            get
            {
                return factionCollection.factionNames[factionNumber];
            }
            set
            {
                factionCollection.factionNames[factionNumber] = value;
            }
        }
        public FactionType FactionType
        {
            get
            {
                return factionCollection.factionTypes[factionNumber];
            }
            set
            {
                factionCollection.factionTypes[factionNumber] = value;
            }
        }
        public List<FactionInfo> GetAllOfDiplomacy(FactionDiplomacy diplomacy)
        {
            return DefaultFactionCollection.GetAllOfDiplomacy(this, diplomacy);
        }


        private static bool IsSubShipofHost(IControlable host,IControlable possibleTarget)
        {
            if (possibleTarget is IShip && host is IShip)
            {
                IShip[] arr = ((IShip)host).SubShips;
                if (arr != null)
                {
                    return Array.IndexOf<IShip>(arr, (IShip)possibleTarget) != -1;
                }
            }
            return false;
        }

        public static TargetingTypes GetTargetingType(IControlable host, IControlable possibleTarget)
        {
            int rv;
            if (host != null)
            {
                rv = (int)DefaultFactionCollection.GetDiplomacy(host.FactionInfo, possibleTarget.FactionInfo);
            }
            else
            {
                rv = (int)FactionDiplomacy.Neutral;
            }
            rv = rv | (int)possibleTarget.FactionInfo.FactionType;
            rv = rv | (int)possibleTarget.ControlableType;
            if (host == possibleTarget || IsSubShipofHost(host,possibleTarget))
            {
                rv = rv | (int)TargetingTypes.Self;
            }
            else
            {
                rv = rv | (int)TargetingTypes.Other;
            }

            return (TargetingTypes)rv;
        }
        public void OnDeserialization(object sender)
        {
            DefaultFactionCollection = factionCollection;
        }
    }
    [Serializable]
    public sealed class FactionCollection
    {
        [StructLayout(LayoutKind.Explicit), Serializable]
        struct LongUnion
        {
            [FieldOffset(0)]
            public long Key;
            [FieldOffset(0)]
            int Value1;
            [FieldOffset(4)]
            int Value2;
            public void Set(int Value1, int Value2)
            {
                if (Value1 > Value2)
                {
                    this.Value1 = Value2;
                    this.Value2 = Value1;
                }
                else
                {
                    this.Value1 = Value1;
                    this.Value2 = Value2;
                }
            }
        }

        internal List<string> factionNames = new List<string>();
        Dictionary<long, FactionDiplomacy> factionDiplomacies = new Dictionary<long, FactionDiplomacy>();
        internal List<FactionType> factionTypes = new List<FactionType>();
        int factioncount = 0;
        public FactionInfo CreateNewOrGetFaction(string factionName)
        {
            for (int pos = 0; pos < factioncount; ++pos)
            {
                if (factionName == factionNames[pos])
                {
                    return new FactionInfo(pos,this);
                }
            }
            factionNames.Add(factionName);
            factionTypes.Add(FactionType.None);
            factionDiplomacies[GetDiplomacyKey(factioncount, factioncount)] = FactionDiplomacy.Ally;
            return new FactionInfo(factioncount++,this);
        }
        private long GetDiplomacyKey(int first, int second)
        {
            LongUnion value = new LongUnion();
            value.Set(first, second);
            return value.Key;
        }
        public void SetDiplomacy(FactionInfo first, FactionInfo second, FactionDiplomacy diplomacy)
        {
            factionDiplomacies[GetDiplomacyKey(first.factionNumber, second.factionNumber)] = diplomacy;
        }
        public bool IsDiplomacy(FactionInfo first, FactionInfo second, FactionDiplomacy diplomacy)
        {
            long key = GetDiplomacyKey(first.factionNumber, second.factionNumber);
            FactionDiplomacy returnvalue;
            int dval = (int)diplomacy;
            if (factionDiplomacies.TryGetValue(key, out returnvalue))
            {
                return (dval & (int)(returnvalue)) == dval;
            }
            else
            {
                return (dval & (int)(factionDiplomacies[key] = FactionDiplomacy.Neutral)) == dval;
            }
        }
        public FactionDiplomacy GetDiplomacy(FactionInfo first, FactionInfo second)
        {
            long key = GetDiplomacyKey(first.factionNumber, second.factionNumber);
            FactionDiplomacy returnvalue;
            if (factionDiplomacies.TryGetValue(key, out returnvalue))
            {
                return returnvalue;
            }
            else
            {
                return factionDiplomacies[key] = FactionDiplomacy.Neutral;
            }
        }
        public List<FactionInfo> GetAllOfDiplomacy(FactionInfo faction, FactionDiplomacy diplomacy)
        {
            List<FactionInfo> returnvalue = new List<FactionInfo>();
            int dval = (int)diplomacy;
            FactionDiplomacy ovalue;
            for (int other = 0; other < factioncount; ++other)
            {
                if (faction.factionNumber != other)
                {
                    long key = GetDiplomacyKey(faction.factionNumber, other);
                    if (factionDiplomacies.TryGetValue(key, out ovalue))
                    {
                        if ((dval & (int)ovalue) == dval)
                        {
                            returnvalue.Add(new FactionInfo(other,this));
                        }
                    }
                    else
                    {
                        factionDiplomacies[key] = FactionDiplomacy.Neutral;
                        if ((dval & (int)FactionDiplomacy.Neutral) == dval)
                        {
                            returnvalue.Add(new FactionInfo(other, this));
                        }
                    }
                }
            }
            return returnvalue;
        }
        public List<FactionInfo> GetAllOfType(FactionType type)
        {
            List<FactionInfo> returnvalue = new List<FactionInfo>();
            int tval = (int)type;
            for (int other = 0; other < factioncount; ++other)
            {
                if ((tval & (int)factionTypes[other]) == tval)
                {
                    returnvalue.Add(new FactionInfo(other,this));
                }
            }
            return returnvalue;
        }
    }

}