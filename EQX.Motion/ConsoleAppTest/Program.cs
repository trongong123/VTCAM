using EQX.Motion;
using Newtonsoft.Json;

List<MotionAjinParameter> motionParameters = new List<MotionAjinParameter>()
{
    new MotionAjinParameter(),
    new MotionAjinParameter(),
    new MotionAjinParameter(),
    new MotionAjinParameter(),
};


File.WriteAllText("MotionPara.config", JsonConvert.SerializeObject(motionParameters, Formatting.Indented));

File.WriteAllText("VinylCleanEncoder.config", JsonConvert.SerializeObject(new MotionParameter(), Formatting.Indented));
