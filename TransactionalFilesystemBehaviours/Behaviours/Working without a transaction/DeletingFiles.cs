#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System.IO;
using System.IO.Transactions;
using Machine.Specifications;

namespace DeletingFilesWithoutATransaction {
	class When_checking_for_the_existing_file : with.an_existing_file
	{
		It should_exist =()=> File.Exists(path).ShouldBeTrue();
	}

	class When_deleting_a_file : with.an_existing_file {
		Because of = () => subject.Delete(path);

		It should_not_exist = () => File.Exists(path).ShouldBeFalse();
	}

	#region contexts
	namespace with {
		[Subject("with an existing file")]
		public abstract class an_existing_file : ContextOf<IFileManager> {
			protected static string path;

			Establish context = () =>
			{
				subject = new TxFileManager();
				path = subject.GetTempFileName();
				File.WriteAllText(path, "abc");
			};

			Cleanup file =()=> {
				if (File.Exists(path)) File.Delete(path);
			};
		}
	}
	#endregion
}
