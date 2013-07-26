#region File Description
//-----------------------------------------------------------------------------
// MainForm.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace WinFormsContentLoading
{
    /// <summary>
    /// ���̃J�X�^�� �t�H�[���́A�v���O�����̃��C�� ���[�U�[ �C���^�[�t�F�C�X��
    /// �񋟂��܂��B���̃T���v���ł̓f�U�C�i�[���g�p���āA[File] / [Open] 
    /// �I�v�V������\�����郁�j���[ �o�[�������t�H�[���S�̂� ModelViewerControl ��
    /// ���߂܂����B
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// �R���e���c�R���p�C���B
        /// </summary>
        private ContentBuilder contentBuilder;

        /// <summary>
        /// �R���e���c�Ǘ��BXNA
        /// </summary>
        private ContentManager contentManager;

        /// <summary>
        /// �G�t�F�N�g�p�����[�^�ݒ�t�H�[���B
        /// </summary>
        public ParameterForm ParameterForm
        {
            get;
            private set;
        }

        /// <summary>
        /// ���C�� �t�H�[�����\�z���܂��B
        /// </summary>
        public MainForm()
        {
            // �R���|�[�l���g������������B
            InitializeComponent();

            // �R���e���c�R���p�C�����쐬����B
            contentBuilder = new ContentBuilder();

            // �R���e���c�Ǘ����쐬����B
            contentManager = new ContentManager(modelViewerControl.Services,
                                                contentBuilder.OutputDirectory);

            /// �ŏ��Ƀt�H�[����\������Ƃ��ɁA�����I�� [Load Model] �_�C�A���O��\�����܂��B
            /// +=��C#�̕��@�B�C�x���g�̓o�^�B
            //this.Shown += OpenMenuClicked;
        }


        /// <summary>
        /// [Exit] ���j���[ �I�v�V�����̃C�x���g �n���h���[�B
        /// </summary>
        void ExitMenuClicked(object sender, EventArgs e)
        {
            // �I���B
            Close();
        }


        /// <summary>
        /// [Open] ���j���[ �I�v�V�����̃C�x���g �n���h���[�B
        /// </summary>
        void OpenMenuClicked(object sender, EventArgs e)
        {
            // �u�t�@�C�����J���v�_�C�A���O���쐬����B
            OpenFileDialog fileDialog = new OpenFileDialog();

            // ����ŃR���e���c �t�@�C�����܂ރf�B���N�g���ɐݒ肵�܂��B
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string relativePath = Path.Combine(assemblyLocation, "../../../../Content");
            string contentPath = Path.GetFullPath(relativePath);

            // �_�C�A���O�ɐݒ肷��B
            // �ŏ��̃t�H���_�̈ʒu�B
            fileDialog.InitialDirectory = contentPath;

            fileDialog.Title = "���f�������[�h����B";

            // �t�B���^�̎w��B�\�����镶����1|�t�B���^1
            fileDialog.Filter = "���f���t�@�C�� (*.fbx;*.x)|*.fbx;*.x|" +
                                "FBX�t�@�C�� (*.fbx)|*.fbx|" +
                                "X�t�@�C�� (*.x)|*.x|" +
                                "���ׂẴt�@�C�� (*.*)|*.*";

            // �_�C�A���O��\������B
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                // �n�j�Ȃ�A���f�������[�h����B
                LoadModel(fileDialog.FileName);
            }
        }


        /// <summary>
        /// �V���� 3D ���f�� �t�@�C���� ModelViewerControl �ɓǂݍ��݂܂��B
        /// </summary>
        void LoadModel(string fileName)
        {
            // �����v�̃J�[�\���ɂ���B
            Cursor = Cursors.WaitCursor;

            // �����̃��f�������ׂăA�����[�h���܂��B
            modelViewerControl.Model = null;
            contentManager.Unload();

            // ContentBuilder �Ƀr���h����Ώۂ��w�����܂��B
            contentBuilder.Clear();
            // "Model"�͖��O�B"ModelProcessor"�͏�������v���O�������B
            contentBuilder.Add(fileName, "xxx", null, "ModelProcessor");

            // ���̐V�������f�� �f�[�^���r���h���܂��B
            string buildError = contentBuilder.Build();

            if (string.IsNullOrEmpty(buildError))
            {
                // �r���h�����������ꍇ�AContentManager ���g�p���āA
                // �쐬�����΂���̈ꎞ .xnb �t�@�C����ǂݍ��݂܂��B
                modelViewerControl.Model = contentManager.Load<Model>("xxx");
            }
            else
            {
                // �r���h�����s�����ꍇ�A�G���[ ���b�Z�[�W��\�����܂��B
                MessageBox.Show(buildError, "Error");
            }

            // �ʏ�̃J�[�\���ɂ���B
            Cursor = Cursors.Arrow;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ParameterForm = new ParameterForm(this);
            ParameterForm.Initialize(modelViewerControl.DefaultEffect);
            ParameterForm.ResetParams();
            ParameterForm.Show();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {

        }

        public void ShowParameters(Camera camera)
        {
            eyeXtextBox.Text = camera.Eye.X.ToString();
            eyeYtextBox.Text = camera.Eye.Y.ToString();
            eyeZtextBox.Text = camera.Eye.Z.ToString();
            eyeAtXtextBox.Text = camera.At.X.ToString();
            eyeAtYtextBox.Text = camera.At.Y.ToString();
            eyeAtZtextBox.Text = camera.At.Z.ToString();
            eyeUpXtextBox.Text = camera.Up.X.ToString();
            eyeUpYtextBox.Text = camera.Up.Y.ToString();
            eyeUpZtextBox.Text = camera.Up.Z.ToString();
            fovtextBox.Text = camera.Fov.ToString();
            aspecttextBox.Text = camera.AspectRatio.ToString();
            nearCliptextBox.Text = camera.NearClip.ToString();
            farCliptextBox.Text = camera.FarClip.ToString();
        }
    }
}
