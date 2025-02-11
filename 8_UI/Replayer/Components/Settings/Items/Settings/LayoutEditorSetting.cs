﻿using BeatLeader.Utils;
using BeatSaberMarkupLanguage.Attributes;
using System;
using Zenject;

namespace BeatLeader.Components.Settings
{
    internal class LayoutEditorSetting : ReeUIComponentV2WithContainer
    {
        [InjectOptional] private readonly LayoutEditor _layoutEditor;

        public event Action EnteredEditModeEvent;

        protected override void OnInitialize()
        {
           Content.gameObject.AddComponent<InputDependentObject>().Init(InputUtils.InputType.FPFC);
        }

        [UIAction("button-clicked")] private void OpenEditor()
        {
            _layoutEditor?.SetEditModeEnabled(true);
            EnteredEditModeEvent?.Invoke();
        }
    }
}
