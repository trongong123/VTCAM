using EQX.Core.InOut;
using EQX.InOut;
using EQX.InOut.Virtual;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Defines.Process
{
    public class VirtualIO
    {
        // 1. TraySupplier
        private readonly IDInputDevice<ETrayInCvInput> _trayInCvInput;
        private readonly IDOutputDevice<ETrayInCvOutput> _trayInCvOutput;
        private readonly IDInputDevice<ETrayInElevatorInput> _trayInElevatorInput;
        private readonly IDOutputDevice<ETrayInElevatorOutput> _trayInElevatorOutput;
        private readonly IDInputDevice<ETrayOutCvInput> _trayOutCvInput;
        private readonly IDOutputDevice<ETrayOutCvOutput> _trayOutCvOutput;
        private readonly IDInputDevice<ETrayOutElevatorInput> _trayOutElevatorInput;
        private readonly IDOutputDevice<ETrayOutElevatorOutput> _trayOutElevatorOutput;

        // 2. TrayHead
        private readonly IDInputDevice<ETrayHeadInput> _trayHeadInput;
        private readonly IDOutputDevice<ETrayHeadOutput> _trayHeadOutput;

        // 3. FlipperSpongeDetach
        private readonly IDInputDevice<ECameraFlipperInput> _cameraFlipperInput;
        private readonly IDOutputDevice<ECameraFlipperOutput> _cameraFlipperOutput;
        private readonly IDInputDevice<ESpongeDetachInput> _tapeDetachInput;
        private readonly IDOutputDevice<ESpongeDetachOutput> _tapeDetachOutput;

        // 4. CameraAssembleHead
        private readonly IDInputDevice<ECameraAssembleHeadInput> _cameraAssembleHeadInput;
        private readonly IDOutputDevice<ECameraAssembleHeadOutput> _cameraAssembleHeadOutput;

        // 5. FilmDetachHead
        private readonly IDInputDevice<EFilmDetachInput> _filmDetachInput;
        private readonly IDOutputDevice<EFilmDetachOutput> _filmDetachOutput;

        // 6. SetCV
        private readonly IDInputDevice<EFrontCvCamAssembleInput> _frontCvCamAssembleInput;
        private readonly IDOutputDevice<EFrontCvCamAssembleOutput> _frontCvCamAssembleOutput;
        private readonly IDInputDevice<EFrontCvFilmDetachInput> _frontCvFilmDetachInput;
        private readonly IDOutputDevice<EFrontCvFilmDetachOutput> _frontCvFilmDetachOutput;
        private readonly IDInputDevice<EFrontInCvSetLoadInput> _frontCvSetLoadInput;
        private readonly IDOutputDevice<EFrontCvSetUnloadOutput> _frontCvSetUnloadOutput;
        private readonly IDInputDevice<ERearCvCamAssembleInput> _rearCvCamAssembleInput;
        private readonly IDOutputDevice<ERearCvCamAssembleOutput> _rearCvCamAssembleOutput;
        private readonly IDInputDevice<ERearCvFilmDetachInput> _rearCvFilmDetachInput;
        private readonly IDOutputDevice<ERearCvFilmDetachOutput> _rearCvFilmDetachOutput;
        private readonly IDInputDevice<ERearInCvSetLoadInput> _rearCvSetLoadInput;
        private readonly IDOutputDevice<ERearCvSetUnloadOutput> _rearCvSetUnloadOutput;

        // 7. VisionProcess
        private readonly IDInputDevice<EVisionProcessInput> _visionProcessInput;
        private readonly IDOutputDevice<EVisionProcessOutput> _visionProcessOutput;

        public VirtualIO([FromKeyedServices("TrayInCvInput")] IDInputDevice<ETrayInCvInput> trayInCvInput,
                        [FromKeyedServices("TrayInCvOutput")] IDOutputDevice<ETrayInCvOutput> trayInCvOutput,
                        [FromKeyedServices("TrayInElevatorInput")] IDInputDevice<ETrayInElevatorInput> trayInElevatorInput,
                        [FromKeyedServices("TrayInElevatorOutput")] IDOutputDevice<ETrayInElevatorOutput> trayInElevatorOutput,
                        [FromKeyedServices("TrayOutCvInput")] IDInputDevice<ETrayOutCvInput> trayOutCvInput,
                        [FromKeyedServices("TrayOutCvOutput")] IDOutputDevice<ETrayOutCvOutput> trayOutCvOutput,
                        [FromKeyedServices("TrayOutElevatorInput")] IDInputDevice<ETrayOutElevatorInput> trayOutElevatorInput,
                        [FromKeyedServices("TrayOutElevatorOutput")] IDOutputDevice<ETrayOutElevatorOutput> trayOutElevatorOutput,
                        [FromKeyedServices("TrayHeadInput")] IDInputDevice<ETrayHeadInput> trayHeadInput,
                        [FromKeyedServices("TrayHeadOutput")] IDOutputDevice<ETrayHeadOutput> trayHeadOutput,
                        [FromKeyedServices("CameraFlipperInput")] IDInputDevice<ECameraFlipperInput> cameraFlipperInput,
                        [FromKeyedServices("CameraFlipperOutput")] IDOutputDevice<ECameraFlipperOutput> cameraFlipperOutput,
                        [FromKeyedServices("SpongeDetachInput")] IDInputDevice<ESpongeDetachInput> tapeDetachInput,
                        [FromKeyedServices("SpongeDetachOutput")] IDOutputDevice<ESpongeDetachOutput> tapeDetachOutput,
                        [FromKeyedServices("CameraAssembleHeadInput")] IDInputDevice<ECameraAssembleHeadInput> cameraAssembleHeadInput,
                        [FromKeyedServices("CameraAssembleHeadOutput")] IDOutputDevice<ECameraAssembleHeadOutput> cameraAssembleHeadOutput,
                        [FromKeyedServices("FilmDetachInput")] IDInputDevice<EFilmDetachInput> filmDetachInput,
                        [FromKeyedServices("FilmDetachOutput")] IDOutputDevice<EFilmDetachOutput> filmDetachOutput,
                        [FromKeyedServices("FrontCvCamAssembleInput")] IDInputDevice<EFrontCvCamAssembleInput> frontCvCamAssembleInput,
                        [FromKeyedServices("FrontCvCamAssembleOutput")] IDOutputDevice<EFrontCvCamAssembleOutput> frontCvCamAssembleOutput,
                        [FromKeyedServices("FrontCvFilmDetachInput")] IDInputDevice<EFrontCvFilmDetachInput> frontCvFilmDetachInput,
                        [FromKeyedServices("FrontCvFilmDetachOutput")] IDOutputDevice<EFrontCvFilmDetachOutput> frontCvFilmDetachOutput,
                        [FromKeyedServices("FrontCvSetLoadInput")] IDInputDevice<EFrontInCvSetLoadInput> frontCvSetLoadInput,
                        [FromKeyedServices("FrontCvSetUnloadOutput")] IDOutputDevice<EFrontCvSetUnloadOutput> frontCvSetUnloadOutput,
                        [FromKeyedServices("RearCvCamAssembleInput")] IDInputDevice<ERearCvCamAssembleInput> rearCvCamAssembleInput,
                        [FromKeyedServices("RearCvCamAssembleOutput")] IDOutputDevice<ERearCvCamAssembleOutput> rearCvCamAssembleOutput,
                        [FromKeyedServices("RearCvFilmDetachInput")] IDInputDevice<ERearCvFilmDetachInput> rearCvFilmDetachInput,
                        [FromKeyedServices("RearCvFilmDetachOutput")] IDOutputDevice<ERearCvFilmDetachOutput> rearCvFilmDetachOutput,
                        [FromKeyedServices("RearCvSetLoadInput")] IDInputDevice<ERearInCvSetLoadInput> rearCvSetLoadInput,
                        [FromKeyedServices("RearCvSetUnloadOutput")] IDOutputDevice<ERearCvSetUnloadOutput> rearCvSetUnloadOutput,
                        [FromKeyedServices("VisionProcessInput")] IDInputDevice<EVisionProcessInput> visionProcessInput,
                        [FromKeyedServices("VisionProcessOutput")] IDOutputDevice<EVisionProcessOutput> visionProcessOutput)
        {
            // 1. TraySupplier
            _trayInCvInput = trayInCvInput;
            _trayInCvOutput = trayInCvOutput;
            _trayInElevatorInput = trayInElevatorInput;
            _trayInElevatorOutput = trayInElevatorOutput;
            _trayOutCvInput = trayOutCvInput;
            _trayOutCvOutput = trayOutCvOutput;
            _trayOutElevatorInput = trayOutElevatorInput;
            _trayOutElevatorOutput = trayOutElevatorOutput;

            // 2. TrayHead
            _trayHeadInput = trayHeadInput;
            _trayHeadOutput = trayHeadOutput;

            // 3. FlipperSpongeDetach
            _cameraFlipperInput = cameraFlipperInput;
            _cameraFlipperOutput = cameraFlipperOutput;
            _tapeDetachInput = tapeDetachInput;
            _tapeDetachOutput = tapeDetachOutput;

            // 4. CameraAssembleHead
            _cameraAssembleHeadInput = cameraAssembleHeadInput;
            _cameraAssembleHeadOutput = cameraAssembleHeadOutput;

            // 5. FilmDetachHead
            _filmDetachInput = filmDetachInput;
            _filmDetachOutput = filmDetachOutput;

            // 6. SetCV
            _frontCvCamAssembleInput = frontCvCamAssembleInput;
            _frontCvCamAssembleOutput = frontCvCamAssembleOutput;
            _frontCvFilmDetachInput = frontCvFilmDetachInput;
            _frontCvFilmDetachOutput = frontCvFilmDetachOutput;
            _frontCvSetLoadInput = frontCvSetLoadInput;
            _frontCvSetUnloadOutput = frontCvSetUnloadOutput;
            _rearCvCamAssembleInput = rearCvCamAssembleInput;
            _rearCvCamAssembleOutput = rearCvCamAssembleOutput;
            _rearCvFilmDetachInput = rearCvFilmDetachInput;
            _rearCvFilmDetachOutput = rearCvFilmDetachOutput;
            _rearCvSetLoadInput = rearCvSetLoadInput;
            _rearCvSetUnloadOutput = rearCvSetUnloadOutput;

            // 7. VisionProcess
            _visionProcessInput = visionProcessInput;
            _visionProcessOutput = visionProcessOutput;
        }

        public void Initialize()
        {
            // 1. TraySupplier
            _trayInCvInput.Initialize();
            _trayInCvOutput.Initialize();
            _trayInElevatorInput.Initialize();
            _trayInElevatorOutput.Initialize();
            _trayOutCvInput.Initialize();
            _trayOutCvOutput.Initialize();
            _trayOutElevatorInput.Initialize();
            _trayOutElevatorOutput.Initialize();

            // 2. TrayHead
            _trayHeadInput.Initialize();
            _trayHeadOutput.Initialize();

            // 3. FlipperSpongeDetach
            _cameraFlipperInput.Initialize();
            _cameraFlipperOutput.Initialize();
            _tapeDetachInput.Initialize();
            _tapeDetachOutput.Initialize();

            // 4. CameraAssembleHead
            _cameraAssembleHeadInput.Initialize();
            _cameraAssembleHeadOutput.Initialize();

            // 5. FilmDetachHead
            _filmDetachInput.Initialize();
            _filmDetachOutput.Initialize();

            // 6. SetCV
            _frontCvCamAssembleInput.Initialize();
            _frontCvCamAssembleOutput.Initialize();
            _frontCvFilmDetachInput.Initialize();
            _frontCvFilmDetachOutput.Initialize();
            _frontCvSetLoadInput.Initialize();
            _frontCvSetUnloadOutput.Initialize();
            _rearCvCamAssembleInput.Initialize();
            _rearCvCamAssembleOutput.Initialize();
            _rearCvFilmDetachInput.Initialize();
            _rearCvFilmDetachOutput.Initialize();
            _rearCvSetLoadInput.Initialize();
            _rearCvSetUnloadOutput.Initialize();

            // 7. VisionProcess
            _visionProcessInput.Initialize();
            _visionProcessOutput.Initialize();
        }

        public void Mappings()
        {
            // Tray In CV
            _trayInCvInput[ETrayInCvInput.TRAY_INPUT_ELEVATOR_REQUEST].
                MapTo(_trayInElevatorOutput[ETrayInElevatorOutput.TRAY_INPUT_ELEVATOR_REQUEST]);

            // Tray In Elevator
            _trayInElevatorInput[ETrayInElevatorInput.TRAY_IN_ELEVATOR_DONE]
                .MapTo(_trayInCvOutput[ETrayInCvOutput.SUPPLY_TRAY_INPUT_ELEVATOR_DONE]);
            _trayInElevatorInput[ETrayInElevatorInput.TRAY_IN_ELEVATOR_TRAYHEAD_VAC_ON]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TRAY_IN_ELEVATOR_TRAYHEAD_VAC_ON]);
            _trayInElevatorInput[ETrayInElevatorInput.TRAY_IN_ELEVATOR_UNLOAD_TRAY_DONE]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TRAY_IN_ELEVATOR_UNLOAD_TRAY_DONE]);
            _trayInElevatorInput[ETrayInElevatorInput.TRAY_IN_ELEVATOR_CAM_UNLOAD_DONE]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TRAY_IN_ELEVATOR_CAM_UNLOAD_DONE]);
            _trayInElevatorInput[ETrayInElevatorInput.TRAYHEAD_CAM_UNLOAD_PICK_FAIL]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TRAYHEAD_CAM_UNLOAD_PICK_FAIL]);
            _trayInElevatorInput[ETrayInElevatorInput.TRAYHEAD_Z_UP_DONE]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TRAYHEAD_Z_UP_DONE]);
            _trayInElevatorInput[ETrayInElevatorInput.TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED]);

            // Tray Out CV
            _trayOutCvInput[ETrayOutCvInput.TRAY_OUT_ELEVATOR_REQUEST]
                .MapTo(_trayOutElevatorOutput[ETrayOutElevatorOutput.TRAY_OUT_ELEVATOR_REQUEST]);

            // Tray Out Elevator
            _trayOutElevatorInput[ETrayOutElevatorInput.TRAY_OUT_ELEVATOR_READY]
                .MapTo(_trayOutCvOutput[ETrayOutCvOutput.TRAY_OUT_ELEVATOR_READY]);
            _trayOutElevatorInput[ETrayOutElevatorInput.TRAY_OUT_ELEVATOR_PLACE_DONE]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TRAY_OUT_ELEVATOR_PLACE_DONE]);

            // Tray Head
            _trayHeadInput[ETrayHeadInput.TRAY_IN_ELEVATOR_UNLOAD_TRAY_REQ]
                .MapTo(_trayInElevatorOutput[ETrayInElevatorOutput.TRAY_IN_ELEVATOR_UNLOAD_TRAY_REQ]);
            _trayHeadInput[ETrayHeadInput.TRAY_IN_ELEVATOR_UNLOAD_CAM_REQ]
                .MapTo(_trayInElevatorOutput[ETrayInElevatorOutput.TRAY_IN_ELEVATOR_UNLOAD_CAM_REQ]);
            _trayHeadInput[ETrayHeadInput.TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED]
                .MapTo(_trayInElevatorOutput[ETrayInElevatorOutput.TRAY_IN_ELEVATOR_UNLOAD_CAM_DONE_RECEIVED]);
            _trayHeadInput[ETrayHeadInput.TRAY_OUT_ELEVATOR_READY_PLACE]
                .MapTo(_trayOutElevatorOutput[ETrayOutElevatorOutput.TRAY_OUT_ELEVATOR_READY_PLACE]);
            _trayHeadInput[ETrayHeadInput.TAPE_DETACH_CAM_IN_REQ]
                .MapTo(_tapeDetachOutput[ESpongeDetachOutput.TAPE_DETACH_CAM_IN_REQ]);
            _trayHeadInput[ETrayHeadInput.TRAY_IN_ELEVATOR_UNALIGN_DONE]
                .MapTo(_trayInElevatorOutput[ETrayInElevatorOutput.TRAY_IN_ELEVATOR_UNALIGN_DONE]);
            _trayHeadInput[ETrayHeadInput.TRAY_HEAD_SCAN_BARCODE_ERROR]
                .MapTo(_visionProcessOutput[EVisionProcessOutput.SCAN_BARCODE_ERROR]);

            _trayHeadInput[ETrayHeadInput.TRAY_HEAD_SCAN_BARCODE_RUN]
                .MapTo(_visionProcessOutput[EVisionProcessOutput.VISION_INSPECTION_RUN]);
            // Sponge Detach 
            _tapeDetachInput[ESpongeDetachInput.TRAYHEAD_OUT_OF_PLACE_AREA]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TRAYHEAD_OUT_OF_PLACE_AREA]);
            _tapeDetachInput[ESpongeDetachInput.TRAYHEAD_Z_UP_DONE]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TRAYHEAD_Z_UP_DONE]);
            _tapeDetachInput[ESpongeDetachInput.TAPE_DETACH_CAM_IN_DONE]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TAPE_DETACH_CAM_IN_DONE]);
            _tapeDetachInput[ESpongeDetachInput.GRIP_ON_DONE]
                .MapTo(_cameraFlipperOutput[ECameraFlipperOutput.GRIPER_ON_DONE]);
            _tapeDetachInput[ESpongeDetachInput.CAM_TAPE_DETACH_OUT_DONE]
                .MapTo(_cameraFlipperOutput[ECameraFlipperOutput.CAM_PICKUP_DONE]);
            _tapeDetachInput[ESpongeDetachInput.FLIPPER_GRIPPER_OFF_DONE]
                .MapTo(_cameraFlipperOutput[ECameraFlipperOutput.FLIPPER_GRIPPER_OFF_DONE]);


            // Camera Flipper
            _cameraFlipperInput[ECameraFlipperInput.TRAYHEAD_OUT_OF_PLACE_AREA]
                .MapTo(_trayHeadOutput[ETrayHeadOutput.TRAYHEAD_Z_UP_DONE]);
            _cameraFlipperInput[ECameraFlipperInput.FLIPPER_IN_REQUEST]
                .MapTo(_tapeDetachOutput[ESpongeDetachOutput.FLIPPER_IN_REQUEST]);
            _cameraFlipperInput[ECameraFlipperInput.TAPE_REMOVE_DONE]
                .MapTo(_tapeDetachOutput[ESpongeDetachOutput.TAPE_REMOVE_DONE]);
            _cameraFlipperInput[ECameraFlipperInput.CAM_OUT_DONE]
                .MapTo(_cameraAssembleHeadOutput[ECameraAssembleHeadOutput.CAM_PICKUP_DONE]);
            _cameraFlipperInput[ECameraFlipperInput.CAMHEAD_VAC_ON_OK]
                .MapTo(_cameraAssembleHeadOutput[ECameraAssembleHeadOutput.VAC_ON_OK]);
            _cameraFlipperInput[ECameraFlipperInput.FLIPPER_WORK_REQUEST]
                .MapTo(_tapeDetachOutput[ESpongeDetachOutput.FLIPPER_WORK_REQUEST]);
            _cameraFlipperInput[ECameraFlipperInput.CAM_ASSEMBLE_HEAD_READY_DONE]
                .MapTo(_cameraAssembleHeadOutput[ECameraAssembleHeadOutput.CAM_ASSEMBLE_HEAD_READY_DONE]);

            // Camera Assemble Head
            _cameraAssembleHeadInput[ECameraAssembleHeadInput.GRIPPER_OFF_DONE]
                .MapTo(_cameraFlipperOutput[ECameraFlipperOutput.GRIPER_OFF_DONE]);
            _cameraAssembleHeadInput[ECameraAssembleHeadInput.FLIPPER_CAM_OUT_REQUEST]
                .MapTo(_cameraFlipperOutput[ECameraFlipperOutput.CAM_OUT_REQUEST]);
            _cameraAssembleHeadInput[ECameraAssembleHeadInput.FRONT_CAM_ASSEMBLE_REQUEST]
                .MapTo(_frontCvCamAssembleOutput[EFrontCvCamAssembleOutput.FRONT_CAM_ASSEMBLE_REQUEST]);
            _cameraAssembleHeadInput[ECameraAssembleHeadInput.REAR_CAM_ASSEMBLE_REQUEST]
                .MapTo(_rearCvCamAssembleOutput[ERearCvCamAssembleOutput.REAR_CAM_ASSEMBLE_REQUEST]);
            _cameraAssembleHeadInput[ECameraAssembleHeadInput.VISION_RUNNING]
                .MapTo(_visionProcessOutput[EVisionProcessOutput.VISION_INSPECTION_RUN]);

            // Film Detach Head
            _filmDetachInput[EFilmDetachInput.FRONT_FILM_DETACH_REQUEST]
                .MapTo(_frontCvFilmDetachOutput[EFrontCvFilmDetachOutput.FRONT_FILM_DETACH_REQUEST]);
            _filmDetachInput[EFilmDetachInput.REAR_FILM_DETACH_REQUEST]
                .MapTo(_rearCvFilmDetachOutput[ERearCvFilmDetachOutput.REAR_FILM_DETACH_REQUEST]);
            _filmDetachInput[EFilmDetachInput.FRONT_FILM_DETACH_START_WORK_REQUEST]
                .MapTo(_frontCvFilmDetachOutput[EFrontCvFilmDetachOutput.FRONT_FILM_DETACH_START_WORK_REQUEST]);
            _filmDetachInput[EFilmDetachInput.REAR_FILM_DETACH_START_WORK_REQUEST]
                .MapTo(_rearCvFilmDetachOutput[ERearCvFilmDetachOutput.REAR_FILM_DETACH_START_WORK_REQUEST]);

            // Input Load Front CV
            _frontCvSetLoadInput[EFrontInCvSetLoadInput.FRONT_DETACH_LOAD_REQUEST]
                .MapTo(_frontCvFilmDetachOutput[EFrontCvFilmDetachOutput.FRONT_DETACH_LOAD_REQUEST]);
            _frontCvSetLoadInput[EFrontInCvSetLoadInput.FRONT_DETACH_LOAD_DONE]
                .MapTo(_frontCvFilmDetachOutput[EFrontCvFilmDetachOutput.FRONT_DETACH_LOAD_DONE]);

            // Film Detach Front CV
            _frontCvFilmDetachInput[EFrontCvFilmDetachInput.FILM_DETACH_DONE]
                .MapTo(_filmDetachOutput[EFilmDetachOutput.FRONT_FILM_DETACH_DONE]);
            _frontCvFilmDetachInput[EFrontCvFilmDetachInput.FRONT_ASSEMBLE_LOAD_REQUEST]
                .MapTo(_frontCvCamAssembleOutput[EFrontCvCamAssembleOutput.FRONT_ASSEMBLE_LOAD_REQUEST]);


            // Cam Assemble Front CV
            _frontCvCamAssembleInput[EFrontCvCamAssembleInput.CAM_ASSEMBLE_DONE]
                .MapTo(_cameraAssembleHeadOutput[ECameraAssembleHeadOutput.CAM_ASSEMBLE_FRONT_DONE]);
            _frontCvCamAssembleInput[EFrontCvCamAssembleInput.CAM_ASSEMBLE_FRONT_WAIT_PUSH]
                .MapTo(_cameraAssembleHeadOutput[ECameraAssembleHeadOutput.CAM_ASSEMBLE_FRONT_PLACE_DONE]);
            _frontCvCamAssembleInput[EFrontCvCamAssembleInput.FRONT_UNLOAD_REQUEST]
                .MapTo(_frontCvSetUnloadOutput[EFrontCvSetUnloadOutput.FRONT_UNLOAD_REQUEST]);
            _frontCvCamAssembleInput[EFrontCvCamAssembleInput.VISION_FRONT_FILM_INSPECTION_ERROR]
                .MapTo(_visionProcessOutput[EVisionProcessOutput.VISION_FRONT_FILM_INSPECTION_ERROR]);
            _frontCvCamAssembleInput[EFrontCvCamAssembleInput.VISION_FRONT_ASSEMBLE_INSPECTION_ERROR]
                .MapTo(_visionProcessOutput[EVisionProcessOutput.VISION_FRONT_ASSEMBLE_INSPECTION_ERROR]);
            _frontCvCamAssembleInput[EFrontCvCamAssembleInput.VISION_INSPECTION_RUN]
                .MapTo(_visionProcessOutput[EVisionProcessOutput.VISION_INSPECTION_RUN]);
            _frontCvCamAssembleInput[EFrontCvCamAssembleInput.CAM_ASSEMBLE_AVOID_TO_VISION]
                .MapTo(_cameraAssembleHeadOutput[ECameraAssembleHeadOutput.CAM_ASSEMBLE_AVOID_TO_VISION_FRONT]);

            // Input Load Rear CV
            _rearCvSetLoadInput[ERearInCvSetLoadInput.REAR_DETACH_LOAD_REQUEST]
                .MapTo(_rearCvFilmDetachOutput[ERearCvFilmDetachOutput.REAR_DETACH_LOAD_REQUEST]);
            _rearCvSetLoadInput[ERearInCvSetLoadInput.REAR_DETACH_LOAD_DONE]
                .MapTo(_rearCvFilmDetachOutput[ERearCvFilmDetachOutput.REAR_DETACH_LOAD_DONE]);

            // Film Detach Rear CV
            _rearCvFilmDetachInput[ERearCvFilmDetachInput.FILM_DETACH_DONE]
                .MapTo(_filmDetachOutput[EFilmDetachOutput.REAR_FILM_DETACH_DONE]);
            _rearCvFilmDetachInput[ERearCvFilmDetachInput.REAR_ASSEMBLE_LOAD_REQUEST]
                .MapTo(_rearCvCamAssembleOutput[ERearCvCamAssembleOutput.REAR_ASSEMBLE_LOAD_REQUEST]);

            // Cam Assemble Rear CV
            _rearCvCamAssembleInput[ERearCvCamAssembleInput.CAM_ASSEMBLE_REAR_WAIT_PUSH]
                .MapTo(_cameraAssembleHeadOutput[ECameraAssembleHeadOutput.CAM_ASSEMBLE_REAR_PLACE_DONE]);
            _rearCvCamAssembleInput[ERearCvCamAssembleInput.CAM_ASSEMBLE_DONE]
                .MapTo(_cameraAssembleHeadOutput[ECameraAssembleHeadOutput.CAM_ASSEMBLE_REAR_DONE]);
            _rearCvCamAssembleInput[ERearCvCamAssembleInput.REAR_UNLOAD_REQUEST]
                .MapTo(_rearCvSetUnloadOutput[ERearCvSetUnloadOutput.REAR_UNLOAD_REQUEST]);
            _rearCvCamAssembleInput[ERearCvCamAssembleInput.VISION_REAR_FILM_INSPECTION_ERROR]
                .MapTo(_visionProcessOutput[EVisionProcessOutput.VISION_REAR_FILM_INSPECTION_ERROR]);
            _rearCvCamAssembleInput[ERearCvCamAssembleInput.VISION_REAR_ASSEMBLE_INSPECTION_ERROR]
                .MapTo(_visionProcessOutput[EVisionProcessOutput.VISION_REAR_ASSEMBLE_INSPECTION_ERROR]);
            _rearCvCamAssembleInput[ERearCvCamAssembleInput.VISION_INSPECTION_RUN]
                .MapTo(_visionProcessOutput[EVisionProcessOutput.VISION_INSPECTION_RUN]);
            _rearCvCamAssembleInput[ERearCvCamAssembleInput.CAM_ASSEMBLE_AVOID_TO_VISION]
                .MapTo(_cameraAssembleHeadOutput[ECameraAssembleHeadOutput.CAM_ASSEMBLE_AVOID_TO_VISION_REAR]);
            // Vision Process
        }
    }
}
