using Puppet.Automation.Services.HomeAssistant;
using Puppet.Automation.Services.Notifiers;
using Puppet.Common.Automation;
using Puppet.Common.Devices;
using Puppet.Common.Events;
using Puppet.Common.Exceptions;
using Puppet.Common.Notifiers;
using Puppet.Common.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Automation.Miscellaneous
{
    [TriggerDevice("Button.ArcadeSceneController", Capability.Pushed)]
    [TriggerDevice("Button.ArcadeSceneController", Capability.Held)]
    [TriggerDevice("Button.ArcadeSceneController", Capability.DoubleTapped)]
    [TriggerDevice("Button.ArcadeSceneController2", Capability.Pushed)]
    [TriggerDevice("Button.ArcadeSceneController2", Capability.Held)]
    [TriggerDevice("Button.ArcadeSceneController2", Capability.DoubleTapped)]
    public class ArcadeSceneController : AutomationBase
    {
        private HassSceneService _hassSceneService;
        private ButtonState _buttonState;
        private GenericDevice _mainController;
        const string LASTSCENEKEY = "arcade-scene-controller-last-scene";

        public ArcadeSceneController(HomeAutomationPlatform hub, HubEvent evt) : base(hub, evt)
        {
            _hassSceneService = new HassSceneService(_hub.Configuration);
            _buttonState = evt.GetButtonState();
        }

        protected override async Task Handle()
        {
            switch (_mainController.IsTriggerDevice(_evt), _buttonState, int.Parse(_evt.Value))
            {
                // Basic lights
                case (true, ButtonState.Pushed, 5):
                case (false, ButtonState.Pushed, 1):
                    await EnsureApplyScene(Scene.Arcade_2);
                    break; 

                // Scene 3 (pinks)
                case (true, ButtonState.Pushed, 1):
                case (false, ButtonState.Held, 1):
                    await EnsureApplyScene(Scene.Arcade_3);
                    break;

                // Northern lights
                case (true, ButtonState.Held, 1):
                    await ApplyEffect(9);
                    break;

                // Scene 4 (blues)
                case (true, ButtonState.Pushed, 2):
                case (false, ButtonState.Held, 2):
                    await EnsureApplyScene(Scene.Arcade_4);
                    break;

                // It's a storm!
                case (true, ButtonState.Held, 2):
                    await ApplyEffect(7);
                    break;

                // Light it up
                case (true, ButtonState.Pushed, 3):
                case (false, ButtonState.DoubleTapped, 1):
                    await EnsureApplyScene(Scene.Arcade_1);
                    break;

                // Everything off
                case (true, ButtonState.Pushed, 4):
                case (false, ButtonState.Pushed, 2):
                    await EnsureApplyScene(Scene.Arcade_0);
                    break;

                // Refresh everything
                case (true, ButtonState.DoubleTapped, 4):
                    await RefreshEverything();
                    break;

                // Reset the LED lights
                case (true, ButtonState.Held, 4):
                    await ResetLEDs();
                    break;
            }
        }

        private async Task ResetLEDs()
        {
            SwitchRelay ledPower = await _hub.GetDeviceByLabel<SwitchRelay>("Arcade Plug 1");
            await ledPower.Off();
            await WaitForCancellationAsync(TimeSpan.FromSeconds(10));
            await ledPower.On();
        }

        private async Task RefreshEverything()
        {

            List<GenericDevice> devicesToRefresh = new List<GenericDevice>();
            devicesToRefresh.Add(await _hub.GetDeviceByLabel<GenericDevice>("Death Star Lamp"));
            devicesToRefresh.Add(await _hub.GetDeviceByLabel<GenericDevice>("Arcade LED 1"));
            devicesToRefresh.Add(await _hub.GetDeviceByLabel<GenericDevice>("Arcade LED 2"));
            devicesToRefresh.Add(await _hub.GetDeviceByLabel<GenericDevice>("Arcade Plug 5")); // Pac-Man neon sign
            for (int i = 1; i <= 9; i++)
            {
                devicesToRefresh.Add(await _hub.GetDeviceByLabel<GenericDevice>($"Arcade Bulb {i}"));
            }

            foreach (var d in devicesToRefresh)
            {
                await d.DoAction("refresh");
                await WaitForCancellationAsync(TimeSpan.FromSeconds(1));
            }
            await WaitForCancellationAsync(TimeSpan.FromSeconds(5));
            await _hassSceneService.ApplyScene(GetLastScene());
        }

        protected async override Task InitDevices()
        {
            _mainController = await _hub.GetDeviceByMappedName<GenericDevice>("Button.ArcadeSceneController");
        }

        private Scene GetLastScene()
        {
            if(!_hub.StateBag.ContainsKey(LASTSCENEKEY))
            {
                return Scene.Arcade_0;
            }
            return (Scene)_hub.StateBag.GetValueOrDefault(LASTSCENEKEY);
        }

        protected async Task EnsureApplyScene(Scene scene)
        {
            _hub.StateBag.AddOrUpdate(LASTSCENEKEY, scene, (key, oldvalue) => scene);
            await _hassSceneService.ApplyScene(scene);
            await WaitForCancellationAsync(TimeSpan.FromSeconds(5));
            await _hassSceneService.ApplyScene(scene);
        }

        protected async Task ApplyEffect(int effect)
        {
            var arcade1 = await _hub.GetDeviceByLabel<GenericDevice>($"Arcade LED 1");
            var arcade2 = await _hub.GetDeviceByLabel<GenericDevice>($"Arcade LED 2");
            await arcade1.DoAction("setEffect", effect.ToString());
            await arcade2.DoAction("setEffect", effect.ToString());
        }
    }
}
