using UnityEngine;
using UnityEngine.UI;

namespace NekoLegends
{
    public class NekoMewDemo : DemoScenes
    {

        [SerializeField] private NekoMew NekoCharacter;

        [Space]
        [SerializeField] private Button AnimBtn;
        [SerializeField] private Button CameraBtn;
        [SerializeField] private Button OutlineBtn;
        [SerializeField] private Button RotateBtn;
        [Space]
        [SerializeField] private Button FurBtn;
        [SerializeField] private Button EyesBtn;
        [SerializeField] private Button EmotionBtn;
        [SerializeField] private Button MiscBtn;

        [Space]
        [SerializeField] private Transform MainCamTransform;
        [SerializeField] private Transform ZoomedCamTransform;

        private const string _title = "Neko Mew";
        private bool isRotating;

        #region Singleton
        public static new NekoMewDemo Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType(typeof(NekoMewDemo)) as NekoMewDemo;

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }
        private static NekoMewDemo _instance;
        #endregion


        protected override void OnEnable()
        {
            RegisterButtonAction(AnimBtn, () => NekoCharacter.AnimPose());
            RegisterButtonAction(EmotionBtn, () => this.NekoCharacter.CycleEmotion());
            if(CameraBtn)
                RegisterButtonAction(CameraBtn, () => CameraBtnClicked());
            if (RotateBtn)
                RegisterButtonAction(RotateBtn, () => isRotating = !isRotating);
            RegisterButtonAction(OutlineBtn, () => NekoCharacter.CycleOutlines());
            RegisterButtonAction(FurBtn, () => NekoCharacter.CycleFur());
            RegisterButtonAction(EyesBtn, () => this.NekoCharacter.CycleEyes());
            RegisterButtonAction(MiscBtn, () => this.NekoCharacter.CycleMisc());

            base.OnEnable();

        }



        protected override void Start()
        {   
            base.Start();
            if(CameraBtn)
                CameraBtnClicked();//start zoomed since small neko
            DescriptionText.SetText(_title);
        }

        private void CameraBtnClicked()
        {
            this.FlyToNextCameraHandler();
        }


        void Update()
        {
            if (isRotating)
            {
                float rotationSpeed = 50f;
                NekoCharacter.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
        }

    }
}
