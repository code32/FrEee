using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FrEee.Game.Interfaces;
using FrEee.Game.Objects.Civilization;
using FrEee.Utility.Extensions;
using System.Threading.Tasks;
using FrEee.Modding;
using System.Drawing;
using FrEee.Utility;
using FrEee.Game.Objects.LogMessages;
using FrEee.Game.Objects.Vehicles;
using System.Reflection;
using FrEee.Game.Objects.Combat;
using FrEee.Game.Objects.Orders;
using FrEee.Game.Setup;
using FrEee.Game.Enumerations;
using FrEee.Game.Objects.VictoryConditions;
using FrEee.Game.Objects.Abilities;
using FrEee.Game.Objects.Civilization.Diplomacy.Clauses;
using System.Text;
using FrEee.Game.Setup.WarpPointPlacementStrategies;
using FrEee.Game.Objects.Combat.Simple;

namespace FrEee.Game.Objects.Space
{
	/// <summary>
	/// A galaxy in which the game is played.
	/// </summary>
	[Serializable]
	public class Galaxy : ICommonAbilityObject
	{
		public Galaxy()
		{
			Galaxy.Current = this;
			StarSystemLocations = new List<ObjectLocation<StarSystem>>();
			Empires = new List<Empire>();
			Name = "Unnamed";
			TurnNumber = 1;
			referrables = new Dictionary<long, IReferrable>();
			VictoryConditions = new List<IVictoryCondition>();
			AbilityCache = new SafeDictionary<IAbilityObject, IEnumerable<Ability>>();
			CommonAbilityCache = new SafeDictionary<Tuple<ICommonAbilityObject, Empire>, IEnumerable<Ability>>();
			SharedAbilityCache = new SafeDictionary<Tuple<IOwnableAbilityObject, Empire>, IEnumerable<Ability>>();
			GivenTreatyClauseCache = new SafeDictionary<Empire, ILookup<Empire, Clause>>();
			ReceivedTreatyClauseCache = new SafeDictionary<Empire, ILookup<Empire, Clause>>();
			Battles = new HashSet<Battle>();
			ScriptNotes = new DynamicDictionary();
		}

		public Galaxy(Mod mod)
			: this()
		{
			Mod = mod;
		}

		#region Properties

		/// <summary>
		/// The current galaxy. Shouldn't change except at loading a game, turn proecssing, or mod patching.
		/// </summary>
		public static Galaxy Current { get; set; }

		/// <summary>
		/// Should players have an omniscient view of all explored systems?
		/// Does not prevent cloaking from working; this is just basic sight.
		/// Also does not give battle reports for other empires' battles.
		/// </summary>
		public bool OmniscientView { get; set; }

		/// <summary>
		/// Should players have sensor data for all systems from the get-go?
		/// </summary>
		public bool AllSystemsExploredFromStart { get; set; }

		/// <summary>
		/// Model to use for standard planetary mining.
		/// </summary>
		public MiningModel StandardMiningModel { get; set; }

		/// <summary>
		/// Model to use for remote mining.
		/// </summary>
		public MiningModel RemoteMiningModel { get; set; }

		public int MinPlanetValue { get; set; }

		public int MinSpawnedPlanetValue { get; set; }

		public int MaxSpawnedPlanetValue { get; set; }

		public int MaxPlanetValue { get; set; }

		public int MinAsteroidValue { get; set; }

		public int MinSpawnedAsteroidValue { get; set; }

		public int MaxSpawnedAsteroidValue { get; set; }

		/// <summary>
		/// Who can view empire scores?
		/// </summary>
		public ScoreDisplay ScoreDisplay { get; set; }

		/// <summary>
		/// Is this a single player game? If so, autoprocess the turn after the player takes his turn.
		/// </summary>
		public bool IsSinglePlayer { get; set; }

		/// <summary>
		/// The mod being played.
		/// </summary>
		[SerializationPriority(1)]
		public Mod Mod { get; set; }

		/// <summary>
		/// The game name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Technology research cost formula.
		/// Low = Level * BaseCost
		/// Medium = BaseCost for level 1, Level ^ 2 * BaseCost / 2 otherwise
		/// Hight = Level ^ 2 * BaseCost
		/// </summary>
		public TechnologyCost TechnologyCost { get; set; }

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

		public string GameFileName
		{
			get
			{
				if (PlayerNumber > 0)
					return Name + "_" + TurnNumber + "_" + PlayerNumber + FrEeeConstants.SaveGameExtension;
				else
					return Name + "_" + TurnNumber + FrEeeConstants.SaveGameExtension;
			}
		}

		public string CommandFileName
		{
			get
			{
				if (PlayerNumber > 0)
					return Name + "_" + TurnNumber + "_" + PlayerNumber + FrEeeConstants.PlayerCommandsSaveGameExtension;
				else
					throw new InvalidOperationException("The game host does not have a command file.");
			}
		}

		public int MinX
		{
			get { return StarSystemLocations.MinOrDefault(ssl => ssl.Location.X); }
		}

		public int MinY
		{
			get { return StarSystemLocations.MinOrDefault(ssl => ssl.Location.Y); }
		}

		public int MaxX
		{
			get { return StarSystemLocations.MaxOrDefault(ssl => ssl.Location.X); }
		}

		public int MaxY
		{
			get { return StarSystemLocations.MaxOrDefault(ssl => ssl.Location.Y); }
		}

		public int Width
		{
			get;
			set;
		}

		public int Height
		{
			get;
			set;
		}

		/// <summary>
		/// Horizontal space occuped by star systems.
		/// </summary>
		public int UsedWidth
		{
			get
			{
				if (!StarSystemLocations.Any())
					return 0;
				return StarSystemLocations.Max(ssl => ssl.Location.X) - StarSystemLocations.Min(ssl => ssl.Location.X) + 1;
			}
		}

		/// <summary>
		/// Vertical space occupied by star systems.
		/// </summary>
		public int UsedHeight
		{
			get
			{
				if (!StarSystemLocations.Any())
					return 0;
				return StarSystemLocations.Max(ssl => ssl.Location.Y) - StarSystemLocations.Min(ssl => ssl.Location.Y) + 1;
			}
		}

		/// <summary>
		/// The current turn number.
		/// </summary>
		public int TurnNumber { get; set; }

		/// <summary>
		/// The current player number (1 is the first player, 0 is the game host).
		/// </summary>
		public int PlayerNumber
		{
			get { return Empires.IndexOf(CurrentEmpire) + 1; }
		}

		/// <summary>
		/// The current stardate. Advances 0.1 years per turn.
		/// </summary>
		public string Stardate
		{
			get
			{
				return TurnNumber.ToStardate();
			}
		}

		/// <summary>
		/// Number of turns of uninterrupted galactic peace (Non-Aggression or better between all surviving empires).
		/// </summary>
		public int TurnsOfPeace
		{
			get
			{
				// TODO - treaties
				return 0;
			}
		}

		/// <summary>
		/// Game victory conditions.
		/// </summary>
		public IList<IVictoryCondition> VictoryConditions { get; private set; }

		/// <summary>
		/// Delay in turns before victory conditions take effect.
		/// </summary>
		public int VictoryDelay { get; set; }

		/// <summary>
		/// Is this a "humans vs. AI" game?
		/// </summary>
		public bool IsHumansVsAI { get; set; }

		/// <summary>
		/// Allowed trades in this game.
		/// </summary>
		public AllowedTrades AllowedTrades { get; set; }

		public bool IsSurrenderAllowed { get; set; }

		public bool IsIntelligenceAllowed { get; set; }

		public bool IsAnalysisAllowed { get; set; }

		public bool CanColonizeOnlyBreathable { get; set; }

		public bool CanColonizeOnlyHomeworldSurface { get; set; }

		public AbilityTargets AbilityTarget
		{
			get { return AbilityTargets.Galaxy; }
		}

		/// <summary>
		/// The battles which have taken place this turn.
		/// </summary>
		public ICollection<Battle> Battles { get; private set; }

		#endregion

		#region Data Access

		/// <summary>
		/// Serializes the player's commands.
		/// </summary>
		/// <exception cref="InvalidOperationException">if no current empire</exception>
		/// <returns></returns>
		private void SerializeCommands(Stream stream)
		{
			if (CurrentEmpire == null)
				throw new InvalidOperationException("Can't serialize commands if there is no current empire.");

			Serializer.Serialize(CurrentEmpire.Commands, stream);
		}

		/// <summary>
		/// Deserializes the player's commands.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		private static IList<ICommand> DeserializeCommands(Stream stream)
		{
			var cmds = Serializer.Deserialize<IList<ICommand>>(stream);

			// check for client safety
			foreach (var cmd in cmds.Where(cmd => cmd != null))
			{
				cmd.CheckForClientSafety();
			}

			return cmds;
		}

		/// <summary>
		/// Assigns IDs to referrable objects in the galaxy.
		/// Doesn't assign IDs to objects via DoNotAssignID properties, or to memories (or sub-objects of them).
		/// </summary>
		public void AssignIDs()
		{
			var parser = new ObjectGraphParser();
			bool canAssign = true;
			parser.Property += (pname, o, val) =>
			{
				var prop = o.GetType().FindProperty(pname);
				var isMemory = val is IFoggable && (val as IFoggable).IsMemory;
				canAssign = !prop.HasAttribute<DoNotAssignIDAttribute>() && !isMemory;
				if (isMemory)
					return false; // no recursion!
				if (prop.GetAttributes<DoNotAssignIDAttribute>().Any(a => a.Recurse))
					return false; // no recursion!
				else
					return true;
			};
			parser.StartObject += o =>
			{
				if (o is IReferrable && canAssign)
				{
					var r = (IReferrable)o;
					AssignID(r);
				}
			};
			parser.Parse(this);
		}

		public void Save(Stream stream, bool assignIDs = true)
		{
			if (assignIDs)
				AssignIDs();
			Serializer.Serialize(this, stream);
		}

		public string SaveToString(bool assignIDs = true)
		{
			if (assignIDs)
				AssignIDs();
			return Serializer.SerializeToString(this);
		}

		/// <summary>
		/// Saves the game to an appropriately named file in the Savegame folder.
		/// Files are named GameName_TurnNumber_PlayerNumber.gam for players (PlayerNumber is 1-indexed)
		/// and GameName_TurnNumber.gam for the host.
		/// </summary>
		/// <returns>The filename saved to without the folder name (which is Savegame).</returns>
		public string Save(bool assignIDs = true)
		{
			if (assignIDs)
				AssignIDs();
			string filename;
			if (CurrentEmpire == null)
				filename = Name + "_" + TurnNumber + ".gam";
			else
				filename = Name + "_" + TurnNumber + "_" + (Empires.IndexOf(CurrentEmpire) + 1).ToString("d4") + ".gam";
			if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), FrEeeConstants.SaveGameDirectory)))
				Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), FrEeeConstants.SaveGameDirectory));
			var fs = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), FrEeeConstants.SaveGameDirectory, filename), FileMode.Create);
			Serializer.Serialize(this, fs);
			fs.Close(); fs.Dispose();
			return filename;
		}

		/// <summary>
		/// Saves the master view and all players' views of the galaxy, unless single player, in which case only the first player's view is saved.
		/// </summary>
		/// <exception cref="InvalidOperationException">if CurrentEmpire is not null.</exception>
		public static void SaveAll(Status status = null, double desiredProgress = 1d)
		{
			if (Current.CurrentEmpire != null)
				throw new InvalidOperationException("Can only save player galaxy views from the master galaxy view.");

			var progressPerSaveLoad = (desiredProgress - (status == null ? 0d : status.Progress)) / (Current.IsSinglePlayer ? 3 : (Current.Empires.Count + 2));
			//Current.SpaceObjectIDCheck("before saving");

			// save master view
			if (status != null)
				status.Message = "Saving game (host)";
			var gamname = Galaxy.Current.Save();
			if (status != null)
				status.Progress += progressPerSaveLoad;
			Current.SpaceObjectIDCheck("after saving master view to disk");

			// save player views
			for (int i = 0; i < Current.Empires.Count; i++)
			{
				Load(gamname);
				if (Current.Empires[i].IsPlayerEmpire)
				{
					if (status != null)
						status.Message = "Saving game (player " + (i + 1) + ")";
					Current.CurrentEmpire = Current.Empires[i];
					Current.Redact();
					Current.SpaceObjectIDCheck("after creating player view for " + Current.Empires[i]);
					Current.Save(false); // already asssigned IDs in the redact phase
					if (status != null)
						status.Progress += progressPerSaveLoad;
				}
			}

			// TODO - only reload master view if we really need to
			if (status != null)
				status.Message = "Saving game";
			Load(gamname);
			if (status != null)
				status.Progress += progressPerSaveLoad;

			Current.SpaceObjectIDCheck("after reloading master view");
		}

		/// <summary>
		/// Loads a savegame from a stream.
		/// </summary>
		/// <param name="stream"></param>
		public static void Load(Stream stream)
		{
			Galaxy.Current = Serializer.Deserialize<Galaxy>(stream);
			Mod.Current = Galaxy.Current.Mod;
			Current.SpaceObjectIDCheck("after loading from disk/memory");

			if (Empire.Current != null)
			{
				// initialize IronPython galaxy on load
				Current.StringValue = Current.SaveToString(false);
				var formula = new ComputedFormula<int>("Galaxy.Current.TurnNumber", null, true);
				var turn = formula.Value;
			}
		}

		/// <summary>
		/// Loads a savegame from the Savegame folder.
		/// Note that if it was renamed, it might have different game name, turn number, player number, etc. than the filename indicates.
		/// </summary>
		/// <param name="filename"></param>
		public static void Load(string filename)
		{
			var fs = new FileStream(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), FrEeeConstants.SaveGameDirectory, filename), FileMode.Open);
			var sr = new StreamReader(fs);
			var s = sr.ReadToEnd();
			LoadFromString(s);
			fs.Close(); fs.Dispose();
		}

		/// <summary>
		/// Loads a host savegame from the Savegame folder.
		/// </summary>
		/// <param name="gameName"></param>
		/// <param name="turnNumber"></param>
		public static void Load(string gameName, int turnNumber)
		{
			Load(gameName + "_" + turnNumber + FrEeeConstants.SaveGameExtension);
		}

		/// <summary>
		/// Loads a player savegame from the Savegame folder.
		/// </summary>
		/// <param name="gameName"></param>
		/// <param name="turnNumber"></param>
		/// <param name="playerNumber"></param>
		public static void Load(string gameName, int turnNumber, int playerNumber)
		{
			Load(gameName + "_" + turnNumber + "_" + playerNumber.ToString("d4") + FrEeeConstants.SaveGameExtension);
		}

		/// <summary>
		/// Loads from a string in memory.
		/// </summary>
		/// <param name="serializedData"></param>
		public static void LoadFromString(string serializedData)
		{
			Galaxy.Current = Serializer.DeserializeFromString<Galaxy>(serializedData);
			Mod.Current = Galaxy.Current.Mod;
			Current.SpaceObjectIDCheck("after loading from memory");

			if (Empire.Current != null)
			{
				// initialize IronPython galaxy on load
				Current.StringValue = serializedData;
				var formula = new ComputedFormula<int>("Galaxy.Current.TurnNumber", null, true);
				var turn = formula.Value;

				// load library of designs, strategies, etc.
				Library.Load();
			}
		}

		/// <summary>
		/// Saves the player's commands to an appropriately named file in the Savegame folder.
		/// Files are named GameName_TurnNumber_PlayerNumber.plr. (PlayerNumber is 1-indexed and padded to 4 digits with zeroes)
		/// This doesn't make sense for the host view, so an exception will be thrown if there is no current empire.
		/// </summary>
		/// <returns>The filename saved to without the folder name (which is Savegame).</returns>
		/// <exception cref="InvalidOperationException">if there is no current empire.</exception>
		public string SaveCommands()
		{
			AssignIDs();
			if (CurrentEmpire == null)
				throw new InvalidOperationException("Can't save commands without a current empire.");
			if (!Directory.Exists(FrEeeConstants.SaveGameDirectory))
				Directory.CreateDirectory(FrEeeConstants.SaveGameDirectory);
			var filename = GetEmpireCommandsSavePath(CurrentEmpire);
			var fs = new FileStream(filename, FileMode.Create);
			SerializeCommands(fs);
			fs.Close(); fs.Dispose();

			// save library of designs, commands, etc.
			Library.Save();

			return filename;
		}

		/// <summary>
		/// Loads player commands into the current game state.
		/// If this is the host view, commands will be loaded for all players.
		/// If this is the player view, commands will be immediately executed so as to provide the player with a fresh game state.
		/// </summary>
		/// <returns>Player empires which did not submit commands and are not defeated.</returns>
		public IEnumerable<Empire> LoadCommands()
		{
			// whose commands are we loading?
			var emps = new List<Empire>();
			if (CurrentEmpire == null)
				emps.AddRange(Empires);
			else
				emps.Add(CurrentEmpire);

			var noCmds = new List<Empire>();

			foreach (var emp in emps)
			{
				var plrfile = GetEmpireCommandsSavePath(emp);
				if (File.Exists(plrfile))
				{
					var fs = new FileStream(plrfile, FileMode.Open);
					var cmds = DeserializeCommands(fs);
					LoadCommands(emp, cmds);
					fs.Close(); fs.Dispose();
				}
				else if (emp.IsPlayerEmpire)
					noCmds.Add(emp);
			}

			if (CurrentEmpire != null)
			{
				foreach (var cmd in CurrentEmpire.Commands)
				{
					if (cmd.Executor == null)
						cmd.Issuer.Log.Add(cmd.Issuer.CreateLogMessage($"{cmd} cannot be issued because its executor does not exist. Probably a bug..."));
					else if (cmd.Issuer != cmd.Executor.Owner && cmd.Issuer != cmd.Executor)
						cmd.Issuer.Log.Add(cmd.Issuer.CreateLogMessage("We cannot issue commands to " + cmd.Executor + " because it does not belong to us!"));
					else
						cmd.Execute();
				}
			}

			return noCmds;
		}

		private void LoadCommands(Empire emp, IList<ICommand> cmds)
		{
			cmds = cmds.Where(cmd => cmd != null).Distinct().ToList(); // HACK - why would we have null/duplicate commands in a plr file?!
			emp.Commands.Clear();
			var idmap = new Dictionary<long, long>();
			foreach (var cmd in cmds)
			{
				if (cmd.NewReferrables.Any(r => r.IsDisposed))
				{
					emp.Log.Add(new GenericLogMessage("Command \"" + cmd + "\" contained a reference to deleted object \"" + cmd.NewReferrables.First(r => r.IsDisposed) + "\" and will be ignored. This may be a game bug."));
					continue;
				}
				emp.Commands.Add(cmd);
				foreach (var r in cmd.NewReferrables)
				{
					var clientid = r.ID;
					var serverid = AssignID(r);
					if (idmap.ContainsKey(clientid))
					{
						if (idmap[clientid] != serverid)
							throw new InvalidOperationException($"Adding {r} with ID {serverid} to client ID {clientid} for {emp} when that client ID is already mapped to server ID {idmap[clientid]}.");
						// else do nothing
					}
					else
						idmap.Add(clientid, serverid);
				}

			}
			foreach (var cmd in cmds)
				cmd.ReplaceClientIDs(idmap); // convert client IDs to server IDs
		}

		public string GetEmpireCommandsSavePath(Empire emp)
		{
			return GetEmpireCommandsSavePath(Name, TurnNumber, Empires.IndexOf(emp) + 1);
		}

		public string GetGameSavePath(Empire emp = null)
		{
			if (emp == null)
				emp = CurrentEmpire;
			return GetGameSavePath(Name, TurnNumber, emp == null ? 0 : (Empires.IndexOf(emp) + 1));
		}

		public static string GetEmpireCommandsSavePath(string gameName, int turnNumber, int empireNumber)
		{
			return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Savegame", String.Format("{0}_{1}_{2:d4}{3}", gameName, turnNumber, empireNumber, FrEeeConstants.PlayerCommandsSaveGameExtension));
		}

		public static string GetGameSavePath(string gameName, int turnNumber, int empireNumber)
		{
			return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Savegame", empireNumber < 1 ?
				String.Format("{0}_{1}{2}", gameName, turnNumber, FrEeeConstants.SaveGameExtension) :
				String.Format("{0}_{1}_{2:d4}{3}", gameName, turnNumber, empireNumber, FrEeeConstants.SaveGameExtension));
		}

		/// <summary>
		/// Current time equals turn number plus tick minus 1.
		/// </summary>
		public double Timestamp { get { return TurnNumber + CurrentTick - 1; } }

		#endregion

		#region Public Methods

		/// <summary>
		/// Removes any space objects, etc. that the current empire cannot see.
		/// </summary>
		public void Redact()
		{
			// save off empire scores first, before data is removed
			foreach (var emp in Empires)
			{
				emp.Scores[TurnNumber] = emp.ComputeScore(Empire.Current);
			}

			// the galaxy data itself
			if (Empire.Current != null)
				ScriptNotes.Clear();

			// redact sub objects
			var parser = new ObjectGraphParser();
			parser.StartObject += redactParser_StartObject;
			parser.Parse(this);

			// clean up redacted objects that are not IFoggable
			foreach (var x in StarSystemLocations.Where(x => x.Item.IsDisposed).ToArray())
				StarSystemLocations.Remove(x);

			// delete memories since they've been copied to "physical" objects already
			foreach (var kvp in Empire.Current.Memory.ToArray())
			{
				kvp.Value.Dispose();
				Empire.Current.Memory.Remove(kvp);
			}
		}

		void redactParser_StartObject(object o)
		{
			if (o is IReferrable)
				AssignID(o as IReferrable);
			if (o is IFoggable)
			{
				var obj = (IFoggable)o;
				if (!obj.IsMemory)
				{
					var id = obj.ID;
					var vis = obj.CheckVisibility(CurrentEmpire);
					if (vis < Visibility.Fogged)
						obj.Dispose();
					if (vis == Visibility.Fogged && CurrentEmpire.Memory.ContainsKey(id))
					{
						CurrentEmpire.Memory[id].CopyToExceptID(obj, IDCopyBehavior.PreserveDestination); // memory sight!
						obj.IsMemory = true;
					}
					obj.Redact(Empire.Current);
				}
				else
				{
					// memories are only visible to the empire which is seeing them!
					// well unless we add some sort of intel project to see them or something...
					if (!CurrentEmpire.Memory.Values.Contains(obj))
						obj.Dispose();
				}
			}
			//SpaceObjectIDCheck("when redacting " + o);
		}

		private bool didLastTick;

		/// <summary>
		/// Processes the turn.
		/// </summary>
		/// <param name="safeMode">Stop processing if PLR files are missing?</param>
		/// <returns>Player empires which did not submit commands and are not defeated.</returns>
		/// <exception cref="InvalidOperationException">if the current empire is not null, or this galaxy is not the current galaxy..</exception>
		// TODO - make non-static so we don't have to say Current. everywhere
		public static IEnumerable<Empire> ProcessTurn(bool safeMode, Status status = null, double desiredProgress = 1d)
		{
			Current.SpaceObjectIDCheck("at start of turn");

			if (Empire.Current != null)
				throw new InvalidOperationException("Can't process the turn if there is a current empire. Load the game host's view of the galaxy instead.");

			Current.didLastTick = false;

			double progressPerOperation;
			if (status == null)
				progressPerOperation = 0d;
			else
				progressPerOperation = (desiredProgress - status.Progress) / (9 + Current.Empires.Count);

			if (status != null)
				status.Message = "Initializing turn";

			// We can enable the ability cache here because space objects aren't changing state yet in any way where order of operations is relevant.
			// For instance, all construction is supposed to take place simultaneously, so there's no reason to allow one construction order to affect other objects' abilities.
			// Plus this speeds up turn processing immensely!
			Current.EnableAbilityCache();

			// clear treaty clause cache (empires might have added treaties)
			Current.GivenTreatyClauseCache.Clear();
			Current.ReceivedTreatyClauseCache.Clear();

			// delete any floating space objects that are unused
			Current.SpaceObjectCleanup();

			//Battle.Previous.Clear();
			Current.Battles = new HashSet<Battle>();
			ScriptEngine.ClearScope(); // no caching galaxy between turns!

			Current.GivenTreatyClauseCache = new SafeDictionary<Empire, ILookup<Empire, Clause>>();
			Current.ReceivedTreatyClauseCache = new SafeDictionary<Empire, ILookup<Empire, Clause>>();


			if (status != null)
				status.Progress += progressPerOperation;

			// AI commands
			if (status != null)
				status.Message = "Playing AI turns";
			if (Current.Empires.Any(e => !e.IsPlayerEmpire && e.AI != null))
			{
				var serializedGalaxy = Galaxy.Current.SaveToString();
				var cmds = new Dictionary<int, IList<ICommand>>();
				var notes = new Dictionary<int, DynamicDictionary>();
				foreach (var i in Current.Empires.Where(e => !e.IsPlayerEmpire && e.AI != null).Select(e => Current.Empires.IndexOf(e)).ToArray())
				{
					LoadFromString(serializedGalaxy);
					Current.CurrentEmpire = Current.Empires[i];
					Current.Redact();
					Current.CurrentEmpire.AI.Act(Current.CurrentEmpire, Current, Current.CurrentEmpire.AI.MinisterNames);
					cmds.Add(i, Current.CurrentEmpire.Commands);
					notes.Add(i, Current.CurrentEmpire.AINotes);
				}
				LoadFromString(serializedGalaxy);
				foreach (var i in Current.Empires.Where(e => !e.IsPlayerEmpire && e.AI != null).Select(e => Current.Empires.IndexOf(e)).ToArray())
				{
					Current.LoadCommands(Current.Empires[i], cmds[i]);
					Current.Empires[i].AINotes = notes[i];
				}
			}
			if (status != null)
				status.Progress += progressPerOperation;

			// load commands
			if (status != null)
				status.Message = "Loading player commands";
			var missingPlrs = Current.LoadCommands();
			if (safeMode && missingPlrs.Any())
				return missingPlrs;
			if (status != null)
				status.Progress += progressPerOperation;

			Current.SpaceObjectIDCheck("after loading commands");

			// advance turn number
			Current.TurnNumber++;

			// colony maintenance
			if (status != null)
				status.Message = "Maintaining colonies";
			if (Current.TurnNumber.IsDivisibleBy(Mod.Current.Settings.ReproductionFrequency.DefaultTo(1)))
				Current.FindSpaceObjects<Planet>(p => p.HasColony).SafeForeach(ProcessPopulationGrowth);
			if (Current.TurnNumber.IsDivisibleBy(Mod.Current.Settings.ValueChangeFrequency.DefaultTo(1)))
				Current.FindSpaceObjects<Planet>(p => p.HasColony).SafeForeach(ProcessResourceValueChange);
			if (status != null)
				status.Progress += progressPerOperation;

			Current.SpaceObjectIDCheck("after colony maintenance");

			// resource generation
			if (status != null)
				status.Message = "Generating resources";

			// resource generation 1: colony income
			Current.FindSpaceObjects<Planet>().Select(p => p.Colony).ExceptSingle(null).SafeForeach(ProcessColonyIncome);

			// resource generation 2: remote mining
			// TODO - multithread remote mining once I can figure out where adjustedValue should go
			var adjustedValue = new SafeDictionary<IMineableSpaceObject, ResourceQuantity>(true);
			foreach (var emp in Current.Empires)
			{
				foreach (var kvp in emp.RemoteMiners)
				{
					// consume supplies
					// unlike most other operations, miners that are out of supplies still function
					// because having to resupply miners would be a pain :P
					var miner = kvp.Key.Item1;
					if (miner is SpaceVehicle)
					{
						var sv = miner as SpaceVehicle;
						var miningComps = sv.Components.Where(c => c.Abilities().Any(a => a.Rule.StartsWith("Remote Resource Generation - ")));
						var burn = miningComps.Sum(c => c.Template.SupplyUsage);
						sv.SupplyRemaining -= burn;
						sv.NormalizeSupplies();
					}

					// adjust resource value
					foreach (var r in Resource.All)
					{
						var amount = kvp.Value[r];
						var mined = kvp.Key.Item2;
						if (amount > 0 && adjustedValue[mined][r] == 0)
						{
							// resource was mined here, but hasn't been adjusted yet
							adjustedValue[mined][r] = Current.RemoteMiningModel.GetDecay(kvp.Value[r], mined.ResourceValue[r]);
							mined.ResourceValue[r] -= adjustedValue[mined][r];
						}
					}
				}

				// give income
				emp.StoredResources += emp.RemoteMiningIncome;
			}


			// resource generation 3: raw resource generation
			foreach (var emp in Current.Empires)
				emp.StoredResources += emp.RawResourceIncome;

			if (status != null)
				status.Progress += progressPerOperation;

			Current.SpaceObjectIDCheck("after resource generation");

			// empire stuff
			// TODO - multithread this, we'll need to get rid of the (1 of 4) or whatever after "Maintaining empires" :(
			foreach (var emp in Current.Empires)
			{
				if (status != null)
					status.Message = "Maintaining empires (" + (Current.Empires.IndexOf(emp) + 1) + " of " + Current.Empires.Count + ")";

				// pay maintenance on on ships/bases
				// TODO - allow mod to specify maintenance on units/facilities too?
				foreach (var v in emp.OwnedSpaceObjects.OfType<SpaceVehicle>().Where(x => !x.IsMemory))
					emp.StoredResources -= v.MaintenanceCost;

				// if not enough funds, lose ships/bases (weighted by maintenance cost)
				// TODO - if mods allow ground-unit/facility maintenance, lose those too?
				// TODO - destroy space units in cargo as well if they pay maintenance?
				// TODO - check if SE4 "saves up" deficits between turns to destroy ships slower than one per turn
				var deficit = -emp.StoredResources.Values.Where(r => r < 0).Sum();
				var lostShips = deficit / Mod.Current.Settings.MaintenanceDeficitToDestroyOneShip;
				for (int i = 0; i < lostShips; i++)
				{
					var ship = emp.OwnedSpaceObjects.OfType<SpaceVehicle>().PickWeighted(x => x.MaintenanceCost.Sum(y => y.Value));
					if (ship != null)
					{
						emp.Log.Add(ship.CreateLogMessage(ship + " fell into disrepair and was scuttled due to lack of funding for maintenance."));
						ship.Dispose();
					}
				}

				// perform treaty actions
				foreach (var clause in emp.GivenTreatyClauses.Flatten())
					clause.PerformAction();

				// don't let stored resources actually fall below zero
				foreach (var r in emp.StoredResources.Keys.Where(r => emp.StoredResources[r] < 0).ToArray())
					emp.StoredResources[r] = 0;

				// execute commands
				foreach (var cmd in emp.Commands.Where(cmd => cmd != null))
				{
					if (cmd.Issuer == emp)
					{
						if (cmd.Executor == null)
							cmd.Issuer.Log.Add(cmd.Issuer.CreateLogMessage("Attempted to issue " + cmd.GetType() + " to a nonexistent object with ID=" + cmd.ExecutorID + ". This is probably a game bug."));
						else if (cmd.Issuer != cmd.Executor.Owner && cmd.Issuer != cmd.Executor)
							cmd.Issuer.Log.Add(cmd.Issuer.CreateLogMessage("We cannot issue commands to " + cmd.Executor + " because it does not belong to us!"));
						else
							cmd.Execute();
					}
					else
					{
						// no hacking!
						cmd.Issuer.Log.Add(new GenericLogMessage(cmd.Issuer.Name + " cannot issue a command to an object belonging to " + emp + "!"));
					}
				}

				// do research
				var Spending = emp.ResearchSpending;
				var Queue = emp.ResearchQueue;
				// spend research from spending % priorities
				var spendable = emp.NetIncome[Resource.Research] + emp.BonusResearch;
				foreach (var tech in Spending.Keys.ToArray())
					emp.Research(tech, Spending[tech] * spendable / 100);

				// spend research from queues
				var leftovers = (100 - Spending.Sum(kvp => kvp.Value)) * spendable / 100;
				while (Queue.Any() && leftovers > 0)
				{
					// first tech in queue
					var tech = Queue.First();
					var toSpend = Math.Min(leftovers, tech.GetNextLevelCost(emp) - emp.AccumulatedResearch[tech]);
					emp.Research(tech, toSpend);
					leftovers -= toSpend;
				}

				// no items queued?
				if (!Queue.Any() && leftovers > 0)
				{
					if (Spending.Any(kvp => kvp.Value > 0))
					{
						// pick tech with highest % focus
						emp.Research(Spending.Where(kvp => kvp.Value == Spending.Max(kvp2 => kvp2.Value)).First().Key, leftovers);
					}
					else
					{
						// no techs queued or prioritized, pick a random tech
						var tech = emp.AvailableTechnologies.PickRandom();
						if (tech != null)
							emp.Research(emp.AvailableTechnologies.PickRandom(), leftovers);
					}
				}

				// clear bonus research for this turn
				emp.BonusResearch = 0;

				if (status != null)
					status.Progress += progressPerOperation;

				Current.SpaceObjectIDCheck("after empire maintenance for " + emp);
			}

			// validate fleets and share supplies
			foreach (var f in Current.FindSpaceObjects<Fleet>().ToArray())
			{
				f.Validate();
				f.ShareSupplies();
			}

			// construction queues
			if (status != null)
				status.Message = "Constructing objects";
			Current.Referrables.OfType<ConstructionQueue>().Where(q => !q.IsMemory).SafeForeach(q => q.ExecuteOrders());
			if (status != null)
				status.Progress += progressPerOperation;

			Current.SpaceObjectIDCheck("after construction");

			// replenish shields
			if (status != null)
				status.Message = "Replenishing shields";
			Current.FindSpaceObjects<ICombatSpaceObject>().SafeForeach(o => o.ReplenishShields());
			if (status != null)
				status.Progress += progressPerOperation;

			Current.SpaceObjectIDCheck("after shield replenishment");

			// ship movement
			if (status != null)
				status.Message = "Moving ships";
			Current.CurrentTick = 0;
			Current.FindSpaceObjects<IMobileSpaceObject>().SafeForeach(CommonExtensions.RefillMovement);
			Current.DisableAbilityCache(); // ships moving about and fighting can affect abilities!
			while (!Current.didLastTick)
			{
				// can at least cache abilities for the duration of a tick
				// seeing as actions within a tick are supposed to be simultaneous
				// the order of execution is arbitrary
				Current.EnableAbilityCache();

				Current.ComputeNextTickSize();
				// Don't let ships in fleets move separate from their fleets!
				Current.MoveShips();
				Current.CurrentTick += Current.NextTickSize;
				if (Current.CurrentTick >= 1d)
				{
					Current.CurrentTick = 1d;
					Current.NextTickSize = 0d;
					Current.MoveShips();
					Current.didLastTick = true;
				}
				foreach (var f in Current.Referrables.OfType<IFoggable>().Where(f => !f.IsMemory))
					f.Timestamp = Current.Timestamp;
				if (status != null && Current.NextTickSize != double.PositiveInfinity)
					status.Progress += progressPerOperation * Current.NextTickSize;

				Current.SpaceObjectIDCheck("after ship movement at T=" + Current.Timestamp);

				Current.DisableAbilityCache();
			}

			Current.EnableAbilityCache();

			// validate fleets again (ships might have been destroyed, consumed supplies, etc...)
			foreach (var f in Current.Referrables.OfType<Fleet>().ToArray())
			{
				f.Validate();
				f.ShareSupplies();
			}

			// TODO - more turn stuff? or do we have everything?

			if (status != null)
				status.Message = "Cleaning up";

			// deal with population in cargo again, in case colonies took damage and lost some population
			// TODO - multithread population cargo maintenance
			foreach (var p in Galaxy.Current.FindSpaceObjects<Planet>().Where(p => p.Colony != null))
			{
				var pop = p.Colony.Population;
				var ratio = (double)pop.Sum(kvp => kvp.Value) / (double)p.MaxPopulation;
				if (ratio < 1)
				{
					var cargo = p.Cargo;
					if (cargo != null)
					{
						// bring population out of cold storage
						// do this by removing and adding the population
						// this will work since population is removed from cargo storage first but added to population storage first
						foreach (var kvp in cargo.Population.ToArray())
						{
							var amount = kvp.Value;
							amount -= p.RemovePopulation(kvp.Key, kvp.Value);
							p.AddPopulation(kvp.Key, kvp.Value);
						}
					}
				}
			}

			// replenish shields again, so the players see the full shield amounts in the GUI
			Current.FindSpaceObjects<ICombatSpaceObject>().SafeForeach(o => o.ReplenishShields());


			// repair facilities
			Current.FindSpaceObjects<Planet>().Select(p => p.Colony).Where(c => c != null).SelectMany(c => c.Facilities).SafeForeach(f => f.Repair());

			// repair units
			Current.FindSpaceObjects<SpaceVehicle>().OfType<IUnit>().SafeForeach(u => u.Repair());
			Current.FindSpaceObjects<ISpaceObject>().OfType<ICargoContainer>().Where(p => p.Cargo != null).SelectMany(p => p.Cargo.Units).SafeForeach(u => u.Repair());

			// repair ships/bases
			// TODO - repair priorities
			foreach (var emp in Current.Empires)
			{
				// component repair is per sector per turn per empire, so we need to track it that way
				var usedPts = new SafeDictionary<Sector, int>();
				foreach (var v in Current.FindSpaceObjects<SpaceVehicle>().Where(v => v.Owner == emp && v.Sector != null && (v is Ship || v is Base)))
				{
					var pts = v.Sector.GetEmpireAbilityValue(emp, "Component Repair").ToInt() - usedPts[v.Sector];
					usedPts[v.Sector] += pts - v.Repair(pts).Value;
				}
			}

			// get supplies from reactors, solar panels, etc.
			Current.FindSpaceObjects<SpaceVehicle>().SafeForeach(v =>
			{
				v.SupplyRemaining += v.GetAbilityValue("Supply Generation Per Turn").ToInt();
				v.SupplyRemaining += v.GetAbilityValue("Solar Supply Generation").ToInt() * v.StarSystem.FindSpaceObjects<Star>().Count();
				v.NormalizeSupplies();
			});


			// resupply space vehicles one last time (after weapons fire and repair which could affect supply remaining/storage)
			Current.FindSpaceObjects<ISpaceObject>().Where(s => s.HasAbility("Supply Generation")).SafeForeach(sobj =>
			{
				var emp = sobj.Owner;
				var sector = sobj.Sector;
				foreach (var v in sector.SpaceObjects.OfType<SpaceVehicle>().Where(v => v.Owner == emp))
					v.SupplyRemaining = v.SupplyStorage;
			});
			// TODO - multithread this... somehow...
			foreach (var emp in Current.Empires)
			{
				foreach (var sys in Current.StarSystemLocations.Select(l => l.Item).Where(s => s.HasAbility("Supply Generation - System", emp) || s.HasAbility("Supply Generation - System")))
				{
					foreach (var v in sys.FindSpaceObjects<SpaceVehicle>().Where(v => v.Owner == emp))
						v.SupplyRemaining = v.SupplyStorage;
				}
			}

			Current.Empires.SafeForeach(emp =>
			{
				emp.StoredResources = ResourceQuantity.Min(emp.StoredResources, emp.ResourceStorage);// resource spoilage
				emp.Commands.Clear(); // clear empire commands
				emp.Scores[Current.TurnNumber] = emp.ComputeScore(null); // update score
			});


			// clear completed orders
			Current.Referrables.OfType<IPathfindingOrder>().Where(o => o.KnownTarget == null).SafeForeach(o => o.IsComplete = true);
			Current.Referrables.OfType<IOrder>().Where(o => o.IsComplete).SafeForeach(o => o.Dispose());

			// update known designs
			// TODO - multithread this somehow
			foreach (var emp in Current.Empires)
			{
				foreach (var design in Current.Referrables.OfType<IDesign>())
				{
					if (design.CheckVisibility(emp) >= Visibility.Scanned && !emp.KnownDesigns.Contains(design))
						emp.KnownDesigns.Add(design);
				}
			}

			// clear obsolete sensor ghosts
			// TODO - multithread this somehow
			foreach (var emp in Current.Empires)
			{
				foreach (var kvp in emp.Memory.ToArray())
				{
					if (kvp.Value.IsObsoleteMemory(emp))
					{
						emp.Memory.Remove(kvp);
						kvp.Value.Dispose();
					}
				}
			}

			// check for victory/defeat
			foreach (var vc in Current.VictoryConditions)
			{
				if (vc is TotalEliminationVictoryCondition || Current.TurnNumber > Current.VictoryDelay)
				{
					var winners = new List<Empire>();
					foreach (var emp in Current.Empires)
					{
						if (vc.GetProgress(emp) >= 1d)
						{
							// empire won!
							emp.Log.Add(emp.CreateLogMessage(vc.GetVictoryMessage(emp)));
						}
					}
					if (winners.Any())
					{
						foreach (var emp in Current.Empires.Where(e => !winners.Contains(e)))
						{
							// empire lost
							emp.Log.Add(emp.CreateLogMessage(vc.GetDefeatMessage(emp, winners)));
						}
					}
				}
			}

			if (status != null)
				status.Progress += progressPerOperation;

			// dispose of invalid waypoints e.g. space object got destroyed
			foreach (var w in Current.Referrables.OfType<Waypoint>().ToArray())
			{
				if (w.Sector == null)
					w.Dispose();
			}

			Current.SpaceObjectIDCheck("after cleanup");

			// end of turn scripts
			if (status != null)
				status.Message = "Executing scripts";
			ScriptEngine.RunScript<object>(Mod.Current.EndTurnScript);
			if (status != null)
				status.Progress += progressPerOperation;

			// delete any floating space objects that are unused
			Current.SpaceObjectCleanup();

			Current.SpaceObjectIDCheck("at end of turn");

			return missingPlrs;
		}

		internal void SpaceObjectIDCheck(string when)
		{
			foreach (var sobj in FindSpaceObjects<ISpaceObject>().ToArray())
			{
				if (!referrables.ContainsKey(sobj.ID))
					AssignID(sobj);
				if (sobj.ID > 0)
				{
					var r = referrables[sobj.ID];
					if (r != sobj)
					{
						// HACK - assume the space object that's actually in space is "real"
						referrables[sobj.ID] = sobj;
						Console.Error.WriteLine("Space object identity mismatch " + when + " for ID=" + sobj.ID + ". " + sobj + " is actually in space so it is replacing " + r + " in the referrables collection.");
					}
				}
			}
		}

		/// <summary>
		/// Only public for unit tests. You should probably call ProcessTurn instead.
		/// </summary>
		/// <param name="p"></param>
		public static void ProcessPopulationGrowth(Planet p)
		{
			var pop = p.Colony.Population;
			var wasFull = p.PopulationStorageFree == 0;
			var deltas = p.PopulationChangePerTurnPerRace;
			foreach (var race in pop.Keys.ToArray())
			{
				pop[race] += deltas[race];
			}

			// deal with overpopulation
			var ratio = (double)pop.Sum(kvp => kvp.Value) / (double)p.MaxPopulation;
			if (ratio > 1)
			{
				foreach (var race in pop.Keys.ToArray())
				{
					// TODO - should planetary population spill over into cargo?
					// this might be annoying for homeworlds, as their cargo space would fill up quickly...
					// especially in Proportions Mod with its 1000kT/1M population!
					pop[race] = (long)(pop[race] / ratio);
				}
				if (!wasFull && p.Owner != null)
					p.Owner.RecordLog(p, "{0} has completely filled up with population. Building colonizers or transports is advised.".F(p));
			}

			// deal with population in cargo
			ratio = (double)pop.Sum(kvp => kvp.Value) / (double)p.MaxPopulation;
			if (ratio < 1)
			{
				var cargo = p.Cargo;
				if (cargo != null)
				{
					// bring population out of cold storage
					// do this by removing and adding the population
					// this will work since population is removed from cargo storage first but added to population storage first
					foreach (var kvp in cargo.Population.ToArray())
					{
						var amount = kvp.Value;
						amount -= p.RemovePopulation(kvp.Key, kvp.Value);
						p.AddPopulation(kvp.Key, kvp.Value);
					}
				}
			}
		}

		/// <summary>
		/// Only public for unit tests. You should probably call ProcessTurn instead.
		/// </summary>
		/// <param name="p"></param>
		public static void ProcessResourceValueChange(Planet p)
		{
			foreach (var r in Resource.All.Where(r => r.HasValue))
			{
				bool wasFull = p.ResourceValue[r] == Current.MaxPlanetValue;
				bool wasEmpty = p.ResourceValue[r] == Current.MinPlanetValue;
				var modifier =
					p.GetAbilityValue("Planet - Change {0} Value".F(r.Name)).ToInt()
					+ p.GetAbilityValue("Sector - Change {0} Value".F(r.Name)).ToInt()
					+ p.GetAbilityValue("System - Change {0} Value".F(r.Name)).ToInt()
					+ p.GetAbilityValue("Empire - Change {0} Value".F(r.Name)).ToInt();
				p.ResourceValue[r] += modifier;
				p.ResourceValue[r] = p.ResourceValue[r].LimitToRange(Current.MinPlanetValue, Current.MaxPlanetValue);
				if (!wasFull && p.ResourceValue[r] == Current.MaxPlanetValue && p.Owner != null)
					p.Owner.RecordLog(p, "{0}'s {1} have been completely replenished. Its value is at the absolute maximum.".F(p, r));
				if (!wasEmpty && p.ResourceValue[r] == Current.MinPlanetValue && p.Owner != null)
					p.Owner.RecordLog(p, "{0} has been stripped dry of {1}. Its value is at the bare minimum.".F(p, r));
			}
		}

		/// <summary>
		/// Only public for unit tests. You should probably call ProcessTurn instead.
		/// </summary>
		/// <param name="p"></param>
		public static void ProcessColonyIncome(Colony c)
		{
			var p = c.Container;
			var sys = p.StarSystem;
			var income = p.GrossIncome();

			// log messages
			if (income < p.GrossIncomeIgnoringSpaceport)
			{
				var ratio = p.Colony.MerchantsRatio;
				if (ratio == 0)
					p.Owner.Log.Add(p.CreateLogMessage(p + " earned no income due to lack of a spaceport."));
				else if (ratio < 1)
					p.Owner.Log.Add(p.CreateLogMessage(p + " earned only " + Math.Floor(ratio * 100) + "% of normal income due to lack of a spaceport."));
			}

			// give owner his income
			lock (p.Owner.StoredResources) p.Owner.StoredResources += income;

			// adjust resource value
			// TODO - have a "is mineable" or "has value" property on Resource class
			var incomeWithoutValue = new ResourceQuantity();
			if (p.ResourceValue[Resource.Minerals] != 0)
				incomeWithoutValue += income[Resource.Minerals] / p.ResourceValue[Resource.Minerals] * Resource.Minerals;
			if (p.ResourceValue[Resource.Organics] != 0)
				incomeWithoutValue += income[Resource.Organics] / p.ResourceValue[Resource.Organics] * Resource.Organics;
			if (p.ResourceValue[Resource.Radioactives] != 0)
				incomeWithoutValue += income[Resource.Radioactives] / p.ResourceValue[Resource.Radioactives] * Resource.Radioactives;
			incomeWithoutValue += income[Resource.Research] * Resource.Research;
			incomeWithoutValue += income[Resource.Intelligence] * Resource.Research;
			foreach (var kvp in incomeWithoutValue)
			{
				p.ResourceValue[kvp.Key] -= Current.StandardMiningModel.GetDecay(kvp.Value, p.ResourceValue[kvp.Key]);
			}
		}

		/// <summary>
		/// Only public for unit tests. You should probably call ProcessTurn instead.
		/// </summary>
		public void MoveShips()
		{
			var vlist = FindSpaceObjects<IMobileSpaceObject>().Where(sobj => sobj.Container == null && !sobj.IsMemory).Shuffle();
			foreach (var v in vlist)
			{
				// mark system explored if not already
				var sys = v.FindStarSystem();
				if (sys == null)
					continue; // space object is dead, or not done being built

				bool didStuff = v.ExecuteOrders();
				if (!sys.ExploredByEmpires.Contains(v.Owner))
					sys.ExploredByEmpires.Add(v.Owner);

				// update memory sight after movement
				if (didStuff)
				{
					v.UpdateEmpireMemories();
					if (v.StarSystem != null && v.Owner != null)
					{
						foreach (var sobj in v.StarSystem.FindSpaceObjects<ISpaceObject>().Where(sobj => sobj != v && !sobj.IsMemory).ToArray())
							v.Owner.UpdateMemory(sobj);
					}

					// replenish shields after moving (who knows, we might be out of supplies, or about to hit a minefield)
					v.ReplenishShields();
				}

				// check for battles
				var sector = v.FindSector();
				if (v.Owner != null && // unowned objects can't pick fights
					sector != null && // can't fight nowhere
					sector.SpaceObjects.OfType<ICombatant>().Any(sobj => sobj.IsHostileTo(v.Owner) || v.IsHostileTo(sobj.Owner)) && // any enemies?
					(!lastBattleTimestamps.ContainsKey(sector) || lastBattleTimestamps[sector] < 1d / v.Speed)) // have we fought here too recently?
				{
					// resolve the battle
					var battle = new Battle(sector);
					battle.Resolve();
					Battles.Add(battle);
					foreach (var emp in battle.Empires)
						emp.Log.Add(battle.CreateLogMessage(battle.NameFor(emp)));
					lastBattleTimestamps[sector] = Current.Timestamp;
				}
			}
		}

		private IDictionary<Sector, double> lastBattleTimestamps = new SafeDictionary<Sector, double>();

		/// <summary>
		/// Anything in the game that can be referenced from the client side
		/// using a Reference object instead of passing whole objects around.
		/// Stuff needs to be registered to be found though!
		/// </summary>
		[SerializationPriority(2)]
		internal IDictionary<long, IReferrable> referrables { get; set; }

		public IReferrable GetReferrable(long key)
		{
			if (!referrables.ContainsKey(key))
				return null;
			return referrables[key];
		}

		public IEnumerable<IReferrable> Referrables { get { return referrables.Values; } }

		/// <summary>
		/// Assigns an ID to an object.
		/// Will dispose of an object that has a negative ID if it hasn't already been disposed of.
		/// </summary>
		/// <param name="r">The object.</param>
		/// <param name="id">The ID, or 0 to generate a new ID (unless the ID is already valid).</param>
		/// <returns>The new ID.</returns>
		public long AssignID(IReferrable r, long id = 0)
		{
			if (r.ID < 0)
			{
				if (!r.IsDisposed)
					r.Dispose();
				return r.ID;
			}

			if (r.HasValidID())
				return r.ID; // no need to reassign ID
			else if (referrables.ContainsKey(r.ID))
			{
				// HACK - already exists, just log an error but don't overwrite anything
				// we need to fix start combatants having the same IDs as the real objects...
				Console.Error.WriteLine("The galaxy thinks that " + referrables[r.ID] + " has the ID " + r.ID + " but " + r + " claims to have that ID as well.");
				return r.ID;
			}

			var oldid = r.ID;
			long newid = oldid <= 0 ? id : oldid;

			if (Referrables.LongCount() == long.MaxValue)
				throw new Exception("No more IDs are available to assign for objects.");

			while (newid <= 0 || referrables.ContainsKey(newid))
			{
				newid = RandomHelper.Range(1L, long.MaxValue);
			}
			r.ID = newid;
			referrables.Add(newid, r);

			// clean up old IDs
			if (oldid > 0 && referrables.ContainsKey(oldid) && oldid != newid)
				referrables.Remove(oldid);

			return newid;
		}

		public void UnassignID(long id)
		{
			if (referrables.ContainsKey(id))
			{
				var r = referrables[id];
				r.ID = -1;
				referrables.Remove(id);
			}
		}

		public void UnassignID(IReferrable r)
		{
			if (r != null && referrables.ContainsKey(r.ID))
			{
				if (referrables[r.ID] == r)
					referrables.Remove(r.ID);
				else
				{
					var galaxyThinksTheIDIs = referrables.SingleOrDefault(kvp => kvp.Value == r);
					referrables.Remove(galaxyThinksTheIDIs);
				}
			}
			if (r != null)
				r.ID = -1;
		}

		/// <summary>
		/// Initializes a new game. Sets Galaxy.Current.
		/// </summary>
		/// <exception cref="InvalidOperationException">if there is no mod loaded.</exception>
		/// <param name="status">A status object to report status back to the GUI.</param>
		/// <param name="desiredProgress">How much progress should we report back to the GUI when we're done initializing the galaxy? 1.0 means all done with everything that needs to be done.</param>
		public static void Initialize(GameSetup gsu, Status status = null, double desiredProgress = 1.0)
		{
			if (Mod.Current == null)
				throw new InvalidOperationException("Cannot initialize a galaxy without a mod. Load a mod into Mod.Current first.");

			var startProgress = status == null ? 0d : status.Progress;
			var progressPerStep = (desiredProgress - startProgress) / 4d;

			// create the game
			var galtemp = gsu.GalaxyTemplate;
			galtemp.GameSetup = gsu;
			Current = galtemp.Instantiate(status, startProgress + progressPerStep);
			if (status != null)
				status.Message = "Populating galaxy";
			gsu.PopulateGalaxy(Current);
			if (status != null)
				status.Progress += progressPerStep;

			// set single player flag
			Current.IsSinglePlayer = gsu.IsSinglePlayer;

			// run init script
			if (status != null)
				status.Message = "Executing script";
			ScriptEngine.RunScript<object>(Mod.Current.GameInitScript);
			if (status != null)
				status.Progress += progressPerStep;

			// save the game
			if (status != null)
				status.Message = "Saving game";
			Galaxy.SaveAll(status, desiredProgress);
		}

		/// <summary>
		/// Searches for space objects matching criteria.
		/// </summary>
		/// <typeparam name="T">The type of space object.</typeparam>
		/// <param name="criteria">The criteria.</param>
		/// <returns>The matching space objects.</returns>
		public IEnumerable<T> FindSpaceObjects<T>(Func<T, bool> criteria = null)
		{
			return StarSystemLocations.SelectMany(l => l.Item.FindSpaceObjects<T>(criteria));
		}

		/// <summary>
		/// The next tick size, for ship movement.
		/// </summary>
		public double NextTickSize { get; private set; }

		/// <summary>
		/// The current tick in turn processing. 0 = start of turn, 1 = end of turn.
		/// </summary>
		public double CurrentTick { get; set; }

		public void ComputeNextTickSize()
		{

			var objs = FindSpaceObjects<IMobileSpaceObject>().Where(obj => obj.Orders.Any());
			objs = objs.Where(obj => !obj.IsMemory);
			if (objs.Any() && CurrentTick < 1.0)
				NextTickSize = Math.Max(Math.Min(1.0 - CurrentTick, objs.Min(v => v.TimeToNextMove)), 1e-15);
			else
				NextTickSize = double.PositiveInfinity;
		}

		public IEnumerable<IAbilityObject> GetContainedAbilityObjects(Empire emp)
		{
			return StarSystemLocations.Select(ssl => ssl.Item).Concat(StarSystemLocations.SelectMany(ssl => ssl.Item.GetContainedAbilityObjects(emp)));
		}

		#endregion

		/// <summary>
		/// Is the ability cache enabled?
		/// Always enabled on the client side; only when a flag is set on the server side.
		/// </summary>
		public bool IsAbilityCacheEnabled
		{
			get
			{
				return Empire.Current != null || isAbilityCacheEnabled;
			}
		}

		private bool isAbilityCacheEnabled;

		/// <summary>
		/// Enables the server side ability cache.
		/// </summary>
		public void EnableAbilityCache()
		{
			isAbilityCacheEnabled = true;
		}

		/// <summary>
		/// Disables the server side ability cache.
		/// </summary>
		public void DisableAbilityCache()
		{
			isAbilityCacheEnabled = false;
			AbilityCache.Clear();
			CommonAbilityCache.Clear();
			SharedAbilityCache.Clear();
		}

		/// <summary>
		/// Cache of abilities belonging to game objects.
		/// </summary>
		[DoNotSerialize]
		internal SafeDictionary<IAbilityObject, IEnumerable<Ability>> AbilityCache { get; private set; }

		/// <summary>
		/// Cache of abilities belonging to common game objects that can have different abilities for each empire.
		/// </summary>
		[DoNotSerialize]
		internal SafeDictionary<Tuple<ICommonAbilityObject, Empire>, IEnumerable<Ability>> CommonAbilityCache { get; private set; }

		/// <summary>
		/// Cache of abilities that are shared to empires from other objects due to treaties.
		/// </summary>
		[DoNotSerialize]
		internal SafeDictionary<Tuple<IOwnableAbilityObject, Empire>, IEnumerable<Ability>> SharedAbilityCache { get; private set; }

		/// <summary>
		/// Cache of treaty clauses given by empires.
		/// </summary>
		[DoNotSerialize]
		internal SafeDictionary<Empire, ILookup<Empire, Clause>> GivenTreatyClauseCache { get; set; }

		/// <summary>
		/// Cache of treaty clauses received by empires.
		/// </summary>
		[DoNotSerialize]
		internal SafeDictionary<Empire, ILookup<Empire, Clause>> ReceivedTreatyClauseCache { get; set; }

		public IEnumerable<Ability> IntrinsicAbilities
		{
			// TODO - galaxy wide abilities?
			get { yield break; }
		}

		public IEnumerable<IAbilityObject> Children
		{
			get { return StarSystemLocations.Select(l => l.Item); }
		}

		public IEnumerable<IAbilityObject> Parents
		{
			get
			{
				yield break;
			}
		}

		/// <summary>
		/// Disposes of any space objects that aren't in space, under construction, or part of the mod definition.
		/// </summary>
		private void SpaceObjectCleanup()
		{
			foreach (var sobj in Referrables.OfType<ISpaceObject>().ToArray())
			{
				bool dispose = true;
				if (sobj.Sector != null)
					dispose = false; // save space objects that are in space
				else if (Mod.Current.StellarObjectTemplates.Contains(sobj as StellarObject))
					dispose = false; // save stellar objects that are part of the mod templates
				else if (Referrables.OfType<ConstructionQueue>().Any(q => q.Orders.Any(o => o.Item == sobj as IConstructable)))
					dispose = false; // save constructable space objects under construction
				if (dispose)
					sobj.Dispose();
			}
		}

		private string stringValue;

		/// <summary>
		/// Serialized string value of the galaxy at the beginning of the turn.
		/// Only valid client side; otherwise returns null.
		/// </summary>
		[DoNotSerialize]
		internal string StringValue
		{
			get
			{
				if (Empire.Current == null)
					return null;
				if (stringValue == null)
					StringValue = SaveToString(false);
				return stringValue;
			}
			private set
			{
				stringValue = value;
			}
		}

		// TODO - replace all those duplicate properties with a reference to the game setup
		/*
		/// <summary>
		/// The game setup used to create this galaxy.
		/// </summary>
		public GameSetup GameSetup { get; set; }
		*/

		public WarpPointPlacementStrategy WarpPointPlacementStrategy { get; set; }

		public override string ToString()
		{
			if (CurrentEmpire == null)
				return Name;
			return CurrentEmpire.Name + " - " + CurrentEmpire.LeaderName + " - " + Stardate;
		}

		/// <summary>
		/// Notes that mod scripts can play with.
		/// </summary>
		public DynamicDictionary ScriptNotes { get; private set; }

		/// <summary>
		/// Finds referrable objects in the galaxy.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="condition"></param>
		/// <returns></returns>
		public IEnumerable<T> Find<T>(Func<T, bool> condition = null) where T : IReferrable
		{
			if (condition == null)
				condition = t => true;
			return Referrables.OfType<T>().Where(r => condition(r));
		}
	}

	/// <summary>
	/// Prevents IDs from being assigned to objects when calling AssignIDs.
	/// TODO - move to utility namespace?
	/// </summary>
	public class DoNotAssignIDAttribute : Attribute
	{
		public DoNotAssignIDAttribute(bool recurse = true)
		{
			Recurse = recurse;
		}

		/// <summary>
		/// Should the "don't assign ID" rule be recursive?
		/// </summary>
		public bool Recurse { get; private set; }
	}

}
