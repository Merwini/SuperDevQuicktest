using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SuperDevQuicktest
{
    public class HarmonyPatches
    {
        [StaticConstructorOnStartup]
        public static class DevQuicktestPatch
        {
            static DevQuicktestPatch()
            {
                Harmony harmony = new Harmony("nuff.rimworld.superquicktest");
                harmony.PatchAll();
            }

            [HarmonyPatch(typeof(Root_Play))]
            [HarmonyPatch(nameof(Root_Play.SetupForQuickTestPlay))]
            public static class Root_play_SetupForQuickTestPlay
            {
                public static bool Prefix()
                {
                    Current.ProgramState = ProgramState.Entry;
                    Game.ClearCaches();
                    Current.Game = new Game();

                    SuperQuicktestSettings settings = SuperQuicktestMod.settings;

                    Current.Game.InitData = new GameInitData();

                    Scenario scen = QuicktestUtilities.ResolveScenario(settings.selectedScenario);
                    Current.Game.Scenario = scen;
                    Find.Scenario.PreConfigure();

                    StorytellerDef storytellerDef = QuicktestUtilities.StorytellerDefByName(settings.selectedStoryteller) ?? StorytellerDefOf.Cassandra;
                    DifficultyDef difficultyDef = DifficultyDefOf.Rough; // TODO customize
                    Current.Game.storyteller = new Storyteller(storytellerDef, difficultyDef);

                    float worldMapCoverage = settings.worldMapCoverage;

                    Current.Game.World = WorldGenerator.GenerateWorld(worldMapCoverage, GenText.RandomSeedString(), OverallRainfall.Normal, OverallTemperature.Normal, OverallPopulation.Normal, LandmarkDensity.Normal);

                    BiomeDef biome = QuicktestUtilities.BiomeDefByName(settings.selectedBiome);
                    Hilliness hill = settings.hillinessSelection;
                    Find.GameInitData.startingTile = QuicktestUtilities.FindOrCreateTile(biome, hill);

                    int mapSize = settings.mapSize;
                    Find.GameInitData.mapSize = mapSize;

                    Find.Scenario.PostIdeoChosen(); // TODO customize,

                    return false;
                }
                
            }
        }
    }
}
