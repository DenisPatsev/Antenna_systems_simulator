using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingSettingsController : MonoBehaviour
{
    [SerializeField] private PostProcessProfile _postProcessVolume;

    private void OnEnable()
    {
        _postProcessVolume.GetSetting<Bloom>().enabled.value = Constants.IsBloomOn;
        _postProcessVolume.GetSetting<AmbientOcclusion>().enabled.value = Constants.IsAOOn;
        _postProcessVolume.GetSetting<DepthOfField>().enabled.value = Constants.IsDepthOfFieldOn;
    }
}