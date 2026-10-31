using RimWorld;

namespace SuperDevQuicktest;

[DefOf]
public static class SDQ_NeedDefOf
{
    public static NeedDef Mood;

    // Food in vanilla

    // Rest in vanilla

    public static NeedDef Joy;

    public static NeedDef Beauty;

    public static NeedDef Comfort;

    static SDQ_NeedDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(SDQ_NeedDefOf));
    }
}
