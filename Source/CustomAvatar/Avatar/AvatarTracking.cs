//  Beat Saber Custom Avatars - Custom player models for body presence in Beat Saber.
//  Copyright � 2018-2020  Beat Saber Custom Avatars Contributors
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.

using CustomAvatar.Tracking;
using System;
using CustomAvatar.Logging;
using UnityEngine;
using Zenject;

namespace CustomAvatar.Avatar
{
    internal class AvatarTracking : MonoBehaviour
    {
        internal bool isCalibrationModeEnabled = false;

        private IAvatarInput _input;
        private SpawnedAvatar _spawnedAvatar;
        private ILogger<AvatarTracking> _logger = new UnityDebugLogger<AvatarTracking>();

        private Vector3 _prevBodyLocalPosition = Vector3.zero;

        #region Behaviour Lifecycle
        #pragma warning disable IDE0051

        [Inject]
        private void Inject(ILoggerProvider loggerProvider, IAvatarInput input, SpawnedAvatar spawnedAvatar)
        {
            _logger = loggerProvider.CreateLogger<AvatarTracking>(spawnedAvatar.avatar.descriptor.name);
            _input = input;
            _spawnedAvatar = spawnedAvatar;
        }

        private void LateUpdate()
        {
            try
            {
                SetPose(DeviceUse.Head,      _spawnedAvatar.head);
                SetPose(DeviceUse.LeftHand,  _spawnedAvatar.leftHand);
                SetPose(DeviceUse.RightHand, _spawnedAvatar.rightHand);

                if (isCalibrationModeEnabled)
                {
                    // TODO fix all of this
                    if (_spawnedAvatar.pelvis)
                    {
                        _spawnedAvatar.pelvis.position = _spawnedAvatar.avatar.pelvis.position;// * _avatar.scale + new Vector3(0, _avatar.verticalPosition, 0);
                        _spawnedAvatar.pelvis.rotation = _spawnedAvatar.avatar.pelvis.rotation;
                    }

                    if (_spawnedAvatar.leftLeg)
                    {
                        _spawnedAvatar.leftLeg.position = _spawnedAvatar.avatar.leftLeg.position;// * _avatar.scale + new Vector3(0, _avatar.verticalPosition, 0);
                        _spawnedAvatar.leftLeg.rotation = _spawnedAvatar.avatar.leftLeg.rotation;
                    }

                    if (_spawnedAvatar.rightLeg)
                    {
                        _spawnedAvatar.rightLeg.position = _spawnedAvatar.avatar.rightLeg.position;// * _avatar.scale + new Vector3(0, _avatar.verticalPosition, 0);
                        _spawnedAvatar.rightLeg.rotation = _spawnedAvatar.avatar.rightLeg.rotation;
                    }
                }
                else
                {
                    SetPose(DeviceUse.Waist,     _spawnedAvatar.pelvis);
                    SetPose(DeviceUse.LeftFoot,  _spawnedAvatar.leftLeg);
                    SetPose(DeviceUse.RightFoot, _spawnedAvatar.rightLeg);
                }

                if (_spawnedAvatar.body)
                {
                    _spawnedAvatar.body.position = _spawnedAvatar.head.position - (_spawnedAvatar.head.up * 0.1f);

                    var vel = new Vector3(_spawnedAvatar.body.localPosition.x - _prevBodyLocalPosition.x, 0.0f,
                        _spawnedAvatar.body.localPosition.z - _prevBodyLocalPosition.z);

                    var rot = Quaternion.Euler(0.0f, _spawnedAvatar.head.localEulerAngles.y, 0.0f);
                    var tiltAxis = Vector3.Cross(transform.up, vel);

                    _spawnedAvatar.body.localRotation = Quaternion.Lerp(_spawnedAvatar.body.localRotation,
                        Quaternion.AngleAxis(vel.magnitude * 1250.0f, tiltAxis) * rot,
                        Time.deltaTime * 10.0f);

                    _prevBodyLocalPosition = _spawnedAvatar.body.localPosition;
                }
            }
            catch (Exception e)
            {
                _logger.Error($"{e.Message}\n{e.StackTrace}");
            }
        }

        #pragma warning restore IDE0051
        #endregion

        private void SetPose(DeviceUse use, Transform target)
        {
            if (!target || !_input.TryGetPose(use, out Pose pose)) return;

            // if avatar transform has a parent, use that as the origin
            if (transform.parent)
            {
                target.position = transform.parent.TransformPoint(pose.position);
                target.rotation = transform.parent.rotation * pose.rotation;
            }
            else
            {
                target.position = pose.position;
                target.rotation = pose.rotation;
            }
        }
    }
}
