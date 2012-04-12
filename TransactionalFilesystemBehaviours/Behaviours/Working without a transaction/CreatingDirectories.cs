#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System.IO;
using System.IO.Transactions;
using Machine.Specifications;

namespace CreatingDirectoriesWithoutATransaction {
	class When_I_get_a_new_temporary_directory : with.a_path_to_a_temporary_directory {
		It should_not_already_exist = () => Directory.Exists(path).ShouldBeFalse();
	}

	class When_creating_a_directory : with.a_path_to_a_temporary_directory
	{
		Because I_create_the_directory =()=> subject.CreateDirectory(path);

		It should_have_a_new_directory_at_the_requested_path =()=> Directory.Exists(path).ShouldBeTrue();
	}

	#region contexts
	namespace with {
		[Subject("with a path to a temporary directory")]
		public abstract class a_path_to_a_temporary_directory : ContextOf<IFileManager> {
			protected static string path;

			Establish context = () => {
				subject = new TxFileManager();
				path = subject.GetTempFileName();
			};

			Cleanup directory =()=> Directory.Delete(path, true);
		}
	}
	#endregion
}
