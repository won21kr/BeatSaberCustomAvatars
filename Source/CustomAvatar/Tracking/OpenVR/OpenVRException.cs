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

using System;
using Valve.VR;

namespace CustomAvatar.Tracking.OpenVR
{
    internal class OpenVRException : Exception
    {
        public ETrackedDeviceProperty Property { get; }
        public ETrackedPropertyError Error { get; }

        public OpenVRException(string message, ETrackedDeviceProperty property, ETrackedPropertyError error) : base(message)
        {
            Property = property;
            Error = error;
        }
    }
}
