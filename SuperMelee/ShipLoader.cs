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
using System.Collections.Generic;
//using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
/*using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;*/
//using Microsoft.DirectX.DirectInput;
using Physics2D;
using Physics2D.CollidableBodies;
using AdvanceMath; using AdvanceSystem;
using AdvanceMath.Geometry2D;
using ReMasters.SuperMelee.Ships;
using ReMasters.SuperMelee.Controlers;
using System.Xml.Serialization;

using SdlDotNet;
using Tao.OpenGl;
using System.Drawing.Drawing2D;
using Physics2D;
namespace ReMasters.SuperMelee
{
    public class ShipInfo
    {
        string fullName = " ";
        string description = " ";
        int pointValue = 0;
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public int PointValue
        {
            get { return pointValue; }
            set { pointValue = value; }
        }
    }
    public abstract class ShipLoader
    {
        static XmlSerializer shipInfoXmlSerializer = new XmlSerializer(typeof(ShipInfo));
        static BinaryFormatter binaryFormatter = new BinaryFormatter();
        public static readonly String ConfigFileName = "Ship.Ser";
        public static readonly String ThumbnailFileName = "Thumbnail.png";
        static readonly String InfoFileName = "Info.xml";

        string shipName;
        IShip ship;
        ShipInfo info;
        Image thumbnail;
        string shipDirectory;


        public ShipInfo Info
        {
            get { return info; }
        }
        public Image Thumbnail
        {
            get { return thumbnail; }
        }
        public string ShipName
        {
            get { return shipName; }
        }
        public IShip Ship
        {
            get
            {
                IShip rv = (IShip)ship.Clone();
                rv.Name = shipName;
                return rv;
            }
        }
        public string ShipDirectory
        {
            get { return shipDirectory; }
            set { shipDirectory = value; }
        }
        protected ShipLoader(string shipName)
        {
            this.shipName = shipName;
            this.info = new ShipInfo();
            this.info.FullName = shipName;
        }
        private void LoadConfig(string configPath)
        {
            ship = CreateHardCodedShip();
            return;  //todo: Remove this useless stuff
            if (File.Exists(configPath))
            {
                try
                {
                    using (Stream stream = new BufferedStream(File.Open(configPath, FileMode.Open, FileAccess.Read)))
                    {
                        ship = (IShip)binaryFormatter.Deserialize(stream);
                    }
                }
                catch
                {
                    SaveHardCoded(configPath);
                }
            }
            else
            {
                SaveHardCoded(configPath);
            }
        }
        private void LoadThumbnail(string imagepath)
        {
            if (File.Exists(imagepath))
            {
                try
                {
                    this.thumbnail = Image.FromFile(imagepath);
                }
                catch
                {
                    this.thumbnail = CreateThumbnail();
                    this.thumbnail.Save(imagepath);
                }
            }
            else
            {
                this.thumbnail = CreateThumbnail();
                this.thumbnail.Save(imagepath);
            }
        }
        private void LoadInfo(string infoPath)
        {
            if (File.Exists(infoPath))
            {
                try
                {
                    using (Stream stream = new BufferedStream(File.Open(infoPath, FileMode.Open, FileAccess.Read)))
                    {
                        info = (ShipInfo)shipInfoXmlSerializer.Deserialize(stream);
                    }
                }
                catch
                {
                    using (FileStream stream = File.Open(infoPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        shipInfoXmlSerializer.Serialize(stream, info);
                    }
                }
            }
            else
            {
                using (FileStream stream = File.Open(infoPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    shipInfoXmlSerializer.Serialize(stream, info);
                }
            }
        }
        public void Load()
        {
            if (shipDirectory == null)
            {
                shipDirectory = SuperMeleePaths.ShipDir + shipName + Path.DirectorySeparatorChar;
            }
            if (!Directory.Exists(shipDirectory))
            {
                Directory.CreateDirectory(shipDirectory);
            }
            LoadConfig(shipDirectory + ConfigFileName);
            LoadThumbnail(shipDirectory + ThumbnailFileName);
            LoadInfo(shipDirectory + InfoFileName);

        }
        private void SaveHardCoded(string path)
        {
            ship = CreateHardCodedShip();
            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                binaryFormatter.Serialize(stream, ship);
            }
        }
        protected virtual Image CreateThumbnail()
        {
            IShip ship = Ship;
            ship.SetAllPositions(ALVector2D.Zero);
            ship.CalcBoundingBox2D();
            BoundingBox2D box = ship.BoundingBox2D;
            Vector2D size = box.Upper - box.Lower;
            int scale = 10;
            int max = Math.Max((int)size.X, (int)size.Y) * scale;
            Size size2 = new Size(max, max);
            Image BitMap = new Bitmap(size2.Width, size2.Height);
            List<ICollidableBody> tmp = new List<ICollidableBody>();
            tmp.Add(ship);
            List<PointF[]> vertexes = HyperMelee.GetVertexes(new WindowState(size2, scale, Vector2D.Zero), tmp);
            Graphics g = Graphics.FromImage(BitMap);
            g.Clear(Color.White);
            foreach (PointF[] points in vertexes)
            {
                Pen pen = new Pen(Color.Gray);
                PathGradientBrush brush = new PathGradientBrush(points);
                brush.CenterPoint = new PointF(0, 0);
                brush.CenterColor = Color.Black;
                int length = points.Length;
                Color[] tmp2 = new Color[length];
                for (int pos = 0; pos < length; ++pos)
                {
                    tmp2[pos] = Color.Gray;
                }
                brush.SurroundColors = tmp2;
                g.DrawPolygon(pen, points);
                g.FillPolygon(brush, points);
            }
            return BitMap.GetThumbnailImage(200, 200, null, IntPtr.Zero);
        }
        protected abstract IShip CreateHardCodedShip();
    }
    public abstract class OrganizedShipLoader : ShipLoader
    {
        protected OrganizedShipLoader(string shipName) : base(shipName) { }
        protected sealed override IShip CreateHardCodedShip()
        {
            return CreateHardCodedShip(GetShape(), new ActionList(GetActions()));
        }
        protected abstract IShip CreateHardCodedShip(RigidBodyTemplate defaultShape, ActionList DefaultActions);
        protected abstract RigidBodyTemplate GetShape();
        protected abstract IEnumerable<IAction> GetActions();
    }
}