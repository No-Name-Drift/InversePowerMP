using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Threading.Tasks;

namespace InversePowerMP
{
    public class InversePower : BaseScript
    {
        readonly float Power = API.GetConvarInt("ip_power", 100) / 100f;
        readonly float Torque = API.GetConvarInt("ip_torque", 80) / 100f;

        readonly float Angle = API.GetConvarInt("ip_angle", 350) / 100f;
        readonly float Speed = API.GetConvarInt("ip_speed", 200) / 100f;

        readonly float Slope = API.GetConvarInt("ip_slope", 35) / 1f;

        public InversePower()
        {
            Tick += OnTick;
        }

        private async Task OnTick()
        {

        }
    }
}
