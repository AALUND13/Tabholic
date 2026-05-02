using BepInEx;

namespace Tabholic {
    [BepInIncompatibility(Tabholic.ModId)]

    [BepInPlugin("com.penial.rounds.Infoholic", "Infoholioc Disabler", "9999.9999.9999")]
    [BepInProcess("Rounds.exe")]
    internal class InfoholiocDisabler : BaseUnityPlugin { }
}
