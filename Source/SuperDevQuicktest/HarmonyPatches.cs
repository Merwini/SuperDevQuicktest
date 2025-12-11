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

            [HarmonyPatch(typeof(Root_Play), nameof(Root_Play.SetupForQuickTestPlay))]
            public static class Root_Play_SetupForQuickTestPlay
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
                    if (biome == null)
                    {
                        Log.Warning($"Failed to find biome with name {settings.selectedBiome}. Resetting to Temperate Forest");
                        settings.selectedBiome = "TemperateForest";
                        biome = QuicktestUtilities.BiomeDefByName(settings.selectedBiome);
                    }

                    Hilliness hill = settings.hillinessSelection;
                    Find.GameInitData.startingTile = QuicktestUtilities.FindOrCreateTile(biome, hill);

                    int mapSize = settings.mapSize;
                    Find.GameInitData.mapSize = mapSize;

                    Find.Scenario.PostIdeoChosen(); // TODO customize,

                    settings.startedGameWithQuicktest = true;

                    return false;
                }
            }

            [HarmonyPatch(typeof(Game), nameof(Game.FinalizeInit))]
            public static class Game_FinalizeInit
            {
                public static void Postfix()
                {
                    SuperQuicktestSettings settings = SuperQuicktestMod.settings;

                    if (!settings.startedGameWithQuicktest)
                        return;

                    int desiredHour = settings.startHour;
                    if (desiredHour < 0 || desiredHour > 23)
                    {
                        desiredHour = 6;
                    }

                    int offsetHour = (desiredHour - 6 + 24) % 24;
                    int targetTicks = offsetHour * 2500;

                    Find.TickManager.DebugSetTicksGame(targetTicks);

                    settings.startedGameWithQuicktest = false;
                }
            }

            // Hopefully catches any edge cases where player tries to Dev Quicktest and it fails back to the main menu so the flag is not consumed
            [HarmonyPatch(typeof(MainMenuDrawer), nameof(MainMenuDrawer.Init))]
            public static class MainMenuDrawer_Init
            {
                public static void Postfix()
                {
                    SuperQuicktestSettings settings = SuperQuicktestMod.settings;

                    settings.startedGameWithQuicktest = false;
                }
            }
        }
    }
}
