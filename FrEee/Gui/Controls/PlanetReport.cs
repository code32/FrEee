﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FrEee.Game;

namespace FrEee.Gui.Controls
{
	public partial class PlanetReport : UserControl
	{
		public PlanetReport()
		{
			InitializeComponent();
		}

		public PlanetReport(Planet planet)
			: this()
		{
			Planet = planet;
		}

		private Planet planet;

		/// <summary>
		/// The planet for which to display a report.
		/// </summary>
		public Planet Planet
		{
			get { return planet; }
			set
			{
				planet = value;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Planet == null)
				Visible = false;
			else
			{
				Visible = true;

				picOwnerFlag.Image = Planet.Owner == null ? null : Planet.Owner.Flag;
				picPortrait.Image = Planet.Portrait;

				txtName.Text = Planet.Name;
				txtSizeSurface.Text = Planet.Size + " " + Planet.Surface + " Planet";
				txtAtmosphere.Text = Planet.Atmosphere;
				txtConditions.Text = ""; // TODO - load conditions

				txtValueMinerals.Text = Planet.ResourceValue["minerals"] + "%";
				txtValueOrganics.Text = Planet.ResourceValue["organics"] + "%";
				txtValueRadioactives.Text = Planet.ResourceValue["radioactives"] + "%";

				txtDescription.Text = Planet.Description;

				txtColonyType.Text = Planet.Owner == null ? "" : Planet.Owner.Name + " Colony"; // TODO - load colony type
				txtPopulation.Text = ""; // TODO - load population
				txtReproduction.Text = ""; // TODO - load reproduction
				txtMood.Text = ""; // TODO - load mood

				// TODO - load income
				resIncomeMinerals.Amount = 0;
				resIncomeOrganics.Amount = 0;
				resIncomeRadioactives.Amount = 0;
				resResearch.Amount = 0;
				resIntel.Amount = 0;

				// TODO - load construction data
				txtConstructionItem.Text = "";
				txtConstructionTime.Text = "";

				lstOrdersDetail.Items.Clear();
				// TODO - load orders

				// TODO - load facilities
				txtFacilitySlotsFree.Text = "0 / 0 slots free";
				lstFacilitiesDetail.Items.Clear();
				if (Planet.Colony != null)
				{
					var il = new ImageList();
					il.ImageSize = new System.Drawing.Size(32, 32);
					lstFacilitiesDetail.LargeImageList = il;
					lstFacilitiesDetail.SmallImageList = il;
					int i = 0;
					foreach (var fg in Planet.Colony.Facilities.GroupBy(f => f))
					{
						var item = new ListViewItem(fg.Count() + "x " + fg.Key.Name);
						item.ImageIndex = i;
						il.Images.Add(fg.Key.Icon);
						lstFacilitiesDetail.Items.Add(item);
						i++;
					}
				}

				// TODO - load cargo
				txtCargoSpaceFree.Text = "0 kT / 0 kT free";
				lstCargoDetail.Items.Clear();

				treeAbilities.Nodes.Clear();
				foreach (var group in Planet.Abilities.GroupBy(abil => abil.Name))
				{
					// TODO - deal with nonstacking abilities
					var branch = new TreeNode(group.Key + ": " + group.Sum(abil =>
						{
							double result = 0;
							double.TryParse(abil.Values.FirstOrDefault(), out result);
							return result;
						}));
					if (group.Any(abil => !Planet.IntrinsicAbilities.Contains(abil)))
						branch.NodeFont = new Font(Font, FontStyle.Italic);
					treeAbilities.Nodes.Add(branch);
					foreach (var abil in group)
					{
						var twig = new TreeNode(abil.Description);
						if (Planet.IntrinsicAbilities.Contains(abil))
							twig.NodeFont = new Font(Font, FontStyle.Italic);
						branch.Nodes.Add(twig);
					}
				}
			}

			base.OnPaint(e);
		}
	}
}
