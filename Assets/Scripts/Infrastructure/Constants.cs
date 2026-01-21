using UnityEngine;

public static class Constants
{
    public const string ComputerViewSceneName = "ComputerViewScene";
    public const string MainSceneName = "MainScene";
    public const string MainMenuSceneName = "MenuScene";
    public const string InitialSceneName = "InitScene";
    public const KeyCode InteractionKeyCode = KeyCode.E;
    public const KeyCode GuideKeyCode = KeyCode.LeftAlt;
    
    public const string PlayerPrefabPath = "Prefabs/Player";
    public const string InfoCanvasPrefabPath = "Prefabs/InfoCanvas";
    public const string ComputerPath = "Prefabs/Computer Monitor OLD";
    public const string DiagramViewPath = "Prefabs/DiagramView";
    public const string FirstStageBlockerPath = "Prefabs/FirstStageBlocker";
    public const string FraungoferDistanceCanvasPath = "Prefabs/FraungoferDistanceCanvas";
    public const string AttentionBlockerPath = "Prefabs/AttentionBlocker";
    public const string GeneratorPath = "Prefabs/Generator_fixed";
    public const string VectorAnalyzerPath = "Prefabs/Vectornik";
    public const string MeasurableAntennaPath = "Prefabs/MeasurableAntenna";
    public const string AntennaRotatorPath = "Prefabs/AntennaRotationCanvas";
    public const string PostProcessingPrefabPath = "Prefabs/Post-process Volume";
    public const string GuideCameraPath = "Prefabs/GuideCamera";

    public const string FraungoferMeasurementStageDataFileID = "FraungoferMeasurementStage";
    public const string AttentionStageDataFileID = "AttentionStage";
    public const string GeneratorActivationStageDataFileID = "GeneratorActivationStage";
    public const string FrequencySetupStageDataID = "FrequencySetupStageData";
    public const string VectorAnalyzerStageDataID = "VectorAnalyzerActivationStage";
    public const string MeasurementStageID = "MeasurementStageData";
    public const string DiagramViewStageID = "DiagramsViewStage";
    
    public const float ComputerPositionX = 2.106f;
    public const float ComputerPositionY = 0.706f;
    public const float ComputerPositionZ = 0.712f;
    public const float ComputerRotationX = -90.502f;
    public const float ComputerRotationY = -83.082f;
    public const float ComputerRotationZ = -10.41699f;
    
    public static float MouseSensitivity = 1f;
    public const float HintAnimationSpeed = 400f;
    
    public static bool IsBloomOn = true;
    public static bool IsAOOn = true;
    public static bool IsDepthOfFieldOn = true;
    
    public static Vector3 FirstStageBlockerPosition => new Vector3(0.371f, 2.224f, -0.18f);
    public static Vector3 FraungoferDistanceCanvasPosition => new Vector3(7.406f, 1.927499f, -0.7318f);
    public static Vector3 FraungoferDistanceCanvasRotation => new Vector3(0, 90, 0);
    public static Vector3 FirstStageBlockerRotation => new Vector3(-90, 90, 0);
    public static Vector3 CameraDefaultPosition => new Vector3(0, 0.711f, 0);
    public static Vector3 AttentionBlockerPosition => new Vector3(0.732f, 1.998f, 0.1439f);
    public static Vector3 AttentionBlockerRotation => new Vector3(0, 0, 90f);
    public static Vector3 GeneratorPosition => new Vector3(2.0927f, 0.846f, 1.9963f);
    public static Vector3 GeneratorRotation => new Vector3(-89.98f, 0, 0);
    public static Vector3 VectorAnalyzerPosition => new Vector3(2.037f, 0.747f, -0.815f);
    public static Vector3 VectorAnalyzerRotation => new Vector3(-90f, 0, -90.874f);
    public static Vector3 MeasurableAntennaPosition => new Vector3(6.319f, 1.267f, -5.658f);
    public static Vector3 MeasurableAntennaRotation => new Vector3(-90f, 0f, 270f);
    public static Vector3 AntennaRotatorPosition => new Vector3(1.745f, 1.658f, -2.874f);
    public static Vector3 AntennaRotatorRotation => new Vector3(0, 90f, 0);
    
    public static char[] ShufflerSymbols = new char[]
    {
        'A', 'B', 'C', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'W',
        'X', 'Y', 'Z', '#', '@', '!', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '/'
    };
}