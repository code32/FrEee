﻿using FrEee.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrEee.Game.Enumerations;
using FrEee.Game.Interfaces;
using FrEee.Game.Objects.Space;
using FrEee.Utility;

namespace FrEee.Modding.Templates
{
	/// <summary>
	/// A template for generating storms.
	/// </summary>
	public class StormTemplate : ITemplate<Storm>
	{
		/// <summary>
		/// Abilities to assign to the storm.
		/// </summary>
		public RandomAbilityTemplate Abilities { get; set; }

		/// <summary>
		/// The size of the storm, or null to choose a size randomly.
		/// </summary>
		public Size? Size { get; set; }

		public Storm Instantiate()
		{
			var candidates = Mod.Current.StellarObjectTemplates.OfType<Storm>();
			if (Size != null)
				candidates = candidates.Where(p => p.Size == Size.Value);
			if (!candidates.Any())
				throw new Exception("No storms in SectType.txt match the criteria!");

			var storm = candidates.PickRandom().Instantiate();

			var abil = Abilities.Instantiate();
			if (abil != null)
				storm.IntrinsicAbilities.Add(abil);

			return storm;
		}
	}
}
