using EQX.Core.InOut;
using EQX.InOut.Virtual;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddVirtualIOExtension
    {
        public static IHostBuilder AddProcessIO(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                // 1. Tray In/Out Elevator + CV
                services.AddKeyedSingleton<IDInputDevice<ETrayInCvInput>, MappableInputDevice<ETrayInCvInput>>("TrayInCvInput");
                services.AddKeyedSingleton<IDOutputDevice<ETrayInCvOutput>, MappableOutputDevice<ETrayInCvOutput>>("TrayInCvOutput");
                services.AddKeyedSingleton<IDInputDevice<ETrayInElevatorInput>, MappableInputDevice<ETrayInElevatorInput>>("TrayInElevatorInput");
                services.AddKeyedSingleton<IDOutputDevice<ETrayInElevatorOutput>, MappableOutputDevice<ETrayInElevatorOutput>>("TrayInElevatorOutput");
                services.AddKeyedSingleton<IDInputDevice<ETrayOutCvInput>, MappableInputDevice<ETrayOutCvInput>>("TrayOutCvInput");
                services.AddKeyedSingleton<IDOutputDevice<ETrayOutCvOutput>, MappableOutputDevice<ETrayOutCvOutput>>("TrayOutCvOutput");
                services.AddKeyedSingleton<IDInputDevice<ETrayOutElevatorInput>, MappableInputDevice<ETrayOutElevatorInput>>("TrayOutElevatorInput");
                services.AddKeyedSingleton<IDOutputDevice<ETrayOutElevatorOutput>, MappableOutputDevice<ETrayOutElevatorOutput>>("TrayOutElevatorOutput");

                // 2. Tray Head
                services.AddKeyedSingleton<IDInputDevice<ETrayHeadInput>, MappableInputDevice<ETrayHeadInput>>("TrayHeadInput");
                services.AddKeyedSingleton<IDOutputDevice<ETrayHeadOutput>, MappableOutputDevice<ETrayHeadOutput>>("TrayHeadOutput");

                // 3. Flipper + Sponge Detach
                services.AddKeyedSingleton<IDInputDevice<ECameraFlipperInput>, MappableInputDevice<ECameraFlipperInput>>("CameraFlipperInput");
                services.AddKeyedSingleton<IDOutputDevice<ECameraFlipperOutput>, MappableOutputDevice<ECameraFlipperOutput>>("CameraFlipperOutput");
                services.AddKeyedSingleton<IDInputDevice<ESpongeDetachInput>, MappableInputDevice<ESpongeDetachInput>>("SpongeDetachInput");
                services.AddKeyedSingleton<IDOutputDevice<ESpongeDetachOutput>, MappableOutputDevice<ESpongeDetachOutput>>("SpongeDetachOutput");

                // 4. Camera Assemble Head
                services.AddKeyedSingleton<IDInputDevice<ECameraAssembleHeadInput>, MappableInputDevice<ECameraAssembleHeadInput>>("CameraAssembleHeadInput");
                services.AddKeyedSingleton<IDOutputDevice<ECameraAssembleHeadOutput>, MappableOutputDevice<ECameraAssembleHeadOutput>>("CameraAssembleHeadOutput");

                // 5. Film Detach Head
                services.AddKeyedSingleton<IDInputDevice<EFilmDetachInput>, MappableInputDevice<EFilmDetachInput>>("FilmDetachInput");
                services.AddKeyedSingleton<IDOutputDevice<EFilmDetachOutput>, MappableOutputDevice<EFilmDetachOutput>>("FilmDetachOutput");

                // 6. Set CV
                services.AddKeyedSingleton<IDInputDevice<EFrontCvCamAssembleInput>, MappableInputDevice<EFrontCvCamAssembleInput>>("FrontCvCamAssembleInput");
                services.AddKeyedSingleton<IDOutputDevice<EFrontCvCamAssembleOutput>, MappableOutputDevice<EFrontCvCamAssembleOutput>>("FrontCvCamAssembleOutput");
                services.AddKeyedSingleton<IDInputDevice<EFrontCvFilmDetachInput>, MappableInputDevice<EFrontCvFilmDetachInput>>("FrontCvFilmDetachInput");
                services.AddKeyedSingleton<IDOutputDevice<EFrontCvFilmDetachOutput>, MappableOutputDevice<EFrontCvFilmDetachOutput>>("FrontCvFilmDetachOutput");
                services.AddKeyedSingleton<IDInputDevice<EFrontInCvSetLoadInput>, MappableInputDevice<EFrontInCvSetLoadInput>>("FrontCvSetLoadInput");
                services.AddKeyedSingleton<IDOutputDevice<EFrontCvSetUnloadOutput>, MappableOutputDevice<EFrontCvSetUnloadOutput>>("FrontCvSetUnloadOutput");

                services.AddKeyedSingleton<IDInputDevice<ERearCvCamAssembleInput>, MappableInputDevice<ERearCvCamAssembleInput>>("RearCvCamAssembleInput");
                services.AddKeyedSingleton<IDOutputDevice<ERearCvCamAssembleOutput>, MappableOutputDevice<ERearCvCamAssembleOutput>>("RearCvCamAssembleOutput");
                services.AddKeyedSingleton<IDInputDevice<ERearCvFilmDetachInput>, MappableInputDevice<ERearCvFilmDetachInput>>("RearCvFilmDetachInput");
                services.AddKeyedSingleton<IDOutputDevice<ERearCvFilmDetachOutput>, MappableOutputDevice<ERearCvFilmDetachOutput>>("RearCvFilmDetachOutput");
                services.AddKeyedSingleton<IDInputDevice<ERearInCvSetLoadInput>, MappableInputDevice<ERearInCvSetLoadInput>>("RearCvSetLoadInput");
                services.AddKeyedSingleton<IDOutputDevice<ERearCvSetUnloadOutput>, MappableOutputDevice<ERearCvSetUnloadOutput>>("RearCvSetUnloadOutput");

                // 7. Vision
                services.AddKeyedSingleton<IDInputDevice<EVisionProcessInput>, MappableInputDevice<EVisionProcessInput>>("VisionProcessInput");
                services.AddKeyedSingleton<IDOutputDevice<EVisionProcessOutput>, MappableOutputDevice<EVisionProcessOutput>>("VisionProcessOutput");

                services.AddSingleton<VirtualIO>();
            });
            return hostBuilder;
        }
    }
}
