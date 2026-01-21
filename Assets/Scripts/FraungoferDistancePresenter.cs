using TMPro;
using UnityEngine;

public class FraungoferDistancePresenter : MonoBehaviour
{
   [SerializeField] private TMP_Text _distanceText;

   public void SetDistance(string distance)
   {
      _distanceText.text = distance + " m";
   }
}
