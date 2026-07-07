namespace EQX.Motion
{
    public class MotionAjinParameter : MotionParameter
    {
        //++ 지정 축의 펄스 출력 방식을 설정합니다.
        //uMethod : (0)OneHighLowHigh   - 1펄스 방식, PULSE(Active High), 정방향(DIR=Low)  / 역방향(DIR=High)
        //          (1)OneHighHighLow   - 1펄스 방식, PULSE(Active High), 정방향(DIR=High) / 역방향(DIR=Low)
        //          (2)OneLowLowHigh    - 1펄스 방식, PULSE(Active Low),  정방향(DIR=Low)  / 역방향(DIR=High)
        //          (3)OneLowHighLow    - 1펄스 방식, PULSE(Active Low),  정방향(DIR=High) / 역방향(DIR=Low)
        //          (4)TwoCcwCwHigh     - 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active High     
        //          (5)TwoCcwCwLow      - 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active Low     
        //          (6)TwoCwCcwHigh     - 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active High
        //          (7)TwoCwCcwLow      - 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active Low
        //          (8)TwoPhase         - 2상(90' 위상차),  PULSE lead DIR(CW: 정방향), PULSE lag DIR(CCW:역방향)
        //          (9)TwoPhaseReverse  - 2상(90' 위상차),  PULSE lead DIR(CCW: 정방향), PULSE lag DIR(CW:역방향)
        public uint PulseOutput { get; set; }

        //++ 지정 축의 Encoder 입력 방식을 설정합니다.
        // uMethod : (0)ObverseUpDownMode - 정방향 Up/Down
        //           (1)ObverseSqr1Mode   - 정방향 1체배
        //           (2)ObverseSqr2Mode   - 정방향 2체배
        //           (3)ObverseSqr4Mode   - 정방향 4체배
        //           (4)ReverseUpDownMode - 역방향 Up/Down
        //           (5)ReverseSqr1Mode   - 역방향 1체배
        //           (6)ReverseSqr2Mode   - 역방향 2체배
        //           (7)ReverseSqr4Mode   - 역방향 4체배
        public uint EncoderInput { get; set; }

        // uProfileMode : (0)SYM_TRAPEZOID_MODE  - Symmetric Trapezoid
        //                (1)ASYM_TRAPEZOID_MODE - Asymmetric Trapezoid
        //                (2)QUASI_S_CURVE_MODE  - Symmetric Quasi-S Curve
        //                (3)SYM_S_CURVE_MODE    - Symmetric S Curve
        //                (4)ASYM_S_CURVE_MODE   - Asymmetric S Curve
        public uint ProfileMode { get; set; }

        public double MinVelocity { get; set; }

        public uint AccelUnit { get; set; }
        
        public uint ServoOnLevel { get; set; }
        public uint ServoAlarmLevel { get; set; }
        public uint ServoInposLevel { get; set; }
        public uint ZPhaseLevel { get; set; }
        public uint HomeSignalLevel { get; set; }
        public uint PositiveLevel { get; set; }
        public uint NegativeLevel { get; set; }

        #region Home parameter
        public int HomeDirect { get; set; }
        public uint HomeSignal { get; set; }
        public uint HomeZPhaseUse { get; set; }
        public double HomeClearTime { get; set; }
        public double HomeOffset { get; set; }
        public double HomeVelFirst { get; set; }
        public double HomeVelSecond { get; set; }
        public double HomeVelThird { get; set; }
        public double HomeVelLast { get; set; }
        public double HomeAccFirst { get; set; }
        public double HomeAccSecond { get; set; }
        #endregion
    }
}
