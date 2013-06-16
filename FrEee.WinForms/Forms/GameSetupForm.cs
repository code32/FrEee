﻿using FrEee.Game;
using FrEee.Game.Enumerations;
using FrEee.Game.Objects.Civilization;
using FrEee.Game.Objects.Space;
using FrEee.Game.Objects.Technology;
using FrEee.Game.Setup;
using FrEee.Game.Setup.WarpPointPlacementStrategies;
using FrEee.Modding;
using FrEee.Modding.Templates;
using FrEee.Utility;
using FrEee.WinForms.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FrEee.WinForms.Forms
{
	public partial class GameSetupForm : Form
	{
		public GameSetupForm()
		{
			InitializeComponent();
			setup = new GameSetup();
			if (Mod.Current == null)
				Mod.Load(null);

			// bind data
			galaxyTemplateBindingSource.DataSource = Mod.Current.GalaxyTemplates;
			warpPointPlacementStrategyBindingSource.DataSource = WarpPointPlacementStrategy.All;
			lstTechs.Items.AddRange(Mod.Current.Technologies.Where(t => t.CanBeRemoved).ToArray());
			stellarObjectSizeBindingSource.DataSource = Mod.Current.StellarObjectTemplates.OfType<Planet>().Select(p => p.Size).Distinct();
			// TODO - set step-amount for racial points spinbox to the greatest common factor of the mod's racial trait costs

			// initialize data
			ddlGalaxyType_SelectedIndexChanged(ddlGalaxyType, new EventArgs());
			spnWidth_ValueChanged(spnWidth, new EventArgs());
			spnHeight_ValueChanged(spnHeight, new EventArgs());
			spnStarSystems_ValueChanged(spnStarSystems, new EventArgs());
			ddlWarpPointLocation_SelectedIndexChanged(ddlWarpPointLocation, new EventArgs());
			ddlStartTech.SelectedIndex = 0;
			for (int i = 0; i < lstTechs.Items.Count; i++)
			{
				lstTechs.SetItemChecked(i, true);
			}
			ddlEmpirePlacement.SelectedIndex = 2; // equidistant
			ddlScoreDisplay.SelectedIndex = 0; // own only, no ranking
		}

		private GameSetup setup;

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			setup.GameName = txtGalaxyName.Text;
			setup.AllSystemsExplored = chkAllSystemsExplored.Checked;
			setup.OmniscientView = chkOmniscient.Checked;
			setup.StandardMiningModel = new MiningModel
			{
				RatePercentage = (double)spnRateStandard.Value,
				ValuePercentageBonus = (double)spnBonusStandard.Value,
				ValueDepletionPerResource = (double)spnDepletionResourceStandard.Value,
				BonusAffectsDepletion = chkBonusDepletionStandard.Checked,
				ValueDepletionPerTurn = (int)spnDepletionTurnStandard.Value,
			};
			setup.RemoteMiningModel = new MiningModel
			{
				RatePercentage = (double)spnRateRemote.Value,
				ValuePercentageBonus = (double)spnBonusRemote.Value,
				ValueDepletionPerResource = (double)spnDepletionResourceRemote.Value,
				BonusAffectsDepletion = chkBonusDepletionRemote.Checked,
				ValueDepletionPerTurn = (int)spnDepletionTurnRemote.Value,
			};
			setup.MinPlanetValue = (int)spnMinValuePlanet.Value;
			setup.MinSpawnedPlanetValue = (int)spnMinSpawnValuePlanet.Value;
			setup.HomeworldValue = (int)spnHomeworldValue.Value;
			setup.MaxSpawnedPlanetValue = (int)spnMaxSpawnValuePlanet.Value;
			setup.MaxPlanetValue = (int)spnMaxValuePlanet.Value;
			setup.MinAsteroidValue = (int)spnMinValueAsteroid.Value;
			setup.MinSpawnedAsteroidValue = (int)spnMinSpawnValueAsteroid.Value;
			setup.MaxSpawnedAsteroidValue = (int)spnMaxSpawnValueAsteroid.Value;
			switch (ddlStartTech.SelectedIndex)
			{
				case 0:
					setup.StartingTechnologyLevel = StartingTechnologyLevel.Low;
					break;
				case 1:
					setup.StartingTechnologyLevel = StartingTechnologyLevel.Medium;
					break;
				case 2:
					setup.StartingTechnologyLevel = StartingTechnologyLevel.High;
					break;
			}
			for (int i = 0; i < lstTechs.Items.Count; i++)
			{
				if (!lstTechs.GetItemChecked(i))
					setup.ForbiddenTechnologies.Add((Technology)lstTechs.Items[i]);
			}
			setup.StartingResources = (int)spnStartResources.Value;
			setup.ResourceStorage = (int)spnResourceStorage.Value;
			setup.StartingResearch = (int)spnStartResearch.Value;
			setup.HomeworldsPerEmpire = (int)spnHomeworlds.Value;
			setup.HomeworldSize = (StellarObjectSize)ddlHomeworldSize.SelectedItem;
			setup.EmpirePlacement = (EmpirePlacement)(ddlEmpirePlacement.SelectedIndex);
			setup.MaxHomeworldDispersion = (int)spnMaxDispersion.Value;
			setup.ScoreDisplay = (ScoreDisplay)(ddlScoreDisplay.SelectedIndex);
			setup.EmpirePoints = (int)spnEmpirePoints.Value;
			setup.RandomAIs = (int)spnRandomAIs.Value;
			setup.MinorEmpires = (int)spnMinorEmpires.Value;

			if (setup.Warnings.Any())
			{
				MessageBox.Show(setup.Warnings.First(), "FrEee");
				return;
			}

			progressBar.Visible = true;
			var status = new Status
			{
				Progress = 0d,
				Message = "Initializing",
				Exception = null,
			};
			Thread t = new Thread(new ThreadStart(() =>
			{
				try
				{
					status.Message = "Setting up galaxy";
					Galaxy.Initialize(setup, status, 1d);
					if (Galaxy.Current.IsSinglePlayer)
					{
						var name = Galaxy.Current.Name;
						var turn = Galaxy.Current.TurnNumber;
						status.Message = "Loading game";
						Galaxy.Load(name + "_" + turn + "_1.gam");
					}
				}
				catch (Exception ex)
				{
					status.Exception = ex;
				}
			}));
			t.Name = "Game Setup";
			t.Start();
			while (t.IsAlive)
			{
				if (status.Exception != null)
				{
					Text = "FrEee - Error";
					MessageBox.Show(status.Exception.Message + "\n\nPlease check errorlog.txt for more details.");
					Enabled = true;
					IsBusy = false;
					progressBar.Value = 0;
					var sw = new StreamWriter("errorlog.txt");
					sw.WriteLine(status.Exception.GetType().Name + " occurred at " + DateTime.Now + ":");
					sw.WriteLine(status.Exception.ToString());
					sw.Close();
					t.Abort();
					break;
				}
				else
				{
					Text = "Game Setup - " + status.Message;
					progressBar.Value = (int)(progressBar.Maximum * status.Progress);
					Application.DoEvents();
				}
			}

			if (status.Exception == null)
			{
				if (Galaxy.Current.IsSinglePlayer)
				{
					var name = Galaxy.Current.Name;
					var turn = Galaxy.Current.TurnNumber;
					status.Message = "Loading game";
					Galaxy.Load(name + "_" + turn + "_1.gam");
					Hide();
					MainMenuForm.GetInstance().ShowChildForm(new GameForm(Galaxy.Current));
				}
				else
				{
					MessageBox.Show("The game \"" + Galaxy.Current.Name + "\" has been created. Please distribute the GAM files to appropriate players.");
					Close();
				}
			}
		}

		private void btnLoadSetup_Click(object sender, EventArgs e)
		{
			// TODO - load setup
			MessageBox.Show("Sorry, loading a game setup is not yet supported.");
		}

		private void btnSaveSetup_Click(object sender, EventArgs e)
		{
			// TODO - save setup
			MessageBox.Show("Sorry, saving a game setup is not yet supported.");
		}

		private void ddlGalaxyType_SelectedIndexChanged(object sender, EventArgs e)
		{
			var gt = (GalaxyTemplate)ddlGalaxyType.SelectedItem;
			if (gt == null)
				txtGalaxyTypeDescription.Text = "Please select a galaxy type.";
			else
				txtGalaxyTypeDescription.Text = gt.Description;
			setup.GalaxyTemplate = gt;
		}

		private bool _isBusy;
		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			set
			{
				_isBusy = value;
				tabs.Enabled = btnCancel.Enabled = btnStart.Enabled = btnLoadSetup.Enabled = btnSaveSetup.Enabled = !IsBusy;
				progressBar.Visible = IsBusy;
			}
		}

		private void spnWidth_ValueChanged(object sender, EventArgs e)
		{
			spnStarSystems.Maximum = spnWidth.Value * spnHeight.Value;
			setup.GalaxySize = new Size((int)spnWidth.Value, (int)spnHeight.Value);
		}

		private void spnHeight_ValueChanged(object sender, EventArgs e)
		{
			spnStarSystems.Maximum = spnWidth.Value * spnHeight.Value;
			setup.GalaxySize = new Size((int)spnWidth.Value, (int)spnHeight.Value);
		}

		private void spnStarSystems_ValueChanged(object sender, EventArgs e)
		{
			setup.StarSystemCount = (int)spnStarSystems.Value;
			spnSystemGroups.Maximum = spnStarSystems.Value;
		}

		private void ddlWarpPointLocation_SelectedIndexChanged(object sender, EventArgs e)
		{
			setup.WarpPointPlacementStrategy = (WarpPointPlacementStrategy)ddlWarpPointLocation.SelectedItem;
			txtWarpPointLocation.Text = setup.WarpPointPlacementStrategy.Description;
		}

		private void spnSystemGroups_ValueChanged(object sender, EventArgs e)
		{
			setup.StarSystemGroups = (int)spnSystemGroups.Value;
		}

		private void btnLoadResourcePreset_Click(object sender, EventArgs e)
		{
			// TODO - customizable presets?
			if (ddlPresets.SelectedIndex == 0)
			{
				// standard, remote mining depletes
				spnRateStandard.Value = spnRateRemote.Value = 0;
				chkLimitStandard.Checked = chkLimitRemote.Checked = false;
				spnBonusStandard.Value = spnBonusRemote.Value = 1;
				spnDepletionResourceStandard.Value = spnDepletionResourceRemote.Value = 0;
				chkBonusDepletionStandard.Checked = chkBonusDepletionRemote.Checked = false;
				spnDepletionTurnStandard.Value = 0;
				spnDepletionTurnRemote.Value = 1;
				spnMinValuePlanet.Value = spnMinValueAsteroid.Value = 0;
				spnMinSpawnValuePlanet.Value = 0;
				spnMinSpawnValueAsteroid.Value = 50;
				spnHomeworldValue.Value = 120;
				spnMaxSpawnValuePlanet.Value = 150;
				spnMaxSpawnValueAsteroid.Value = 300;
				spnMaxValuePlanet.Value = 250;
				spnStartValue.Value = 100;
			}
			else if (ddlPresets.SelectedIndex == 1)
			{
				// standard, remote mining doesn't deplete
				spnRateStandard.Value = spnRateRemote.Value = 0;
				chkLimitStandard.Checked = chkLimitRemote.Checked = false;
				spnBonusStandard.Value = spnBonusRemote.Value = 1;
				spnDepletionResourceStandard.Value = spnDepletionResourceRemote.Value = 0;
				chkBonusDepletionStandard.Checked = chkBonusDepletionRemote.Checked = false;
				spnDepletionTurnStandard.Value = 0;
				spnDepletionTurnRemote.Value = 0;
				spnMinValuePlanet.Value = spnMinValueAsteroid.Value = 0;
				spnMinSpawnValuePlanet.Value = 0;
				spnMinSpawnValueAsteroid.Value = 50;
				spnStartValue.Value = spnHomeworldValue.Value = 120;
				spnMaxSpawnValuePlanet.Value = 150;
				spnMaxSpawnValueAsteroid.Value = 300;
				spnMaxValuePlanet.Value = 250;
				spnStartValue.Value = 100;
			}
			else if (ddlPresets.SelectedIndex == 2)
			{
				// finite
				spnRateStandard.Value = spnRateRemote.Value = 100;
				chkLimitStandard.Checked = chkLimitRemote.Checked = true;
				spnBonusStandard.Value = spnBonusRemote.Value = 0;
				spnDepletionResourceStandard.Value = spnDepletionResourceRemote.Value = 1;
				chkBonusDepletionStandard.Checked = chkBonusDepletionRemote.Checked = true;
				spnDepletionTurnStandard.Value = 0;
				spnDepletionTurnRemote.Value = 0;
				spnMinValuePlanet.Value = spnMinValueAsteroid.Value = 0;
				spnMinSpawnValuePlanet.Value = 0;
				spnMinSpawnValueAsteroid.Value = 100e3m;
				spnStartValue.Value = spnHomeworldValue.Value = 2e6m;
				spnMaxSpawnValuePlanet.Value = 500e3m;
				spnMaxSpawnValueAsteroid.Value = 800e3m;
				spnMaxValuePlanet.Value = 10e6m;
				spnStartValue.Value = 500000;
			}
			btnRefreshGraphs_Click(btnLoadResourcePreset, new EventArgs());
		}

		private int startValue = 120;
		private int miningRate = 1000;
		private bool remote = false;
		private bool limit = false;

		private void btnRefreshGraphs_Click(object sender, EventArgs e)
		{
			picMiningGraph.Invalidate();
			picValueGraph.Invalidate();
		}

		private void picValueGraph_Paint(object sender, PaintEventArgs e)
		{
			var data = RunMiningSimulation(100).Select(t => t.Item2);
			DrawLineGraph(e.Graphics, new RectangleF(Point.Empty, picValueGraph.Size), "Value", Color.Green, data);
		}

		private void picMiningGraph_Paint(object sender, PaintEventArgs e)
		{
			var data = RunMiningSimulation(100).Select(t => t.Item1);
			DrawLineGraph(e.Graphics, new RectangleF(Point.Empty, picMiningGraph.Size), "Mining", Color.Blue, data);
		}

		private void DrawLineGraph(Graphics g, RectangleF bounds, string label, Color lineColor, IEnumerable<int> dataPoints)
		{
			var max = dataPoints.Max();
			if (max <= 0)
				return;
			var xScale = bounds.Width / (dataPoints.Count() - 1);
			var yScale = -bounds.Height / max;

			// draw Y-axis
			var pen = Pens.White;
			g.DrawLine(pen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
			var font = new Font(FontFamily.GenericSansSerif, 9);
			var brush = Brushes.White;
			g.DrawString(label, font, brush, 0, 0);

			// draw horizontals
			var sf = new StringFormat { Alignment = StringAlignment.Far };
			for (int line = 0; line <= 5; line++)
			{
				var ypos = line * bounds.Height / -5 + bounds.Bottom;
				g.DrawLine(pen, bounds.Left, ypos, bounds.Right, ypos);
				g.DrawString((max * line / 5).ToString(), font, Brushes.White, bounds.Right, ypos, sf);
			}

			// draw chart
			int prev = dataPoints.First();
			var linePen = new Pen(lineColor);
			for (int i = 1; i < dataPoints.Count(); i++)
			{
				var cur = dataPoints.ElementAt(i);
				var xpos1 = bounds.Left + (i - 1) * xScale;
				var xpos2 = bounds.Left + i * xScale;
				var ypos1 = bounds.Bottom + prev * yScale;
				var ypos2 = bounds.Bottom + cur * yScale;
				g.DrawLine(linePen, xpos1, ypos1, xpos2, ypos2);
				prev = cur;
			}
		}

		private IEnumerable<Tuple<int, int>> RunMiningSimulation(int turns)
		{
			var model = new MiningModel();
			if (remote)
			{
				model.RatePercentage = (double)spnRateRemote.Value;
				model.LimitRateToValue = chkLimitRemote.Checked;
				model.ValuePercentageBonus = (double)spnBonusRemote.Value;
				model.ValueDepletionPerResource = (double)spnDepletionResourceRemote.Value;
				model.BonusAffectsDepletion = chkBonusDepletionRemote.Checked;
				model.ValueDepletionPerTurn = (int)spnDepletionTurnRemote.Value;
			}
			else
			{
				model.RatePercentage = (double)spnRateStandard.Value;
				model.LimitRateToValue = chkLimitStandard.Checked;
				model.ValuePercentageBonus = (double)spnBonusStandard.Value;
				model.ValueDepletionPerResource = (double)spnDepletionResourceStandard.Value;
				model.BonusAffectsDepletion = chkBonusDepletionStandard.Checked;
				model.ValueDepletionPerTurn = (int)spnDepletionTurnStandard.Value;
			}
			int mined;
			int value = startValue;
			for (int i = 0; i < turns; i++)
			{
				mined = model.GetRate(miningRate, value, 1d);
				yield return Tuple.Create(mined, value);
				value -= model.GetDecay(miningRate, value);
			}
		}

		private void spnStartValue_ValueChanged(object sender, EventArgs e)
		{
			startValue = (int)spnStartValue.Value;
		}

		private void spnMiningRate_ValueChanged(object sender, EventArgs e)
		{
			miningRate = (int)spnMiningRate.Value;
		}

		private void chkRemote_CheckedChanged(object sender, EventArgs e)
		{
			remote = chkRemote.Checked;
		}

		private void spnResourceStorage_ValueChanged(object sender, EventArgs e)
		{
			spnResourceStorage.Maximum = spnResourceStorage.Value;
		}

		private void BindEmpires()
		{
			lstEmpires.Initialize(32, 32);
			foreach (var et in setup.EmpireTemplates)
				lstEmpires.AddItemWithImage(null, et.IsPlayerEmpire ? (et.Name ?? et.PrimaryRace.EmpireName) : ("(AI) " + (et.Name ?? et.PrimaryRace.EmpireName)), et, et.Insignia);
		}

		private void btnCreateEmpire_Click(object sender, EventArgs e)
		{
			var et = new EmpireTemplate();
			var form = new EmpireSetupForm();
			form.PointsToSpend = (int)spnEmpirePoints.Value;
			form.EmpireTemplate = et;
			this.ShowChildForm(form);
			if (form.DialogResult == DialogResult.OK)
			{
				setup.EmpireTemplates.Add(et);
				BindEmpires();
			}
		}

		private void btnLoadEmpire_Click(object sender, EventArgs e)
		{

		}

		private void btnEditEmpire_Click(object sender, EventArgs e)
		{

		}

		private void btnRemoveEmpire_Click(object sender, EventArgs e)
		{

		}

		private void btnSaveEmpire_Click(object sender, EventArgs e)
		{

		}

		private void btnToggleAI_Click(object sender, EventArgs e)
		{

		}
	}
}
