﻿using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Internal.Notifications;
using MareSynchronos.API.Data;
using MareSynchronos.API.Dto;
using MareSynchronos.API.Dto.Group;
using MareSynchronos.PlayerData.Handlers;
using MareSynchronos.PlayerData.Pairs;
using MareSynchronos.Services.Events;
using MareSynchronos.WebAPI.Files.Models;
using System.Numerics;

namespace MareSynchronos.Services.Mediator;

#pragma warning disable MA0048 // File name must match type name
#pragma warning disable S2094
public record SwitchToIntroUiMessage : MessageBase;
public record SwitchToMainUiMessage : MessageBase;
public record OpenSettingsUiMessage : MessageBase;
public record DalamudLoginMessage : MessageBase;
public record DalamudLogoutMessage : MessageBase;
public record PriorityFrameworkUpdateMessage : SameThreadMessage;
public record FrameworkUpdateMessage : SameThreadMessage;
public record ClassJobChangedMessage(GameObjectHandler GameObjectHandler) : MessageBase;
public record DelayedFrameworkUpdateMessage : SameThreadMessage;
public record ZoneSwitchStartMessage : MessageBase;
public record ZoneSwitchEndMessage : MessageBase;
public record CutsceneStartMessage : MessageBase;
public record GposeStartMessage : MessageBase;
public record GposeEndMessage : MessageBase;
public record CutsceneEndMessage : MessageBase;
public record CutsceneFrameworkUpdateMessage : SameThreadMessage;
public record ConnectedMessage(ConnectionDto Connection) : MessageBase;
public record DisconnectedMessage : SameThreadMessage;
public record PenumbraModSettingChangedMessage : MessageBase;
public record PenumbraInitializedMessage : MessageBase;
public record PenumbraDisposedMessage : MessageBase;
public record PenumbraRedrawMessage(IntPtr Address, int ObjTblIdx, bool WasRequested) : SameThreadMessage;
public record GlamourerChangedMessage(IntPtr Address) : MessageBase;
public record HeelsOffsetMessage : MessageBase;
public record PenumbraResourceLoadMessage(IntPtr GameObject, string GamePath, string FilePath) : SameThreadMessage;
public record CustomizePlusMessage(string ProfileName) : MessageBase;
public record PalettePlusMessage(Character Character) : MessageBase;
public record HonorificMessage(string NewHonorificTitle) : MessageBase;
public record HonorificReadyMessage : MessageBase;
public record PlayerChangedMessage(CharacterData Data) : MessageBase;
public record CharacterChangedMessage(GameObjectHandler GameObjectHandler) : MessageBase;
public record TransientResourceChangedMessage(IntPtr Address) : MessageBase;
public record AddWatchedGameObjectHandler(GameObjectHandler Handler) : MessageBase;
public record RemoveWatchedGameObjectHandler(GameObjectHandler Handler) : MessageBase;
public record HaltScanMessage(string Source) : MessageBase;
public record ResumeScanMessage(string Source) : MessageBase;
public record NotificationMessage
    (string Title, string Message, NotificationType Type, uint TimeShownOnScreen = 3000) : MessageBase;
public record CreateCacheForObjectMessage(GameObjectHandler ObjectToCreateFor) : MessageBase;
public record ClearCacheForObjectMessage(GameObjectHandler ObjectToCreateFor) : MessageBase;
public record CharacterDataCreatedMessage(CharacterData CharacterData) : SameThreadMessage;
public record CharacterDataAnalyzedMessage : MessageBase;
public record PenumbraStartRedrawMessage(IntPtr Address) : MessageBase;
public record PenumbraEndRedrawMessage(IntPtr Address) : MessageBase;
public record HubReconnectingMessage(Exception? Exception) : SameThreadMessage;
public record HubReconnectedMessage(string? Arg) : SameThreadMessage;
public record HubClosedMessage(Exception? Exception) : SameThreadMessage;
public record DownloadReadyMessage(Guid RequestId) : MessageBase;
public record DownloadStartedMessage(GameObjectHandler DownloadId, Dictionary<string, FileDownloadStatus> DownloadStatus) : MessageBase;
public record DownloadFinishedMessage(GameObjectHandler DownloadId) : MessageBase;
public record UiToggleMessage(Type UiType) : MessageBase;
public record PlayerUploadingMessage(GameObjectHandler Handler, bool IsUploading) : MessageBase;
public record ClearProfileDataMessage(UserData? UserData = null) : MessageBase;
public record CyclePauseMessage(UserData UserData) : MessageBase;
public record ProfilePopoutToggle(Pair? Pair) : MessageBase;
public record CompactUiChange(Vector2 Size, Vector2 Position) : MessageBase;
public record ProfileOpenStandaloneMessage(Pair Pair) : MessageBase;
public record RemoveWindowMessage(WindowMediatorSubscriberBase Window) : MessageBase;
public record PairHandlerVisibleMessage(PairHandler Player) : MessageBase;
public record RefreshUiMessage : MessageBase;
public record OpenReportPopupMessage(Pair PairToReport) : MessageBase;
public record OpenBanUserPopupMessage(Pair PairToBan, GroupFullInfoDto GroupFullInfoDto) : MessageBase;
public record OpenCensusPopupMessage() : MessageBase;
public record OpenSyncshellAdminPanel(GroupFullInfoDto GroupInfo) : MessageBase;
public record OpenPermissionWindow(Pair Pair) : MessageBase;
public record DownloadLimitChangedMessage() : SameThreadMessage;
public record CensusUpdateMessage(byte Gender, byte RaceId, byte TribeId) : MessageBase;
public record TargetPairMessage(Pair Pair) : MessageBase;
public record CombatOrPerformanceStartMessage : MessageBase;
public record CombatOrPerformanceEndMessage : MessageBase;
public record EventMessage(Event Event) : MessageBase;

#pragma warning restore S2094
#pragma warning restore MA0048 // File name must match type name