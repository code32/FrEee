﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using FrEee.Utility;
using FrEee.Utility.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FrEee.Tests.Utility.Extensions
{
	/// <summary>
	/// Tests extension methods.
	/// </summary>
	[TestClass]
	public class DataTest
	{
		[TestInitialize]
		public void TestInit()
		{
			barack = new Person("Barack", null, null);
			michelle = new Person("Michelle", null, null);
			malia = new Person("Malia", barack, michelle);
			sasha = new Person("Sasha", barack, michelle);
		}

		private Person barack, michelle, malia, sasha;

		/// <summary>
		/// Tests full-fledged (object oriented) data operations.
		/// </summary>
		[TestMethod]
		public void Data()
		{
			var data = barack.Data;
			Assert.AreEqual(barack.Name, data[nameof(barack.Name)]);
			Assert.AreEqual(barack.Children, data[nameof(barack.Children)]);
			var clone = new Person(null, null, null);
			clone.Data = data;
			Assert.AreEqual(barack.Name, clone.Name);
			Assert.AreEqual(barack.Children, clone.Children); // well, the DNA test would say they're the clone's as well ;)
		}

		/// <summary>
		/// Tests simple (string-only) data operations.
		/// </summary>
		[TestMethod]
		public void SimpleData()
		{
			var simple = new SimpleDataObject<Person>(barack, null);
			Assert.AreEqual(barack.Name, simple.Data[nameof(barack.Name)]);
			Assert.AreEqual(barack.Children, simple.Data[nameof(barack.Children)]);
			var clone = simple.Value;
			Assert.AreEqual(barack.Name, clone.Name);
			Assert.AreEqual(barack.Children, clone.Children); // well, the DNA test would say they're the clone's as well ;)
		}

		/// <summary>
		/// Tests sending simple data over app domain boundaries.
		/// </summary>
		[TestMethod]
		public void AppDomains()
		{
			//Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder 
			//other than the one in which the sandboxer resides.
			AppDomainSetup adSetup = new AppDomainSetup();
			adSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
			adSetup.ApplicationName = "FrEee";
			adSetup.DynamicBase = "ScriptEngine";

			//Setting the permissions for the AppDomain. We give the permission to execute and to 
			//read/discover the location where the untrusted code is loaded.
			var evidence = new Evidence();
			evidence.AddHostEvidence(new Zone(SecurityZone.MyComputer));
			var permissions = SecurityManager.GetStandardSandbox(evidence);
			var reflection = new ReflectionPermission(PermissionState.Unrestricted);
			permissions.AddPermission(reflection);

			//Now we have everything we need to create the AppDomain, so let's create it.
			var sandbox = AppDomain.CreateDomain("Test", null, adSetup, permissions, AppDomain.CurrentDomain.GetAssemblies().Select(a => a.Evidence.GetHostEvidence<StrongName>()).Where(sn => sn != null).ToArray());

			// can we send Barack over?
			sandbox.SetData("data", new SimpleDataObject<Person>(barack));

			// can we make a new person (well, person data) over there and poke him?
			var data = (SimpleDataObject<Person>)sandbox.CreateInstanceAndUnwrap(Assembly.GetAssembly(typeof(SimpleDataObject<Person>)).FullName, typeof(SimpleDataObject<Person>).FullName);
			var nobody = new Person(null, null, null);
			data.Data = nobody.Data;
			nobody.Data = data.Data;
			Assert.AreEqual("Hi, I'm nobody!", nobody.SayHi());
		}

		private class Person : IDataObject
		{
			public Person(string name, Person father, Person mother)
			{
				Name = name;
				Mother = mother;
				Father = father;
				if (Mother != null)
					Mother.Children.Add(this);
				if (Father != null)
					Father.Children.Add(this);
			}

			public string Name { get; set; }

			public Person Mother { get; private set; }

			public Person Father { get; private set; }

			public ISet<Person> Children { get; private set; } = new HashSet<Person>();

			public string SayHi()
			{
				return $"Hi, I'm {Name ?? "nobody"}!";
			}

			public SafeDictionary<string, object> Data
			{
				get
				{
					var dict = new SafeDictionary<string, object>();
					dict[nameof(Name)] = Name;
					dict[nameof(Mother)] = Mother;
					dict[nameof(Father)] = Father;
					dict[nameof(Children)] = Children;
					return dict;
				}

				set
				{
					Name = value[nameof(Name)].Default<string>();
					Mother = value[nameof(Mother)].Default<Person>();
					Father = value[nameof(Father)].Default<Person>();
					Children = value[nameof(Children)].Default(new HashSet<Person>());
				}
			}
		}
	}
}