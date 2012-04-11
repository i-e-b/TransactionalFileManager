#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System.IO;
using System.Transactions;
using Machine.Specifications;
using Scaffold;

namespace MovingFiles {
	class When_moving_a_file : with.an_existing_file_and_a_new_non_existant_path {
		Because I_move_the_file_to_a_new_location =()=> subject.Move(file_name, new_file_name);

		It should_no_longer_exist_in_the_original_location =()=> File.Exists(file_name).ShouldBeFalse();
		It should_exist_in_the_new_location =()=> File.Exists(new_file_name).ShouldBeTrue();
		It should_have_the_same_contents_as_before =()=> File.ReadAllText(new_file_name).ShouldEqual(contents);
	}

	class When_I_move_a_file_and_rollback : with.an_existing_file_and_a_new_non_existant_path
	{
		Because I_move_the_file_then_rollback_the_transaction =()=> {
			using (new TransactionScope(TransactionScopeOption.RequiresNew))
				subject.Move(file_name, new_file_name);
		};

		It should_still_exist_in_the_original_location =()=> File.Exists(file_name).ShouldBeTrue();
		It should_not_exist_in_the_new_location =()=> File.Exists(new_file_name).ShouldBeFalse();
		It should_have_the_same_contents_as_before =()=> File.ReadAllText(file_name).ShouldEqual(contents);
	}

	class When_I_move_a_file_and_complete_the_transaction : with.an_existing_file_and_a_new_non_existant_path
	{
		Because I_move_the_file_and_complete_the_transaction =()=> {
			using (var t = new TransactionScope())
			{
				subject.Move(file_name, new_file_name);
				t.Complete();
			}
		};

		
		It should_move_the_file =()=> File.ReadAllText(new_file_name).ShouldEqual(contents);
		It should_clean_up_files_after_outer_transaction_is_rolled_back =()=> {
			scope1.Dispose();
			File.Exists(file_name).ShouldBeFalse();
			File.Exists(new_file_name).ShouldBeFalse();
		};
	}

	#region contexts
	namespace with {
		[Subject("with a existing file and a new non-existant path")]
		public class an_existing_file_and_a_new_non_existant_path : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected static string file_name;
			protected static string new_file_name;

			Establish context = () =>
			{
				file_name = subject.GetTempFileName();
				new_file_name = subject.GetTempFileName();
				subject.WriteAllText(file_name, contents);
			};
		}
	}
	#endregion
}
