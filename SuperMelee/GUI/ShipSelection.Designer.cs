namespace ReMasters.SuperMelee.GUI
{
    partial class ShipSelection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.bChoose = new System.Windows.Forms.Button();
            this.bRandom = new System.Windows.Forms.Button();
            this.lvShips = new ReMasters.SuperMelee.GUI.ShipListView();
            this.bAlwaysRandom = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bChoose
            // 
            this.bChoose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bChoose.Location = new System.Drawing.Point(410, 185);
            this.bChoose.Name = "bChoose";
            this.bChoose.Size = new System.Drawing.Size(75, 23);
            this.bChoose.TabIndex = 1;
            this.bChoose.Text = "Choose!";
            this.bChoose.UseVisualStyleBackColor = true;
            this.bChoose.Click += new System.EventHandler(this.CheckForValidity);
            // 
            // bRandom
            // 
            this.bRandom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bRandom.Location = new System.Drawing.Point(180, 185);
            this.bRandom.Name = "bRandom";
            this.bRandom.Size = new System.Drawing.Size(75, 23);
            this.bRandom.TabIndex = 2;
            this.bRandom.Text = "Random!";
            this.bRandom.UseVisualStyleBackColor = true;
            this.bRandom.Click += new System.EventHandler(this.bRandom_Click);
            // 
            // lvShips
            // 
            this.lvShips.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvShips.Location = new System.Drawing.Point(12, 12);
            this.lvShips.Name = "lvShips";
            this.lvShips.Size = new System.Drawing.Size(473, 167);
            this.lvShips.TabIndex = 0;
            this.lvShips.UseCompatibleStateImageBehavior = false;
            this.lvShips.DoubleClick += new System.EventHandler(this.CheckForValidity);
            // 
            // bAlwaysRandom
            // 
            this.bAlwaysRandom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bAlwaysRandom.Location = new System.Drawing.Point(261, 185);
            this.bAlwaysRandom.Name = "bAlwaysRandom";
            this.bAlwaysRandom.Size = new System.Drawing.Size(104, 23);
            this.bAlwaysRandom.TabIndex = 3;
            this.bAlwaysRandom.Text = "Always Random!!!";
            this.bAlwaysRandom.UseVisualStyleBackColor = true;
            this.bAlwaysRandom.Click += new System.EventHandler(this.bAlwaysRandom_Click);
            // 
            // ShipSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 220);
            this.ControlBox = false;
            this.Controls.Add(this.bAlwaysRandom);
            this.Controls.Add(this.bRandom);
            this.Controls.Add(this.bChoose);
            this.Controls.Add(this.lvShips);
            this.Name = "ShipSelection";
            this.Text = "ShipSelection";
            this.ResumeLayout(false);

        }

        #endregion

        private ShipListView lvShips;
        private System.Windows.Forms.Button bChoose;
        private System.Windows.Forms.Button bRandom;
        private System.Windows.Forms.Button bAlwaysRandom;
    }
}