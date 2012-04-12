#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System.IO;
using System.IO.Transactions;
using Machine.Specifications;

namespace MovingFilesWithoutATransaction {
	class When_moving_existing_file_to_new_path : with.an_existing_source_file_and_a_destination_path {
		Because of = () => subject.Move(source_path, destination_path);

		It should_exist_in_destination_path =()=> File.Exists(destination_path).ShouldBeTrue();
		It should_not_exist_in_source_path =()=> File.Exists(source_path).ShouldBeFalse();
		It should_have_original_content =()=> File.ReadAllText(destination_path).ShouldEqual(contents);
	}

	#region contexts
	namespace with {
		[Subject("with an existing source file and a destination path")]
		public abstract class an_existing_source_file_and_a_destination_path : ContextOf<IFileManager>{
			protected static string source_path, destination_path;
			protected static string contents = "abc";

			Establish context = () =>
			{
				subject = new TxFileManager();
				source_path = subject.GetTempFileName();
				destination_path = subject.GetTempFileName();

				File.WriteAllText(source_path, contents);
			};

			Cleanup files =()=> {
				if (File.Exists(source_path)) File.Delete(source_path);
				if (File.Exists(destination_path)) File.Delete(destination_path);
			};
		}
	}
	#endregion
}
