#region File Description
//-----------------------------------------------------------------------------
// ServiceContainer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace WinFormsContentLoading
{
    /// <summary>
    /// ServiceContainer �N���X�� IServiceProvider �C���^�[�t�F�C�X���������܂��B
    /// ����́A�قȂ�R���|�[�l���g�Ԃŋ��L�T�[�r�X��n�����߂Ɏg�p����܂��B���Ƃ��΁A
    /// ContentManager �͂�����g�p���āAIGraphicsDeviceService �������擾���܂��B
    /// </summary>
    public class ServiceContainer : IServiceProvider
    {
        // �}�b�v(�L�[�ƃf�[�^)
        // a = array["trident"];    // �A�z�z��Bassociated array
        Dictionary<Type, object> services = new Dictionary<Type, object>();


        /// <summary>
        /// �R���N�V�����ɐV�����T�[�r�X��ǉ����܂��B
        /// </summary>
        public void AddService<T>(T service)
        {
            // �}�b�v�ɒǉ�����B
            services.Add(typeof(T), service);
        }


        /// <summary>
        /// �w��̃T�[�r�X���擾���܂��B
        /// </summary>
        public object GetService(Type serviceType)
        {
            object service;

            // �L�[���w�肵�ăf�[�^�����o���B
            services.TryGetValue(serviceType, out service);

            return service;
        }
    }
}
