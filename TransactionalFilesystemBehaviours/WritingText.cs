#pragma warning disable 169 // ReSharper disable CheckNamespace, InconsistentNaming
using System.IO;
using System.IO.Transactions;
using System.Transactions;
using Machine.Specifications;

namespace WritingText {
	class When_writing_text_to_a_file : with.a_non_existent_file {
		Because of = () => subject.AppendAllText(file_name, contents);
		
		It should_write_contents_to_new_file = () => File.ReadAllText(file_name).ShouldEqual(contents);
	}

	#region contexts
	namespace with {
		[Subject("with a non existent file")]
		public class a_non_existent_file : ContextOf<IFileManager> {
			protected const string contents = "123";
			protected static string file_name;
			protected static TransactionScope scope1;
			protected static int old_temp_file_count;

			Establish context = () => {
				subject = new TxFileManager();
				file_name = subject.GetTempFileName();

				scope1 = new TransactionScope();
				old_temp_file_count = TempFileCount;
			};

			It should_clean_up_temporary_files =()=> {
				scope1.Dispose();
				var new_temp_file_count = TempFileCount;
				new_temp_file_count.ShouldEqual(old_temp_file_count);
			};

			Cleanup transactions =()=> scope1.Dispose();

			private static int TempFileCount
			{
				get
				{
					if (!Directory.Exists(FileUtils.TempFolder)) return 0;
					return Directory.GetFiles(FileUtils.TempFolder).Length;
				}
			}
		}
	}
	#endregion
}



#pragma warning restore 169