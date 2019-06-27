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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using System.Collections;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ReMasters.SuperMelee.GUI
{

    public partial class ShipListView : ListView
    {
        ImageList originalimageList = new ImageList();
        ImageList imageList = new ImageList();
        IShip currentShip;
        int currentIndex;
        public ShipListView()
        {
            this.LargeImageList = imageList;
            InitializeComponent();
        }
        public void Setup(ImageList originalimageList, List<IShip> ships)
        {
            this.originalimageList = originalimageList;
            this.Items.Clear();
            foreach (IShip ship in ships)
            {
                ListViewItem item = new ListViewItem(ship.Name);
                item.Tag = ship;
                this.Items.Add(item);
            }
            RefreshImages();
            currentShip = ships[0];
            this.SelectedIndices.Add(0);
        }
        public void Add(IShip ship)
        {
            ListViewItem item = new ListViewItem(ship.Name);
            item.Tag = ship;
            this.Items.Add(item);
            RefreshImages();
        }
        public IShip CurrentShip
        {
            get
            { 
                return currentShip;
            }
        }
        public IShip RemoveCurrent()
        {
            IShip rv = currentShip;
            this.Items.RemoveAt(currentIndex);
            if (this.Items.Count > 0)
            {
                this.SelectedIndices.Add(0);
            }
            currentShip = null;
            return rv;
        }
        public void RefreshImages()
        {
            imageList.Images.Clear();
            foreach (ListViewItem item in Items)
            {
                IShip ship = (IShip)item.Tag;
                item.ImageIndex = imageList.Images.Count;
                imageList.Images.Add(GenerateImage(originalimageList.Images[ship.Name], ship)) ;
            }
        }
        Image GenerateImage(Image imageold, IShip ship)
        {
            Image image = (Image)imageold.Clone();

            Graphics g = Graphics.FromImage(image);
            int lowerX = image.Width / 5;
            int upperX = image.Width - lowerX;

            int HY = (int)(ship.ShipState.Health.Percent * image.Height);
            int EY = (int)(ship.ShipState.Energy.Percent * image.Height);

            
            Pen health = new Pen(Color.Green);
            Pen energy = new Pen(Color.Blue);
            g.FillRectangle(health.Brush, 0, image.Height-HY, lowerX, image.Height);
            g.FillRectangle(energy.Brush, upperX, image.Height - EY, lowerX, image.Height);
            return image;
        }
        private void ShipListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count == 1)
            {
                currentShip = (IShip)this.SelectedItems[0].Tag;
                currentIndex = this.SelectedIndices[0];
            }
        }
    }
}
