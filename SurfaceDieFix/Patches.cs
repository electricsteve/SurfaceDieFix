using System.Collections;
using System;
using UnityEngine;
using Zorro.Core;

namespace SurfaceDieFix;

public class Patches
{
    internal static void Init()
    {
        On.PhotonGameLobbyHandler.CheckForAllDead += PhotonGameLobbyHandlerOnCheckForAllDead;
        On.Player.RPCA_PlayerDie += PlayerOnRPCA_PlayerDie;
    }

    private static void PlayerOnRPCA_PlayerDie(On.Player.orig_RPCA_PlayerDie orig, Player self)
    {
        orig(self);
        if (!PhotonGameLobbyHandler.IsSurface) return;
        if (self.refs.view.IsMine) Plugin.Instance.StartCoroutine(WaitAndRevive(2.0f));
    }

    private static void PhotonGameLobbyHandlerOnCheckForAllDead(On.PhotonGameLobbyHandler.orig_CheckForAllDead orig, PhotonGameLobbyHandler self)
    {
        if (PhotonGameLobbyHandler.IsSurface)
        {
            Plugin.Logger.LogDebug("Not checking for all dead, is surface. Returning...");
            return;
        }
        orig(self);
    }

    private static IEnumerator WaitAndRevive(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        RetrievableResourceSingleton<TransitionHandler>.Instance.TransitionToBlack(0.0f, (Action) (() =>
        {
            Player.localPlayer.Teleport(Hospital.instance.transform.position, Vector3.back);
            Player.localPlayer.data.fallTime = 0f;
            RetrievableResourceSingleton<TransitionHandler>.Instance.FadeOut((Action) (() => {}), 1f);
        }), 0f);
        yield return new WaitForSeconds(1f);
        Plugin.Logger.LogDebug("Reviving player...");
        Player.localPlayer.CallRevive();
        Plugin.Logger.LogDebug("Healing player...");
        float healAmount = Player.PlayerData.maxHealth - Player.localPlayer.data.health;
        Player.localPlayer.CallHeal(healAmount);
    }
}