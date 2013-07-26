#region File Description
//-----------------------------------------------------------------------------
// GraphicsDeviceService.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
#endregion

// IGraphicsDeviceService �C���^�[�t�F�C�X�́ADeviceCreated �C�x���g��K�v�Ƃ��܂����A
// ��ɃR���X�g���N�^�[�����Ńf�o�C�X���쐬���邾���Ȃ̂ŁA���̃C�x���g�𔭐�������
// �ꏊ������܂���BC# �R���p�C������A�C�x���g���g�p����Ă��Ȃ��A�ƌx������܂����A
// �����ł͐S�z�Ȃ��̂ŁA�P�ɂ��̌x���𖳌��ɂ��܂��B
#pragma warning disable 67

namespace WinFormsContentLoading
{
    /// <summary>
    /// GraphicsDevice �̍쐬�ƊǗ���S������w���p�[ �N���X�B
    /// ���ׂĂ� GraphicsDeviceControl �C���X�^���X�́A����� GraphicsDeviceService ��
    /// ���L����̂ŁA�����̃R���g���[�������݂���ꍇ�ł��A�w��� GraphicsDevice �� 
    /// 1 �������݂��܂���B���̃w���p�[�͕W���� IGraphicsDeviceService 
    /// �C���^�[�t�F�C�X���������܂��B���̃C���^�[�t�F�C�X�́A�f�o�C�X�����Z�b�g�܂���
    /// �j�����ꂽ�Ƃ��ɂ���Ɋւ���ʒm�C�x���g�𐶐����܂��B
    /// </summary>
    class GraphicsDeviceService : IGraphicsDeviceService
    {
        #region Fields


        /// <summary>
        /// �V���O���g�� �f�o�C�X �T�[�r�X �C���X�^���X�B
        /// singleton�̓f�U�C���p�^�[���̂P�B
        /// static �v���O�����S�̂łP�B
        /// </summary>
        static GraphicsDeviceService singletonInstance;


        // singletonInstance �����L���Ă���R���g���[���̐���ǐՂ��܂��B
        static int referenceCount;


        /// <summary>
        /// ���݂̃O���t�B�b�N �f�o�C�X���擾���܂��B
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return graphicsDevice; 
            }
        }

        GraphicsDevice graphicsDevice;


        // ���݂̃f�o�C�X�ݒ���i�[���܂��B
        PresentationParameters parameters;


        // IGraphicsDeviceService �C�x���g�B
        public event EventHandler DeviceCreated;
        public event EventHandler DeviceDisposing;
        public event EventHandler DeviceReset;
        public event EventHandler DeviceResetting;

        #endregion


        /// <summary>
        /// �V���O���g�� �N���X�Ȃ̂ŃR���X�g���N�^�[�̓v���C�x�[�g�ł��B
        /// �N���C�A���g �R���g���[���́A����Ƀp�u���b�N�� AddRef ���\�b�h��
        /// �g�p����K�v������܂��B
        /// 
        /// private�ȃR���X�g���N�^�Ȃ̂ŁA�ق��̃N���X�ł�
        /// new�ł��Ȃ��B
        /// </summary>
        private GraphicsDeviceService(IntPtr windowHandle, int width, int height)
        {
            parameters = new PresentationParameters();

            parameters.BackBufferWidth = Math.Max(width, 1);
            parameters.BackBufferHeight = Math.Max(height, 1);
            parameters.BackBufferFormat = SurfaceFormat.Color;

            parameters.EnableAutoDepthStencil = true;
            parameters.AutoDepthStencilFormat = DepthFormat.Depth24;

            // �O���t�B�b�N�X�f�o�C�X���쐬����B
            graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter,
                                                DeviceType.Hardware,
                                                windowHandle,
                                                parameters);
        }

        /// <summary>
        /// �V���O���g�� �C���X�^���X�ɑ΂���Q�Ƃ��擾���܂��B
        /// 
        /// static���\�b�h�ŃC���X�^���X���쐬�܂��͎擾����B
        /// GraphicsDeviceService device = GraphicsDeviceService.AddRef(�E�E�E);
        /// </summary>
        public static GraphicsDeviceService AddRef(IntPtr windowHandle,
                                                   int width, int height)
        {
            // "�f�o�C�X�����L���Ă���R���g���[���̐�" �J�E���^�[�𑝂₵�܂��B
            if (Interlocked.Increment(ref referenceCount) == 1)
            {
                // ���ꂪ�A�f�o�C�X���g�p���n�߂�ŏ��̃R���g���[���̏ꍇ�A
                // �V���O���g�� �C���X�^���X���쐬����K�v������܂��B
                // �����ł́A�����N���X���Ȃ̂�new�ł���B
                singletonInstance = new GraphicsDeviceService(windowHandle,
                                                              width, height);
            }

            return singletonInstance;
        }


        /// <summary>
        /// �V���O���g�� �C���X�^���X�ɑ΂���Q�Ƃ�������܂��B
        /// </summary>
        public void Release(bool disposing)
        {
            // "�f�o�C�X�����L����R���g���[����" �J�E���^�[�����炵�܂��B
            if (Interlocked.Decrement(ref referenceCount) == 0)
            {
                // ���ꂪ�A�f�o�C�X���g�p���I����Ō�̃R���g���[���̏ꍇ�A
                // �V���O���g�� �C���X�^���X��j������K�v������܂��B
                if (disposing)
                {
                    if (DeviceDisposing != null)
                        DeviceDisposing(this, EventArgs.Empty);

                    // �j������B
                    graphicsDevice.Dispose();
                }

                graphicsDevice = null;
            }
        }


        /// <summary>
        /// �O���t�B�b�N �f�o�C�X���A�w�肵���𑜓x�܂��͌��ݕۗL���Ă���
        /// �R���g���[�� �T�C�Y�̑傫�����ɍ��킹�ă��Z�b�g���܂��B���̓���́A�f�o�C�X���A
        /// ���ׂĂ� GraphicsDeviceControl �N���C�A���g�̂����ő�̂��̂ɉ�����
        /// �傫���Ȃ邱�Ƃ��Ӗ����܂��B
        /// </summary>
        public void ResetDevice(int width, int height)
        {
            if (DeviceResetting != null)
                DeviceResetting(this, EventArgs.Empty);

            parameters.BackBufferWidth = Math.Max(parameters.BackBufferWidth, width);
            parameters.BackBufferHeight = Math.Max(parameters.BackBufferHeight, height);

            graphicsDevice.Reset(parameters);

            if (DeviceReset != null)
                DeviceReset(this, EventArgs.Empty);
        }

    }
}
