﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FrEee.Game.Objects.Combat2;
using System.Runtime.InteropServices;

using Mogre;
using System.Windows.Forms;
using System.Drawing;
using FrEee.WinForms.Forms;

namespace FrEee.WinForms.MogreCombatRender
{
	class ShutdownException : Exception { }
	class MogreFreeMain
	{
		private static int tickcounter = 0; //the tick age of the game.
		private Root mRoot;
		private RenderWindow mRenderWindow;
		private SceneManager mSceneMgr;
		private Viewport mViewport;

		private MOIS.Keyboard mKeybrd;
		private MOIS.Mouse mMouse;

		protected Camera mCamera;
		protected CameraMan mCameraMan;

		private MogreCombatForm form;
        private float replaySpeed = 1f;

		//protected RaySceneQuery mRaySceneQuery = null;      // The ray scene query pointer
		protected SceneNode mNode_lines = null;

		System.Diagnostics.Stopwatch physicsstopwatch = new System.Diagnostics.Stopwatch();

		//Dictionary<string, CombatObject> renderObjects = new Dictionary<string, CombatObject>();
		private Battle_Space battle;

        private int SelectedComObj = 1;

		public MogreFreeMain(Battle_Space battle)
		{
			this.battle = battle;
            battle.IsReplay = true;
			

			try
			{
				CreateRoot();
				DefineResources();
				CreateRenderSystem();
				CreateRenderWindow();
				InitializeResources();


				CreateScene();
				InitializeInput();
				CreateFrameListeners();
                
                setup();
				Go();

			}
			catch (OperationCanceledException) { }
		}

		private void setup()
		{
            battle.SetUpPieces();
			foreach (CombatObject comObj in battle.CombatObjects)
			{
                CreateNewEntity(comObj);

                do_graphics(comObj, comObj.cmbt_loc);
			}
		}


		#region mogresetup

		protected virtual void CreateCamera()
		{
			mCamera = mSceneMgr.CreateCamera("PlayerCam");
			mCamera.Position = new Vector3(0, 0, 1000f);
			mCamera.LookAt(Vector3.ZERO);
			mCamera.NearClipDistance = 5;
			mCamera.FarClipDistance = 10000f;
			mCameraMan = new CameraMan(mCamera);
		}

		private void CreateRoot()
		{
			mRoot = new Root(); //can change location/name of plugins.cfg ogre.cfg and Ogre.log files here using the Root parameters. 
		}

		private void DefineResources()
		{
			ConfigFile cf = new ConfigFile();
			cf.Load("resources.cfg", "\t:=", true);
            
			var section = cf.GetSectionIterator();
			while (section.MoveNext())
			{
				foreach (var line in section.Current)
				{
					ResourceGroupManager.Singleton.AddResourceLocation(
						line.Value, line.Key, section.CurrentKey);
				}
			}
		}

		private void CreateRenderSystem()
		{
			if (!mRoot.ShowConfigDialog())
				throw new OperationCanceledException();
		}

		private void CreateRenderWindow()
		{
			// Create Render Window
			mRoot.Initialise(false, "Main Ogre Window");
			NameValuePairList misc = new NameValuePairList();
			form = new MogreCombatForm(battle);
			misc["externalWindowHandle"] = form.Handle.ToString();
			mRenderWindow = mRoot.CreateRenderWindow("Main RenderWindow", 800, 600, false, misc);
			form.Size = new Size(800, 600);
			form.Disposed += form_Disposed;
			form.Resize += form_Resize;
			form.Show();
		}

		void form_Resize(object sender, EventArgs e)
		{
			mRenderWindow.WindowMovedOrResized();
			mCamera.AspectRatio = (float)mViewport.ActualWidth / mViewport.ActualHeight;            
		}

		void form_Disposed(object sender, EventArgs e)
		{
			mRoot.Dispose();
			mRoot = null;
		}

		private void InitializeResources()
		{
			TextureManager.Singleton.DefaultNumMipmaps = 5;
			ResourceGroupManager.Singleton.InitialiseAllResourceGroups();
		}

		protected void InitializeInput()
		{

			MOIS.ParamList pl = new MOIS.ParamList();
			int windowHnd;
			mRenderWindow.GetCustomAttribute("WINDOW", out windowHnd);
			pl.Insert("WINDOW", windowHnd.ToString());
			pl.Insert("w32_mouse", "DISCL_FOREGROUND");
			pl.Insert("w32_mouse", "DISCL_NONEXCLUSIVE");
			pl.Insert("w32_keyboard", "DISCL_FOREGROUND");
			pl.Insert("w32_keyboard", "DISCL_NONEXCLUSIVE");
			//var inputMgr = MOIS.InputManager.CreateInputSystem((uint)windowHnd);
			var inputMgr = MOIS.InputManager.CreateInputSystem(pl);

			mKeybrd = (MOIS.Keyboard)inputMgr.CreateInputObject(MOIS.Type.OISKeyboard, true);
			mMouse = (MOIS.Mouse)inputMgr.CreateInputObject(MOIS.Type.OISMouse, true);

			MOIS.MouseState_NativePtr mouseState = mMouse.MouseState;
			mouseState.width = mViewport.ActualWidth; //! update after resize window
			mouseState.height = mViewport.ActualHeight;

			mKeybrd.KeyPressed += new MOIS.KeyListener.KeyPressedHandler(OnKeyPressed);
			mKeybrd.KeyReleased += new MOIS.KeyListener.KeyReleasedHandler(OnKeyReleased);
			mMouse.MouseMoved += new MOIS.MouseListener.MouseMovedHandler(OnMouseMoved);
			mMouse.MousePressed += new MOIS.MouseListener.MousePressedHandler(OnMousePressed);
			mMouse.MouseReleased += new MOIS.MouseListener.MouseReleasedHandler(OnMouseReleased);

			//mRaySceneQuery = mSceneMgr.CreateRayQuery(new Ray());
		}

		private void CreateScene()
		{
			mSceneMgr = mRoot.CreateSceneManager(SceneType.ST_GENERIC);

			CreateCamera();
			mViewport = mRenderWindow.AddViewport(mCamera);
			mViewport.BackgroundColour = ColourValue.Black;
			mCamera.AspectRatio = (float)mViewport.ActualWidth / mViewport.ActualHeight;




			String resourceGroupName = "lines";
			if (ResourceGroupManager.Singleton.ResourceGroupExists(resourceGroupName) == false)
				ResourceGroupManager.Singleton.CreateResourceGroup(resourceGroupName);

			MaterialPtr moMaterialblue = MaterialManager.Singleton.Create("line_blue", resourceGroupName);
			moMaterialblue.ReceiveShadows = false;
			moMaterialblue.GetTechnique(0).SetLightingEnabled(true);
			moMaterialblue.GetTechnique(0).GetPass(0).SetDiffuse(0, 0, 1, 0);
			moMaterialblue.GetTechnique(0).GetPass(0).SetAmbient(0, 0, 1);
			moMaterialblue.GetTechnique(0).GetPass(0).SetSelfIllumination(0, 0, 1);
			moMaterialblue.Dispose();  // dispose pointer, not the material
			MaterialPtr moMaterialred = MaterialManager.Singleton.Create("line_red", resourceGroupName);
			moMaterialred.ReceiveShadows = false;
			moMaterialred.GetTechnique(0).SetLightingEnabled(true);
			moMaterialred.GetTechnique(0).GetPass(0).SetDiffuse(1, 0, 0, 0);
			moMaterialred.GetTechnique(0).GetPass(0).SetAmbient(1, 0, 0);
			moMaterialred.GetTechnique(0).GetPass(0).SetSelfIllumination(1, 0, 0);
			moMaterialred.Dispose();  // dispose pointer, not the material
			MaterialPtr moMaterialgreen = MaterialManager.Singleton.Create("line_green", resourceGroupName);
			moMaterialgreen.ReceiveShadows = false;
			moMaterialgreen.GetTechnique(0).SetLightingEnabled(true);
			moMaterialgreen.GetTechnique(0).GetPass(0).SetDiffuse(0, 1, 0, 0);
			moMaterialgreen.GetTechnique(0).GetPass(0).SetAmbient(0, 1, 0);
			moMaterialgreen.GetTechnique(0).GetPass(0).SetSelfIllumination(0, 1, 0);
			moMaterialgreen.Dispose();  // dispose pointer, not the material
            MaterialPtr moMaterialpurple = MaterialManager.Singleton.Create("line_purple", resourceGroupName);
            moMaterialpurple.ReceiveShadows = false;
            moMaterialpurple.GetTechnique(0).SetLightingEnabled(true);
            moMaterialpurple.GetTechnique(0).GetPass(0).SetDiffuse(1, 0, 1, 0);
            moMaterialpurple.GetTechnique(0).GetPass(0).SetAmbient(1, 0, 1);
            moMaterialpurple.GetTechnique(0).GetPass(0).SetSelfIllumination(1, 0, 1);
            moMaterialpurple.Dispose();  // dispose pointer, not the material
            MaterialPtr moMaterialyellow = MaterialManager.Singleton.Create("line_yellow", resourceGroupName);
            moMaterialyellow.ReceiveShadows = false;
            moMaterialyellow.GetTechnique(0).SetLightingEnabled(true);
            moMaterialyellow.GetTechnique(0).GetPass(0).SetDiffuse(1, 1, 0, 0);
            moMaterialyellow.GetTechnique(0).GetPass(0).SetAmbient(1, 1, 0);
            moMaterialyellow.GetTechnique(0).GetPass(0).SetSelfIllumination(1, 1, 0);
            moMaterialyellow.Dispose();  // dispose pointer, not the material
            MaterialPtr moMaterialcyan = MaterialManager.Singleton.Create("line_cyan", resourceGroupName);
            moMaterialcyan.ReceiveShadows = false;
            moMaterialcyan.GetTechnique(0).SetLightingEnabled(true);
            moMaterialcyan.GetTechnique(0).GetPass(0).SetDiffuse(0, 1, 1, 0);
            moMaterialcyan.GetTechnique(0).GetPass(0).SetAmbient(0, 1, 1);
            moMaterialcyan.GetTechnique(0).GetPass(0).SetSelfIllumination(0, 1, 1);
            moMaterialcyan.Dispose();  // dispose pointer, not the material

			mNode_lines = mSceneMgr.RootSceneNode.CreateChildSceneNode(" ", Vector3.ZERO);
            mNode_lines.Position = new Vector3(0, 0, 0);
			mSceneMgr.AmbientLight = new ColourValue(0.5f, 0.5f, 0.5f);

			Light l = mSceneMgr.CreateLight("MainLight");
			l.Position = new Vector3(0, 0, 5000);

		}

		private void CreateFrameListeners()
		{
			mRoot.FrameRenderingQueued += new FrameListener.FrameRenderingQueuedHandler(ProcessBufferedInput);
		}

		private bool ProcessBufferedInput(FrameEvent evt)
		{
			//mTimer -= evt.timeSinceLastFrame;
			//return (mTimer > 0);
			mKeybrd.Capture();
			mMouse.Capture();
			mCameraMan.UpdateCamera(evt.timeSinceLastFrame);
			return true;
		}

		private void EnterRenderLoop()
		{
			if (mRoot != null)
				mRoot.StartRendering();
		}

		protected void Shutdown()
		{
			throw new ShutdownException();
		}

        #region input control
        protected bool OnKeyPressed(MOIS.KeyEvent evt)
		{
			switch (evt.key)
			{
				case MOIS.KeyCode.KC_W:
				case MOIS.KeyCode.KC_UP:
					//shiplist[0].Thrusting = 1;
					//mCameraMan.GoingForward = true;
                    mCameraMan.GoingUp = true;
					break;

				case MOIS.KeyCode.KC_S:
				case MOIS.KeyCode.KC_DOWN:
					//shiplist[0].Thrusting = -1;
					//mCameraMan.GoingBack = true;
                    mCameraMan.GoingDown = true;
					break;

				case MOIS.KeyCode.KC_A:
				case MOIS.KeyCode.KC_LEFT:
					//shiplist[0].Strafing = -1;
					mCameraMan.GoingLeft = true;
					break;

				case MOIS.KeyCode.KC_D:
				case MOIS.KeyCode.KC_RIGHT:
					//shiplist[0].Strafing = 1;
					mCameraMan.GoingRight = true;
					break;

				case MOIS.KeyCode.KC_E:
                    break;

				case MOIS.KeyCode.KC_PGUP:
                    replaySpeed *= 2f;
					break;

				case MOIS.KeyCode.KC_Q:
                    break;

				case MOIS.KeyCode.KC_PGDOWN:
                    replaySpeed *= 0.5f;
					break;

				case MOIS.KeyCode.KC_SPACE:
					//Console.Out.WriteLine("space was pushed.");
					//Rocks rocksphere = new Rocks(60, 6, 8);
					//Console.Out.WriteLine(rocksphere.ToString());
					//Console.Out.WriteLine(rocksphere.verts[1][1].ToString());
					break;

				case MOIS.KeyCode.KC_LSHIFT:
				case MOIS.KeyCode.KC_RSHIFT:
					mCameraMan.FastMove = true;
					break;

                case MOIS.KeyCode.KC_LBRACKET:
                    selectPrev();                    
                    break;

                case MOIS.KeyCode.KC_RBRACKET:
                    selectNext();  
                    break;

				case MOIS.KeyCode.KC_T:
					//CycleTextureFilteringMode();
					break;

				case MOIS.KeyCode.KC_R:
					//CyclePolygonMode();
					break;

				case MOIS.KeyCode.KC_F5:
					//ReloadAllTextures();
					break;

				case MOIS.KeyCode.KC_SYSRQ:
					//TakeScreenshot();
					break;



				case MOIS.KeyCode.KC_ESCAPE:
					Shutdown();
					break;
			}

			return true;
		}

		protected bool OnKeyReleased(MOIS.KeyEvent evt)
		{
			switch (evt.key)
			{
				case MOIS.KeyCode.KC_W:
				case MOIS.KeyCode.KC_UP:

					//mCameraMan.GoingForward = false;
                    mCameraMan.GoingUp = false;
					break;

				case MOIS.KeyCode.KC_S:
				case MOIS.KeyCode.KC_DOWN:

					//mCameraMan.GoingBack = false;
                    mCameraMan.GoingDown = false;
					break;

				case MOIS.KeyCode.KC_A:
				case MOIS.KeyCode.KC_LEFT:

					mCameraMan.GoingLeft = false;
					break;

				case MOIS.KeyCode.KC_D:
				case MOIS.KeyCode.KC_RIGHT:

					mCameraMan.GoingRight = false;
					break;

				case MOIS.KeyCode.KC_E:
				case MOIS.KeyCode.KC_PGUP:

					mCameraMan.GoingUp = false;
					break;

				case MOIS.KeyCode.KC_Q:
				case MOIS.KeyCode.KC_PGDOWN:

					mCameraMan.GoingDown = false;
					break;

				case MOIS.KeyCode.KC_LSHIFT:
				case MOIS.KeyCode.KC_RSHIFT:
					mCameraMan.FastMove = false;
					break;
			}

			return true;
		}

		protected virtual bool OnMouseMoved(MOIS.MouseEvent evt)
		{
			if (mCameraMan.MouseLook == true)
			{
				mCameraMan.MouseMovement(evt.state.X.rel, evt.state.Y.rel, evt.state.Z.rel);
			}
			else
				mCameraMan.MouseMovement(0, 0, evt.state.Z.rel);
			return true;
		}

		protected virtual bool OnMousePressed(MOIS.MouseEvent evt, MOIS.MouseButtonID id)
		{
			if (id == MOIS.MouseButtonID.MB_Right)
			{
				mCameraMan.MouseLook = true;
			}
			else if (id == MOIS.MouseButtonID.MB_Left)
			{

			}
			return true;
		}

		protected virtual bool OnMouseReleased(MOIS.MouseEvent evt, MOIS.MouseButtonID id)
		{
			if (id == MOIS.MouseButtonID.MB_Right)
			{
				mCameraMan.MouseLook = false;
			}
			return true;
		}

        #endregion

        private void CreateNewEntity(CombatObject obj)
		{
			Entity objEnt = mSceneMgr.CreateEntity(obj.ID.ToString(), "DeltaShip.mesh");
			SceneNode objNode = mSceneMgr.RootSceneNode.CreateChildSceneNode(obj.ID.ToString());
			float sizex = objEnt.BoundingBox.Size.x;
			float sizey = objEnt.BoundingBox.Size.y;
			float sizez = objEnt.BoundingBox.Size.z;
			var desiredSize = (float)System.Math.Pow(obj.cmbt_mass, 1d / 3d);
			float scalex = (desiredSize / sizex);
			float scaley = (desiredSize / sizey);
			float scalez = (desiredSize / sizez);
			float scale = System.Math.Min(System.Math.Min(scalex, scaley), scalez);
			objNode.AttachObject(objEnt);
			objNode.Scale(scale, scale, scale);
			objNode.Scale(10, 10, 10);
			//do_graphics(obj); // set up initial position and orientation
            //Entity objEn = mSceneMgr.CreateEntity(
            //objNode.AttachObject()

		}


        private void CreateNewEntity(CombatNode obj)
        {
            Entity objEnt = mSceneMgr.CreateEntity(obj.ID.ToString(), "DeltaShip.mesh");
            SceneNode objNode = mSceneMgr.RootSceneNode.CreateChildSceneNode(obj.ID.ToString());
            float sizex = objEnt.BoundingBox.Size.x;
            float sizey = objEnt.BoundingBox.Size.y;
            float sizez = objEnt.BoundingBox.Size.z;
            var desiredSize = 0.5f;
            float scalex = (desiredSize / sizex);
            float scaley = (desiredSize / sizey);
            float scalez = (desiredSize / sizez);
            float scale = System.Math.Min(System.Math.Min(scalex, scaley), scalez);
            objNode.AttachObject(objEnt);
            objNode.Scale(scale, scale, scale);
            objNode.Scale(10, 10, 10);
        }

		#endregion

        public CombatObject selectedObj()
        {
            return battle.CombatObjects.ToList<CombatObject>()[SelectedComObj]; 
        }
        public CombatObject selectNext()
        {
            SelectedComObj++;
            if (SelectedComObj == battle.CombatObjects.Count())
            {
                SelectedComObj = 0;
            }
            return selectedObj();
        }
        public CombatObject selectPrev()
        {
            SelectedComObj--;
            if (SelectedComObj == -1)
            {
                SelectedComObj = battle.CombatObjects.Count()-1;
            }
            return selectedObj();
        }

		private void Go()
		{
			bool running = true;
			
            int battletic = 0;			
			double cmdfreq_countr = 0;

			while (running && mRoot != null && mRoot.RenderOneFrame())
			{
				physicsstopwatch.Restart();
				while (physicsstopwatch.ElapsedMilliseconds < (100 / replaySpeed))
				{
					foreach (CombatObject comObj in battle.CombatObjects)
					{
						Point3d renderloc = battle.InterpolatePosition(comObj, physicsstopwatch.ElapsedMilliseconds / (100f / replaySpeed));
						do_graphics(comObj, renderloc);
					}
                    foreach (CombatNode comNode in battle.CombatNodes) //update bullet and explosion objects.
                    {
                        Point3d renderloc = battle.InterpolatePosition(comNode, physicsstopwatch.ElapsedMilliseconds / (100f / replaySpeed));
                        do_graphics(comNode, renderloc);
                    }
				}


				foreach (CombatObject comObj in battle.CombatObjects.ToArray())
				{
                    comObj.debuginfo = "";
                    //heading and thrust
					battle.helm(comObj);

                    //firecontrol, these get logged, but we still need to run through it
                    //so that prng.next happens.
                    battle.firecontrol(battletic, comObj);

                    //physicsmove objects. 
                    Point3d renderloc = battle.SimNewtonianPhysics(comObj);
					
					do_graphics(comObj, renderloc);
					
                  
                    var ourLogs = battle.ReplayLog.EventsForObjectAtTick(comObj, battletic);
					foreach (var comEvent in ourLogs)
					{
						if (comEvent is CombatFireOnTargetEvent)
						{
							// TODO - if type projectile, create whatever sprite and render it flying towards target.
							// or if type beam, draw a beam sprite
							// seekers should really be their own event type, spawning new combat objects that track enemies
                            CombatFireOnTargetEvent fireEvent = (CombatFireOnTargetEvent)comEvent;
                            var wpninfo = fireEvent.Weapon.weapon.Template.ComponentTemplate.WeaponInfo;
                            //Mogre.Image sprite = ImageConv.ImagetoImage(wpninfo.DisplayEffect.Icon);
                            //sprite.
							if (fireEvent.Weapon.weaponType == "Bolt")
							{
								
								//create an entity for the bullet node.
								double boltTTT = Battle_Space.boltTimeToTarget(fireEvent.Object, fireEvent.Weapon, fireEvent.TakeFireEvent.Object);
								double boltSpeed = Battle_Space.boltClosingSpeed(fireEvent.Object, fireEvent.Weapon, fireEvent.TakeFireEvent.Object);
								double rThis_distance = boltSpeed * boltTTT;
								Point3d bulletVector = Trig.intermediatePoint(fireEvent.Location, fireEvent.TakeFireEvent.Location, rThis_distance);
                                if (!fireEvent.TakeFireEvent.IsHit) //jitter it!
                                {
                                    //double jitterAmount = fireEvent.Weapon.weapon.HitChance; //somethingsomethingsomething... this is backwards. 
                                    //do *NOT* use ship prng here!!!! (since this is not done during normal processing, it'll cause differences, use any rand)
                                    //Point3d jitter = new Point3d(FrEee.Utility.RandomHelper.Range(-jitterAmount, jitterAmount), FrEee.Utility.RandomHelper.Range(-jitterAmount, jitterAmount), FrEee.Utility.RandomHelper.Range(-jitterAmount, jitterAmount)) //aaand the randomHelper takes an int not a double.
                                    //bulletVector += jitter;
                                }
								long id = -battle.CombatNodes.Count - 2; // negative numbers other than -1 aren't used by game objects
								CombatNode bullet = new CombatNode(fireEvent.Location, bulletVector, id);
								battle.CombatNodes.Add(bullet);
								CreateNewEntity(bullet);
							}
                            
						}
                        else if (comEvent is CombatTakeFireEvent)
                        {
                            // TODO - kersplosions
                            CombatTakeFireEvent takefireEvent = (CombatTakeFireEvent)comEvent;

                            if (takefireEvent.fireOnEvent.Weapon.weaponType == "Bolt")
                            {//remove the node, stop rendering. TODO remove this at its TTL?
                                //ie should go past the ship.
                                battle.CombatNodes.Remove(takefireEvent.BulletNode);
                            }
                        }
                        else if (comEvent is CombatDestructionEvent)
                        {
                            // - kersplosions and removal of model. 
                        }
					}
				}
                foreach (CombatNode comNode in battle.CombatNodes) //update bullet and explosion objects.
                {
                    Point3d renderloc = battle.SimNewtonianPhysics(comNode);
                    do_graphics(comNode, renderloc);
                }

                if (cmdfreq_countr >= Battle_Space.CommandFrequency)
                {               
                    foreach (CombatObject comObj in battle.CombatObjects)
                    {   
                        battle.commandAI(comObj);
                        
                    }
                    cmdfreq_countr = 0;
                }

                do_txt();
                cmdfreq_countr++;
                battletic++;
                
				Application.DoEvents();
			}
		}

        private void do_txt()
        {
            CombatObject comObj = selectedObj();
            Game.Objects.Vehicles.Ship ship = (Game.Objects.Vehicles.Ship)comObj.icomobj;

            string txt = ship.Name + "\r\n";
            txt += "Location:\t" 
                + comObj.cmbt_loc.X.ToString() + "\r\n\t" 
                + comObj.cmbt_loc.Y.ToString() + "\r\n\t" 
                + comObj.cmbt_loc.Z.ToString() + "\r\n";
            double speed = Trig.hypotinuse(comObj.cmbt_vel);
            txt += "Speed:\t" + speed.ToString() + "\r\n";
            txt += "Heading:\t" + comObj.cmbt_head.Degrees.ToString() + "\r\n";
           
            txt += "\r\n";
            
            Game.Objects.Vehicles.Ship tgtship = (Game.Objects.Vehicles.Ship)comObj.weaponTarget[0].icomobj;
            txt += "Target:\t" + tgtship.Name + "\r\n";
            txt += "Distance\t" + Trig.hypotinuse(comObj.cmbt_loc - comObj.weaponTarget[0].cmbt_loc) + "\r\n";

            txt += comObj.debuginfo;

            form.updateText(txt);
        }
        private void do_graphics(CombatNode comNode, Point3d renderloc)
		{
            string IDName = comNode.ID.ToString();

            SceneNode node = mSceneMgr.GetSceneNode(IDName);
            
			node.Position = new Vector3((float)renderloc.X, (float)renderloc.Y, (float)renderloc.Z);
            Quaternion quat = new Quaternion((float)comNode.cmbt_head.Radians, Vector3.NEGATIVE_UNIT_Z);
            node.Orientation = quat;

            if (comNode is CombatObject)
            {
                CombatObject comObj = (CombatObject)comNode;

                

                //mSceneMgr.DestroyManualObject("toWaypointLine" + IDName);
                //ManualObject toWaypointLine = mSceneMgr.CreateManualObject("toWaypointLine" + IDName);
                //mNode_lines.AttachObject(toWaypointLine);
                //toWaypointLine.Begin("line_purple", RenderOperation.OperationTypes.OT_LINE_LIST);
                //toWaypointLine.Position(node.Position);
                ////toWaypointLine.Position(new Vector3((float)comObj.waypointTarget.cmbt_loc.X, (float)comObj.waypointTarget.cmbt_loc.Y, (float)comObj.waypointTarget.cmbt_loc.Z));
                //toWaypointLine.Position(TranslateMogrePhys.smVector_mVector3_xyz(comObj.waypointTarget.cmbt_loc));
                //toWaypointLine.End();


                //mSceneMgr.DestroyManualObject("forceLine" + IDName);
                //ManualObject forceLine = mSceneMgr.CreateManualObject("forceLine" + IDName);
                ////forceLine.
                //mNode_lines.AttachObject(forceLine);
                //forceLine.Begin("line_blue", RenderOperation.OperationTypes.OT_LINE_LIST);
                //forceLine.Position(node.Position);
                //forceLine.Position(node.Position + (TranslateMogrePhys.smVector_mVector3_xyz(comObj.cmbt_thrust)));
                //forceLine.End();

                //mSceneMgr.DestroyManualObject("toTargetLine" + IDName);
                //ManualObject toTargetLine = mSceneMgr.CreateManualObject("toTargetLine" + IDName);
                //node.AttachObject(toTargetLine);
                //toTargetLine.Begin("line_yellow", RenderOperation.OperationTypes.OT_LINE_LIST);
                //toTargetLine.Position(new Vector3(0, 0, 0));
                //toTargetLine.Position(new Vector3((float)comObj.waypointTarget.comObj.cmbt_loc.X, (float)comObj.waypointTarget.comObj.cmbt_loc.Y, (float)comObj.waypointTarget.comObj.cmbt_loc.Z));
                //toTargetLine.End();


                //mSceneMgr.DestroyManualObject("forceLine2" + IDName);
                //ManualObject forceLine2 = mSceneMgr.CreateManualObject("forceLine2" + IDName);
                //node.AttachObject(forceLine2);
                //forceLine2.Begin("line_red", RenderOperation.OperationTypes.OT_LINE_LIST);
                //forceLine2.Position(new Vector3(0, 0, 0));
                //forceLine2.Position(forceVec2);
                //forceLine2.End();
                //Console.Out.WriteLine(obj.waypointTarget.comObj.cmbt_loc.ToString());
            }
		}
	}
}