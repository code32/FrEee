﻿namespace FrEee.WinForms
{
	partial class CombatSimulatorForm
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.chkHideObsolete = new System.Windows.Forms.CheckBox();
			this.chkForeign = new System.Windows.Forms.CheckBox();
			this.ddlVehicleType = new System.Windows.Forms.ComboBox();
			this.gamePanel1 = new FrEee.WinForms.Controls.GamePanel();
			this.lstDesigns = new System.Windows.Forms.ListView();
			this.colDesign = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colTotalCost = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblQuantityUnit = new System.Windows.Forms.Label();
			this.btnCancel = new FrEee.WinForms.Controls.GameButton();
			this.btnOK = new FrEee.WinForms.Controls.GameButton();
			this.panel3 = new System.Windows.Forms.Panel();
			this.gamePanel2 = new FrEee.WinForms.Controls.GamePanel();
			this.lstEmpires = new System.Windows.Forms.ListView();
			this.btnDuplicateEmpire = new FrEee.WinForms.Controls.GameButton();
			this.panel4 = new System.Windows.Forms.Panel();
			this.gameButton1 = new FrEee.WinForms.Controls.GameButton();
			this.gamePanel3 = new FrEee.WinForms.Controls.GamePanel();
			this.listView1 = new System.Windows.Forms.ListView();
			this.panel5 = new System.Windows.Forms.Panel();
			this.btnDuplicateVehicle = new FrEee.WinForms.Controls.GameButton();
			this.gamePanel4 = new FrEee.WinForms.Controls.GamePanel();
			this.lstSpaceObjects = new System.Windows.Forms.ListView();
			this.btnAddVehicle = new FrEee.WinForms.Controls.GameButton();
			this.btnAddPlanet = new FrEee.WinForms.Controls.GameButton();
			this.btnRemoveEmpire = new FrEee.WinForms.Controls.GameButton();
			this.btnRemoveVehicle = new FrEee.WinForms.Controls.GameButton();
			this.panel6 = new System.Windows.Forms.Panel();
			this.btnRemoveCargo = new FrEee.WinForms.Controls.GameButton();
			this.btnAddUnit = new FrEee.WinForms.Controls.GameButton();
			this.btnDuplicateCargo = new FrEee.WinForms.Controls.GameButton();
			this.gamePanel5 = new FrEee.WinForms.Controls.GamePanel();
			this.lstCargo = new System.Windows.Forms.ListView();
			this.gamePanel6 = new FrEee.WinForms.Controls.GamePanel();
			this.designReport = new FrEee.WinForms.Controls.DesignReport();
			this.designReport1 = new FrEee.WinForms.Controls.DesignReport();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.gamePanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.gamePanel2.SuspendLayout();
			this.panel4.SuspendLayout();
			this.gamePanel3.SuspendLayout();
			this.panel5.SuspendLayout();
			this.gamePanel4.SuspendLayout();
			this.panel6.SuspendLayout();
			this.gamePanel5.SuspendLayout();
			this.gamePanel6.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.gamePanel6, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel6, 3, 3);
			this.tableLayoutPanel1.Controls.Add(this.panel5, 3, 2);
			this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.label2, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 3, 4);
			this.tableLayoutPanel1.Controls.Add(this.panel3, 3, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1098, 654);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.chkHideObsolete);
			this.panel2.Controls.Add(this.chkForeign);
			this.panel2.Controls.Add(this.ddlVehicleType);
			this.panel2.Controls.Add(this.gamePanel1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 27);
			this.panel2.Name = "panel2";
			this.tableLayoutPanel1.SetRowSpan(this.panel2, 3);
			this.panel2.Size = new System.Drawing.Size(380, 597);
			this.panel2.TabIndex = 18;
			// 
			// chkHideObsolete
			// 
			this.chkHideObsolete.AutoSize = true;
			this.chkHideObsolete.Checked = true;
			this.chkHideObsolete.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkHideObsolete.Location = new System.Drawing.Point(3, 30);
			this.chkHideObsolete.Name = "chkHideObsolete";
			this.chkHideObsolete.Size = new System.Drawing.Size(93, 17);
			this.chkHideObsolete.TabIndex = 8;
			this.chkHideObsolete.Text = "Hide Obsolete";
			this.chkHideObsolete.UseVisualStyleBackColor = true;
			// 
			// chkForeign
			// 
			this.chkForeign.AutoSize = true;
			this.chkForeign.Location = new System.Drawing.Point(96, 30);
			this.chkForeign.Name = "chkForeign";
			this.chkForeign.Size = new System.Drawing.Size(102, 17);
			this.chkForeign.TabIndex = 7;
			this.chkForeign.Text = "Foreign Designs";
			this.chkForeign.UseVisualStyleBackColor = true;
			// 
			// ddlVehicleType
			// 
			this.ddlVehicleType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ddlVehicleType.DisplayMember = "Name";
			this.ddlVehicleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlVehicleType.FormattingEnabled = true;
			this.ddlVehicleType.Location = new System.Drawing.Point(3, 3);
			this.ddlVehicleType.Name = "ddlVehicleType";
			this.ddlVehicleType.Size = new System.Drawing.Size(377, 21);
			this.ddlVehicleType.TabIndex = 6;
			this.ddlVehicleType.ValueMember = "VehicleTypes";
			// 
			// gamePanel1
			// 
			this.gamePanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gamePanel1.BackColor = System.Drawing.Color.Black;
			this.gamePanel1.BorderColor = System.Drawing.Color.CornflowerBlue;
			this.gamePanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.gamePanel1.Controls.Add(this.lstDesigns);
			this.gamePanel1.ForeColor = System.Drawing.Color.White;
			this.gamePanel1.Location = new System.Drawing.Point(3, 53);
			this.gamePanel1.Name = "gamePanel1";
			this.gamePanel1.Padding = new System.Windows.Forms.Padding(3);
			this.gamePanel1.Size = new System.Drawing.Size(387, 544);
			this.gamePanel1.TabIndex = 5;
			// 
			// lstDesigns
			// 
			this.lstDesigns.BackColor = System.Drawing.Color.Black;
			this.lstDesigns.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstDesigns.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDesign,
            this.colTotalCost});
			this.lstDesigns.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstDesigns.ForeColor = System.Drawing.Color.White;
			this.lstDesigns.FullRowSelect = true;
			this.lstDesigns.Location = new System.Drawing.Point(3, 3);
			this.lstDesigns.Name = "lstDesigns";
			this.lstDesigns.Size = new System.Drawing.Size(379, 536);
			this.lstDesigns.TabIndex = 1;
			this.lstDesigns.UseCompatibleStateImageBehavior = false;
			this.lstDesigns.View = System.Windows.Forms.View.Details;
			// 
			// colDesign
			// 
			this.colDesign.Text = "Design";
			this.colDesign.Width = 125;
			// 
			// colTotalCost
			// 
			this.colTotalCost.Text = "Cost";
			this.colTotalCost.Width = 50;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.label2.Location = new System.Drawing.Point(714, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(155, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Empires/Vehicles/Cargo";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.label1.Location = new System.Drawing.Point(3, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(118, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Available Designs";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.lblQuantityUnit);
			this.panel1.Controls.Add(this.btnCancel);
			this.panel1.Controls.Add(this.btnOK);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(711, 627);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(387, 27);
			this.panel1.TabIndex = 11;
			// 
			// lblQuantityUnit
			// 
			this.lblQuantityUnit.AutoSize = true;
			this.lblQuantityUnit.Location = new System.Drawing.Point(3, 6);
			this.lblQuantityUnit.Name = "lblQuantityUnit";
			this.lblQuantityUnit.Size = new System.Drawing.Size(0, 13);
			this.lblQuantityUnit.TabIndex = 2;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.BackColor = System.Drawing.Color.Black;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnCancel.Location = new System.Drawing.Point(178, 3);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(100, 21);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = false;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.BackColor = System.Drawing.Color.Black;
			this.btnOK.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnOK.Location = new System.Drawing.Point(284, 3);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(100, 21);
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = false;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.panel4);
			this.panel3.Controls.Add(this.btnDuplicateEmpire);
			this.panel3.Controls.Add(this.gamePanel2);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(714, 27);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(381, 195);
			this.panel3.TabIndex = 20;
			// 
			// gamePanel2
			// 
			this.gamePanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gamePanel2.BackColor = System.Drawing.Color.Black;
			this.gamePanel2.BorderColor = System.Drawing.Color.CornflowerBlue;
			this.gamePanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.gamePanel2.Controls.Add(this.lstEmpires);
			this.gamePanel2.ForeColor = System.Drawing.Color.White;
			this.gamePanel2.Location = new System.Drawing.Point(3, 0);
			this.gamePanel2.Name = "gamePanel2";
			this.gamePanel2.Padding = new System.Windows.Forms.Padding(3);
			this.gamePanel2.Size = new System.Drawing.Size(278, 195);
			this.gamePanel2.TabIndex = 21;
			// 
			// lstEmpires
			// 
			this.lstEmpires.BackColor = System.Drawing.Color.Black;
			this.lstEmpires.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstEmpires.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstEmpires.Location = new System.Drawing.Point(3, 3);
			this.lstEmpires.Margin = new System.Windows.Forms.Padding(0);
			this.lstEmpires.Name = "lstEmpires";
			this.lstEmpires.Size = new System.Drawing.Size(270, 187);
			this.lstEmpires.TabIndex = 0;
			this.lstEmpires.UseCompatibleStateImageBehavior = false;
			this.lstEmpires.View = System.Windows.Forms.View.List;
			// 
			// btnDuplicateEmpire
			// 
			this.btnDuplicateEmpire.BackColor = System.Drawing.Color.Black;
			this.btnDuplicateEmpire.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnDuplicateEmpire.Location = new System.Drawing.Point(293, 0);
			this.btnDuplicateEmpire.Name = "btnDuplicateEmpire";
			this.btnDuplicateEmpire.Size = new System.Drawing.Size(97, 24);
			this.btnDuplicateEmpire.TabIndex = 22;
			this.btnDuplicateEmpire.Text = "Duplicate";
			this.btnDuplicateEmpire.UseVisualStyleBackColor = false;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.btnRemoveEmpire);
			this.panel4.Controls.Add(this.gameButton1);
			this.panel4.Controls.Add(this.gamePanel3);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(381, 195);
			this.panel4.TabIndex = 23;
			// 
			// gameButton1
			// 
			this.gameButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.gameButton1.BackColor = System.Drawing.Color.Black;
			this.gameButton1.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.gameButton1.Location = new System.Drawing.Point(284, 0);
			this.gameButton1.Name = "gameButton1";
			this.gameButton1.Size = new System.Drawing.Size(97, 24);
			this.gameButton1.TabIndex = 22;
			this.gameButton1.Text = "Duplicate";
			this.gameButton1.UseVisualStyleBackColor = false;
			// 
			// gamePanel3
			// 
			this.gamePanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gamePanel3.BackColor = System.Drawing.Color.Black;
			this.gamePanel3.BorderColor = System.Drawing.Color.CornflowerBlue;
			this.gamePanel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.gamePanel3.Controls.Add(this.listView1);
			this.gamePanel3.ForeColor = System.Drawing.Color.White;
			this.gamePanel3.Location = new System.Drawing.Point(3, 0);
			this.gamePanel3.Name = "gamePanel3";
			this.gamePanel3.Padding = new System.Windows.Forms.Padding(3);
			this.gamePanel3.Size = new System.Drawing.Size(278, 195);
			this.gamePanel3.TabIndex = 21;
			// 
			// listView1
			// 
			this.listView1.BackColor = System.Drawing.Color.Black;
			this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.Location = new System.Drawing.Point(3, 3);
			this.listView1.Margin = new System.Windows.Forms.Padding(0);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(270, 187);
			this.listView1.TabIndex = 0;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.List;
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.btnRemoveVehicle);
			this.panel5.Controls.Add(this.btnAddPlanet);
			this.panel5.Controls.Add(this.btnAddVehicle);
			this.panel5.Controls.Add(this.btnDuplicateVehicle);
			this.panel5.Controls.Add(this.gamePanel4);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel5.Location = new System.Drawing.Point(714, 228);
			this.panel5.Name = "panel5";
			this.panel5.Size = new System.Drawing.Size(381, 195);
			this.panel5.TabIndex = 21;
			// 
			// btnDuplicateVehicle
			// 
			this.btnDuplicateVehicle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDuplicateVehicle.BackColor = System.Drawing.Color.Black;
			this.btnDuplicateVehicle.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnDuplicateVehicle.Location = new System.Drawing.Point(284, 0);
			this.btnDuplicateVehicle.Name = "btnDuplicateVehicle";
			this.btnDuplicateVehicle.Size = new System.Drawing.Size(97, 24);
			this.btnDuplicateVehicle.TabIndex = 22;
			this.btnDuplicateVehicle.Text = "Duplicate";
			this.btnDuplicateVehicle.UseVisualStyleBackColor = false;
			// 
			// gamePanel4
			// 
			this.gamePanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gamePanel4.BackColor = System.Drawing.Color.Black;
			this.gamePanel4.BorderColor = System.Drawing.Color.CornflowerBlue;
			this.gamePanel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.gamePanel4.Controls.Add(this.lstSpaceObjects);
			this.gamePanel4.ForeColor = System.Drawing.Color.White;
			this.gamePanel4.Location = new System.Drawing.Point(3, 0);
			this.gamePanel4.Name = "gamePanel4";
			this.gamePanel4.Padding = new System.Windows.Forms.Padding(3);
			this.gamePanel4.Size = new System.Drawing.Size(278, 195);
			this.gamePanel4.TabIndex = 21;
			// 
			// lstSpaceObjects
			// 
			this.lstSpaceObjects.BackColor = System.Drawing.Color.Black;
			this.lstSpaceObjects.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstSpaceObjects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstSpaceObjects.Location = new System.Drawing.Point(3, 3);
			this.lstSpaceObjects.Margin = new System.Windows.Forms.Padding(0);
			this.lstSpaceObjects.Name = "lstSpaceObjects";
			this.lstSpaceObjects.Size = new System.Drawing.Size(270, 187);
			this.lstSpaceObjects.TabIndex = 0;
			this.lstSpaceObjects.UseCompatibleStateImageBehavior = false;
			this.lstSpaceObjects.View = System.Windows.Forms.View.List;
			// 
			// btnAddVehicle
			// 
			this.btnAddVehicle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddVehicle.BackColor = System.Drawing.Color.Black;
			this.btnAddVehicle.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnAddVehicle.Location = new System.Drawing.Point(284, 30);
			this.btnAddVehicle.Name = "btnAddVehicle";
			this.btnAddVehicle.Size = new System.Drawing.Size(97, 24);
			this.btnAddVehicle.TabIndex = 23;
			this.btnAddVehicle.Text = "Add Vehicle";
			this.btnAddVehicle.UseVisualStyleBackColor = false;
			// 
			// btnAddPlanet
			// 
			this.btnAddPlanet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddPlanet.BackColor = System.Drawing.Color.Black;
			this.btnAddPlanet.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnAddPlanet.Location = new System.Drawing.Point(284, 60);
			this.btnAddPlanet.Name = "btnAddPlanet";
			this.btnAddPlanet.Size = new System.Drawing.Size(97, 24);
			this.btnAddPlanet.TabIndex = 24;
			this.btnAddPlanet.Text = "Add Planet";
			this.btnAddPlanet.UseVisualStyleBackColor = false;
			// 
			// btnRemoveEmpire
			// 
			this.btnRemoveEmpire.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveEmpire.BackColor = System.Drawing.Color.Black;
			this.btnRemoveEmpire.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnRemoveEmpire.Location = new System.Drawing.Point(284, 30);
			this.btnRemoveEmpire.Name = "btnRemoveEmpire";
			this.btnRemoveEmpire.Size = new System.Drawing.Size(97, 24);
			this.btnRemoveEmpire.TabIndex = 23;
			this.btnRemoveEmpire.Text = "Remove";
			this.btnRemoveEmpire.UseVisualStyleBackColor = false;
			// 
			// btnRemoveVehicle
			// 
			this.btnRemoveVehicle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveVehicle.BackColor = System.Drawing.Color.Black;
			this.btnRemoveVehicle.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnRemoveVehicle.Location = new System.Drawing.Point(284, 90);
			this.btnRemoveVehicle.Name = "btnRemoveVehicle";
			this.btnRemoveVehicle.Size = new System.Drawing.Size(97, 24);
			this.btnRemoveVehicle.TabIndex = 25;
			this.btnRemoveVehicle.Text = "Remove";
			this.btnRemoveVehicle.UseVisualStyleBackColor = false;
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.btnRemoveCargo);
			this.panel6.Controls.Add(this.btnAddUnit);
			this.panel6.Controls.Add(this.btnDuplicateCargo);
			this.panel6.Controls.Add(this.gamePanel5);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel6.Location = new System.Drawing.Point(714, 429);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(381, 195);
			this.panel6.TabIndex = 22;
			// 
			// btnRemoveCargo
			// 
			this.btnRemoveCargo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveCargo.BackColor = System.Drawing.Color.Black;
			this.btnRemoveCargo.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnRemoveCargo.Location = new System.Drawing.Point(283, 61);
			this.btnRemoveCargo.Name = "btnRemoveCargo";
			this.btnRemoveCargo.Size = new System.Drawing.Size(97, 24);
			this.btnRemoveCargo.TabIndex = 25;
			this.btnRemoveCargo.Text = "Remove";
			this.btnRemoveCargo.UseVisualStyleBackColor = false;
			// 
			// btnAddUnit
			// 
			this.btnAddUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddUnit.BackColor = System.Drawing.Color.Black;
			this.btnAddUnit.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnAddUnit.Location = new System.Drawing.Point(284, 31);
			this.btnAddUnit.Name = "btnAddUnit";
			this.btnAddUnit.Size = new System.Drawing.Size(97, 24);
			this.btnAddUnit.TabIndex = 23;
			this.btnAddUnit.Text = "Add Unit";
			this.btnAddUnit.UseVisualStyleBackColor = false;
			// 
			// btnDuplicateCargo
			// 
			this.btnDuplicateCargo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDuplicateCargo.BackColor = System.Drawing.Color.Black;
			this.btnDuplicateCargo.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.btnDuplicateCargo.Location = new System.Drawing.Point(284, 1);
			this.btnDuplicateCargo.Name = "btnDuplicateCargo";
			this.btnDuplicateCargo.Size = new System.Drawing.Size(97, 24);
			this.btnDuplicateCargo.TabIndex = 22;
			this.btnDuplicateCargo.Text = "Duplicate";
			this.btnDuplicateCargo.UseVisualStyleBackColor = false;
			// 
			// gamePanel5
			// 
			this.gamePanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gamePanel5.BackColor = System.Drawing.Color.Black;
			this.gamePanel5.BorderColor = System.Drawing.Color.CornflowerBlue;
			this.gamePanel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.gamePanel5.Controls.Add(this.lstCargo);
			this.gamePanel5.ForeColor = System.Drawing.Color.White;
			this.gamePanel5.Location = new System.Drawing.Point(3, 0);
			this.gamePanel5.Name = "gamePanel5";
			this.gamePanel5.Padding = new System.Windows.Forms.Padding(3);
			this.gamePanel5.Size = new System.Drawing.Size(278, 195);
			this.gamePanel5.TabIndex = 21;
			// 
			// lstCargo
			// 
			this.lstCargo.BackColor = System.Drawing.Color.Black;
			this.lstCargo.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lstCargo.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstCargo.Location = new System.Drawing.Point(3, 3);
			this.lstCargo.Margin = new System.Windows.Forms.Padding(0);
			this.lstCargo.Name = "lstCargo";
			this.lstCargo.Size = new System.Drawing.Size(270, 187);
			this.lstCargo.TabIndex = 0;
			this.lstCargo.UseCompatibleStateImageBehavior = false;
			this.lstCargo.View = System.Windows.Forms.View.List;
			// 
			// gamePanel6
			// 
			this.gamePanel6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gamePanel6.BackColor = System.Drawing.Color.Black;
			this.gamePanel6.BorderColor = System.Drawing.Color.CornflowerBlue;
			this.gamePanel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tableLayoutPanel1.SetColumnSpan(this.gamePanel6, 2);
			this.gamePanel6.Controls.Add(this.designReport1);
			this.gamePanel6.Controls.Add(this.designReport);
			this.gamePanel6.ForeColor = System.Drawing.Color.White;
			this.gamePanel6.Location = new System.Drawing.Point(389, 27);
			this.gamePanel6.Name = "gamePanel6";
			this.gamePanel6.Padding = new System.Windows.Forms.Padding(3);
			this.tableLayoutPanel1.SetRowSpan(this.gamePanel6, 3);
			this.gamePanel6.Size = new System.Drawing.Size(319, 597);
			this.gamePanel6.TabIndex = 23;
			// 
			// designReport
			// 
			this.designReport.BackColor = System.Drawing.Color.Black;
			this.designReport.Design = null;
			this.designReport.Dock = System.Windows.Forms.DockStyle.Fill;
			this.designReport.ForeColor = System.Drawing.Color.White;
			this.designReport.Location = new System.Drawing.Point(3, 3);
			this.designReport.Name = "designReport";
			this.designReport.Size = new System.Drawing.Size(317, 414);
			this.designReport.TabIndex = 1;
			// 
			// designReport1
			// 
			this.designReport1.BackColor = System.Drawing.Color.Black;
			this.designReport1.Design = null;
			this.designReport1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.designReport1.ForeColor = System.Drawing.Color.White;
			this.designReport1.Location = new System.Drawing.Point(-62, 90);
			this.designReport1.Name = "designReport1";
			this.designReport1.Size = new System.Drawing.Size(317, 414);
			this.designReport1.TabIndex = 2;
			// 
			// CombatSimulatorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(1098, 654);
			this.Controls.Add(this.tableLayoutPanel1);
			this.ForeColor = System.Drawing.Color.White;
			this.Name = "CombatSimulatorForm";
			this.Text = "Combat Simulator";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.gamePanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.gamePanel2.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.gamePanel3.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.gamePanel4.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			this.gamePanel5.ResumeLayout(false);
			this.gamePanel6.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lblQuantityUnit;
		private Controls.GameButton btnCancel;
		private Controls.GameButton btnOK;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.CheckBox chkHideObsolete;
		private System.Windows.Forms.CheckBox chkForeign;
		private System.Windows.Forms.ComboBox ddlVehicleType;
		private Controls.GamePanel gamePanel1;
		private System.Windows.Forms.ListView lstDesigns;
		private System.Windows.Forms.ColumnHeader colDesign;
		private System.Windows.Forms.ColumnHeader colTotalCost;
		private System.Windows.Forms.Panel panel3;
		private Controls.GamePanel gamePanel2;
		private System.Windows.Forms.ListView lstEmpires;
		private Controls.GameButton btnDuplicateEmpire;
		private System.Windows.Forms.Panel panel5;
		private Controls.GameButton btnAddVehicle;
		private Controls.GameButton btnDuplicateVehicle;
		private Controls.GamePanel gamePanel4;
		private System.Windows.Forms.ListView lstSpaceObjects;
		private System.Windows.Forms.Panel panel4;
		private Controls.GameButton gameButton1;
		private Controls.GamePanel gamePanel3;
		private System.Windows.Forms.ListView listView1;
		private Controls.GameButton btnRemoveVehicle;
		private Controls.GameButton btnAddPlanet;
		private Controls.GameButton btnRemoveEmpire;
		private System.Windows.Forms.Panel panel6;
		private Controls.GameButton btnRemoveCargo;
		private Controls.GameButton btnAddUnit;
		private Controls.GameButton btnDuplicateCargo;
		private Controls.GamePanel gamePanel5;
		private System.Windows.Forms.ListView lstCargo;
		private Controls.GamePanel gamePanel6;
		private Controls.DesignReport designReport;
		private Controls.DesignReport designReport1;
	}
}