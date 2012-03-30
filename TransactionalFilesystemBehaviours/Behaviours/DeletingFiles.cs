#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System.IO;
using System.Transactions;
using Machine.Specifications;
using WritingText;

namespace DeletingFiles {
	class When_deleting_a_file : with.an_existing_file {
		Because I_delete_the_file_and_complete_the_transaction =()=> {
			using (var t = new TransactionScope())
			{
				subject.Delete(file_name);
				t.Complete();
			}
		};

		It should_no_longer_exist =()=>File.Exists(file_name).ShouldBeFalse();
	}

	class When_deleting_and_rolling_back : with.an_existing_file
	{
		Because I_delete_the_file_then_rollback_the_transaction =()=> {
			using (new TransactionScope(TransactionScopeOption.RequiresNew))
			{
				subject.Delete(file_name);
			}
		};

		It should_still_exist =()=> File.Exists(file_name).ShouldBeTrue();
		It should_have_the_same_content_as_before =()=> File.ReadAllText(file_name).ShouldEqual(contents);

		It should_clean_up_files_when_outer_transaction_is_disposed =()=> {
			scope1.Dispose();
			File.Exists(file_name).ShouldBeFalse();
		};
	}

	#region contexts
	namespace with {
		[Subject("with a existing file")]
		public class an_existing_file : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected static string file_name;

			Establish context = () =>
			{
				file_name = subject.GetTempFileName();
				subject.WriteAllText(file_name, contents);
			};
		}
	}
	#endregion
}
