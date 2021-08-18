﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace HelpMyStreetFE.Specs.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [TechTalk.SpecRun.FeatureAttribute("RequestHelp", new string[] {
            "StartAtHomePage",
            "AcceptAllCookies"}, Description="\tRequest Help form", SourceFile="Features\\RequestHelp.feature", SourceLine=2)]
    public partial class RequestHelpFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "StartAtHomePage",
                "AcceptAllCookies"};
        
#line 1 "RequestHelp.feature"
#line hidden
        
        [TechTalk.SpecRun.FeatureInitialize()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "RequestHelp", "\tRequest Help form", ProgrammingLanguage.CSharp, new string[] {
                        "StartAtHomePage",
                        "AcceptAllCookies"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [TechTalk.SpecRun.FeatureCleanup()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        [TechTalk.SpecRun.ScenarioCleanup()]
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
#line hidden
#line 7
 testRunner.Given("the url is request-help/", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
        }
        
        public virtual void CorrectActivitiesVisibleOnGenericRequestHelpForm(string index, string supportActivityId, string name, string description, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("index", index);
            argumentsOfScenario.Add("SupportActivityId", supportActivityId);
            argumentsOfScenario.Add("Name", name);
            argumentsOfScenario.Add("Description", description);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Correct activities visible on generic Request Help form", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 9
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
#line 10
 testRunner.Then(string.Format("the element selected by .sm4:nth-child({0}) .tiles__tile should have id #task_{1}" +
                            "", index, supportActivityId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 11
 testRunner.And(string.Format("the element selected by #task_{0} .tiles__tile__content__header should have text " +
                            "{1}", supportActivityId, name), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 12
 testRunner.And(string.Format("the element selected by #task_{0} .tiles__tile__content__description should have " +
                            "text {1}", supportActivityId, description), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Correct activities visible on generic Request Help form, 1", SourceLine=15)]
        public virtual void CorrectActivitiesVisibleOnGenericRequestHelpForm_1()
        {
#line 9
this.CorrectActivitiesVisibleOnGenericRequestHelpForm("1", "1", "Shopping", "Picking up groceries and other essentials (e.g. food, toiletries, household produ" +
                    "cts)", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Correct activities visible on generic Request Help form, 2", SourceLine=15)]
        public virtual void CorrectActivitiesVisibleOnGenericRequestHelpForm_2()
        {
#line 9
this.CorrectActivitiesVisibleOnGenericRequestHelpForm("2", "12", "Face Covering", "Finding someone to provide washable fabric face coverings", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Correct activities visible on generic Request Help form, 3", SourceLine=15)]
        public virtual void CorrectActivitiesVisibleOnGenericRequestHelpForm_3()
        {
#line 9
this.CorrectActivitiesVisibleOnGenericRequestHelpForm("3", "10", "Check In", "Checking that someone is OK", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Correct activities visible on generic Request Help form, 4", SourceLine=15)]
        public virtual void CorrectActivitiesVisibleOnGenericRequestHelpForm_4()
        {
#line 9
this.CorrectActivitiesVisibleOnGenericRequestHelpForm("4", "2", "Prescriptions", "Collecting prescriptions from a local pharmacy", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Correct activities visible on generic Request Help form, 5", SourceLine=15)]
        public virtual void CorrectActivitiesVisibleOnGenericRequestHelpForm_5()
        {
#line 9
this.CorrectActivitiesVisibleOnGenericRequestHelpForm("5", "3", "Errands", "Running essential local errands (e.g. posting mail)", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Correct activities visible on generic Request Help form, 6", SourceLine=15)]
        public virtual void CorrectActivitiesVisibleOnGenericRequestHelpForm_6()
        {
#line 9
this.CorrectActivitiesVisibleOnGenericRequestHelpForm("6", "6", "Prepared Meal", "Getting a hot / pre-prepared meal", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Correct activities visible on generic Request Help form, 7", SourceLine=15)]
        public virtual void CorrectActivitiesVisibleOnGenericRequestHelpForm_7()
        {
#line 9
this.CorrectActivitiesVisibleOnGenericRequestHelpForm("7", "7", "Friendly Chat", "A friendly chat on the phone", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Correct activities visible on generic Request Help form, 8", SourceLine=15)]
        public virtual void CorrectActivitiesVisibleOnGenericRequestHelpForm_8()
        {
#line 9
this.CorrectActivitiesVisibleOnGenericRequestHelpForm("8", "9", "Homework", "Remote support for children being home-schooled", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Correct activities visible on generic Request Help form, 9", SourceLine=15)]
        public virtual void CorrectActivitiesVisibleOnGenericRequestHelpForm_9()
        {
#line 9
this.CorrectActivitiesVisibleOnGenericRequestHelpForm("9", "11", "Other", "Please tell us more below", ((string[])(null)));
#line hidden
        }
    }
}
#pragma warning restore
#endregion
