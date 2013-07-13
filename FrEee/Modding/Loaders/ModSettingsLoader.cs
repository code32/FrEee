﻿using FrEee.Game.Objects.Civilization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrEee.Modding.Loaders
{
	/// <summary>
	/// Loads mod settings from Settings.txt.
	/// </summary>
	public class ModSettingsLoader : DataFileLoader
	{
		public const string Filename = "Settings.txt";

		public ModSettingsLoader(string modPath)
			: base(modPath, Filename, DataFile.Load(modPath, Filename))
		{
		}

		public override void Load(Mod mod)
		{
			var rec = DataFile.Records.FirstOrDefault();
			if (rec == null)
			{
				Mod.Errors.Add(new DataParsingException("Could not load Settings.txt - no record found.", Filename));
				return;
			}

			int index = -1;

			var settings = new ModSettings();
			mod.Settings = settings;

			// TODO - load more settings

			settings.MaintenanceDeficitToDestroyOneShip = rec.GetInt("Maintenance Cost Amt Per Dead", ref index);
			settings.ShipBaseMaintenanceRate = rec.GetNullInt("Empire Ship And Base Percent Maint Cost", ref index) ?? rec.GetInt("Empire Starting Percent Maint Cost", ref index);
			settings.UnitMaintenanceRate = rec.GetNullInt("Empire Unit Percent Maint Cost", ref index) ?? 0;
			settings.UnitMaintenanceRate = rec.GetNullInt("Empire Facility Percent Maint Cost", ref index) ?? 0;
			settings.Reproduction = rec.GetNullInt("Empire Starting Percent Reproduction", ref index) ?? 10;
			settings.ReproductionMultiplier = rec.GetNullDouble("Reproduction Multiplier", ref index) ?? 0.1;

			// TODO - load more settings

			// load aptitudes
			foreach (var a in Aptitude.All)
			{
				a.MinPercent = rec.GetInt("Characteristic " + a.Name + " Min Pct", ref index);
				a.MaxPercent = rec.GetInt("Characteristic " + a.Name + " Max Pct", ref index);
				a.Cost = rec.GetInt("Characteristic " + a.Name + " Pct Cost", ref index);
				a.Threshold = rec.GetInt("Characteristic " + a.Name + " Threshold", ref index);
				a.LowCost = rec.GetInt("Characteristic " + a.Name + " Threshhold Pct Cost Neg", ref index);
				a.HighCost = rec.GetInt("Characteristic " + a.Name + " Threshhold Pct Cost Pos", ref index);
			}

			// TODO - load more settings

			settings.ReproductionDelay = rec.GetNullInt("Reproduction Check Frequency", ref index) ?? 1;

			// TODO - load more settings
		}
	}
}
