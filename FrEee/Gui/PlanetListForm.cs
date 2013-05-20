﻿using FrEee.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrEee.Gui
{
	public partial class PlanetListForm : Form
	{
		public PlanetListForm(Galaxy galaxy)
		{
			InitializeComponent();
			this.galaxy = galaxy;
            this.Icon = new Icon(Properties.Resources.FrEeeIcon);
		}

		private Galaxy galaxy;

		private void PlanetListForm_Load(object sender, EventArgs e)
		{
			if (galaxy == null)
				return;

			// show planet counts
			var systems = galaxy.ExploredStarSystems;
			txtSystems.Text = systems.Count().ToString();
			// HACK - why are there null explored star systems?
			var planets = systems.Where(sys => sys != null).SelectMany(sys => sys.FindSpaceObjects<Planet>().SelectMany(g => g));
			txtPlanets.Text = planets.Count().ToString();
			// TODO - colonizable planets and various subcategories
			// TODO - colony ships

			// show galaxy view
			galaxyView.Galaxy = galaxy;

			// show planet data
			planetBindingSource.DataSource = planets.ToList();
		}

		private void gridPlanets_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			// ignore silly errors
			e.ThrowException = false;
		}

		private void gridPlanets_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
			var planet = (Planet)gridPlanets.Rows[e.RowIndex].DataBoundItem;
			foreach (var sys in galaxy.ExploredStarSystems)
			{
				if (sys.FindSpaceObjects<Planet>().SelectMany(g => g).Any(p => p == planet))
				{
					galaxyView.SelectedStarSystem = sys;
					break;
				}
			}
		}

		private void gridPlanets_RowLeave(object sender, DataGridViewCellEventArgs e)
		{
			galaxyView.SelectedStarSystem = null;
		}
	}
}
