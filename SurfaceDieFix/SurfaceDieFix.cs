global using Plugin = SurfaceDieFix.SurfaceDieFix;
using BepInEx;
using BepInEx.Logging;
using System.Reflection;
using MonoMod.RuntimeDetour.HookGen;

namespace SurfaceDieFix;

[ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, false)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SurfaceDieFix : BaseUnityPlugin
{
    public static SurfaceDieFix Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;
        Logger.LogDebug("Hooking...");
        Patches.Init();
        Logger.LogDebug("Finished Hooking!");
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    private void OnDestroy()
    {
        Logger.LogDebug("Unhooking...");
        HookEndpointManager.RemoveAllOwnedBy(Assembly.GetExecutingAssembly());
        Logger.LogDebug("Finished Unhooking!");
    }
}