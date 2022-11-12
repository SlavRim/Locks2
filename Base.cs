using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Locks2.Core;
using UnityEngine;
using Verse;
using static Locks2.Core.LockConfig;

namespace Locks2
{
    [StaticConstructorOnStartup]
    public class Base : Mod
    {
        private readonly Settings settings;

        public Base(ModContentPack content) : base(content)
        {
            settings = Finder.settings = GetSettings<Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            //var leftStandard = new Listing_Standard();
            //var rightStandard = new Listing_Standard();
            //var font = Text.Font;
            //Text.Font = GameFont.Tiny;
            //var leftRect = inRect.LeftPart(0.45f);
            //var rightRect = inRect.RightPart(0.45f);
            //{
            //    leftStandard.Begin(leftRect);
            //    leftStandard.Gap();
            //    Text.Font = GameFont.Tiny;
            //    leftStandard.Label("Locks2SettingsPerformance".Translate());
            //    {
            //        var section = leftStandard.BeginSection_NewTemp(50);
            //        Text.Font = GameFont.Tiny;
            //        section.Label("Locks2SettingsCache".Translate() + " " + settings.cacheTimeOut,
            //            tooltip: "Locks2SettingsCacheTooltip".Translate());
            //        settings.cacheTimeOut = (int) section.Slider(settings.cacheTimeOut, 60, 5000);
            //        section.EndSection(section);
            //    }
            //    leftStandard.Gap();
            //    Text.Font = GameFont.Tiny;
            //    leftStandard.Label("Locks2SettingsAccessTab".Translate());
            //    {
            //        var section = leftStandard.BeginSection_NewTemp(95);
            //        Text.Font = GameFont.Tiny;
            //        section.Label("Locks2SettingsAccessTabX".Translate() + " " + Finder.settings.tabSizeX);
            //        Finder.settings.tabSizeX = (int) section.Slider(Finder.settings.tabSizeX, 200, 500);
            //        section.Gap(3);

            //        section.Label("Locks2SettingsAccessTabY".Translate() + " " + Finder.settings.tabSizeY);
            //        Finder.settings.tabSizeY = (int) section.Slider(Finder.settings.tabSizeY, 200, 700);
            //        section.EndSection(section);
            //    }
            //    leftStandard.Gap();
            //    Text.Font = GameFont.Tiny;
            //    leftStandard.Label("Locks2SettingsDefaults".Translate());
            //    {
            //        var section = leftStandard.BeginSection_NewTemp(settings.defaultRules.Count * 35 + 50);
            //        Text.Font = GameFont.Tiny;
            //        section.Label("Locks2SettingsDefaultsAre".Translate());
            //        var removalRules = new List<Type>();
            //        foreach (var type in settings.defaultRules)
            //            if (section.ButtonText(type.Name.Translate()))
            //            {
            //                removalRules.Add(type);
            //                break;
            //            }

            //        foreach (var type in removalRules) settings.defaultRules.Remove(type);
            //        if (settings.defaultRules.Count <= 4 && section.ButtonText("+"))
            //            Find.WindowStack.Add(new Selector_RuleSelection(rule =>
            //                settings.defaultRules.Add(rule.GetType())));
            //        section.EndSection(section);
            //    }

            //    leftStandard.End();
            //}
            //{
            //    rightStandard.Begin(rightRect);
            //    Text.Font = GameFont.Tiny;
            //    rightStandard.Label("Locks2SettingsOthers".Translate());
            //    if (rightStandard.ButtonText("Locks2SettingReset".Translate()))
            //        Find.WindowStack.Add(new Window_Confirm(() => { ResetSettings(); },
            //            () => { Log.Warning("LOCKS2: Action not confirmed"); }));
            //    rightStandard.Gap();
            //    {
            //        var section = rightStandard.BeginSection_NewTemp(50);
            //        Text.Font = GameFont.Tiny;
            //        section.CheckboxLabeled("Locks2SettingsEnableDebugging".Translate(), ref Finder.debug,
            //            "Locks2SettingsDebuggingTooltip".Translate());
            //        rightStandard.EndSection(rightStandard);
            //    }
            //    rightStandard.Gap();
            //    if (Finder.debug)
            //    {
            //        var section = rightStandard.BeginSection_NewTemp(400);
            //        Text.Font = GameFont.Small;
            //        section.Label("Locks2SettingsDebuggingWarning".Translate());
            //        section.Gap(5.0f);
            //        Text.Font = GameFont.Tiny;
            //        FillDebugSettings(section);
            //        rightStandard.EndSection(rightStandard);
            //    }

            //    rightStandard.End();
            //}
            //Text.Font = font;
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            settings.Write();
        }

        public override string SettingsCategory()
        {
            return "Locks 2";
        }

        private void FillDebugSettings(Listing_Standard standard)
        {
        }

        private void ResetSettings()
        {
            settings.tabSizeX = 433;
            settings.tabSizeY = 418;
            settings.cacheTimeOut = 1800;
            settings.defaultRules = new List<Type>
            {
                typeof(ConfigRuleAnimals),
                typeof(ConfigRuleColonists),
                typeof(ConfigRuleGuests),
                typeof(ConfigRuleIgnorDrafted)
            };
            Finder.debug = false;
            WriteSettings();
        }
    }

    public class Settings : ModSettings
    {
        public int cacheTimeOut = 1800;

        public List<Type> defaultRules = new List<Type>();
        public int tabSizeX = 433;
        public int tabSizeY = 650;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref cacheTimeOut, "cacheTimeOut", 1800, true);
            Scribe_Values.Look(ref tabSizeX, "tabSizeX", 433);
            Scribe_Values.Look(ref tabSizeY, "tabSizeY", 418);
            Scribe_Values.Look(ref Finder.debug, "debug");
            ExposeDefaultRules();
        }

        private void ExposeDefaultRules()
        {
            var types = defaultRules.Select(t => t.Name).ToList();
            Scribe_Collections.Look(ref types, "defaultRules", LookMode.Value);
            if (types != null) defaultRules = types.Select(t => AccessTools.TypeByName(t)).ToList();
        }
    }
}