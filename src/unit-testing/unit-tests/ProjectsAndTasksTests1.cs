﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SemWeb;
using UnitTests.Properties;
using System.IO;
using LinqToRdf;
using SemWeb.Inference;
using System.Data.Linq;
using UnitTests.TaskEntityModel;

namespace UnitTests
{
    namespace TaskEntityModel
    {
        [OwlResource(OntologyName = "Tasks", RelativeUriReference = "Project")]
        public class Project : OwlInstanceSupertype
        {
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "name")]
            public string Name { get; set; }
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "hasTask")]
            public EntitySet<Task> Tasks { get; set; }
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "usesResource")]
            public EntitySet<ProjectResource> ResourcesUsed { get; set; }
        }

        [OwlResource(OntologyName = "Tasks", RelativeUriReference = "Task")]
        public class Task : OwlInstanceSupertype
        {
            //:name rdfs:domain :Task; rdfs:range xsdt:string.
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "name")]
            public string Name { get; set; }
            //:inProject rdfs:domain :Task; rdfs:range :Project.
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "inProject")]
            public EntityRef<Project> Project { get; set; }
            //:requiresResource rdfs:domain :Task; rdfs:range :Resource.
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "requiresResource")]
            public EntityRef<ProjectResource> RequiresResource { get; set; }
            //:description rdfs:domain :Task; rdfs:range xsdt:string.
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "description")]
            public string Description { get; set; }
            //:duration rdfs:domain :Task; rdfs:range xsdt:duration.
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "duration")]
            public TimeSpan duration { get; set; }
            //:startDate rdfs:domain :Task; rdfs:range xsdt:date.
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "startDate")]
            public DateTime StartDate { get; set; }
            //:endDate rdfs:domain :Task; rdfs:range xsdt:date.
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "endDate")]
            public DateTime EndDate { get; set; }
            //:predecessor a daml:TransitiveProperty; rdfs:domain :Task; rdfs:range :Task.
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "predecessor")]
            public EntitySet<Task> Predecessors { get; set; }
            //:successor daml:inverseOf :predecessor.
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "successor")]
            public EntitySet<Task> Successors { get; set; }
            //:isConcurrentWith a owl:SymmetricProperty; rdfs:domain :Task; rdfs:range :Task.
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "isConcurrentWith")]
            public EntitySet<Task> ConcurrentTasks { get; set; }
        }

        [OwlResource(OntologyName = "Tasks", RelativeUriReference = "Resource")]
        public class ProjectResource : OwlInstanceSupertype
        {
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "name")]
            public string Name { get; set; }
            [OwlResource(OntologyName = "Tasks", RelativeUriReference = "email")]
            public string Email { get; set; }
        }
    }
    [TestFixture]
    public class ProjectsAndTasksTests1
    {
        #region housekeeping
        protected static TripleStore store;

        public ProjectsAndTasksTests1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        //
        // Use TestInitialize to run code before running each test 
        [SetUp]
        public void MyTestInitialize() 
        {
            CreateSparqlTripleStore();
        }
        //
        // Use TestCleanup to run code after each test has run
       [TearDown]
        public void MyTestCleanup() { }
        //

        internal TripleStore CreateSparqlTripleStore()
        {
            string tasksOntology = Path.Combine(Settings.Default.taskStorePath, Settings.Default.tasksOntologyName);
            string tasks = Path.Combine(Settings.Default.taskStorePath, Settings.Default.tasksDataName);
            MemoryStore store = new MemoryStore();
            store.AddReasoner(new Euler(new N3Reader(tasksOntology)));
            //store.Import(new N3Reader(tasksOntology));
            store.Import(new N3Reader(tasks));
            return new TripleStore(store);
        }
        #endregion

        [Test]
        public void TestGetAllTasks()
        {
            RDF ctx = new RDF(CreateSparqlTripleStore());
            var q = (from t in ctx.ForType<Task>()
                    select t).Distinct();
            List<Task> l = q.ToList();
            Assert.IsTrue(l.Count == 12);
        }
    }
}
