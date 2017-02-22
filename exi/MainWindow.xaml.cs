using EXI.Annotations;
using ExiLibary;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace EXI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string OryginalXML
        {
            get
            {
                return _oryginalXML;
            }
            set
            {
                if (value != _oryginalXML)
                {
                    _oryginalXML = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CommpressedXML
        {
            get
            {
                return _compressedXML;
            }
            set
            {
                if (value != _compressedXML)
                {
                    _compressedXML = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DecompressedXML
        {
            get
            {
                return _decompressedXML;
            }
            set
            {
                if (value != _decompressedXML)
                {
                    _decompressedXML = value;
                    OnPropertyChanged();
                }
            }
        }

        public long CompressedSize
        {
            get
            {
                return _compressedSize;
            }
            set
            {
                if (value != _compressedSize)
                {
                    _compressedSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public long OryginalSize
        {
            get
            {
                return _oryginalSize;
            }
            set
            {
                if (value != _oryginalSize)
                {
                    _oryginalSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ProcentOfCompression
        {
            get
            {
                return _procentOfCompression;
            }
            set
            {
                if (value != _procentOfCompression)
                {
                    _procentOfCompression = value;
                    OnPropertyChanged();
                }
            }
        }

        private Code _code;
        private Decode _decode;

        private OpenFileDialog _openFileDialog;
        private SaveFileDialog _saveFileDialog;
        private string _selectedFileName;
        private string _readedXmlFile;

        private string _oryginalXML;
        private string _compressedXML;
        private string _decompressedXML;
        private string _procentOfCompression;
        private long _oryginalSize;
        private long _compressedSize;

        public MainWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            _openFileDialog = new OpenFileDialog()
            {
                Filter = Properties.Resources.FileDialogArgs
            };

            if (_openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ResetValuesOfSize();
                DecompressedXML = string.Empty;
                _selectedFileName = _openFileDialog.FileName;

                OryginalSize = new FileInfo(_selectedFileName).Length;

                if (_selectedFileName.Contains(".exi"))
                {
                    CommpressedXML = File.ReadAllText(_selectedFileName);
                    ButtonBase_OnClick(this, null);

                    return;
                }

                _readedXmlFile = File.ReadAllText(_selectedFileName);

                OryginalXML = _readedXmlFile;

                _code = new Code(_readedXmlFile);

                string commpressedxml = string.Empty;

                await Task.Run(() =>
                 {
                     commpressedxml = _code.CodeToExi();
                 });

                CommpressedXML = commpressedxml;
            }
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CommpressedXML))
            {
                MessageBox.Show(Properties.Resources.ErrorMessageBoxCaptionOnEmptyCommpressText, Properties.Resources.ErrorMessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _decode = new Decode(CommpressedXML);

            string tempDecompressionXml = string.Empty;

            await Task.Run(() =>
             {
                 tempDecompressionXml = _decode.DecodeToXml();
             });

            DecompressedXML = tempDecompressionXml;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CommpressedXML))
            {
                MessageBox.Show(Properties.Resources.ErrorMessageBoxCaptionOnEmptyCommpressText, Properties.Resources.ErrorMessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _saveFileDialog = new SaveFileDialog()
            {
                Filter = Properties.Resources.SaveFileDialogArgs
            };

            if (_saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var fileNameToSave = _saveFileDialog.FileName;

                File.WriteAllText(fileNameToSave, CommpressedXML);

                CompressedSize = new FileInfo(fileNameToSave).Length;

                ProcentOfCompression = string.Format("{0:F1}%", (100 - ((double)CompressedSize / (double)OryginalSize) * 100));
            }
        }

        private void ResetValuesOfSize()
        {
            ProcentOfCompression = string.Empty;
            CompressedSize = OryginalSize = 0;
        }
    }
}