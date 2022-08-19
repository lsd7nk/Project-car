using UnityEngine;

namespace ProjectCar
{
    namespace Configs
    {
        [CreateAssetMenu(fileName = "Training obj", menuName = "Config/Training object", order = 52)]
        public sealed class TrainingObjConfig : ScriptableObject
        {
            [SerializeField] private string _name;
            [SerializeField] private string _description;

            public string Name => _name;
            public string Description => _description;
        }
    }
}
