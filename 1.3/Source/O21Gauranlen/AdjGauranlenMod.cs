using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Anima
{
    public class AdjGauranlenMod : Mod
    {
        public static AdjGauranlenMod mod;
        public static AdjGauranlenSettings settings;

        public AdjGauranlenPage currentPage = AdjGauranlenPage.Gauranlen_Tree;

        public AdjGauranlenMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<AdjGauranlenSettings>();
            mod = this;
        }

        public override string SettingsCategory()
        {
            return "Adjustable Gauranlen";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            float secondStageHeight;
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.ValueLabeled("Settings Page", "Cycle this setting to change page.", ref currentPage);
            listingStandard.GapLine();
            listingStandard.Gap(38);
            secondStageHeight = listingStandard.CurHeight;
            listingStandard.End();

            Listing_Standard ls = new Listing_Standard();
            inRect.yMin = secondStageHeight;
            ls.Begin(inRect);
            if(currentPage == AdjGauranlenPage.Gauranlen_Tree)
            {
                ls.Label("Initial Connection Range", tooltip: "Initial connection strength will be in this range.");
                ls.Label("Default: 0.25-0.45, Min-Max: 0 - 1, Current: " + settings.initialConnectionStrengthRange.min.ToString("0.00") + "-" + settings.initialConnectionStrengthRange.max.ToString("0.00"));
                settings.initialConnectionStrengthRange.min = ls.Slider(settings.initialConnectionStrengthRange.min, 0.01f, 1.00f);
                settings.initialConnectionStrengthRange.max = ls.Slider(settings.initialConnectionStrengthRange.max, 0.01f, 1.00f);
                ls.Label("Artificial Building Radius", tooltip: "The max radius of the tree where it detects artificial buildings.");
                ls.Label("Default: 7.6, Min-Max: 0.1 - 40, Current: " + settings.tree_artificialBuildingRadius.ToString("0.0"));
                settings.tree_artificialBuildingRadius = ls.Slider(settings.tree_artificialBuildingRadius, 0.1f, 40f);
                ls.GapLine();
                ls.Label("Moss Growth Radius", tooltip: "The max radius of the tree where it grows moss.");
                ls.Label("Default: 7.6, Min-Max: 0.1 - 40, Current: " + settings.tree_plantGrowthRadius.ToString("0.0"));
                settings.tree_plantGrowthRadius = ls.Slider(settings.tree_plantGrowthRadius, 0.1f, 40f);
                ls.GapLine();
                ls.Label("Connection Gain While Puning", tooltip: "Connection gained per hour of pruning.");
                ls.Label("Default: 0.01, Min-Max: 0.01 - 1, Current: " + settings.tree_connectionGainPerHourPruning.ToString("0.00"));
                settings.tree_connectionGainPerHourPruning = ls.Slider(settings.tree_connectionGainPerHourPruning, 0.01f, 1.00f);
                ls.GapLine();
                ls.Label("Connection Loss Rate", tooltip: "This is the connection loss rate per level of connection, which is usually a curve, but that's complicated as shit so here's a multiplier instead.");
                ls.Label("Default: 1.0, Min-Max: 0 - 2, Current: " + settings.tree_connectionLossPerLevel.ToString("0.0"));
                settings.tree_connectionLossPerLevel = ls.Slider(settings.tree_connectionLossPerLevel, 0f, 2.0f);
                ls.GapLine();
                ls.Label("Connection Loss Per Building", tooltip: "This is the connection loss rate per artificial building, which is usually a curve, but that's complicated as shit so here's a multiplier instead.");
                ls.Label("Default: 1.0, Min-Max: 0 - 2, Current: " + settings.tree_lossPerBuilding.ToString("0.0"));
                settings.tree_lossPerBuilding = ls.Slider(settings.tree_lossPerBuilding, 0f, 2.0f);
            }
            if (currentPage == AdjGauranlenPage.Gauranlen_Dryads)
            {
                ls.Label("Max Dryads", tooltip: "Also a curve so a pain to make settings for, so this is just the max number you get beyond 75% connection strength.");
                ls.Label("Default: 4, Min-Max: 3 - 30, Current: " + settings.tree_maxDryads.ToString("0"));
                settings.tree_maxDryads = Mathf.RoundToInt(ls.Slider(settings.tree_maxDryads, 3, 30));
                ls.GapLine();
                ls.Label("Dryad Spawn Days", tooltip: "Days between spawning of Dryads.");
                ls.Label("Default: 8, Min-Max: 1 - 30, Current: " + settings.tree_dryadSpawnDays.ToString("0"));
                settings.tree_dryadSpawnDays = Mathf.RoundToInt(ls.Slider(settings.tree_dryadSpawnDays, 1, 30));
                ls.Label("Dryad Cocoon Days", tooltip: "Days it takes for a Dryad Cocoon to complete.");
                ls.Label("Default: 6, Min-Max: 1 - 30, Current: " + settings.cocoon_normal_daysToComplete.ToString("0"));
                settings.cocoon_normal_daysToComplete = Mathf.RoundToInt(ls.Slider(settings.cocoon_normal_daysToComplete, 1, 30));
            }
            if (currentPage == AdjGauranlenPage.Gauranlen_Misc)
            {
                ls.Label("Moss Growth Days", tooltip: "Amount of Days till Gauralen Moss grows to maturity.");
                ls.Label("Default: 5, Min-Max: 1 - 30, Current: " + settings.moss_growDays.ToString("0"));
                settings.moss_growDays = Mathf.RoundToInt(ls.Slider(settings.moss_growDays, 1, 30));
                ls.Label("Gaumaker Pod Days", tooltip: "Days it takes for a Gaumaker pod to mature.");
                ls.Label("Default: 4, Min-Max: 1 - 30, Current: " + settings.cocoon_gaumaker_daysToComplete.ToString("0"));
                settings.cocoon_gaumaker_daysToComplete = Mathf.RoundToInt(ls.Slider(settings.cocoon_gaumaker_daysToComplete, 1, 30));
                ls.Label("Seed Yield", tooltip: "Amount of seeds obtained from a Gaumaker Pod");
                ls.Label("Default: 1, Min-Max: 1 - 10, Current: " + settings.pod_harvestYield.ToString("0"));
                settings.pod_harvestYield = Mathf.RoundToInt(ls.Slider(settings.pod_harvestYield, 1, 10));
            }
            ls.End();

            AdjGauranlenStartup.ApplySettingsNow(settings);

            base.DoSettingsWindowContents(inRect);
        }
    }

    public enum AdjGauranlenPage
    {
        Gauranlen_Tree,
        Gauranlen_Dryads,
        Gauranlen_Misc
    }

    public class AdjGauranlenSettings : ModSettings
    {
        // Gauranlen Tree
        public FloatRange initialConnectionStrengthRange = new FloatRange(0.25f, 0.45f);
        public float tree_connectionGainPerHourPruning = 0.01f;
        public int tree_dryadSpawnDays = 8;
        public int tree_maxDryads = 4;
        public float tree_artificialBuildingRadius = 7.9f;
        public float tree_connectionLossPerLevel = 1f;
        public float tree_lossPerBuilding = 1f;
        public float tree_plantGrowthRadius = 7.9f;

        // Gauranlen Misc
        public int moss_growDays = 5;
        public int cocoon_normal_daysToComplete = 6;
        public int cocoon_gaumaker_daysToComplete = 4;
        public int pod_harvestYield = 1;

        public override void ExposeData()
        {
            base.ExposeData();

            // Gauranlen Tree
            Scribe_Values.Look(ref initialConnectionStrengthRange, "initialConnectionStrengthRange");
            Scribe_Values.Look(ref tree_connectionGainPerHourPruning, "tree_connectionGainPerHourPruning");
            Scribe_Values.Look(ref tree_dryadSpawnDays, "tree_dryadSpawnDays");
            Scribe_Values.Look(ref tree_maxDryads, "tree_maxDryads");
            Scribe_Values.Look(ref tree_artificialBuildingRadius, "tree_artificialBuildingRadius");
            Scribe_Values.Look(ref tree_connectionLossPerLevel, "tree_connectionLossPerLevel");
            Scribe_Values.Look(ref tree_lossPerBuilding, "tree_lossPerBuilding");
            Scribe_Values.Look(ref tree_plantGrowthRadius, "tree_plantGrowthRadius");

            // Gauranlen Misc
            Scribe_Values.Look(ref moss_growDays, "moss_growDays");
            Scribe_Values.Look(ref cocoon_normal_daysToComplete, "cocoon_normal_daysToComplete");
            Scribe_Values.Look(ref cocoon_gaumaker_daysToComplete, "cocoon_gaumaker_daysToComplete");
            Scribe_Values.Look(ref pod_harvestYield, "pod_harvestYield");
        }

        public IEnumerable<string> GetEnabledSettings
        {
            get
            {
                return GetType().GetFields().Where(p => p.FieldType == typeof(bool) && (bool)p.GetValue(this)).Select(p => p.Name);
            }
        }
    }

    [StaticConstructorOnStartup]
    public static class AdjGauranlenStartup
    {
        public static SimpleCurve original_maxDryadsPerConnectionStrengthCurve = new SimpleCurve();
        public static SimpleCurve adjusted_maxDryadsPerConnectionStrengthCurve = new SimpleCurve();

        public static SimpleCurve original_connectionLossPerLevelCurve = new SimpleCurve();
        public static SimpleCurve adjusted_connectionLossPerLevelCurve = new SimpleCurve();

        public static SimpleCurve original_connectionLossDailyPerBuildingDistanceCurve = new SimpleCurve();
        public static SimpleCurve adjusted_connectionLossDailyPerBuildingDistanceCurve = new SimpleCurve();

        static AdjGauranlenStartup()
        {
            {
                CompProperties_TreeConnection props = GauranlenDefOf.Plant_TreeGauranlen.GetCompProperties<CompProperties_TreeConnection>();
                original_connectionLossDailyPerBuildingDistanceCurve = props.connectionLossDailyPerBuildingDistanceCurve;
                original_connectionLossPerLevelCurve = props.connectionLossPerLevelCurve;
                original_maxDryadsPerConnectionStrengthCurve = props.maxDryadsPerConnectionStrengthCurve;
            }

            ApplySettingsNow(AdjGauranlenMod.settings);
        }

        public static void ApplySettingsNow(AdjGauranlenSettings s)
        {
            // Deal with the Tree
            {
                {
                    CompProperties_SpawnSubplant props = GauranlenDefOf.Plant_TreeGauranlen.GetCompProperties<CompProperties_SpawnSubplant>();
                    props.maxRadius = s.tree_plantGrowthRadius;
                }
                {
                    CompProperties_TreeConnection props = GauranlenDefOf.Plant_TreeGauranlen.GetCompProperties<CompProperties_TreeConnection>();
                    props.spawnDays = s.tree_dryadSpawnDays;
                    props.radiusToBuildingForConnectionStrengthLoss = s.tree_artificialBuildingRadius;
                    props.connectionStrengthGainPerHourPruningBase = s.tree_connectionGainPerHourPruning;
                    {
                        adjusted_maxDryadsPerConnectionStrengthCurve = new SimpleCurve();
                        adjusted_maxDryadsPerConnectionStrengthCurve.Add(original_maxDryadsPerConnectionStrengthCurve.First());
                        //for (int i = 0; i < original_maxDryadsPerConnectionStrengthCurve.Count(); i++)
                        //{
                        //    adjusted_maxDryadsPerConnectionStrengthCurve.Add(original_maxDryadsPerConnectionStrengthCurve[i]);
                        //}
                        adjusted_maxDryadsPerConnectionStrengthCurve.Add(new CurvePoint(0.76f, s.tree_maxDryads));

                        props.maxDryadsPerConnectionStrengthCurve = adjusted_maxDryadsPerConnectionStrengthCurve;
                    }
                    {
                        adjusted_connectionLossPerLevelCurve = new SimpleCurve();
                        adjusted_connectionLossPerLevelCurve.Add(original_connectionLossPerLevelCurve.First());
                        adjusted_connectionLossPerLevelCurve.Add(new CurvePoint(0.66f, 0.06f * s.tree_connectionLossPerLevel));

                        props.connectionLossPerLevelCurve = adjusted_connectionLossPerLevelCurve;
                    }
                    {
                        adjusted_connectionLossDailyPerBuildingDistanceCurve = new SimpleCurve();
                        adjusted_connectionLossDailyPerBuildingDistanceCurve.Add(new CurvePoint(0, 0.07f * s.tree_lossPerBuilding));
                        adjusted_connectionLossDailyPerBuildingDistanceCurve.Add(new CurvePoint(s.tree_artificialBuildingRadius, 0.01f * s.tree_lossPerBuilding));

                        props.connectionLossDailyPerBuildingDistanceCurve = adjusted_connectionLossDailyPerBuildingDistanceCurve;
                    }
                }
            }

            // Deal with the Moss
            {
                GauranlenDefOf.Plant_MossGauranlen.plant.growDays = s.moss_growDays;
            }

            // Deal with the Pods
            {
                CompProperties_DryadCocoon props = GauranlenDefOf.DryadCocoon.GetCompProperties<CompProperties_DryadCocoon>();
                props.daysToComplete = s.cocoon_normal_daysToComplete;
            }
            {
                CompProperties_DryadCocoon props = GauranlenDefOf.GaumakerCocoon.GetCompProperties<CompProperties_DryadCocoon>();
                props.daysToComplete = s.cocoon_gaumaker_daysToComplete;
            }
            {
                GauranlenDefOf.Plant_PodGauranlen.plant.growDays = s.moss_growDays;
                GauranlenDefOf.Plant_PodGauranlen.plant.harvestYield = s.pod_harvestYield;
            }
        }
    }

    [DefOf]
    public static class GauranlenDefOf
    {
        static GauranlenDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(GauranlenDefOf));
        }

        public static ThingDef Plant_TreeGauranlen;
        public static ThingDef Plant_MossGauranlen;
        public static ThingDef Plant_PodGauranlen;

        public static ThingDef DryadCocoon;
        public static ThingDef GaumakerCocoon;
    }
}
