﻿//  Beat Saber Custom Avatars - Custom player models for body presence in Beat Saber.
//  Copyright © 2018-2020  Beat Saber Custom Avatars Contributors
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

using UnityEngine;

namespace CustomAvatar.Tracking.OpenVR
{
    internal class OpenVRDeviceState : ITrackedDeviceState
    {
        public DeviceUse use { get; }
        public uint deviceIndex { get; set; }
        public string modelName { get; set; }
        public string serialNumber { get; set; }
        public string role { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public bool isConnected { get; set; }
        public bool isTracking { get; set; }

        public OpenVRDeviceState(DeviceUse use)
        {
            this.use = use;

            deviceIndex = 0;
            modelName = null;
            serialNumber = null;
            role = null;
            position = Vector3.zero;
            rotation = Quaternion.identity;
            isConnected = false;
            isTracking = false;
        }
    }
}
