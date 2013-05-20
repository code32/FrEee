﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FrEee.Game
{
	/// <summary>
	/// A galaxy in which the game is played.
	/// </summary>
	public class Galaxy
	{
		public Galaxy()
		{
			StarSystemLocations = new List<ObjectLocation<StarSystem>>();
			Empires = new List<Empire>();
		}

		/// <summary>
		/// The locations of the star systems in the galaxy.
		/// </summary>
		public ICollection<ObjectLocation<StarSystem>> StarSystemLocations { get; private set; }

		/// <summary>
		/// The empires participating in the game.
		/// </summary>
		public IList<Empire> Empires { get; private set; }

		/// <summary>
		/// The empire whose turn it is.
		/// </summary>
		public Empire CurrentEmpire { get; set; }

		public int MinX { get { return StarSystemLocations.MinOrDefault(ssl => ssl.Location.X); } }

		public int MinY { get { return StarSystemLocations.MinOrDefault(ssl => ssl.Location.Y); } }

		public int MaxX { get { return StarSystemLocations.MaxOrDefault(ssl => ssl.Location.X); } }

		public int MaxY { get { return StarSystemLocations.MaxOrDefault(ssl => ssl.Location.Y); } }

		public int Width { get { return MaxX - MinX + 1; } }

		public int Height { get { return MaxY - MinY + 1; } }

		/// <summary>
		/// Removes any space objects, etc. that the current empire cannot see.
		/// </summary>
		public void Redact()
		{
			if (CurrentEmpire != null)
			{
				foreach (var ssl in StarSystemLocations)
				{
					ssl.Item.Redact(this);
				}
			}
		}

		/// <summary>
		/// Star systems explored by the current empire.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<StarSystem> ExploredStarSystems
		{
			get { return StarSystemLocations.Select(ssl => ssl.Item).Where(sys => sys.ExploredByEmpires.Contains(CurrentEmpire)); }
		}

		/// <summary>
		/// Planets colonized by the current empire.
		/// </summary>
		[JsonIgnore]
		public IEnumerable<Planet> ColonizedPlanets
		{
			get
			{
				return StarSystemLocations.Select(ssl => ssl.Item).SelectMany(ss => ss.FindSpaceObjects<Planet>(p => p.Owner == CurrentEmpire).Flatten());
			}
		}

		/// <summary>
		/// Income (minus expenses) of the current empire.
		/// </summary>
		[JsonIgnore]
		public Resources Income
		{
			get
			{
				// TODO - take into account maintenance costs
				return ColonizedPlanets.Select(p => p.Income).Aggregate((r1, r2) => r1 + r2);
			}
		}
	}
}
