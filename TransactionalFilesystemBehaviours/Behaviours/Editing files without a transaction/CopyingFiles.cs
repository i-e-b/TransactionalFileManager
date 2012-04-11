#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System.IO;
using System.IO.Transactions;
using Machine.Specifications;

namespace CopyingFilesWithoutATransaction {
	class When_copying_a_file : with.a_file_manager_and_no_open_transaction {
		Because of = () => subject.Copy(source_file_name, destination_file_name, true);

		It should_create_the_destination_file_with_the_same_content_as_source = () => 
			File.ReadAllText(destination_file_name).ShouldEqual(File.ReadAllText(source_file_name));
	}

	#region contexts
	namespace with {
		[Subject("with a file manager and no open transaction")]
		public abstract class a_file_manager_and_no_open_transaction : ContextOf<IFileManager>  {
			protected static string source_file_name;
			protected static string destination_file_name;
			protected static string contents = "abc123"; 

			Establish context = () =>
			{
				subject = new TxFileManager();
				source_file_name = subject.GetTempFileName();
				destination_file_name = subject.GetTempFileName();
				File.WriteAllText(source_file_name, contents);
			};

			Cleanup files =()=> {
				File.Delete(source_file_name);
				File.Delete(destination_file_name);
			};
		}
	}
	#endregion
}
