﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using VRUIControls;

namespace BeatLeader.Replayer
{
    internal class OrientedVRInputModule : VRInputModule
    {
        public BaseRaycaster raycaster;

        protected override MouseState GetMousePointerEventData(int id)
        {
            PointerEventData data;
            bool pointerData = GetPointerData(-1, out data, true);
            data.Reset();
            data.button = PointerEventData.InputButton.Left;
            VRController vrController = _vrPointer.vrController;
            if (vrController.active)
            {
                data.pointerCurrentRaycast = new RaycastResult
                {
                    worldPosition = vrController.position,
                    worldNormal = vrController.forward
                };
                Vector2 scrollDelta = new Vector2(vrController.horizontalAxisValue, vrController.verticalAxisValue) * 1f;
                scrollDelta.x *= -1f;
                data.scrollDelta = scrollDelta;
            }

            Raycast(data, m_RaycastResultCache);
            m_RaycastResultCache.Sort(_raycastComparer);
            RaycastResult raycastResult2 = (data.pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache));
            m_RaycastResultCache.Clear();
            Vector2 screenPosition = raycastResult2.screenPosition;
            if (pointerData)
            {
                data.delta = new Vector2(0f, 0f);
            }
            else
            {
                data.delta = screenPosition - data.position;
            }

            data.position = screenPosition;
            PointerEventData.FramePressState stateForMouseButton = PointerEventData.FramePressState.NotChanged;
            if (vrController.active)
            {
                float num = 0f;
                num = ((!enabled) ? 0f : ((!useMouseForPressInput) ? vrController.triggerValue : (Input.GetMouseButton(0) ? 1f : 0f)));
                ButtonState buttonState = _mouseState.GetButtonState(PointerEventData.InputButton.Left);
                if (buttonState.pressedValue < 0.9f && num >= 0.9f)
                {
                    stateForMouseButton = PointerEventData.FramePressState.Pressed;
                }
                else if (buttonState.pressedValue >= 0.9f && num < 0.9f)
                {
                    stateForMouseButton = PointerEventData.FramePressState.Released;
                }

                buttonState.pressedValue = num;
            }

            _mouseState.SetButtonState(PointerEventData.InputButton.Left, stateForMouseButton, data);
            return _mouseState;
        }
        private void Raycast(PointerEventData data, List<RaycastResult> result)
        {
            if (raycaster == null)
            {
                eventSystem.RaycastAll(data, result);
                return;
            }

            foreach (var item in raycaster.GetComponentsInChildren<BaseRaycaster>())
            {
                item.Raycast(data, result);
            }
        }
    }
}
