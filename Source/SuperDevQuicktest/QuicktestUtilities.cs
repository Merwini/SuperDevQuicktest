using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SuperDevQuicktest
{
    public static class QuicktestUtilities
    {
        //public static ScenarioDef ScenarioDefByName(string defName)
        //{
        //    if (string.IsNullOrEmpty(defName)) return null;
        //    return DefDatabase<ScenarioDef>.GetNamedSilentFail(defName);
        //}

        public static Scenario ResolveScenario(string name)
        {
            foreach (ScenarioDef def in DefDatabase<ScenarioDef>.AllDefsListForReading)
            {
                if (def.scenario.name == name)
                    return def.scenario;
            }

            foreach (Scenario scen in AllAvailableScenarios())
            {
                if (scen.name == name)
                    return scen;
            }

            return ScenarioDefOf.Crashlanded.scenario;
        }

        public static BiomeDef BiomeDefByName(string defName)
        {
            if (string.IsNullOrEmpty(defName)) return null;
            return DefDatabase<BiomeDef>.GetNamedSilentFail(defName);
        }

        public static List<BiomeDef> AllowedBiomes(SuperQuicktestSettings settings)
        {
            return DefDatabase<BiomeDef>.AllDefs
                .Where(b => !settings.biomeBlackList.Contains(b.defName))
                .OrderBy(b => b.label).ToList();
        }

        public static StorytellerDef StorytellerDefByName(string defName)
        {
            if (string.IsNullOrEmpty(defName)) return null;
            return DefDatabase<StorytellerDef>.GetNamedSilentFail(defName);
        }

        //public static int FindTileForBiome(BiomeDef biome)
        //{
        //    WorldGrid grid = Find.WorldGrid;
        //    int tileCount = grid.TilesCount;

        //    for (int i = 0; i < tileCount; i++)
        //    {
        //        if (grid[i].biome == biome)
        //        {
        //            return i;
        //        }
        //    }

        //    Log.Error($"No world tile found with biome {biome.label}");
        //    return TileFinder.RandomSettlementTileFor(Faction.OfPlayer);
        //}

        public static int FindOrCreateTile(BiomeDef biome, Hilliness hill)
        {
            WorldGrid grid = Find.WorldGrid;
            int tileCount = grid.TilesCount;

            // Try to find tile of correct biome and hilliness
            for (int i = 0; i < tileCount; i++)
            {
                Tile t = grid[i];
                if (t.biome == biome && t.hilliness == hill && !t.WaterCovered)
                {
                    return i;
                }
            }

            // Failing that, try to find tile of just correct biome and mutate it
            for (int i = 0; i < tileCount; i++)
            {
                Tile t = grid[i];
                if (t.biome == biome && !t.WaterCovered)
                {
                    t.hilliness = hill;
                    return i;
                }
            }

            // Failing that, find a random tile and try to mutate both
            int fallback = TileFinder.RandomSettlementTileFor(Faction.OfPlayer);
            Tile tileFallback = grid[fallback];
            tileFallback.biome = biome;
            tileFallback.hilliness = hill;

            Log.Warning($"SuperQuicktest: Could not find tile with biome {biome.defName} " +
                        $"and hilliness {hill}; mutated tile #{fallback}.");

            return fallback;
        }

        public static List<Scenario> AllAvailableScenarios()
        {
            List<Scenario> result = new List<Scenario>();

            // ScenarioDefs
            foreach (var scenDef in DefDatabase<ScenarioDef>.AllDefsListForReading)
            {
                if (scenDef?.scenario != null)
                {
                    Scenario scenCopy = scenDef.scenario;
                    scenCopy.fileName = scenDef.defName;
                    result.Add(scenCopy);
                }
            }

            // Custom scenarios
            string saveDataPath = GenFilePaths.SaveDataFolderPath;
            string scenarioFolderPath = Path.Combine(saveDataPath, "Scenarios");
            if (Directory.Exists(scenarioFolderPath))
            {
                foreach (string file in Directory.GetFiles(scenarioFolderPath, "*.rsc"))
                {
                    try
                    {
                        GameDataSaveLoader.TryLoadScenario(file, ScenarioCategory.CustomLocal, out Scenario scen);

                        if (scen != null)
                        {
                            scen.fileName = Path.GetFileNameWithoutExtension(file);

                            if (string.IsNullOrEmpty(scen.name))
                                scen.name = scen.fileName;

                            result.Add(scen);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning($"SuperQuicktest: Failed to load scenario file named '{file}': {e}");
                    }
                }
            }

            return result;
        }
    }
}
