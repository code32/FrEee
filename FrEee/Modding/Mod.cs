﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FrEee.Game;
using System.Drawing;
using FrEee.Modding.Loaders;
using FrEee.Modding.Templates;

namespace FrEee.Modding
{
	/// <summary>
	/// A set of data files containing templates for game objects.
	/// </summary>
	public class Mod
	{		
		/// <summary>
		/// The currently loaded mod.
		/// </summary>
		public static Mod Current { get; private set; }

		/// <summary>
		/// The file name being loaded. (For error reporting)
		/// </summary>
		public static string CurrentFileName { get; private set; }

		/// <summary>
		/// Loads a mod.
		/// </summary>
		/// <param name="path">The mod root path, relative to the Mods folder. Or null to load the stock mod.</param>
		/// <param name="setCurrent">Set the current mod to the new mod?</param>
		public static Mod Load(string path, bool setCurrent = true)
		{
			var mod = new Mod();

			var datapath = path == null ? "Data" : Path.Combine("Mods", path, "Data");

			// TODO - fall back on stock when mod data not found

			foreach (var line in File.ReadAllLines(Path.Combine(datapath, "SystemNames.txt")))
				mod.StarSystemNames.Add(line);

			CurrentFileName = Path.Combine(datapath, "AbilityRules.txt");
			new AbilityRuleLoader().Load(new DataFile(File.ReadAllText(CurrentFileName)), mod);

			CurrentFileName = Path.Combine(datapath, "SectType.txt");
			new StellarObjectLoader().Load(new DataFile(File.ReadAllText(CurrentFileName)), mod);

			CurrentFileName = Path.Combine(datapath, "TechArea.txt");
			new TechnologyLoader().Load(new DataFile(File.ReadAllText(CurrentFileName)), mod);

			CurrentFileName = Path.Combine(datapath, "Facility.txt");
			new FacilityLoader().Load(new DataFile(File.ReadAllText(CurrentFileName)), mod);

			CurrentFileName = Path.Combine(datapath, "StellarAbilityTypes.txt");
			new StellarAbilityLoader().Load(new DataFile(File.ReadAllText(CurrentFileName)), mod);

			CurrentFileName = Path.Combine(datapath, "SystemTypes.txt");
			new StarSystemLoader().Load(new DataFile(File.ReadAllText(CurrentFileName)), mod);

			CurrentFileName = Path.Combine(datapath, "QuadrantTypes.txt");
			new GalaxyLoader().Load(new DataFile(File.ReadAllText(CurrentFileName)), mod);

			CurrentFileName = null;

			if (setCurrent)
				Current = mod;

			// TODO - display errors to user
			foreach (var err in Mod.Errors)
				Console.WriteLine(err.Message);

			return mod;
		}

		public Mod()
		{
			Errors = new List<DataParsingException>();
			AbilityRules = new List<AbilityRule>();
			StarSystemNames = new List<string>();
			Technologies = new List<Technology>();
			Facilities = new List<Facility>();
			StarSystemTemplates = new List<StarSystemTemplate>();
			StellarAbilityTemplates = new List<RandomAbilityTemplate>();
			GalaxyTemplates = new List<GalaxyTemplate>();
			StellarObjectTemplates = new List<StellarObject>();
		}

		/// <summary>
		/// Rules for grouping and stacking abilities.
		/// </summary>
		public ICollection<AbilityRule> AbilityRules { get; private set; }

		/// <summary>
		/// Names to use for star systems.
		/// </summary>
		public ICollection<string> StarSystemNames { get; private set; }

		/// <summary>
		/// Templates for star systems.
		/// </summary>
		public ICollection<StarSystemTemplate> StarSystemTemplates { get; private set; }

		/// <summary>
		/// The facilities in the mod.
		/// </summary>
		public ICollection<Facility> Facilities { get; private set; }

		/// <summary>
		/// Templates for stellar abilities.
		/// </summary>
		public ICollection<RandomAbilityTemplate> StellarAbilityTemplates { get; private set; }

		/// <summary>
		/// Templates for galaxies.
		/// </summary>
		public ICollection<GalaxyTemplate> GalaxyTemplates { get; private set; }

		/// <summary>
		/// Templates for stellar objects.
		/// </summary>
		public ICollection<StellarObject> StellarObjectTemplates { get; private set; }

		/// <summary>
		/// The technologies in the game.
		/// </summary>
		public ICollection<Technology> Technologies { get; private set; }

		/// <summary>
		/// Errors encountered when loading the mod.
		/// </summary>
		public static IList<DataParsingException> Errors { get; private set; }

		// TODO - load these constants from Settings.txt
		public readonly int MinPlanetResourceValue = 0;
		public readonly int MaxPlanetResourceValue = 150;
		public readonly int MinAsteroidResourceValue = 50;
		public readonly int MaxAsteroidResourceValue = 300;
	}
}
