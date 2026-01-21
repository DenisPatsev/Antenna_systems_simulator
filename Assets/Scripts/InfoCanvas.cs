using TMPro;
using UnityEngine;

public class InfoCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text _objectInfoText;
    [SerializeField] private TMP_Text _hintText;
    
     private GameObject _player;

    private void Update()
    {
        Vector3 direction = _player.transform.position - transform.position;
        direction.y = 0; 
        transform.rotation = Quaternion.LookRotation(-direction);
    }

    public void DisplayInfo(string info, Vector3 position)
    {
        gameObject.SetActive(true);
        _objectInfoText.text = info;
        _hintText.text = "Нажмите 'E' для взаимодействия";

        transform.position = position;
    }

    public void HideInfo()
    {
        gameObject.SetActive(false);
    }

    public void InitializePlayer(GameObject player)
    {
        _player = player;
    }
}