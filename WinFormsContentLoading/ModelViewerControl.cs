#region File Description
///-----------------------------------------------------------------------------
/// ModelViewer for FBX/X
///
/// Microsoft XNA Community Game Platform
/// Copyright (C) Microsoft Corporation. All rights reserved.
/// Copyright (C) 2019-2013 Dr.JIRO Software. All rights reserved.
/////-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace WinFormsContentLoading
{
    /// <summary>
    /// ���̃R���g���[���� GraphicsDeviceControl ����p�����A��]���� 
    /// 3D ���f����\�����܂��B���C�� �t�H�[�� �N���X�����f���̓ǂݍ��݂�
    /// �S���̂ŁA���̃R���g���[���͂����\�����邾���ł��B
    /// </summary>
    class ModelViewerControl : GraphicsDeviceControl
    {
        /// <summary>
        /// ���݂̃��f�����擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public Model Model
        {
            get
            {
                return model; 
            }

            set
            {
                model = value;

                if (model != null)
                {
                    // ���f���̑傫�����͂���B
                    MeasureModel();
                    // ���_�̓��f���̒��S�B
                    camera.Eye = modelCenter;

                    // Z�ʒu�͔��aX2�AY�ʒu�͔��a
                    camera.Eye.Z += modelRadius * 2;
                    camera.Eye.Y += modelRadius;
                    camera.At = modelCenter;
                    camera.SetupView();

                    camera.NearClip = modelRadius / 100;
                    camera.FarClip = modelRadius * 100;
                    camera.SetupProjection();
                }
            }
        }

        /// <summary>
        /// ���f���B
        /// </summary>
        private Model model;


        // ���f���̃T�C�Y�ƈʒu�Ɋւ�������L���b�V�����܂��B
        Matrix[] boneTransforms;
        Vector3 modelCenter;
        float modelRadius;


        // �^�C�}�[�͉�]���x�𐧌䂵�܂��B
        Stopwatch timer;

        /// <summary>
        /// �f�t�H���g�̃G�t�F�N�g�B
        /// </summary>
        public BasicEffect DefaultEffect;

        /// <summary>
        /// �J�����B
        /// </summary>
        private Camera camera;

        /// <summary>
        /// �e�L�X�g�`��B
        /// </summary>
        private TextRenderer textRender;

        /// <summary>
        /// �R���g���[�������������܂��B
        /// </summary>
        protected override void Initialize()
        {
            // �A�j���[�V���� �^�C�}�[���J�n���܂��B
            timer = Stopwatch.StartNew();

            // �A�j���[�V���������I�ɍĕ`�悷�邽�߂ɃA�C�h�� �C�x���g���t�b�N���܂��B
            Application.Idle += delegate { Invalidate(); };

            // �f�t�H���g�̃G�t�F�N�g���擾����B
            DefaultEffect = new BasicEffect(GraphicsDevice, null);

            camera = new Camera();
            camera.AspectRatio = GraphicsDevice.Viewport.AspectRatio;
        }


        /// <summary>
        /// �R���g���[����`�悵�܂��B
        /// </summary>
        protected override void Draw()
        {
            HandleInput();

            ((MainForm)Parent).ShowParameters(camera);

            // ����̃R���g���[���̔w�i�F�ŃN���A���܂��B
            Color backColor = new Color(BackColor.R, BackColor.G, BackColor.B);

            GraphicsDevice.Clear(backColor);

            if (model != null)
            {
                // �J�����̍s����v�Z���܂��B
                //float rotation = (float)timer.Elapsed.TotalSeconds;

//                Matrix world = Matrix.CreateRotationY(rotation);
                Matrix world = Matrix.Identity;
                // ���f����`�悵�܂��B
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = boneTransforms[mesh.ParentBone.Index] * world;
                        effect.View = camera.ViewMatrix;
                        effect.Projection = camera.ProjectionMatrix;

                        // �t�H�[���̒l�𔽉f����B
                        ((MainForm)Parent).ParameterForm.SetParams(effect, GraphicsDevice);

                        //effect.EnableDefaultLighting();
                        //effect.PreferPerPixelLighting = true;
                        //effect.SpecularPower = 16;
                    }

                    mesh.Draw();
                }
            }
        }

        /// <summary>
        /// ���̓n���h���[�B
        /// </summary>
        private void HandleInput()
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                camera.RotateHorizontally(1.0f);
            }
            else if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                camera.RotateHorizontally(-1.0f);
            }
            if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                camera.RotateVirtically(1.0f);
            }
            else if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                camera.RotateVirtically(-1.0f);
            }
            if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.PageUp))
            {
                camera.Zoom(0.1f);
            }
            else if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.PageDown))
            {
                camera.Zoom(-0.1f);
            }
            camera.SetupView();
            camera.SetupProjection();
        }

        /// <summary>
        /// �V�������f�����I�������ƁA���̃��f���̑傫���ƒ��S�̈ʒu���m�F���܂��B
        /// ����ɂ��A�����I�ɕ\�����g��܂��͏k���ł���̂ŁA�ǂ̃X�P�[����
        /// ���f���ł������������ł��܂��B
        /// </summary>
        void MeasureModel()
        {
            // ���̃��f���̐�΃{�[�� �g�����X�t�H�[�����������܂��B
            boneTransforms = new Matrix[model.Bones.Count];
            
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // ���ׂẴ��b�V���̊e���E���̒��S�𕽋ς��邱�Ƃɂ���āA
            // ���f���̂����悻�̒��S�ʒu���v�Z���܂��B
            modelCenter = Vector3.Zero;

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                modelCenter += meshCenter;
            }

            // ���S�͂��ׂẴ��b�V���̒��S�̕��ϒl
            modelCenter /= model.Meshes.Count;

            // ����Œ��S�_���킩�����̂ŁA���b�V���̊e���E���̔��a��
            // ���ׂ邱�Ƃɂ���āA���f���̔��a���v�Z�ł��܂��B
            modelRadius = 0;

            foreach (ModelMesh mesh in model.Meshes)
            {
                BoundingSphere meshBounds = mesh.BoundingSphere;
                Matrix transform = boneTransforms[mesh.ParentBone.Index];
                Vector3 meshCenter = Vector3.Transform(meshBounds.Center, transform);

                float transformScale = transform.Forward.Length();
                
                float meshRadius = (meshCenter - modelCenter).Length() +
                                   (meshBounds.Radius * transformScale);

                modelRadius = Math.Max(modelRadius,  meshRadius);
            }
        }

    }
}
