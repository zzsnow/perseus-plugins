using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BaseLib.Param;
using BaseLib.Util;
using PerseusApi.Document;
using PerseusApi.Generic;
using PerseusApi.Matrix;

namespace PerseusPluginLib.Rearrange{
    public class ProcessTextColumns : IMatrixProcessing{
        public bool HasButton { get { return false; } }
		public Bitmap DisplayImage { get { return null; } }
        public string HelpDescription { get { return "Values in string columns can be manipulated according to a regular expression."; } }
        public DocumentType HelpDescriptionType { get { return DocumentType.PlainText; } }
        public string HelpOutput { get { return ""; } }
        public string[] HelpSupplTables { get { return new string[0]; } }
        public DocumentType HelpOutputType { get { return DocumentType.PlainText; } }
        public DocumentType[] HelpSupplTablesType { get { return new DocumentType[0]; } }
        public int NumSupplTables { get { return 0; } }
        public string Name { get { return "Process text column"; } }
        public string Heading { get { return "Rearrange"; } }
        public bool IsActive { get { return true; } }
        public float DisplayOrder { get { return 22; } }
        public string[] HelpDocuments { get { return new string[0]; } }
        public DocumentType[] HelpDocumentTypes { get { return new DocumentType[0]; } }
        public int NumDocuments { get { return 0; } }

        public int GetMaxThreads(Parameters parameters){
            return 1;
        }

        public void ProcessData(IMatrixData mdata, Parameters param, ref IMatrixData[] supplTables,
                                ref IDocumentData[] documents, ProcessInfo processInfo){
            string regexStr = param.GetStringParam("Regular expression").Value;
            Regex regex = new Regex(regexStr);
            int[] inds = param.GetMultiChoiceParam("Columns").Value;
            bool keepColumns = param.GetBoolParam("Keep original columns").Value;

            foreach (int col in inds){
                ProcessCol(mdata, regex, col, keepColumns);
            }
        }

        private static void ProcessCol(IMatrixData mdata, Regex regex, int col, bool keepColumns){
            string[] values = new string[mdata.RowCount];
            for (int row = 0; row < mdata.RowCount; row++){
                values[row] = regex.Match(mdata.StringColumns[col][row]).Groups[1].ToString();
            }
            if (keepColumns){
                mdata.AddStringColumn(mdata.StringColumnNames[col], null, values);
            }
            else{
                mdata.StringColumns[col] = values;
            }
        }

        public Parameters GetParameters(IMatrixData mdata, ref string errorString){
            return
                new Parameters(new Parameter[]{
                    new MultiChoiceParam("Columns"){Values = mdata.StringColumnNames},
                    new StringParam("Regular expression", "^([^;]+)"),
                    new BoolParam("Keep original columns", false)
                });
        }
    }
}