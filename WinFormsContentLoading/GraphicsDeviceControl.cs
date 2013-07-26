#region File Description
//-----------------------------------------------------------------------------
// GraphicsDeviceControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace WinFormsContentLoading
{
    // System.Drawing �� XNA Framework �̗����ŁAColor �^�� Rectangle �^��
    // ��`����Ă��܂��B����������邽�߂ɁA�ǂ�����g�p���邩���m�Ɏw�肵�܂��B
    using Color = System.Drawing.Color;
    using Rectangle = Microsoft.Xna.Framework.Rectangle;


    /// <summary>
    /// ���̃J�X�^�� �R���g���[���́AXNA Framework GraphicsDevice ���g�p���āA
    /// Windows �t�H�[���Ƀ����_�����O���܂��B�h���N���X�́AInitialize ���\�b�h�� 
    /// Draw ���\�b�h���I�[�o�[���C�h���āA�Ǝ��̕`��R�[�h��ǉ��ł��܂��B
    /// </summary>
    abstract public class GraphicsDeviceControl : Control
    {
        #region �t�B�[���h


        // �g�p���� GraphicsDeviceControl �C���X�^���X�̐��Ɋ֌W�Ȃ��A�w��ł́A
        // ���ׂĂ��̃w���p�[ �T�[�r�X�ŊǗ�����铯��� GraphicsDevice �����L���܂��B
        GraphicsDeviceService graphicsDeviceService;


        #endregion

        #region �v���p�e�B


        /// <summary>
        /// ���̃R���g���[���ւ̕`��Ɏg�p�ł��� GraphicsDevice ���擾���܂��B
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            // get�v���p�e�B�̎g����
            // GraphicsDevice device = xxxx.GraphicsDevice;
            // set�v���p�e�B�̎g�����B���̏ꍇ�͎g���Ȃ��B
            // xxxx.GraphicsDevice = yyyy;
            get
            {
                return graphicsDeviceService.GraphicsDevice; 
            }
        }


        /// <summary>
        /// ���̃T���v���� IGraphicsDeviceService ���܂� IServiceProvider ���擾���܂��B
        /// ����́AContentManager �Ȃǂ̃R���|�[�l���g�Ŏg�p�ł��܂��B������
        /// �R���|�[�l���g�͂��̃T�[�r�X���g�p���āAGraphicsDevice ���擾���܂��B
        /// </summary>
        public ServiceContainer Services
        {
            get
            {
                return services; 
            }
        }

        ServiceContainer services = new ServiceContainer();


        #endregion

        #region Initialization


        /// <summary>
        /// �R���g���[�������������܂��B
        /// </summary>
        protected override void OnCreateControl()
        {
            // �f�U�C�i�[���Ŏ��s���Ă���ꍇ�́A
            // �O���t�B�b�N �f�o�C�X�����������܂���B
            if (!DesignMode)
            {
                graphicsDeviceService = GraphicsDeviceService.AddRef(Handle,
                                                                     ClientSize.Width,
                                                                     ClientSize.Height);

                // ContentManager �Ȃǂ̃R���|�[�l���g���猟�o�ł���悤�ɁA�T�[�r�X��o�^���܂��B
                services.AddService<IGraphicsDeviceService>(graphicsDeviceService);

                // �h���N���X�Ɏ��g������������@���^���܂��B
                Initialize();
            }

            base.OnCreateControl();
        }


        /// <summary>
        /// �R���g���[����j�����܂��B
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (graphicsDeviceService != null)
            {
                graphicsDeviceService.Release(disposing);
                graphicsDeviceService = null;
            }

            base.Dispose(disposing);
        }


        #endregion

        #region Paint


        /// <summary>
        /// WinForms �y�C���g ���b�Z�[�W�ɉ������ăR���g���[�����ĕ`�悵�܂��B
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            string beginDrawError = BeginDraw();

            // IsNullOrEmpty�́Anull��""
            if (string.IsNullOrEmpty(beginDrawError))
            {
                // GraphicsDevice ���g�p���ăR���g���[����`�悵�܂��B
                Draw();
                EndDraw();
            }
            else
            {
                // BeginDraw �����s�����ꍇ�ASystem.Drawing ���g�p���ăG���[ ���b�Z�[�W��\�����܂��B
                PaintUsingSystemDrawing(e.Graphics, beginDrawError);
            }
        }


        /// <summary>
        /// �R���g���[���̕`����J�n���悤�Ƃ��܂��B�O���t�B�b�N �f�o�C�X�����X�g���Ă�����A
        /// �t�H�[�� �f�U�C�i�[�����Ŏ��s���Ă���ꍇ�ɊJ�n�ł��Ȃ����Ƃ�����܂����A
        /// ���̏ꍇ�́A�G���[ ���b�Z�[�W�������Ԃ��܂��B
        /// </summary>
        string BeginDraw()
        {
            // �O���t�B�b�N �f�o�C�X���Ȃ��ꍇ�́A�f�U�C�i�[���Ŏ��s���Ă��܂��B
            if (graphicsDeviceService == null)
            {
                return Text + "\n\n" + GetType();
            }

            // �O���t�B�b�N �f�o�C�X���\���ɑ傫���A���X�g���Ă��Ȃ����Ƃ��m�F���܂��B
            string deviceResetError = HandleDeviceReset();

            if (!string.IsNullOrEmpty(deviceResetError))
            {
                return deviceResetError;
            }

            // ������ GraphicsDeviceControl �C���X�^���X���A����� GraphicsDevice ��
            // ���L�ł��܂��B�f�o�C�X �o�b�N�o�b�t�@�[�́A�����̂����ő�̃R���g���[��
            // �ɍ��킹�ăT�C�Y���ύX����܂��B�ł́A���݂�菬���ȃR���g���[����
            // �`�悵�Ă���ꍇ�͂ǂ��ł��傤���B�s�v�Ɉ����L�΂���邱�Ƃ�����邽�߁A
            // �t�� �o�b�N�o�b�t�@�[�̍��㕔���������g�p����悤�ɁA�r���[�|�[�g��ݒ肵�܂��B
            Viewport viewport = new Viewport();

            viewport.X = 0;
            viewport.Y = 0;

            viewport.Width = ClientSize.Width;
            viewport.Height = ClientSize.Height;

            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            GraphicsDevice.Viewport = viewport;

            return null;
        }


        /// <summary>
        /// �R���g���[���̕`����I�����܂��B����́A�h���N���X�� Draw ���\�b�h���I������
        /// ��ɌĂяo����A���������C���[�W����ʏ�ɕ\�����������S���܂��B
        /// �K�؂� WinForms �R���g���[�� �n���h�����g�p���āA�������ʒu��
        /// �\�������悤�ɂ��܂��B
        /// </summary>
        void EndDraw()
        {
            try
            {
                Rectangle sourceRectangle = new Rectangle(0, 0, ClientSize.Width,
                                                                ClientSize.Height);

                GraphicsDevice.Present(sourceRectangle, null, this.Handle);
            }
            catch
            {
                // Present �́A�f�o�C�X���`�撆�Ɏ���ꂽ�ꍇ�ɃX���[����ꍇ������܂��B
                // ���X�g�����f�o�C�X�́A���� BeginDraw �ŏ��������̂ŁA��O�͎󂯓���邾���ŁA
                // ���ʂȏ����͂��܂���B
            }
        }


        /// <summary>
        /// BeginDraw �ɂ��g�p�����w���p�[�B����́A�O���t�B�b�N �f�o�C�X�̃X�e�[�^�X��
        /// �`�F�b�N���āA���݂̃R���g���[���̕`��ɏ\���傫���A�f�o�C�X�����X�g���Ă��Ȃ�
        /// ���Ƃ��m�F���܂��B�f�o�C�X�����Z�b�g�ł��Ȃ������ꍇ�A�G���[�������Ԃ��܂��B
        /// </summary>
        string HandleDeviceReset()
        {
            bool deviceNeedsReset = false;

            switch (GraphicsDevice.GraphicsDeviceStatus)
            {
                case GraphicsDeviceStatus.Lost:
                    // �O���t�B�b�N �f�o�C�X�����X�g���Ă���ꍇ�́A�܂������g�p�ł��܂���B
                    return "Graphics device lost";

                case GraphicsDeviceStatus.NotReset:
                    // �f�o�C�X�����Z�b�g����Ă��Ȃ���Ԃ̏ꍇ�́A���Z�b�g�����݂�K�v������܂��B
                    deviceNeedsReset = true;
                    break;

                default:
                    // �f�o�C�X�̏�Ԃ����Ȃ��ꍇ�A�\���ɑ傫�����ǂ������`�F�b�N���܂��B
                    PresentationParameters pp = GraphicsDevice.PresentationParameters;

                    deviceNeedsReset = (ClientSize.Width > pp.BackBufferWidth) ||
                                       (ClientSize.Height > pp.BackBufferHeight);
                    break;
            }

            // �f�o�C�X�����Z�b�g����K�v������܂����B
            if (deviceNeedsReset)
            {
                try
                {
                    graphicsDeviceService.ResetDevice(ClientSize.Width,
                                                      ClientSize.Height);
                }
                catch (Exception e)
                {
                    return "Graphics device reset failed\n\n" + e;
                }
            }

            return null;
        }


        /// <summary>
        /// �L���ȃO���t�B�b�N �f�o�C�X���Ȃ��ꍇ (���Ƃ��΁A�f�o�C�X�������Ă�����A
        /// �t�H�[�� �f�U�C�i�[���Ŏ��s���Ă���ꍇ)�A�ʏ�� System.Drawing 
        /// ���\�b�h���g�p���āA�X�e�[�^�X ���b�Z�[�W��\������K�v������܂��B
        /// </summary>
        protected virtual void PaintUsingSystemDrawing(Graphics graphics, string text)
        {
            graphics.Clear(Color.CornflowerBlue);

            using (Brush brush = new SolidBrush(Color.Black))
            {
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    graphics.DrawString(text, Font, brush, ClientRectangle, format);
                }
            }
        }


        /// <summary>
        /// �w�i�̕`����w������AWinForms �̃��b�Z�[�W�𖳎����܂��B����̎����ł́A
        /// �R���g���[�������݂̔w�i�F�ŃN���A���܂��B������ OnPaint ������ XNA 
        /// Framework �� GraphicsDevice ���g�p���Ē����ɕʂ̐F���d�˂ĕ`�悷��̂ŁA
        /// ��ʂ���������Ă��܂��܂��B
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }


        #endregion

        #region Abstract Methods


        /// <summary>
        /// �h���N���X�́A������I�[�o�[���C�h���āA�`��R�[�h�����������܂��B
        /// </summary>
        protected abstract void Initialize();


        /// <summary>
        /// �h���N���X�́A������I�[�o�[���C�h���āAGraphicsDevice ���g�p���Ď��g��`�悵�܂��B
        /// </summary>
        protected abstract void Draw();


        #endregion
    }
}
