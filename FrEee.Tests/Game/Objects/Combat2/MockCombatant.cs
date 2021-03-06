﻿using FrEee.Game.Enumerations;
using FrEee.Game.Interfaces;
using FrEee.Game.Objects.Abilities;
using FrEee.Game.Objects.Civilization;
using FrEee.Game.Objects.Combat;
using FrEee.Game.Objects.Space;
using FrEee.Game.Objects.Technology;
using FrEee.Game.Objects.Vehicles;
using FrEee.Modding;
using FrEee.Modding.Templates;
using FrEee.Tests.Modding;
using FrEee.Utility;
using FrEee.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace FrEee.Tests.Game.Objects.Combat2
{
	/// <summary>
	/// A mock combatant, used for testing.
	/// </summary>
	public class MockCombatant : ICombatant, ISpaceObject
	{
		public MockCombatant(Empire owner)
		{
			Owner = owner;
			Weapons = new List<Component>();
		}

		public ICollection<Component> Weapons
		{
			get;
			private set;
		}

		public bool CanTarget(ITargetable target)
		{
			return true;
		}

		public WeaponTargets WeaponTargetType
		{
			get { return WeaponTargets.Ship; }
		}

		IEnumerable<Component> ICombatant.Weapons
		{
			get { return Weapons; }
		}

		public bool IsHostileTo(Empire emp)
		{
			// don't bother with fancy politics
			return Owner != emp;
		}

		public int Accuracy
		{
			get { return 0; }
		}

		public int Evasion
		{
			get { return 0; }
		}

		public Image Icon
		{
			get { return Pictures.GetGenericImage(typeof(Ship)); }
		}

		public Image Portrait
		{
			get { return Pictures.GetGenericImage(typeof(Ship)); }
		}

		public Empire Owner
		{
			get;
			set;
		}

		public int Hitpoints
		{
			get { return HullHitpoints + ArmorHitpoints; }
			set
			{
				if (value > MaxHullHitpoints)
				{
					HullHitpoints = MaxHullHitpoints;
					ArmorHitpoints = Math.Min(MaxArmorHitpoints, value - MaxHullHitpoints);
				}
				else
				{
					HullHitpoints = value;
					ArmorHitpoints = 0;
				}
			}
		}

		public int NormalShields
		{
			get;
			set;
		}

		public int PhasedShields
		{
			get;
			set;
		}

		public int MaxHitpoints
		{
			get;
			set;
		}

		public int MaxNormalShields
		{
			get;
			set;
		}

		public int MaxPhasedShields
		{
			get;
			set;
		}

		public int ShieldHitpoints
		{
			get { return NormalShields + PhasedShields; }
		}

		public int ArmorHitpoints
		{
			get;
			set;
		}

		public int HullHitpoints
		{
			get;
			set;
		}

		public int MaxShieldHitpoints
		{
			get { return MaxNormalShields + MaxPhasedShields; }
		}

		public int MaxArmorHitpoints
		{
			get;
			set;
		}

		public int MaxHullHitpoints
		{
			get;
			set;
		}

		public void ReplenishShields(int? amount = null)
		{
			if (amount == null)
			{
				NormalShields = MaxNormalShields;
				PhasedShields = MaxPhasedShields;
			}
			else
			{
				PhasedShields += amount.Value;
				if (PhasedShields > MaxPhasedShields)
				{
					var overflow = PhasedShields - MaxPhasedShields;
					PhasedShields = MaxPhasedShields;
					NormalShields += overflow;
					if (NormalShields > MaxNormalShields)
						NormalShields = MaxNormalShields;
				}
			}
		}

		public int? Repair(int? amount = null)
		{
			// mock combat objects don't really have components, so just repair them fully
			HullHitpoints = MaxHullHitpoints;
			ArmorHitpoints = MaxArmorHitpoints;
			return amount;
		}

		public int TakeDamage(Hit hit, PRNG dice = null)
		{
			// TODO - special damage types
			var dmg = hit.NominalDamage;
			if (NormalShields >= dmg)
			{
				NormalShields -= dmg;
				return 0;
			}
			else
			{
				dmg -= NormalShields;
				NormalShields = 0;
			}
			if (PhasedShields >= dmg)
			{
				PhasedShields -= dmg;
				return 0;
			}
			else
			{
				dmg -= PhasedShields;
				PhasedShields = 0;
			}
			if (ArmorHitpoints >= dmg)
			{
				ArmorHitpoints -= dmg;
				return 0;
			}
			else
			{
				dmg -= ArmorHitpoints;
				ArmorHitpoints = 0;
			}
			if (HullHitpoints >= dmg)
			{
				HullHitpoints -= dmg;
				return 0;
			}
			else
			{
				dmg -= HullHitpoints;
				HullHitpoints = 0;
			}
			// oh noes! no more HP!
			IsDestroyed = true;
			return dmg;
		}

		public bool IsDestroyed
		{
			get;
			private set;
		}

		public int HitChance
		{
			get { return 1; }
		}

		public void Dispose()
		{
			IsDisposed = true;
		}

		public Visibility CheckVisibility(Empire emp)
		{
			// duh, it's combat
			return Visibility.Scanned;
		}

		public void Redact(Empire emp)
		{
			// nothing to do
		}

		public bool IsMemory
		{
			get;
			set;
		}

		public double Timestamp
		{
			get;
			set;
		}

		public bool IsObsoleteMemory(Empire emp)
		{
			return false;
		}

		public long ID
		{
			get;
			set;
		}

		public bool IsDisposed
		{
			get;
			set;
		}

		public Sector Sector
		{
			get { return null; }
			set { }
		}

		public StarSystem StarSystem
		{
			get { return null; }
		}

		public string Name { get; set; }


		public ConstructionQueue ConstructionQueue
		{
			get { return null; }
		}

		public bool CanBeInFleet
		{
			get { return true; }
		}

		public int SupplyStorage
		{
			get { return 0; }
		}

		public bool HasInfiniteSupplies
		{
			get { return true; }
		}

		public bool CanWarp
		{
			get { return false; }
		}

		public bool IsIdle
		{
			get { return false; }
		}

		public IEnumerable<Ability> IntrinsicAbilities
		{
			get { yield break; }
		}

		public IEnumerable<IAbilityObject> Children
		{
			get { yield break; }
		}

		public IEnumerable<IAbilityObject> Parents
		{
			get
			{
				if (Owner != null)
					yield return Owner;
			}
		}

		public AbilityTargets AbilityTarget
		{
			get { return AbilityTargets.None; }
		}

		/// <summary>
		/// Resources stored on this space vehicle.
		/// </summary>
		public ResourceQuantity StoredResources { get; private set; }

		public IEnumerable<string> IconPaths
		{
			get
			{
				yield break;
			}
		}

		public IEnumerable<string> PortraitPaths
		{
			get
			{
				yield break;
			}
		}
	}
}
