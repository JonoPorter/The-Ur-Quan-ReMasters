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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ReMasters.SuperMelee.GUI
{
    public partial class ShipLoaderListView : System.Windows.Forms.ListView
    {

        List<ShipLoader> shiploaders;
        public List<IShip> Ships
        {
            get
            {
                List<IShip> rv = new List<IShip>();
                foreach(ListViewItem item in this.Items)
                {
                    ShipLoader loader = (ShipLoader)item.Tag;
                    rv.Add(loader.Ship);
                }
                return rv;
            }

        }
        public List<ShipLoader> Shiploaders
        {
            get { return new List<ShipLoader>(  shiploaders); }
        }

        ShipLoader currentLoader;

        public ShipLoader CurrentLoader
        {
            get { return currentLoader; }
        }
        public ShipLoaderListView()
        {
            InitializeComponent();
        }
        public void Setup(List<ShipLoader> shiploaders)
        {
            this.shiploaders = shiploaders;
            InitializeComponent();

            ImageList imageList = new System.Windows.Forms.ImageList();
            foreach (ShipLoader shiploader in shiploaders)
            {
                imageList.Images.Add(shiploader.ShipName,shiploader.Thumbnail);
            }

            this.LargeImageList = imageList;

            foreach (ShipLoader shiploader in shiploaders)
            {
                ListViewItem item = new ListViewItem(shiploader.ShipName, shiploader.ShipName);
                item.Tag = shiploader;
                this.Items.Add(item);
            }
            currentLoader = shiploaders[0];
        }

        private void ShipLoaderSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count == 1)
            {
                currentLoader = (ShipLoader)this.SelectedItems[0].Tag;
            }
        }
    }
}
