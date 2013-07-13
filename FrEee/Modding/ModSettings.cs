﻿using FrEee.Game.Enumerations;
using FrEee.Game.Objects.Civilization;
using FrEee.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrEee.Modding
{
	/// <summary>
	/// General settings for a mod.
	/// </summary>
	public class ModSettings
	{
		public ModSettings()
		{
			DefaultColonyConstructionRate = new ResourceQuantity();
			// TODO - moddable default colony construction rate
			DefaultColonyConstructionRate.Add(Resource.Minerals, 2000);
			DefaultColonyConstructionRate.Add(Resource.Organics, 2000);
			DefaultColonyConstructionRate.Add(Resource.Radioactives, 2000);

			PopulationFactor = (long)1e6; // TODO - let population factor be specified by mod files

			PopulationModifiers = new SortedDictionary<int, PopulationModifier>();

			MoodModifiers = new Dictionary<Mood, int>();

			MoodThresholds = new Dictionary<Mood, int>();
			MoodThresholds.Add(Mood.Rioting, 750);
			MoodThresholds.Add(Mood.Unhappy, 450);
			MoodThresholds.Add(Mood.Indifferent, 300);
			MoodThresholds.Add(Mood.Happy, 150);
			MoodThresholds.Add(Mood.Jubilant, 0);

			IntroSongs = new List<string>();
			GameplaySongs = new List<string>();
			CombatSongs = new List<string>();
			VictorySongs = new List<string>();
			DefeatSongs = new List<string>();

			VictoryPictures = new List<string>();
			DefeatPictures = new List<string>();
		}

		/// <summary>
		/// How many people does 1 population represent in the mod files?
		/// </summary>
		public long PopulationFactor { get; set; }

		/// <summary>
		/// Minimum income for an empire, even if it doesn't have any normal resource income.
		/// </summary>
		public ResourceQuantity MinimumEmpireIncome { get; set; }

		/// <summary>
		/// Percentage of facility cost returned when scrapping.
		/// </summary>
		public int ScrapFacilityReturnRate { get; set; }

		/// <summary>
		/// Percentage of unit cost returned when scrapping.
		/// </summary>
		public int ScrapUnitReturnRate { get; set; }

		/// <summary>
		/// Percentage of ship/base cost returned when scrapping.
		/// </summary>
		public int ScrapShipOrBaseReturnRate { get; set; }

		/// <summary>
		/// Percentage of ship/base cost to unmothball.
		/// </summary>
		public int UnmothballPercentCost { get; set; }

		/// <summary>
		/// Trade percentage increase per turn for treaties.
		/// </summary>
		public double TradePercentPerTurn { get; set; }

		/// <summary>
		/// Maxiumum trade percentage for treaties.
		/// </summary>
		public int MaxTradePercent { get; set; }

		/// <summary>
		/// Tribute percent of income paid by subjugated empires.
		/// </summary>
		public int SubjugationTributePercent { get; set; }

		/// <summary>
		/// Tribute percent of income paid by protected empires.
		/// </summary>
		public int ProtectorateTributePercent { get; set; }

		/// <summary>
		/// Percentage cost (of the new facility) to upgrade a facility.
		/// </summary>
		public int UpgradeFacilityPercentCost { get; set; }

		/// <summary>
		/// Maintenance deficit required to destroy one ship per turn.
		/// </summary>
		public int MaintenanceDeficitToDestroyOneShip { get; set; }

		/// <summary>
		/// Standard maintenance cost for ships and bases (%/turn).
		/// </summary>
		public int ShipBaseMaintenanceRate { get; set; }

		/// <summary>
		/// Standard maintenance cost for units (%/turn).
		/// </summary>
		public int UnitMaintenanceRate { get; set; }

		/// <summary>
		/// Standard maintenance cost for facilities (%/turn).
		/// </summary>
		public int FacilityMaintenanceRate { get; set; }

		/// <summary>
		/// Standard reproduction rate for populations (%/year).
		/// </summary>
		public int Reproduction { get; set; }

		/// <summary>
		/// Population required to spawn one militia unit.
		/// </summary>
		public long PopulationPerMilitia { get; set; }

		/// <summary>
		/// Damage inflicted when a militia unit attacks.
		/// </summary>
		public int MilitiaFirepower { get; set; }

		/// <summary>
		/// Hitpoints of a militia unit.
		/// </summary>
		public int MilitiaHitpoints { get; set; }

		/// <summary>
		/// Population of the colony ship crew's race to spawn on colonization.
		/// </summary>
		public long AutomaticColonizationPopulation { get; set; }

		/// <summary>
		/// Loss of resource value of glassed planets.
		/// </summary>
		public int GlassedPlanetValueLoss { get; set; }

		/// <summary>
		/// Loss of conditions of glassed planets.
		/// </summary>
		public int GlassedPlanetConditionsLoss { get; set; }

		/// <summary>
		/// Cost to replace a component when retrofitting, as a percentage of the new component's cost.
		/// </summary>
		public int RetrofitReplacementCostPerecnt { get; set; }

		/// <summary>
		/// Cost to remove a component when retrofitting, as a percentage of the component's cost.
		/// </summary>
		public int RetrofitRemovalCostPercent { get; set; }

		/// <summary>
		/// The construction rate for colonies lacking a spaceyard.
		/// </summary>
		public ResourceQuantity DefaultColonyConstructionRate { get; set; }

		/// <summary>
		/// Maximum number of consecutive turns a queue can be on emergency build.
		/// </summary>
		public int MaxEmergencyBuildTurns { get; set; }

		/// <summary>
		/// Percentage of normal rate for emergency build.
		/// </summary>
		public int EmergencyBuildRate { get; set; }

		/// <summary>
		/// Percentage of normal rate for slow build.
		/// </summary>
		public int SlowBuildRate { get; set; }

		/// <summary>
		/// Damage required to kill 1 person.
		/// </summary>
		public double PopulationHitpoints { get; set; }

		/// <summary>
		/// Modifiers to production and construction rates from population amounts.
		/// </summary>
		public SortedDictionary<int, PopulationModifier> PopulationModifiers { get; private set; }

		public double GetPopulationProductionFactor(long population)
		{
			double result = 1d;
			foreach (var pm in PopulationModifiers.OrderBy(pm => pm.Key))
			{
				if (pm.Key < population)
					result = pm.Value.ProductionRate / 100d;
				else
					break;
			}
			return result;
		}

		public double GetPopulationConstructionFactor(long population)
		{
			double result = 1d;
			foreach (var pm in PopulationModifiers.OrderBy(pm => pm.Key))
			{
				if (pm.Key < population)
					result = pm.Value.ConstructionRate / 100d;
				else
					break;
			}
			return result;
		}

		/// <summary>
		/// Racial aptitudes.
		/// </summary>
		public IEnumerable<Aptitude> Aptitudes { get { return Aptitude.All; } }

		/// <summary>
		/// Modifiers to production and construction rates from population mood.
		/// </summary>
		public IDictionary<Mood, int> MoodModifiers { get; private set; }

		/// <summary>
		/// Minimum anger thresholds for each mood, in tenths of a percent.
		/// </summary>
		public IDictionary<Mood, int> MoodThresholds { get; private set; }

		public double GetMoodFactor(int anger)
		{
			Mood mood = Mood.Emotionless;
			foreach (var mt in MoodThresholds.OrderBy(mt => mt.Value))
			{
				if (mt.Value < anger)
					mood = mt.Key;
				else
					break;
			}
			if (MoodModifiers.ContainsKey(mood))
				return MoodModifiers[mood] / 100d;
			return 1d;
		}

		/// <summary>
		/// Below this much supply a ship gets a low supply warning.
		/// </summary>
		public int LowSupplyWarningAmount { get; set; }

		/// <summary>
		/// Below this % supply a ship gets a low supply warning.
		/// </summary>
		public int LowSupplyWarningPercent { get; set; }

		/// <summary>
		/// Music tracks for the title screen.
		/// </summary>
		public IList<string> IntroSongs { get; private set; }

		/// <summary>
		/// Music tracks for gameplay.
		/// </summary>
		public IList<string> GameplaySongs { get; private set; }

		/// <summary>
		/// Music tracks for combat.
		/// </summary>
		public IList<string> CombatSongs { get; private set; }

		/// <summary>
		/// Music tracks for victory.
		/// </summary>
		public IList<string> VictorySongs { get; private set; }

		/// <summary>
		/// Music tracks for defeat.
		/// </summary>
		public IList<string> DefeatSongs { get; private set; }

		/// <summary>
		/// Weapon accuracy at 0 squares distance.
		/// </summary>
		public int WeaponAccuracyPointBlank { get; set; }

		/// <summary>
		/// Weapon accuracy loss per square of distance to the target.
		/// </summary>
		public int WeaponAccuracyLossPerSquare { get; set; }

		/// <summary>
		/// Pictures for victory.
		/// </summary>
		public IList<string> VictoryPictures { get; set; }

		/// <summary>
		/// Pictures for defeat.
		/// </summary>
		public IList<string> DefeatPictures { get; set; }

		/// <summary>
		/// Percent effectiveness of intelligence defense.
		/// </summary>
		public int IntelligenceDefensePercent { get; set; }

		/// <summary>
		/// Percent effectiveness of ground combat weapons.
		/// </summary>
		public int GroundCombatDamagePercent { get; set; }

		/// <summary>
		/// Number of turns in space combat.
		/// </summary>
		public int SpaceCombatTurns { get; set; }

		/// <summary>
		/// Number of turns in ground combat.
		/// </summary>
		public int GroundCombatTurns { get; set; }

		/// <summary>
		/// How much of the ramming ship's HP gets applied as damage to the target in ramming?
		/// </summary>
		public int RammingSourceHitpointsDamagePercent { get; set; }

		/// <summary>
		/// How much of the target's HP gets applied as damage to the ramming ship in ramming?
		/// </summary>
		public int RammingTargetHitpointsDamagePercent { get; set; }

		/// <summary>
		/// Does the AI use Mega Evil Empire to gang up on the leading player?
		/// </summary>
		public bool MegaEvilEmpireEnabled { get; set; }

		/// <summary>
		/// Score threshold to trigger Mega Evil Empire.
		/// </summary>
		public int MegaEvilEmpireScoreThreshold { get; set; }

		/// <summary>
		/// Score percent (of second place player) to trigger Mega Evil Empire on a human player.
		/// </summary>
		public int MegaEvilEmpireHumanScorePercent { get; set; }

		/// <summary>
		/// Score percent (of second place player) to trigger Mega Evil Empire on an AI player.
		/// </summary>
		public int MegaEvilEmpireAIScorePercent { get; set; }

		/// <summary>
		/// Home systems can still produce some resources even without a spaceport.
		/// </summary>
		public int HomeSystemValueWithoutSpaceport { get; set; }

		/// <summary>
		/// Additional reload time for weapons on a captured ship.
		/// </summary>
		public int CapturedShipReloadDelay { get; set; }

		/// <summary>
		/// Maximum sight obscuration level for artificial storms.
		/// </summary>
		public int CreatedStormMaxCloakLevel { get; set; }

		/// <summary>
		/// Maximum damage for artificial storms.
		/// </summary>
		public int CreatedStormMaxDamage { get; set; }

		/// <summary>
		/// Maximum shield disruption for artificial storms.
		/// </summary>
		public int CreatedStormMaxShieldDisruption { get; set; }

		/// <summary>
		/// Maximum mines per player per sector.
		/// </summary>
		public int MaxPlayerMinesPerSector { get; set; }

		/// <summary>
		/// Maximum satellites per player per sector.
		/// </summary>
		public int MaxPlayerSatellitesPerSector { get; set; }

		/// <summary>
		/// Maximum population for which an "abandon colony" order can be given.
		/// </summary>
		public int MaxPopulationToAbandonColony { get; set; }

		/// <summary>
		/// Automatic supply drain each turn for drones.
		/// </summary>
		public int DroneSupplyDrain { get; set; }

		/// <summary>
		/// Automatic supply drain each turn for fighters.
		/// </summary>
		public int FighterSupplyDrain { get; set; }

		/// <summary>
		/// Can drones be affected by mines?
		/// </summary>
		public bool MinesAffectDrones { get; set; }

		/// <summary>
		/// Can fighters be affected by mines?
		/// </summary>
		public bool MinesAffectFighters { get; set; }

		/// <summary>
		/// Cargo space used to store 1 person.
		/// </summary>
		public double PopulationSize { get; set; }

		/// <summary>
		/// Number of turns between population reproduction.
		/// </summary>
		public int ReproductionDelay { get; set; }

		/// <summary>
		/// Can bases join fleets?
		/// </summary>
		public bool BasesCanJoinFleets { get; set; }

		/// <summary>
		/// Can a spaceyard component be added via retrofit?
		/// </summary>
		public bool CanAddSpaceyardViaRetrofit { get; set; }

		/// <summary>
		/// Can a colonizer component be added via retrofit?
		/// </summary>
		public bool CanAddColonizerViaRetrofit { get; set; }

		/// <summary>
		/// Evasion rating of seekers.
		/// </summary>
		public int SeekerEvasion { get; set; }

		/// <summary>
		/// Accuracy rating of planets.
		/// </summary>
		public int PlanetAccuracy { get; set; }

		/// <summary>
		/// Evasion rating of planets.
		/// </summary>
		public int PlanetEvasion { get; set; }

		/// <summary>
		/// Global reproduction rate multiplier to convert mod values to per-turn values.
		/// Defaults to 0.1 since 20%/year reproduction in SE4 really meant 2% per turn.
		/// </summary>
		public double ReproductionMultiplier { get; set; }
	}
}
