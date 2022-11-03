namespace Service;

public static class TriggersManager
{
    static private List<Trigger> Triggers;

    static TriggersManager()
    {
        Triggers = new();

        Triggers.Add(
            new Trigger("JS", "<script>evil_script()</script>", new string[] {".js"}));
        Triggers.Add(new Trigger("rm -rf", "rm -rf %userprofile%\\Documents", new string[] { }));
        Triggers.Add(new Trigger("RunDll32", "Rundll32 sus.dll SusEntry", new string[] { }));
    }

    public static List<Trigger> GetNewTriggers()
    {
        return Triggers.Select(x => (Trigger) x.Clone()).ToList();
    }
}