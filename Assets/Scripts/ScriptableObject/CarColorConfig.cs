using UnityEngine;

namespace ProjectCar
{
    namespace Configs
    {
        [CreateAssetMenu(fileName = "Car colors", menuName = "Config/Car Colors", order = 52)]
        public sealed class CarColorsConfig : ScriptableObject
        {
            [SerializeField] private string _carName;
            [SerializeField] private Material[] _carMaterials;
            [SerializeField] private string[] _carMaterialsName;
        }
    }
}