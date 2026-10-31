using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SuperDevQuicktest;

public class SuperQuicktestSettings : ModSettings
{
    public string selectedScenario = "Crashlanded";
    public string selectedBiome = "TemperateForest";
    public int mapSize = 250;
    public string selectedStoryteller = "Cassandra";
    public float worldMapCoverage = 0.05f;
    public Hilliness hillinessSelection = Hilliness.Flat;
    public int startHour = 12;

    public static bool preventIncapableOfViolence = false;

    public static bool disableNeedMood = false;
    public static bool disableNeedFood = false;
    public static bool disableNeedRest = false;
    public static bool disableNeedJoy = false;
    public static bool disableNeedBeauty = false;
    public static bool disableNeedComfort = false;

    public bool startedGameWithQuicktest = false;

    public List<string> biomeBlackList = new List<string>
    {
        "MetalHell",
        "Undercave",
        "Space",
        "Orbit",
        "Ocean",
        "Underground",
        "Labyrinth"
    };

    public override void ExposeData()
    {
        Scribe_Values.Look(ref selectedScenario, "selectedScenario", "Crashlanded");
        Scribe_Values.Look(ref selectedBiome, "selectedBiome", "TemperateForest");
        Scribe_Values.Look(ref mapSize, "mapSize", 250);
        Scribe_Values.Look(ref selectedStoryteller, "selectedStoryteller", "Cassandra");
        Scribe_Values.Look(ref worldMapCoverage, "worldMapCoverage", 0.05f);
        Scribe_Values.Look(ref hillinessSelection, "hillinessSelection", Hilliness.Flat);
        Scribe_Values.Look(ref startHour, "startHour", 12);

        Scribe_Values.Look(ref preventIncapableOfViolence, "preventIncapableOfViolence", false);

        Scribe_Values.Look(ref disableNeedMood, "disableNeedMood" , false);
        Scribe_Values.Look(ref disableNeedFood, "disableNeedFood" , false);
        Scribe_Values.Look(ref disableNeedRest, "disableNeedRest" , false);
        Scribe_Values.Look(ref disableNeedJoy, "disableNeedJoy" , false);
        Scribe_Values.Look(ref disableNeedBeauty, "disableNeedBeauty" , false);
        Scribe_Values.Look(ref disableNeedComfort, "disableNeedComfort" , false);
        
        base.ExposeData();
    }
}
