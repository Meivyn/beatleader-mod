using System.Collections.Generic;
using System.Reflection;
using BeatLeader.Replayer;
using BeatLeader.Core.Managers.ReplayEnhancer;
using BeatLeader.Utils;
using HarmonyLib;
using SiraUtil.Submissions;
using Zenject;
using BeatLeader.Replayer.Camera;
using UnityEngine;
using BeatLeader.Replayer.Emulation;
using BeatLeader.Components;
using BeatLeader.ViewControllers;
using SiraUtil.Tools.FPFC;
using System;
using BeatLeader.Replayer.Tweaking;
using BeatLeader.Replayer.Binding;
using static BeatLeader.Utils.InputUtils;
using BeatLeader.UI;
using BeatLeader.Models;
using Vector3 = UnityEngine.Vector3;

namespace BeatLeader.Installers
{
    public class OnGameplayCoreInstaller : Installer<OnGameplayCoreInstaller>
    {
        public override void InstallBindings()
        {
            Plugin.Log.Debug("OnGameplayCoreInstaller");
            if (ReplayerLauncher.IsStartedAsReplay) InitReplayer();
            else InitRecorder();
        }

        #region Recorder

        private static readonly List<string> modes = new() { "Solo", "Multiplayer" };

        private void InitRecorder()
        {
            RecorderUtils.buffer = RecorderUtils.shouldRecord;

            if (RecorderUtils.shouldRecord)
            {
                RecorderUtils.shouldRecord = false;

                #region Gates
                if (ScoreSaber_playbackEnabled != null && (bool)ScoreSaber_playbackEnabled.Invoke(null, null) == false)
                {
                    Plugin.Log.Debug("SS replay is running, BL Replay Recorder will not be started!");
                    return;
                }
                if (!(MapEnhancer.previewBeatmapLevel.levelID.StartsWith(CustomLevelLoader.kCustomLevelPrefixId)))
                {
                    Plugin.Log.Debug("OST level detected, BL Replay Recorder will not be started!");
                    return;
                }
                var gameMode = GetGameMode();
                if (gameMode != null && !modes.Contains(gameMode))
                {
                    Plugin.Log.Debug("Not allowed game mode, BL Replay Recorder will not be started!");
                    return;
                }
                #endregion

                Plugin.Log.Debug("Starting a BL Replay Recorder...");

                Container.BindInterfacesAndSelfTo<ReplayRecorder>().AsSingle();
                Container.BindInterfacesAndSelfTo<TrackingDeviceEnhancer>().AsTransient();
            }
            else
            {
                //Plugin.Log.Info("Unknown flow detected, recording would not be started.");
            }
        }

        #endregion

        #region Replayer

        private static readonly ReplayerCameraController.InitData cameraInitData = new(

           new StaticCameraPose(0, "LeftView", new Vector3(-3.70f, 1.70f, 0), new Vector3(0, 90, 0), InputType.FPFC),
           new StaticCameraPose(1, "RightView", new Vector3(3.70f, 1.70f, 0), new Vector3(0, -90, 0), InputType.FPFC),
           new StaticCameraPose(2, "BehindView", new Vector3(0f, 1.9f, -2f), Vector3.zero, InputType.FPFC),
           new StaticCameraPose(3, "CenterView", new Vector3(0f, 1.7f, 0f), Vector3.zero, InputType.FPFC),

           new StaticCameraPose(0, "LeftView", new Vector3(-3.70f, 0, -1.10f), new Vector3(0, 60, 0), InputType.VR),
           new StaticCameraPose(1, "RightView", new Vector3(3.70f, 0, -1.10f), new Vector3(0, -60, 0), InputType.VR),
           new StaticCameraPose(2, "BehindView", new Vector3(0, 0, -2), Vector3.zero, InputType.VR),
           new StaticCameraPose(3, "CenterView", Vector3.zero, Vector3.zero, InputType.VR),

           new FlyingCameraPose(new Vector2(0.5f, 0.5f), new Vector2(0, 1.7f), 4, true, "FreeView"),
           new PlayerViewCameraPose(3)

        );

        private void InitReplayer()
        {
            DisableScoreSubmission();

            //Dependencies
            Container.Bind<ReplayLaunchData>().FromInstance(ReplayerLauncher.LaunchData).AsSingle();

            //Core logic(Playback)
            Container.BindInterfacesAndSelfTo<PlaybackController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BeatmapTimeController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<VRControllersProvider>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<VRControllersMovementEmulator>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

            //Core logic(Notes handling)
            Container.BindInterfacesAndSelfTo<ReplayEventsProcessor>().AsSingle();
            Container.Bind<ReplayerScoreProcessor>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<ReplayerNotesCutter>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

            //Tweaks and tools
            Container.Bind<BeatmapVisualsController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<ReplayerCameraController.InitData>().FromInstance(cameraInitData).AsSingle();
            Container.Bind<ReplayerCameraController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<TweaksLoader>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<HotkeysHandler>().AsSingle().NonLazy();
            Container.BindInterfacesTo<SettingsLoader>().AsSingle().NonLazy();
            Container.Bind<ReplayWatermark>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

            //UI
            Container.Bind<Replayer2DViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<ReplayerVRViewController>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<ScreenSpaceScreen>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            Container.Bind<ReplayerUIBinder>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

            if (InputUtils.IsInFPFC)
            {
                PatchSiraFreeView();
            }

            Plugin.Log.Notice("[Installer] Replay system successfully installed!");
        }
        private void PatchSiraFreeView()
        {
            try
            {
                Container.Resolve<IFPFCSettings>().Enabled = false;
                Assembly assembly = typeof(IFPFCSettings).Assembly;

                Type smoothCameraListenerType = assembly.GetType("SiraUtil.Tools.FPFC.SmoothCameraListener");
                Type FPFCToggleType = assembly.GetType("SiraUtil.Tools.FPFC.FPFCToggle");
                Type simpleCameraControllerType = assembly.GetType("SiraUtil.Tools.FPFC.SimpleCameraController");

                Container.Unbind<IFPFCSettings>();
                Container.UnbindInterfacesTo(smoothCameraListenerType);
                Container.UnbindInterfacesTo(FPFCToggleType);
                //GameObject.Destroy((UnityEngine.Object)Container.TryResolve(simpleCameraControllerType));
            }
            catch (Exception ex)
            {
                Plugin.Log.Error($"[Installer] Error during attemping to remove Sira's FPFC system! \r\n {ex}");
            }
        }

        #endregion

        #region Submission

        private static Ticket _submissionTicket;

        private void DisableScoreSubmission()
        {
            _submissionTicket = Container.Resolve<Submission>()?.DisableScoreSubmission("BeatLeaderReplayer", "Playback");
            ReplayerLauncher.LaunchData.ReplayWasFinishedEvent += HandleReplayWasFinished;
        }
        private void HandleReplayWasFinished(StandardLevelScenesTransitionSetupDataSO data, Models.ReplayLaunchData launchData)
        {
            launchData.ReplayWasFinishedEvent -= HandleReplayWasFinished;
            if (_submissionTicket != null)
            {
                Container.Resolve<Submission>()?.Remove(_submissionTicket);
                _submissionTicket = null;
            }
        }

        #endregion

        #region Game mode stuff

        private string GetGameMode()
        {
            string singleGM = GetStandardDataSO()?.gameMode;
            string multiGM = GetMultiDataSO()?.gameMode;

            foreach (string mode in new[] { singleGM, multiGM })
            {
                if (mode != null && mode.Length > 0)
                {
                    return mode;
                }
            }

            return null;
        }

        private StandardLevelScenesTransitionSetupDataSO GetStandardDataSO()
        {
            try
            {
                return Container.TryResolve<StandardLevelScenesTransitionSetupDataSO>();
            }
            catch
            {
                return null;
            }
        }
        private MultiplayerLevelScenesTransitionSetupDataSO GetMultiDataSO()
        {
            try
            {
                return Container.TryResolve<MultiplayerLevelScenesTransitionSetupDataSO>();
            }
            catch
            {
                return null;
            }
        }

        #endregion

        // TODO: remove this after verifying of an "allowed flows" strategy
        private static readonly MethodBase ScoreSaber_playbackEnabled = AccessTools.Method("ScoreSaber.Core.ReplaySystem.HarmonyPatches.PatchHandleHMDUnmounted:Prefix");
    }
}