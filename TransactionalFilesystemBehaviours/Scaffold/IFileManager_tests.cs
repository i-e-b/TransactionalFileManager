#pragma warning disable 169 // ReSharper disable CheckNamespace, InconsistentNaming
using System.IO;
using System.IO.Transactions;
using System.Transactions;
using Machine.Specifications;

namespace WritingText.with
{
	public class IFileManager_tests : ContextOf<IFileManager> 
	{
		protected static TransactionScope scope1;
		protected static int old_temp_file_count;

		Establish context = () => {
		                          	subject = new TxFileManager();
		                          	scope1 = new TransactionScope();
		                          	old_temp_file_count = TempFileCount;
		};

		Cleanup transactions =()=> scope1.Dispose();

		public static int TempFileCount
		{
			get
			{
				if (!Directory.Exists(FileUtils.TempFolder)) return 0;
				return Directory.GetFiles(FileUtils.TempFolder).Length;
			}
		}
	}
}