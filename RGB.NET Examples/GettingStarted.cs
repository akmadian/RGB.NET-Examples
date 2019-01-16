using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RGB.NET.Core;
using RGB.NET.Devices;
using RGB.NET.Groups;
using RGB.NET.Brushes;
using RGB.NET.Brushes.Gradients;
using RGB.NET.Decorators;

using RGB.NET.Devices.Asus;
using RGB.NET.Devices.CoolerMaster;
using RGB.NET.Devices.Corsair;
using RGB.NET.Devices.DMX;
using RGB.NET.Devices.Logitech;
using RGB.NET.Devices.Novation;
using RGB.NET.Devices.Razer;



/* This file contains some code examples showing how to get started with RGB.NET
 * If you need more help or have suggestions, please join the RGB.NET Discord Server > https://discord.gg/de8dCf3
 * RGB.NET is in heavy development, and some of the syntax in these examples may stop working.
 * This documentation was created by Ari Madian > https://github.com/akmadian
 */
namespace RGB.NET_Examples {
    class Program {

        // The RGB Surface holds all of the IRGBDevices loaded into it
        // IRGBDevices are enumerators of their LEDs
        public static RGBSurface surface = RGBSurface.Instance;

        static void Main(string[] args) {
            surface.Exception += args_ => Console.WriteLine(args_.Exception.Message);
            LoadDevices();

            /* LED Updates are triggered by an UpdateTrigger
             * UpdateTriggers must be registered to a surface 
             */
            TimerUpdateTrigger updateTrigger = new TimerUpdateTrigger();
            surface.RegisterUpdateTrigger(updateTrigger);
            
            /* You can also trigger an update manually by calling surface.Update();
            *  If calling surface.Update() more than once, 
            *      an update trigger is almost always a better option.
            */
            surface.Update();

            /* To apply an led effect to some device(s), you must first make a ListLedGroup
             * ListLedGroups can be created from an individual IRGBDevice by passing
             *      the IRGBDevice as a parameter to the ListLedGroup constructor.
             */
            ILedGroup ledGroup = new ListLedGroup(surface.Leds);

            /* Once you have an ILedGroup with all of your desired Leds in it,
             *     To apply an effect, you will start with a brush.
             *     
             * Brushes are used to "paint" leds, and they are kinda like 
             *     bases that you can apply gradients and decorators to
             *      - Gradients tell the brush what colors to paint where, and when.
             *      - Decorators are like mods to a brush.
             */
            PaintGradient(ledGroup, updateTrigger);

            Console.ReadLine();
        }


        /// <summary>
        /// Loads devices from the selected DeviceProviders into the RGBSurface "surface"
        /// The AsusDeviceProvider can cause lots of issues.
        ///     Don't use it on a system that does not have any Aura devices.
        ///     Even when a system has Aura devices, it can still cause issues.
        /// </summary>
        public static void LoadDevices() {

            // This method is not best practice, but it does work, 
            //     and there are no plans to deprecate it
            
            //surface.LoadDevices(AsusDeviceProvider.Instance);
            surface.LoadDevices(LogitechDeviceProvider.Instance);
            surface.LoadDevices(CorsairDeviceProvider.Instance);
            surface.LoadDevices(CoolerMasterDeviceProvider.Instance);
            surface.LoadDevices(DMXDeviceProvider.Instance);
            surface.LoadDevices(NovationDeviceProvider.Instance);
            surface.LoadDevices(RazerDeviceProvider.Instance);
        }

        /// <summary>
        /// Paints a moving gradient effect on a given ILedGroup
        /// </summary>
        /// <param name="group">The ILedGroup to apply the brush to</param>
        /// <param name="trigger">The UpdateTrigger registered to the RGBSurface used</param>
        public static void PaintGradient(ILedGroup group, TimerUpdateTrigger trigger) {
            // Create new gradient
            IGradient gradient = new RainbowGradient();

            // Add a MoveGradientDecorator to the gradient
            gradient.AddDecorator(new RGB.NET.Decorators.Gradient.MoveGradientDecorator());

            // Make new LinearGradientBrush from gradient
            LinearGradientBrush nBrush = new LinearGradientBrush(gradient);

            // Apply LinearGradientBrush to led group
            group.Brush = nBrush;

            // Start UpdateTrigger, without this, the gradient will be drawn, but will not move.
            trigger.Start();
        }

        /// <summary>
        /// Paints all leds in a given ILedGroup with a static color
        /// </summary>
        /// <param name="group">The ILedGroup to apply the brush to</param>
        public static void PaintStaticColor(ILedGroup group) {
            Color c = new Color(255, 255, 255);
            group.Brush = new SolidColorBrush(c);
        }
    }
}
