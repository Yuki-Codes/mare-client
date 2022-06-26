﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using MareSynchronos.Models;
using MareSynchronos.Utils;
using MareSynchronos.WebAPI;

namespace MareSynchronos.Managers;

public class CachedPlayersManager : IDisposable
{
    private readonly ApiController _apiController;
    private readonly ClientState _clientState;
    private readonly DalamudUtil _dalamudUtil;
    private readonly Framework _framework;
    private readonly IpcManager _ipcManager;
    private readonly List<CachedPlayer> _onlineCachedPlayers = new();
    private readonly List<string> _localVisiblePlayers = new();
    private DateTime _lastPlayerObjectCheck = DateTime.Now;

    public CachedPlayersManager(ClientState clientState, Framework framework,
        ApiController apiController, DalamudUtil dalamudUtil, IpcManager ipcManager)
    {
        Logger.Debug("Creating " + nameof(CachedPlayersManager));

        _clientState = clientState;
        _framework = framework;
        _apiController = apiController;
        _dalamudUtil = dalamudUtil;
        _ipcManager = ipcManager;

        _clientState.Login += ClientStateOnLogin;
        _clientState.Logout += ClientStateOnLogout;

        _apiController.PairedClientOnline += ApiControllerOnPairedClientOnline;
        _apiController.PairedClientOffline += ApiControllerOnPairedClientOffline;
        _apiController.PairedWithOther += ApiControllerOnPairedWithOther;
        _apiController.UnpairedFromOther += ApiControllerOnUnpairedFromOther;
        _apiController.Disconnected += ApiControllerOnDisconnected;
        _ipcManager.PenumbraDisposed += IpcManagerOnPenumbraDisposed;

        if (clientState.IsLoggedIn)
        {
            ClientStateOnLogin(null, EventArgs.Empty);
        }
    }
    
    private void IpcManagerOnPenumbraDisposed(object? sender, EventArgs e)
    {
        _onlineCachedPlayers.ForEach(p => p.DisposePlayer());
    }

    private void ApiControllerOnDisconnected(object? sender, EventArgs e)
    {
        RestoreAllCharacters();
    }

    private void ClientStateOnLogin(object? sender, EventArgs e)
    {
        _framework.Update += FrameworkOnUpdate;
    }

    private void ClientStateOnLogout(object? sender, EventArgs e)
    {
        _framework.Update -= FrameworkOnUpdate;
    }

    public void AddInitialPairs(List<string> apiTaskResult)
    {
        _onlineCachedPlayers.Clear();
        _onlineCachedPlayers.AddRange(apiTaskResult.Select(CreateCachedPlayer));
        Logger.Debug("Online and paired users: " + string.Join(",", _onlineCachedPlayers));
    }

    public void Dispose()
    {
        Logger.Debug("Disposing " + nameof(CachedPlayersManager));

        RestoreAllCharacters();

        _apiController.PairedClientOnline -= ApiControllerOnPairedClientOnline;
        _apiController.PairedClientOffline -= ApiControllerOnPairedClientOffline;
        _apiController.PairedWithOther -= ApiControllerOnPairedWithOther;
        _apiController.UnpairedFromOther -= ApiControllerOnUnpairedFromOther;
        _ipcManager.PenumbraDisposed -= ApiControllerOnDisconnected;
        _framework.Update -= FrameworkOnUpdate;
        _clientState.Login -= ClientStateOnLogin;
        _clientState.Logout -= ClientStateOnLogout;
    }

    private void RestoreAllCharacters()
    {
        _onlineCachedPlayers.ForEach(p => p.DisposePlayer());
        _onlineCachedPlayers.Clear();
    }

    public async Task UpdatePlayersFromService(Dictionary<string, int> playerJobIds)
    {
        if (!playerJobIds.Any()) return;
        Logger.Debug("Getting data for new players: " + string.Join(Environment.NewLine, playerJobIds));
        await _apiController.GetCharacterData(playerJobIds);
    }

    private void ApiControllerOnPairedClientOffline(object? sender, EventArgs e)
    {
        Logger.Debug("Player offline: " + sender!);
        RemovePlayer((string)sender!);
    }

    private void ApiControllerOnPairedClientOnline(object? sender, EventArgs e)
    {
        Logger.Debug("Player online: " + sender!);
        AddPlayer((string)sender!);
        return;
    }

    private void ApiControllerOnPairedWithOther(object? sender, EventArgs e)
    {
        var characterHash = (string?)sender;
        if (string.IsNullOrEmpty(characterHash)) return;
        Logger.Debug("Pairing with " + characterHash);
        AddPlayer(characterHash);
    }

    private void ApiControllerOnUnpairedFromOther(object? sender, EventArgs e)
    {
        var characterHash = (string?)sender;
        if (string.IsNullOrEmpty(characterHash)) return;
        Logger.Debug("Unpairing from " + characterHash);
        RemovePlayer(characterHash);
    }

    private void AddPlayer(string characterNameHash)
    {
        if (_onlineCachedPlayers.Any(p => p.PlayerNameHash == characterNameHash)) return;
        _onlineCachedPlayers.Add(CreateCachedPlayer(characterNameHash));
    }

    private void RemovePlayer(string characterHash)
    {
        var cachedPlayer = _onlineCachedPlayers.Single(p => p.PlayerNameHash == characterHash);
        cachedPlayer.DisposePlayer();
        _onlineCachedPlayers.Remove(cachedPlayer);
    }

    private void FrameworkOnUpdate(Framework framework)
    {
        if (!_dalamudUtil.IsPlayerPresent || !_ipcManager.Initialized || !_apiController.IsConnected) return;

        if (DateTime.Now < _lastPlayerObjectCheck.AddSeconds(0.25)) return;

        _localVisiblePlayers.Clear();
        var playerCharacters = _dalamudUtil.GetPlayerCharacters();
        foreach (var pChar in playerCharacters)
        {
            var pObjName = pChar.Name.ToString();
            _localVisiblePlayers.Add(pObjName);
            var existingCachedPlayer = _onlineCachedPlayers.SingleOrDefault(p => p.PlayerName == pObjName);
            if (existingCachedPlayer != null)
            {
                existingCachedPlayer.IsVisible = true;
                continue;
            }

            var hashedName = Crypto.GetHash256(pChar);
            _onlineCachedPlayers.SingleOrDefault(p => p.PlayerNameHash == hashedName)?.InitializePlayer(pChar);
        }

        _onlineCachedPlayers.Where(p => !string.IsNullOrEmpty(p.PlayerName) && !_localVisiblePlayers.Contains(p.PlayerName))
            .ToList().ForEach(p => p.DisposePlayer());

        Task.Run(async () => await UpdatePlayersFromService(_onlineCachedPlayers
            .Where(p => p.PlayerCharacter != null && p.IsVisible && !p.WasVisible)
            .ToDictionary(k => k.PlayerNameHash, k => (int)k.PlayerCharacter!.ClassJob.Id)));

        _lastPlayerObjectCheck = DateTime.Now;
    }

    private CachedPlayer CreateCachedPlayer(string hashedName)
    {
        return new CachedPlayer(hashedName, _ipcManager, _apiController, _dalamudUtil);
    }
}