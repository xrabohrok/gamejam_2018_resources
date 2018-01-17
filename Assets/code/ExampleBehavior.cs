using UnityEngine;

namespace Assets.code
{
    [RequireComponent(typeof(BaseUnitAI))]
    class ExampleBehavior: MonoBehaviour
    {
        public Material UnselectedMaterial;
        public Material SelectedMaterial;
        private BaseUnitAI aiHandle;
        private Renderer myRenderer;

        public void Start()
        {
            aiHandle = this.GetComponent<BaseUnitAI>();
            myRenderer = this.GetComponent<Renderer>();
        }

        public void Update()
        {
            if (aiHandle.IsSelected)
            {
                myRenderer.sharedMaterials = new[] {SelectedMaterial};
            }
            else
            {
                myRenderer.sharedMaterials = new[] {UnselectedMaterial};
            }
        }
    }
}
