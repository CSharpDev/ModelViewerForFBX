#region File Description
//-----------------------------------------------------------------------------
// ErrorLogger.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Build.Framework;
#endregion

namespace WinFormsContentLoading
{
    /// <summary>
    /// MSBuild ILogger �C���^�[�t�F�C�X���J�X�^���Ɏ������A���[�U�[���ォ��
    /// �m�F�ł���悤�ɁA�R���e���c �r���h �G���[���L�^���܂��B
    /// </summary>
    class ErrorLogger : ILogger
    {
        /// <summary>
        /// ErrorRaised �ʒm�C�x���g���t�b�N���āA�J�X�^�� ���K�[�����������܂��B
        /// </summary>
        public void Initialize(IEventSource eventSource)
        {
            if (eventSource != null)
            {
                eventSource.ErrorRaised += ErrorRaised;
            }
        }


        /// <summary>
        /// �J�X�^�� ���K�[���V���b�g�_�E�����܂��B
        /// </summary>
        public void Shutdown()
        {
        }


        /// <summary>
        /// �G���[ ���b�Z�[�W��������i�[���邱�Ƃɂ���āA�G���[�ʒm�C�x���g���������܂��B
        /// </summary>
        void ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            errors.Add(e.Message);
        }


        /// <summary>
        /// �L�^���ꂽ���ׂẴG���[�̃��X�g���擾���܂��B
        /// </summary>
        public List<string> Errors
        {
            get { return errors; }
        }

        List<string> errors = new List<string>();


        #region ILogger Members

        
        /// <summary>
        /// ILogger.Parameters �v���p�e�B���������܂��B
        /// </summary>
        string ILogger.Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        string parameters;


        /// <summary>
        /// ILogger.Verbosity �v���p�e�B���������܂��B
        /// </summary>
        LoggerVerbosity ILogger.Verbosity
        {
            get { return verbosity; }
            set { verbosity = value; }
        }

        LoggerVerbosity verbosity = LoggerVerbosity.Normal;


        #endregion
    }
}
