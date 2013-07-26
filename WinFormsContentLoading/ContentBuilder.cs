#region File Description
//-----------------------------------------------------------------------------
// ContentBuilder.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Build.BuildEngine;
#endregion

namespace WinFormsContentLoading
{
    /// <summary>
    /// ���̃N���X�́A���s���ɓ��I�� XNA Framework �R���e���c���r���h���邽�߂ɕK�v�� 
    /// MSBuild �@�\�����b�v���܂��B�������[���Ɉꎞ MSBuild �v���W�F�N�g���쐬���A
    /// �C�ӂőI�������R���e���c �t�@�C�������̃v���W�F�N�g�ɒǉ����܂��B
    /// �����ăv���W�F�N�g���r���h���܂��B����ɂ��ꎞ�f�B���N�g���ɃR���p�C���ς݂� .xnb 
    /// �R���e���c �t�@�C�����쐬����܂��B�r���h���I��������A��ʓI�� ContentManager 
    /// ���g�p���āA�ʏ�ǂ���ɂ����̈ꎞ .xnb �t�@�C����ǂݍ��߂܂��B
    /// </summary>
    class ContentBuilder : IDisposable
    {
        #region Fields


        // �ǂ̃C���|�[�^�[�܂��̓v���Z�b�T��ǂݍ��݂܂����B
        const string xnaVersion = ", Version=3.0.0.0, PublicKeyToken=6d5c3888ef60e27d";

        static string[] pipelineAssemblies =
        {
            "Microsoft.Xna.Framework.Content.Pipeline.FBXImporter" + xnaVersion,
            "Microsoft.Xna.Framework.Content.Pipeline.XImporter" + xnaVersion,
            "Microsoft.Xna.Framework.Content.Pipeline.TextureImporter" + xnaVersion,
            "Microsoft.Xna.Framework.Content.Pipeline.EffectImporter" + xnaVersion,
        };


        // ���I�ɃR���e���c���r���h���邽�߂Ɏg�p����� MSBuild �I�u�W�F�N�g�B
        Engine msBuildEngine;
        Project msBuildProject;
        ErrorLogger errorLogger;


        // �R���e���c �r���h�Ŏg�p�����ꎞ�f�B���N�g���B
        string buildDirectory;
        string processDirectory;
        string baseDirectory;


        // ������ ContentBuilder ������ꍇ�ɁA��ӂ̃f�B���N�g�����𐶐����܂��B
        static int directorySalt;


        // �������I���܂������B
        bool isDisposed;


        #endregion

        #region Properties


        /// <summary>
        /// �o�̓f�B���N�g�����擾���܂��B�����ɂ́A�������ꂽ .xnb �t�@�C�����܂܂�܂��B
        /// </summary>
        public string OutputDirectory
        {
            get { return Path.Combine(buildDirectory, "bin/Content"); }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// �V���� ContentBuilder �̃R���X�g���N�^�[
        /// </summary>
        public ContentBuilder()
        {
            CreateTempDirectory();
            CreateBuildProject();
        }


        /// <summary>
        /// ContentBuilder �̃f�R���X�g���N�^�[
        /// </summary>
        ~ContentBuilder()
        {
            Dispose(false);
        }


        /// <summary>
        /// �K�v���Ȃ��Ȃ����Ƃ��ɁAContentBuilder ��j�����܂��B
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// �W���� .NET IDisposable �p�^�[�����������܂��B
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                isDisposed = true;

                DeleteTempDirectory();
            }
        }


        #endregion

        #region MSBuild


        /// <summary>
        /// �ꎞ MSBuild �R���e���c �v���W�F�N�g���������[���ɍ쐬���܂��B
        /// </summary>
        void CreateBuildProject()
        {
            string projectPath = Path.Combine(buildDirectory, "content.contentproj");
            string outputPath = Path.Combine(buildDirectory, "bin");

            // �r���h �G���W�����쐬���܂��B
            msBuildEngine = new Engine(RuntimeEnvironment.GetRuntimeDirectory());

            // �J�X�^�� �G���[ ���K�[��o�^���܂��B
            errorLogger = new ErrorLogger();

            msBuildEngine.RegisterLogger(errorLogger);

            // �r���h �v���W�F�N�g���쐬���܂��B
            msBuildProject = new Project(msBuildEngine);

            msBuildProject.FullFileName = projectPath;

            msBuildProject.SetProperty("XnaPlatform", "Windows");
            msBuildProject.SetProperty("XnaFrameworkVersion", "v2.0");
            msBuildProject.SetProperty("Configuration", "Release");
            msBuildProject.SetProperty("OutputPath", outputPath);

            // �J�X�^�� �C���|�[�^�[�܂��̓v���Z�b�T��o�^���܂��B
            foreach (string pipelineAssembly in pipelineAssemblies)
            {
                msBuildProject.AddNewItem("Reference", pipelineAssembly);
            }

            // XNA Framework �R���e���c�̃r���h���@���`����W���^�[�Q�b�g �t�@�C�����܂߂܂��B
            msBuildProject.AddNewImport("$(MSBuildExtensionsPath)\\Microsoft\\XNA " +
                                        "Game Studio\\v3.0\\Microsoft.Xna.GameStudio" +
                                        ".ContentPipeline.targets", null);
        }


        /// <summary>
        /// �V�����R���e���c �t�@�C���� MSBuild �v���W�F�N�g�ɒǉ����܂��B�C���|�[�^�[�����
        /// �v���Z�b�T�͏ȗ��\�ł��B�C���|�[�^�[�� null �̂܂܂ɂ���ƁA�C���|�[�^�[��
        /// �t�@�C���g���q�Ɋ�Â��Ď����I�Ɍ��o����܂��B�v���Z�b�T�� null �̂܂܂ɂ���ƁA
        /// �f�[�^�͏������ꂸ�Ƀp�X �X���[����܂��B
        /// </summary>
        public void Add(string filename, string name, string importer, string processor)
        {
            BuildItem buildItem = msBuildProject.AddNewItem("Compile", filename);

            buildItem.SetMetadata("Link", Path.GetFileName(filename));
            buildItem.SetMetadata("Name", name);

            if (!string.IsNullOrEmpty(importer))
                buildItem.SetMetadata("Importer", importer);

            if (!string.IsNullOrEmpty(processor))
                buildItem.SetMetadata("Processor", processor);
        }


        /// <summary>
        /// ���ׂẴR���e���c �t�@�C���� MSBuild �v���W�F�N�g����폜���܂��B
        /// </summary>
        public void Clear()
        {
            msBuildProject.RemoveItemsByName("Compile");
        }


        /// <summary>
        /// �v���W�F�N�g�ɒǉ��������ׂẴR���e���c �t�@�C�����r���h���A
        /// OutputDirectory �� .xnb �t�@�C���𓮓I�ɐ������܂��B
        /// �r���h�����s�����ꍇ�A�G���[ ���b�Z�[�W��Ԃ��܂��B
        /// </summary>
        public string Build()
        {
            // �ȑO�̋L�^�����G���[�����ׂăN���A���܂��B
            errorLogger.Errors.Clear();

            // �v���W�F�N�g���r���h���܂��B
            if (!msBuildProject.Build())
            {
                // �r���h�����s�����ꍇ�A�G���[�������Ԃ��܂��B
                return string.Join("\n", errorLogger.Errors.ToArray());
            }

            return null;
        }


        #endregion

        #region Temp Directories


        /// <summary>
        /// �R���e���c���r���h����ꎞ�f�B���N�g�����쐬���܂��B
        /// </summary>
        void CreateTempDirectory()
        {
            // �f�B���N�g���̖��O�̊��͎��̂Ƃ���ł��B
            //
            //  %temp%\WinFormsContentLoading.ContentBuilder
            baseDirectory = Path.Combine(Path.GetTempPath(), GetType().FullName);

            // �����Ɏ��s����v���O�����̃R�s�[����������ꍇ�́A
            // ���̂悤�Ƀv���Z�X ID ���܂߂܂��B
            //
            //  %temp%\WinFormsContentLoading.ContentBuilder\<ProcessId>
            int processId = Process.GetCurrentProcess().Id;
            processDirectory = Path.Combine(baseDirectory, processId.ToString());

            // �v���O�����ŕ����� ContentBuilder �C���X�^���X���쐬�����ꍇ�́A
            // ���̂悤�Ƀ\���g�l���܂߂܂��B
            //
            //  %temp%\WinFormsContentLoading.ContentBuilder\<ProcessId>\<Salt>
            directorySalt++;
            buildDirectory = Path.Combine(processDirectory, directorySalt.ToString());

            // �ꎞ�f�B���N�g�����쐬���܂��B
            Directory.CreateDirectory(buildDirectory);

            PurgeStaleTempDirectories();
        }


        /// <summary>
        /// �ꎞ�f�B���N�g�����K�v�Ȃ��Ȃ����ꍇ�A������폜���܂��B
        /// </summary>
        void DeleteTempDirectory()
        {
            Directory.Delete(buildDirectory, true);

            // �e���̈ꎞ�f�B���N�g�����܂��g�p���Ă��� ContentBuilder �̃C���X�^���X��
            // �ق��ɂȂ��ꍇ�́A�v���Z�X �f�B���N�g�����폜�ł��܂��B
            if (Directory.GetDirectories(processDirectory).Length == 0)
            {
                Directory.Delete(processDirectory);

                // �e���̈ꎞ�f�B���N�g�����܂��g�p���Ă���v���O�����̃R�s�[��
                // �ق��ɂȂ��ꍇ�́A��{�f�B���N�g�����폜�ł��܂��B
                if (Directory.GetDirectories(baseDirectory).Length == 0)
                {
                    Directory.Delete(baseDirectory);
                }
            }
        }


        /// <summary>
        /// �g�p����K�v���Ȃ��Ȃ����Ƃ��Ɉꎞ�f�B���N�g�����폜�ł��邱�Ƃ��A���z�I�ł��B
        /// DeleteTempDirectory ���\�b�h (Dispose �܂��̓f�R���X�g���N�^�[�̂����ŏ��ɔ�������
        /// ���̂ɂ���ČĂяo����܂�) ���܂�����������s���܂��B���́A�����̃N���[���A�b�v 
        /// ���\�b�h���܂��������s����Ȃ��ꍇ�����邱�Ƃł��B���Ƃ��΁A�v���O������
        /// �N���b�V��������A�f�o�b�K�[�Œ�~���ꂽ�ꍇ�A�폜���s���@������܂���B
        /// ����ɋN������ƁA���̃��\�b�h�́A�ȑO�̎��s�Ő������V���b�g�_�E���ł��Ȃ��������߂�
        /// �c���ꂽ�ꎞ�f�B���N�g�������ׂĒ��ׂ܂��B����ɂ��A�����Ǘ������f�B���N�g���́A
        /// �i�v�ɎU�������܂܎c����邱�Ƃ��Ȃ��Ȃ�܂��B
        /// </summary>
        void PurgeStaleTempDirectories()
        {
            // ���̏ꏊ�̃T�u�f�B���N�g�������ׂĒ��ׂ܂��B
            foreach (string directory in Directory.GetDirectories(baseDirectory))
            {
                // �T�u�f�B���N�g�����́A�쐬�����v���Z�X�� ID �ɂȂ�܂��B
                int processId;

                if (int.TryParse(Path.GetFileName(directory), out processId))
                {
                    try
                    {
                        // �N���G�[�^�[ �v���Z�X�͂܂����s���Ă��܂����B
                        Process.GetProcessById(processId);
                    }
                    catch (ArgumentException)
                    {
                        // �v���Z�X�����݂��Ȃ��ꍇ�A���̈ꎞ�f�B���N�g�����폜�ł��܂��B
                        Directory.Delete(directory, true);
                    }
                }
            }
        }

        
        #endregion
    }
}
