using EQX.Core.Process;
using EQX.Process;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Process;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddProcessExtension
    {
        public static IHostBuilder AddProcesses(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostcontext, services) =>
            {
                services.AddKeyedScoped<IProcess<ESequence>, RootProcess<ESequence, ESemiSequence>>(EProcess.Root.ToString());

                services.AddKeyedScoped<IProcess<ESequence>, TrayInCVProcess>(EProcess.TrayInCV.ToString());
                services.AddKeyedScoped<IProcess<ESequence>, TrayInElevatorProcess>(EProcess.TrayInElevator.ToString());
                services.AddKeyedScoped<IProcess<ESequence>, TrayOutCVProcess>(EProcess.TrayOutCV.ToString());
                services.AddKeyedScoped<IProcess<ESequence>, TrayOutElevatorProcess>(EProcess.TrayOutElevator.ToString());

                services.AddKeyedScoped<IProcess<ESequence>, TrayHeadProcess>(EProcess.TrayHead.ToString());

                services.AddKeyedScoped<IProcess<ESequence>, SpongeDetachProcess>(EProcess.SpongeDetach.ToString());
                services.AddKeyedScoped<IProcess<ESequence>, CameraFlipperProcess>(EProcess.CameraRotator.ToString());

                services.AddKeyedScoped<IProcess<ESequence>, FilmDetachProcess>(EProcess.FilmDetach.ToString());

                services.AddKeyedScoped<IProcess<ESequence>, CameraAssembleProcess>(EProcess.CameraAssemble.ToString());

                services.AddKeyedScoped<IProcess<ESequence>, SetConveyerInProcess>(EProcess.FrontSetCVIn.ToString());
                services.AddKeyedScoped<IProcess<ESequence>, SetConveyerDetachProcess>(EProcess.FrontSetCVDetach.ToString());
                services.AddKeyedScoped<IProcess<ESequence>, SetConveyerAssembleProcess >(EProcess.FrontSetCVAssemble.ToString());
                services.AddKeyedScoped<IProcess<ESequence>, SetConveyerOutProcess>(EProcess.FrontSetCVOut.ToString());

                services.AddKeyedScoped<IProcess<ESequence>, SetConveyerInProcess>(EProcess.RearSetCVIn.ToString());
                services.AddKeyedScoped<IProcess<ESequence>, SetConveyerDetachProcess>(EProcess.RearSetCVDetach.ToString());
                services.AddKeyedScoped<IProcess<ESequence>, SetConveyerAssembleProcess >(EProcess.RearSetCVAssemble.ToString());
                services.AddKeyedScoped<IProcess<ESequence>, SetConveyerOutProcess>(EProcess.RearSetCVOut.ToString());

                services.AddSingleton<Processes>((ser) =>
                {
                    List<IProcess<ESequence>> processList = new List<IProcess<ESequence>>();

                    foreach (EProcess process in Enum.GetValues(typeof(EProcess)))
                    {
                        var proc = ser.GetKeyedService<IProcess<ESequence>>(process.ToString());
                        if (proc != null)
                        {
                            ((ProcessBase<ESequence>)proc).Name = process.ToString();
                            processList.Add(proc);
                        }
                    }
                    return new Processes(processList, ser.GetRequiredService<ProcessConfig>());
                });
            });
            return hostBuilder;
        }
    }
}
