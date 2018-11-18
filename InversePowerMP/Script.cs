using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace InversePowerMP
{
    public class InversePower : BaseScript
    {
        readonly float Power = API.GetConvarInt("ip_power", 100) / 100f;
        readonly float Torque = API.GetConvarInt("ip_torque", 80) / 100f;

        readonly float Angle = API.GetConvarInt("ip_angle", 350) / 100f;
        readonly float Speed = API.GetConvarInt("ip_speed", 200) / 100f;

        readonly float Base = API.GetConvarInt("ip_slope", 35) / 1f;
        readonly float Deadzone = API.GetConvarInt("ip_deadzone", 0) / 1.0f;

        public InversePower()
        {
            Tick += OnTick;
        }

        private async Task OnTick()
        {
            // If the player does not exists, cannot be controlled or is dead, return
            if (!LocalPlayer.Character.Exists() || !LocalPlayer.CanControlCharacter || LocalPlayer.IsDead)
            {
                return;
            }

            // If the player vehicle is non existing, return
            if (LocalPlayer.Character.CurrentVehicle == null)
            {
                return;
            }

            // If the player vehicle is not a car, return
            if (!LocalPlayer.Character.CurrentVehicle.Model.IsCar)
            {
                return;
            }

            // Store the multiplier
            float Multiplier = 0f;

            // Get the speed and relative vector
            float Speed = API.GetEntitySpeed(LocalPlayer.Character.CurrentVehicle.GetHashCode()); ;
            Vector3 RelVector = API.GetEntitySpeedVector(LocalPlayer.Character.CurrentVehicle.GetHashCode(), true);

            // Calculate the vehicle angle
            double Angle = Math.Acos(RelVector.Y / Speed) * 180f / 3.14159265f;

            // If angle is not a number, set it to zero
            if (double.IsNaN(Angle))
            {
                Angle = 0;
            }

            // If the speed is lower than the base, calculate the multiplier
            if (Speed < Base)
            {
                Multiplier = (Base - Speed) / Base;
            }

            // Calculate the power and torque multipliers
            double PowerMultiplier = 1f + (Power * ((Angle / 90 * Angle) + (Angle / 90 * Multiplier * Speed)));
            double TorqueMultiplier = 1f + (Torque * ((Angle / 90 * Angle) + (Angle / 90 * Multiplier * Speed)));

            // Get the values of the triggers
            int AcceleratorValue = API.GetControlValue(0, (int)Control.VehicleAccelerate);
            int BrakeValue = API.GetControlValue(0, (int)Control.VehicleBrake);

            // I don't know what the fuck is this, but the original note says:
            // "no need to worry about values since we compare them with each other"
            if (!(Angle > 135 && BrakeValue > AcceleratorValue + 12) && Angle > Deadzone)
            {
                LocalPlayer.Character.CurrentVehicle.EngineTorqueMultiplier = (float)TorqueMultiplier;
                LocalPlayer.Character.CurrentVehicle.EnginePowerMultiplier = (float)PowerMultiplier;
            }
            else
            {
                LocalPlayer.Character.CurrentVehicle.EngineTorqueMultiplier = 1f;
                LocalPlayer.Character.CurrentVehicle.EnginePowerMultiplier = 1f;
            }
        }
    }
}
