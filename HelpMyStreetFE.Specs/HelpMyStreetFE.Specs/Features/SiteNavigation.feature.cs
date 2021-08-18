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
    [TechTalk.SpecRun.FeatureAttribute("SiteNavigation", new string[] {
            "StartVolunteerBrowser",
            "AcceptAllCookies"}, Description="\tNavigation using top menu bar on home page", SourceFile="Features\\SiteNavigation.feature", SourceLine=2)]
    public partial class SiteNavigationFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "StartVolunteerBrowser",
                "AcceptAllCookies"};
        
#line 1 "SiteNavigation.feature"
#line hidden
        
        [TechTalk.SpecRun.FeatureInitialize()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "SiteNavigation", "\tNavigation using top menu bar on home page", ProgrammingLanguage.CSharp, new string[] {
                        "StartVolunteerBrowser",
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
        
        public virtual void TopLevelNavigationWideScreen(string name, string index, string url, string pageTitle, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "MaximiseWindows"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("Name", name);
            argumentsOfScenario.Add("index", index);
            argumentsOfScenario.Add("url", url);
            argumentsOfScenario.Add("Page Title", pageTitle);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Top level navigation (wide screen)", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 7
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
#line 8
 testRunner.Given(string.Format("the volunteer\'s element selected by #site-nav ul li:nth-child({0}) a should have " +
                            "text {1}", index, name), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 9
 testRunner.When(string.Format("the volunteer clicks the element selected by #site-nav ul li:nth-child({0}) a", index), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 10
 testRunner.Then(string.Format("the volunteer\'s url should be {0}", url), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 11
 testRunner.And(string.Format("the volunteer\'s page title should be {0}", pageTitle), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Top level navigation (wide screen), Request Help", new string[] {
                "MaximiseWindows"}, SourceLine=14)]
        public virtual void TopLevelNavigationWideScreen_RequestHelp()
        {
#line 7
this.TopLevelNavigationWideScreen("Request Help", "1", "request-help/", "Request Help - Help My Street", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Top level navigation (wide screen), Case Studies", new string[] {
                "MaximiseWindows"}, SourceLine=14)]
        public virtual void TopLevelNavigationWideScreen_CaseStudies()
        {
#line 7
this.TopLevelNavigationWideScreen("Case Studies", "2", "case-studies", "Case Studies - Help My Street", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Top level navigation (wide screen), Resources", new string[] {
                "MaximiseWindows"}, SourceLine=14)]
        public virtual void TopLevelNavigationWideScreen_Resources()
        {
#line 7
this.TopLevelNavigationWideScreen("Resources", "3", "resources", "Resources - Help My Street", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Top level navigation (wide screen), Questions", new string[] {
                "MaximiseWindows"}, SourceLine=14)]
        public virtual void TopLevelNavigationWideScreen_Questions()
        {
#line 7
this.TopLevelNavigationWideScreen("Questions", "4", "questions", "Frequently Asked Questions - Help My Street", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("No further navigation links (wide screen)", new string[] {
                "MaximiseWindows"}, SourceLine=20)]
        public virtual void NoFurtherNavigationLinksWideScreen()
        {
            string[] tagsOfScenario = new string[] {
                    "MaximiseWindows"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("No further navigation links (wide screen)", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 21
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
#line 22
 testRunner.Then("the volunteer\'s element selected by #site-nav ul li:nth-child(5) a should not be " +
                        "visible", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        public virtual void TopLevelNavigationNarrowScreen(string name, string index, string url, string pageTitle, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("Name", name);
            argumentsOfScenario.Add("index", index);
            argumentsOfScenario.Add("url", url);
            argumentsOfScenario.Add("Page Title", pageTitle);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Top level navigation (narrow screen)", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 25
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
#line 26
 testRunner.Given("the volunteer has clicked the element #site-nav-toggle", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 27
 testRunner.And(string.Format("the volunteer\'s element selected by #sitenavCollapsed #site-nav ul li:nth-child({" +
                            "0}) a should have text {1}", index, name), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 28
 testRunner.When(string.Format("the volunteer clicks the element selected by #sitenavCollapsed #site-nav ul li:nt" +
                            "h-child({0}) a", index), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 29
 testRunner.Then(string.Format("the volunteer\'s url should be {0}", url), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 30
 testRunner.And(string.Format("the volunteer\'s page title should be {0}", pageTitle), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Top level navigation (narrow screen), Request Help", SourceLine=33)]
        public virtual void TopLevelNavigationNarrowScreen_RequestHelp()
        {
#line 25
this.TopLevelNavigationNarrowScreen("Request Help", "1", "request-help/", "Request Help - Help My Street", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Top level navigation (narrow screen), Case Studies", SourceLine=33)]
        public virtual void TopLevelNavigationNarrowScreen_CaseStudies()
        {
#line 25
this.TopLevelNavigationNarrowScreen("Case Studies", "2", "case-studies", "Case Studies - Help My Street", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Top level navigation (narrow screen), Resources", SourceLine=33)]
        public virtual void TopLevelNavigationNarrowScreen_Resources()
        {
#line 25
this.TopLevelNavigationNarrowScreen("Resources", "3", "resources", "Resources - Help My Street", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Top level navigation (narrow screen), Questions", SourceLine=33)]
        public virtual void TopLevelNavigationNarrowScreen_Questions()
        {
#line 25
this.TopLevelNavigationNarrowScreen("Questions", "4", "questions", "Frequently Asked Questions - Help My Street", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("No further navigation links (narrow screen)", SourceLine=38)]
        public virtual void NoFurtherNavigationLinksNarrowScreen()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("No further navigation links (narrow screen)", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 39
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
#line 40
 testRunner.Given("the volunteer has clicked the element #site-nav-toggle", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 41
 testRunner.Then("the volunteer\'s element selected by #sitenavCollapsed #site-nav ul li:nth-child(5" +
                        ") a should not be visible", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
