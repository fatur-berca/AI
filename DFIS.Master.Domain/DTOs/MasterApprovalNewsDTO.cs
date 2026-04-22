using System;

namespace DFIS.Master.Domain.DTOs
{
    public class MasterApprovalNewsDTO
    {
        private string _linkImage = "";
        private string _namaImage = "";
        private string _linkFile = "";
        private string _namaFile = "";

        public int IDNewsHighlight { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string FileUpload { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string Remarks { get; set; }
        public int Click { get; set; }

        public string LinkImage
        {
            get
            {
                if(Image.Length > 0)
                    _linkImage = Image.Substring(2);
                return _linkImage;
            }
            set { _linkImage = Image; }
        }

        public string NamaImage
        {
            get
            {
                string[] tokens = Image.Split('/');
                _namaImage = tokens[tokens.Length-1];
                string[] tokens2 = _namaImage.Split('.');
                _namaImage = tokens2[0];
                return _namaImage;
            }
            set { _namaImage = Image; }
        }

        public string LinkFile
        {
            get
            {
                if (FileUpload.Length > 0)
                    _linkFile = FileUpload.Substring(2);
                return _linkFile;
            }
            set { _linkFile = FileUpload; }
        }

        public string NamaFile
        {
            get
            {
                string[] tokens = FileUpload.Split('/');
                _namaFile = tokens[tokens.Length - 1];
                string[] tokens2 = _namaFile.Split('.');
                _namaFile = tokens2[0];
                return _namaFile;
            }
            set { _namaFile = FileUpload; }
        }
    }
}
