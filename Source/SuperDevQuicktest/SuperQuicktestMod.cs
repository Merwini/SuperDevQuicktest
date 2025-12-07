using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
using RimWorld.Planet;

namespace SuperDevQuicktest
{
    public class SuperQuicktestMod : Mod
    {
        public static SuperQuicktestSettings settings;

        public SuperQuicktestMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<SuperQuicktestSettings>();
        }

        public override string SettingsCategory() => "Super Dev Quicktest";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard();
            list.Begin(inRect);

            // Scenario
            List<Scenario> allScenarios = QuicktestUtilities.AllAvailableScenarios();
            string currentScenarioLabel = allScenarios.FirstOrDefault(s => s.name == settings.selectedScenario)?.name ?? settings.selectedScenario ?? "(none)";

            if (list.ButtonTextLabeled("Scenario", currentScenarioLabel))
            {
                FloatMenuUtility.MakeMenu(
                    allScenarios,
                    scen => scen.name,
                    scen => (Action)(() =>
                    {
                        settings.selectedScenario = scen.name;
                    })
                );
            }

            // Biome
            List<BiomeDef> biomes = QuicktestUtilities.AllowedBiomes(settings);
            string currentBiomeLabel = QuicktestUtilities.BiomeDefByName(settings.selectedBiome)?.label ?? "(none)";
            if (list.ButtonTextLabeled("Biome", currentBiomeLabel))
            {
                FloatMenuUtility.MakeMenu<BiomeDef>(
                    biomes,
                    b => b.label,
                    b => (Action)(() => { settings.selectedBiome = b.defName; })
                );
            }

            // Hilliness
            if (list.ButtonTextLabeled("Terrain", settings.hillinessSelection.ToString()))
            {
                List<FloatMenuOption> hills = new List<FloatMenuOption>();

                foreach (Hilliness hill in Enum.GetValues(typeof(Hilliness)))
                {
                    if (hill != Hilliness.Undefined)
                    {
                        hills.Add(new FloatMenuOption(hill.ToString(),
                            () => settings.hillinessSelection = hill));
                    }
                }

                Find.WindowStack.Add(new FloatMenu(hills));
            }

            // Storyteller
            var storytellers = DefDatabase<StorytellerDef>.AllDefsListForReading;
            string storytellerLabel = QuicktestUtilities.StorytellerDefByName(settings.selectedStoryteller)?.label ?? "(none)";
            if (list.ButtonTextLabeled("Storyteller", storytellerLabel))
            {
                FloatMenuUtility.MakeMenu<StorytellerDef>(
                    storytellers,
                    st => st.label,
                    st => (Action)(() => settings.selectedStoryteller = st.defName)
                );
            }

            // Map size
            list.Label($"Map Size: {settings.mapSize}");
            float newMapSizeFloat = list.Slider(settings.mapSize, 50f, 500f);
            settings.mapSize = Mathf.RoundToInt(newMapSizeFloat);

            // World coverage
            int coverageInt = Mathf.RoundToInt(settings.worldMapCoverage * 100f);
            list.Label($"World map coverage: {settings.worldMapCoverage * 100}%");
            coverageInt = (int)list.Slider(coverageInt, 5, 100);
            settings.worldMapCoverage = coverageInt / 100f;

            // TODO Ideo? Might need separate assembly

            list.End();

            settings.Write();
        }
    }
}
