using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToKorean.IO
{
    internal class FileDialogHelper : IDisposable
    {

        // ==============================================================================
        // ENUM
        // ==============================================================================

        public enum EFilter
        {
            YML = 1,
            TXT = 2,
            JSON = 4,
            ALL = 1048,
        }


        // ==============================================================================
        // FIELD
        // ==============================================================================

        private bool disposedValue;

        private OpenFileDialog _dialog;


        // ==============================================================================
        // PROPERTY
        // ==============================================================================

        public bool MultiSelect { get; set; }

        public string? Title { get; set; }


        // ==============================================================================
        // CONSTRUCTOR
        // ==============================================================================

        public FileDialogHelper(EFilter filter)
        {
            _dialog = new OpenFileDialog()
            {
                Filter = GetFilter(filter),
                Multiselect = MultiSelect,
                Title = Title
            };
        }


        // ==============================================================================
        // METHOD
        // ==============================================================================

        public string GetFilePath()
        {
            if (_dialog.ShowDialog() == DialogResult.OK)
            {
                return _dialog.FileName;
            }

            return string.Empty;
        }

        private string GetFilter(EFilter filter)
        {
            StringBuilder sb = new StringBuilder();

            if ((filter & EFilter.YML) == EFilter.YML)
            {
                sb.Append(".yml|*.yml");
            }

            if ((filter & EFilter.TXT) == EFilter.TXT)
            {
                sb.Append("|.txt(Text Files)|*.txt");
            }

            if ((filter & EFilter.JSON) == EFilter.JSON)
            {
                sb.Append("|.json(Json Files)|*.json");
            }

            if ((filter & EFilter.ALL) == EFilter.ALL)
            {
                sb.Append("|*.*(All Files)|*.*");
            }

            return sb.ToString();
        }

        #region DISPOSABLE

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리형 상태(관리형 개체)를 삭제합니다.

                    _dialog?.Dispose();
                }

                // TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.
                disposedValue = true;
            }
        }

        // // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
        // ~FileDialogHelper()
        // {
        //     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
