//using Engine;
//
//

//namespace Scripts
//{
//    class ModelRenderer : Renderer
//    {
//        private Texture2D texture;
//        [System.ComponentModel.Editor(typeof(Editor.TextureEditor), typeof(System.Drawing.Design.UITypeEditor))]
//        [System.Xml.Serialization.XmlIgnore] [ShowInEditor] public Texture2D Texture { get { return texture; } set { texturePath = value.Name; texture = value; } }
//        [System.Xml.Serialization.XmlIgnore] [ShowInEditor] public Model model { get; set; }

//        private string texturePath;
//        private string modelPath;
//        public void LoadModel(string path)
//        {
//            modelPath = path;
//            model = Scene.Instance.Content.Load<Model>(path);

//        }
//        public override void Awake()
//        {
//            if (texture == null && texturePath != null)
//            {
//                var scene = Scene.Instance;
//                System.IO.Stream stream = TitleContainer.OpenStream(texturePath);
//                texture = Texture2D.FromStream(scene.GraphicsDevice, stream);
//                stream.Close();
//            }
//            if (modelPath != null)
//            {
//                LoadModel(modelPath);
//            }
//            base.Awake();
//        }
//        public override void Draw(SpriteBatch batch)
//        {
//            if (GameObject == null || model == null) { return; }
//            foreach (var mesh in model.Meshes)
//            {
//                // "Effect" refers to a shader. Each mesh may
//                // have multiple shaders applied to it for more
//                // advanced visuals. 
//                foreach (BasicEffect effect in mesh.Effects)
//                {

//                    // The world matrix can be used to position, rotate
//                    // or resize (scale) the model. Identity means that
//                    // the model is unrotated, drawn at the origin, and
//                    // its size is unchanged from the loaded content file.
//                    effect.World = Matrix.Identity *
//                        Matrix.CreateScale(transform.Scale.MaxVectorMember()) *
//                        Matrix.CreateRotationX(transform.Rotation.X) *
//                        Matrix.CreateRotationY(transform.Rotation.Y) *
//                        Matrix.CreateRotationZ(transform.Rotation) *
//                        Matrix.CreateTranslation(transform.Position);

//                    // Move the camera 8 units away from the origin:
//                    var cameraPosition = Camera.GetInstance().transform.Position;
//                    // Tell the camera to look at the origin:
//                    var cameraLookAtVector = Camera.GetInstance().transform.forward;// sin a cos aby sa podla hodnoty rotacie tocil lookat
//                    // Tell the camera that positive Z is up
//                    var cameraUpVector = Camera.GetInstance().transform.up;
//                    effect.View = Matrix.CreateLookAt(
//                        cameraPosition, cameraLookAtVector, cameraUpVector) *
//                        Matrix.CreateRotationX(Camera.GetInstance().transform.Rotation.X) *
//                        Matrix.CreateRotationY(Camera.GetInstance().transform.Rotation.Y);

//                    // We want the aspect ratio of our display to match
//                    // the entire screen's aspect ratio:
//                    float aspectRatio =
//                        Scene.Instance.graphics.PreferredBackBufferWidth / (float)Scene.Instance.graphics.PreferredBackBufferHeight;
//                    // Field of view measures how wide of a view our camera has.
//                    // Increasing this value means it has a wider view, making everything
//                    // on screen smaller. This is conceptually the same as "zooming out".
//                    // It also 
//                    float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
//                    // Anything closer than this will not be drawn (will be clipped)
//                    float nearClipPlane = 0.01f;
//                    // Anything further than this will not be drawn (will be clipped)
//                    float farClipPlane = 5000;

//                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
//                        fieldOfView, aspectRatio, nearClipPlane, farClipPlane);




//                    effect.LightingEnabled = true; // turn on the lighting subsystem.
//                    //effect.DiffuseColor = Color.White * 4f;
//                    effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 0, 0); // a red light
//                    effect.DirectionalLight0.Direction = new Vector3(-1, 1, 0);  // coming along the x-axis
//                    effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0); // with green highlights

//                    effect.DirectionalLight1.DiffuseColor = new Vector3(0f, 0.3f, 1); // a red light
//                    effect.DirectionalLight1.Direction = new Vector3(-1, 0, 1);  // coming along the x-axis
//                    effect.DirectionalLight1.SpecularColor = new Vector3(1, 1, 0); // with green highlights

//                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 1f);
//                    effect.EmissiveColor = new Vector3(1, 0.2f, 0);
//                    effect.EnableDefaultLighting();
//                }
//                mesh.Draw();

//            }
//            base.Draw(batch);
//        }
//    }
//}
