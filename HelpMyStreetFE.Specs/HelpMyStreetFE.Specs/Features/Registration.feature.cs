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
    [TechTalk.SpecRun.FeatureAttribute("Registration", new string[] {
            "StartVolunteerBrowser",
            "AcceptAllCookies",
            "Browser_Chrome",
            "Browser_Edge",
            "Browser_Firefox"}, Description="\tRegistration flow", SourceFile="Features\\Registration.feature", SourceLine=5)]
    public partial class RegistrationFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "StartVolunteerBrowser",
                "AcceptAllCookies",
                "Browser_Chrome",
                "Browser_Edge",
                "Browser_Firefox"};
        
#line 1 "Registration.feature"
#line hidden
        
        [TechTalk.SpecRun.FeatureInitialize()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Registration", "\tRegistration flow", ProgrammingLanguage.CSharp, new string[] {
                        "StartVolunteerBrowser",
                        "AcceptAllCookies",
                        "Browser_Chrome",
                        "Browser_Edge",
                        "Browser_Firefox"});
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
#line 9
#line hidden
#line 10
 testRunner.Given("the volunteer\'s url is registration", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
        }
        
        public virtual void InvalidEmailAddresses(string email, string validationError, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("Email", email);
            argumentsOfScenario.Add("Validation error", validationError);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Invalid email addresses", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 12
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
#line 9
this.FeatureBackground();
#line hidden
#line 13
 testRunner.Given(string.Format("the volunteer\'s element #email has value {0}", email), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 14
 testRunner.And("the volunteer\'s element #password has value asd!\"£123ASD", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 15
 testRunner.And("the volunteer\'s element #confirm-password has value asd!\"£123ASD", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 16
 testRunner.And("the volunteer has clicked the element #privacy-and-terms+span", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 17
 testRunner.When("the volunteer clicks the element #submit_button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 18
 testRunner.Then("the volunteer\'s url should be registration", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 19
 testRunner.And(string.Format("if visible, the volunteer\'s element #registration_form .input:first-child span sh" +
                            "ould have text {0}", validationError), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid email addresses, ", SourceLine=22)]
        public virtual void InvalidEmailAddresses_()
        {
#line 12
this.InvalidEmailAddresses("", "Please enter an email address", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid email addresses, abc", SourceLine=22)]
        public virtual void InvalidEmailAddresses_Abc()
        {
#line 12
this.InvalidEmailAddresses("abc", "Please enter a valid email address", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid email addresses, @.com", SourceLine=22)]
        public virtual void InvalidEmailAddresses_Com()
        {
#line 12
this.InvalidEmailAddresses("@.com", "Please enter a valid email address", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid email addresses, a@b.c", SourceLine=22)]
        public virtual void InvalidEmailAddresses_AB_C()
        {
#line 12
this.InvalidEmailAddresses("a@b.c", "Please enter a valid email address", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid email addresses, abc@def.com", SourceLine=22)]
        public virtual void InvalidEmailAddresses_AbcDef_Com()
        {
#line 12
this.InvalidEmailAddresses("abc@def.com", "The email address is already in use by another account.", ((string[])(null)));
#line hidden
        }
        
        public virtual void InvalidPassword(string password, string validationError, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("Password", password);
            argumentsOfScenario.Add("Validation error", validationError);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Invalid password", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 29
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
#line 9
this.FeatureBackground();
#line hidden
#line 30
 testRunner.Given("the volunteer\'s element #email has a new email address", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 31
 testRunner.And(string.Format("the volunteer\'s element #password has value {0}", password), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 32
 testRunner.And(string.Format("the volunteer\'s element #confirm-password has value {0}", password), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 33
 testRunner.And("the volunteer has clicked the element #privacy-and-terms+span", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 34
 testRunner.When("the volunteer clicks the element #submit_button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 35
 testRunner.Then("the volunteer\'s url should be registration", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 36
 testRunner.And(string.Format("the volunteer\'s element #registration_form .input:nth-child(2) span.error should " +
                            "have text {0}", validationError), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid password, ", SourceLine=39)]
        public virtual void InvalidPassword_()
        {
#line 29
this.InvalidPassword("", "Please use a strong password", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid password, a", SourceLine=39)]
        public virtual void InvalidPassword_A()
        {
#line 29
this.InvalidPassword("a", "Please use a strong password", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid password, aA1!bB2\"", SourceLine=39)]
        public virtual void InvalidPassword_AA1BB2()
        {
#line 29
this.InvalidPassword("aA1!bB2\"", "Please use a strong password", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid password, !\"£$%QWERT123456", SourceLine=39)]
        public virtual void InvalidPassword_QWERT123456()
        {
#line 29
this.InvalidPassword("!\"£$%QWERT123456", "Please use a strong password", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid password, !\"£$%qwert123456", SourceLine=39)]
        public virtual void InvalidPassword_Qwert123456()
        {
#line 29
this.InvalidPassword("!\"£$%qwert123456", "Please use a strong password", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid password, !\"£$%qwertQWERTY", SourceLine=39)]
        public virtual void InvalidPassword_QwertQWERTY()
        {
#line 29
this.InvalidPassword("!\"£$%qwertQWERTY", "Please use a strong password", ((string[])(null)));
#line hidden
        }
        
        public virtual void InvalidConfirmPassword(string password, string confirmation, string validationError, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("Password", password);
            argumentsOfScenario.Add("Confirmation", confirmation);
            argumentsOfScenario.Add("Validation error", validationError);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Invalid confirm password", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 47
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
#line 9
this.FeatureBackground();
#line hidden
#line 48
 testRunner.Given("the volunteer\'s element #email has a new email address", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 49
 testRunner.And(string.Format("the volunteer\'s element #password has value {0}", password), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 50
 testRunner.And(string.Format("the volunteer\'s element #confirm-password has value {0}", confirmation), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 51
 testRunner.And("the volunteer has clicked the element #privacy-and-terms+span", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 52
 testRunner.When("the volunteer clicks the element #submit_button", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 53
 testRunner.Then("the volunteer\'s url should be registration", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 54
 testRunner.And(string.Format("the volunteer\'s element #registration_form .input:nth-child(3) span.error should " +
                            "have text {0}", validationError), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Invalid confirm password, abcABC123!\"£", SourceLine=57)]
        public virtual void InvalidConfirmPassword_AbcABC123()
        {
#line 47
this.InvalidConfirmPassword("abcABC123!\"£", "abcABC123!\"£999", "Please ensure passwords match", ((string[])(null)));
#line hidden
        }
    }
}
#pragma warning restore
#endregion
