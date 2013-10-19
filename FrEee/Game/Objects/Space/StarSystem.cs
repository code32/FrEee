using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FrEee.Game.Enumerations;
using FrEee.Game.Interfaces;
using FrEee.Game.Objects.Abilities;
using FrEee.Game.Objects.Civilization;
using FrEee.Utility;
using FrEee.Utility.Extensions;
using FrEee.Modding.Templates;

namespace FrEee.Game.Objects.Space
{
	/// <summary>
	/// A star system containing a grid of sectors.
	/// Is always square and always has an odd number of sectors across.
	/// </summary>
	[Serializable]
	public class StarSystem : IReferrable, IPictorial, IFoggable
	{
		/// <summary>
		/// Creates a star system.
		/// </summary>
		/// <param name="radius">The number of sectors counting outward from the center to the edge.</param>
		public StarSystem(int radius)
		{
			Radius = radius;
			Abilities = new List<Ability>();
			SpaceObjectLocations = new HashSet<ObjectLocation<ISpaceObject>>();
			ExploredByEmpires = new HashSet<Empire>();
		}

		/// <summary>
		/// The name of this star system.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The number of sectors counting outward from the center to the edge.
		/// </summary>
		public int Radius { get; private set; }

		/// <summary>
		/// The description of this star system.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The path to the background image, relative to Pictures/Systems.
		/// </summary>
		public string BackgroundImagePath { get; set; }

		public Image BackgroundImage
		{
			get
			{
				if (BackgroundImagePath == null)
					return null;
				return Pictures.GetCachedImage(Path.Combine("Pictures", "Systems", BackgroundImagePath));
			}
		}

		/// <summary>
		/// If true, empire homeworlds can be located in this system.
		/// </summary>
		public bool EmpiresCanStartIn { get; set; }

		/// <summary>
		/// If true, the background image for this system will be centered, not tiled, in combat.
		/// </summary>
		public bool NonTiledCenterCombatImage { get; set; }

		/// <summary>
		/// Any special abilities possessed by this star system.
		/// </summary>
		public IList<Ability> Abilities { get; private set; }

		/// <summary>
		/// Abilities for random warp points that appear in this system.
		/// </summary>
		public RandomAbilityTemplate WarpPointAbilities { get; set; }

		/// <summary>
		/// The number of sectors across the star system.
		/// </summary>
		public int Diameter
		{
			get { return Math.Max(0, Radius * 2 + 1); }
		}

		public bool AreCoordsInBounds(int x, int y)
		{
			return x >= -Radius && x <= Radius && y >= -Radius && y <= Radius;
		}

		public bool AreCoordsInBounds(Point p)
		{
			return AreCoordsInBounds(p.X, p.Y);
		}

		/// <summary>
		/// The space objects contained in this star system.
		/// </summary>
		public ICollection<ObjectLocation<ISpaceObject>> SpaceObjectLocations { get; private set; }

		/// <summary>
		/// Searches for space objects matching criteria.
		/// </summary>
		/// <typeparam name="T">The type of space object.</typeparam>
		/// <param name="criteria">The criteria.</param>
		/// <returns>The matching space objects, grouped by location.</returns>
		public ILookup<Point, T> FindSpaceObjects<T>(Func<T, bool> criteria = null) where T : ISpaceObject
		{
			return SpaceObjectLocations.Where(l => l.Item is T && (criteria == null || criteria((T)l.Item))).ToLookup(l => l.Location, l => (T)l.Item);
		}

		public IEnumerable<T> FindSpaceObjectsInSector<T>(Point coords, Func<T, bool> criteria = null) where T : ISpaceObject
		{
			var lookup = FindSpaceObjects<T>(criteria);
			if (lookup.Contains(coords))
				return lookup[coords];
			return Enumerable.Empty<T>();
		}

		public bool Contains(ISpaceObject sobj)
		{
			return SpaceObjectLocations.Any(l => l.Item == sobj);
		}

		public Point FindCoordinates(ISpaceObject sobj)
		{
			try
			{
				return SpaceObjectLocations.Single(l => l.Item == sobj).Location;
			}
			catch (Exception ex)
			{
				throw new Exception("Can't find coordinates of " + sobj + " in " + this + ".", ex);
			}
		}

		/// <summary>
		/// Empires which have explored this star system.
		/// </summary>
		public ICollection<Empire> ExploredByEmpires { get; private set; }

		/// <summary>
		/// Removes any space objects, etc. that the current empire cannot see.
		/// </summary>
		/// <param name="galaxy">The galaxy, for context.</param>
		public void Redact(Empire emp)
		{
			// TODO - just scan through the entire galaxy using reflection for objects of type IFoggable? maybe do this as part of serialization so we don't actually need to reload the galaxy each time?
			// hide space objects
			// TODO - don't use tuples, we don't use the point value anymore...
			var toRemove = new List<Tuple<Point, ISpaceObject>>();
			foreach (var group in FindSpaceObjects<ISpaceObject>().ToArray())
			{
				foreach (var sobj in group)
				{
					var vis = sobj.CheckVisibility(emp);
					if (vis != Visibility.Unknown)
						sobj.Redact(emp);
					else
						toRemove.Add(Tuple.Create(group.Key, sobj));
				}
			}
			foreach (var t in toRemove)
				Remove(t.Item2);

			// hide explored-by empires
			foreach (var e in ExploredByEmpires.Where(e => e != emp).ToArray())
				ExploredByEmpires.Remove(e);

			// hide background image (so player can't see what kind of system it is) and name and abilities
			if (!ExploredByEmpires.Contains(emp))
			{
				BackgroundImagePath = null;
				Name = "(Unexplored)";
				Abilities.Clear();
			}
		}

		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Aggregates abilities across a star system for an empire's space objects.
		/// </summary>
		/// <param name="emp"></param>
		/// <param name="name"></param>
		/// <param name="index"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public string GetAbilityValue(Empire emp, string name, int index = 1, Func<Ability, bool> filter = null)
		{
			var abils = FindSpaceObjects<ISpaceObject>(o => o.Owner == emp).Flatten().SelectMany(o => o.UnstackedAbilities).Where(a => a.Name == name && (filter == null || filter(a))).Stack(this);
			if (!abils.Any())
				return null;
			return abils.First().Values[index - 1];
		}

		/// <summary>
		/// Aggregates abilities across a sector for an empire's space objects.
		/// </summary>
		/// <param name="emp"></param>
		/// <param name="name"></param>
		/// <param name="index"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public string GetSectorAbilityValue(Point coords, Empire emp, string name, int index = 1, Func<Ability, bool> filter = null)
		{
			var sobjs = FindSpaceObjects<ISpaceObject>()[coords].Where(o => o.Owner == emp);
			var sector = new Sector(this, coords);
			var abils = sobjs.SelectMany(o => o.UnstackedAbilities).Where(a => a.Name == name && (filter == null || filter(a))).Stack(sector);
			if (!abils.Any())
				return null;
			return abils.First().Values[index - 1];
		}

		/// <summary>
		/// Do any of the empire's space objects in this system have an ability?
		/// </summary>
		/// <param name="emp"></param>
		/// <param name="name"></param>
		/// <param name="index"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public bool HasAbility(Empire emp, string name, int index = 1, Func<Ability, bool> filter = null)
		{
			return FindSpaceObjects<ISpaceObject>(o => o.Owner == emp).Flatten().SelectMany(o => o.UnstackedAbilities).Where(a => a.Name == name && (filter == null || filter(a))).Any();
		}

		/// <summary>
		/// Do any of the empire's space objects in a sector have an ability?
		/// </summary>
		/// <param name="emp"></param>
		/// <param name="name"></param>
		/// <param name="index"></param>
		/// <param name="filter"></param>
		/// <returns></returns>
		public bool DoesSectorHaveAbility(Point coords, Empire emp, string name, int index = 1, Func<Ability, bool> filter = null)
		{
			var sobjs = FindSpaceObjects<ISpaceObject>()[coords].Where(o => o.Owner == emp);
			return sobjs.SelectMany(o => o.UnstackedAbilities).Where(a => a.Name == name && (filter == null || filter(a))).Any();
		}

		public Visibility CheckVisibility(Empire emp)
		{
			if (FindSpaceObjects<ISpaceObject>(sobj => sobj.Owner == emp).Any())
				return Visibility.Visible;
			else if (emp.ExploredStarSystems.Contains(this))
				return Visibility.Fogged;
			return Visibility.Unknown;
		}

		public long ID
		{
			get;
			set;
		}

		public void Dispose()
		{
			Galaxy.Current.UnassignID(this);
			this.UpdateEmpireMemories();
		}

		/// <summary>
		/// Star systems are not owned, per se.
		/// </summary>
		public Empire Owner
		{
			get { return null; }
		}

		public void Place(ISpaceObject sobj, Point coords)
		{
			var sys = sobj.FindStarSystem();
			if (sys != null)
				sys.Remove(sobj);
			SpaceObjectLocations.Add(new ObjectLocation<ISpaceObject>(sobj, coords));
		}

		public void Remove(ISpaceObject sobj)
		{
			foreach (var l in SpaceObjectLocations.ToArray())
			{
				if (l.Item == sobj)
					SpaceObjectLocations.Remove(l);
			}
		}

		public Sector GetSector(int x, int y)
		{
			return GetSector(new Point(x, y));
		}

		public Sector GetSector(Point p)
		{
			if (!AreCoordsInBounds(p))
				throw new Exception("Sector coordinates (" + p.X + ", " + p.Y + ") are out of bounds for star system of radius " + Radius + ".");
			return new Sector(this, p);
		}

		public IEnumerable<Sector> Sectors
		{
			get
			{
				for (var x = -Radius; x <= Radius; x++)
				{
					for (var y = -Radius; y <= Radius; y++)
					{
						yield return new Sector(this, new Point(x, y));
					}
				}
			}
		}

		public Image Icon
		{
			get { return BackgroundImage; }
		}

		public Image Portrait
		{
			get { return BackgroundImage; }
		}

		public bool IsMemory
		{
			get;
			set;
		}

		public double Timestamp { get; set; }

		public bool IsObsoleteMemory(Empire emp)
		{
			return CheckVisibility(emp) >= Visibility.Visible && Timestamp < Galaxy.Current.Timestamp - 1;
		}
	}
}