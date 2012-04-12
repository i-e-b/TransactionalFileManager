#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System.IO;
using System.Transactions;
using Machine.Specifications;
using Scaffold;

namespace Snapshots {
	class When_creating_a_file_from_outside_the_manager_and_rolling_back : with.a_path_to_a_non_existent_file {
		Because I_snapshot_a_file_name_then_create_the_file_then_roll_back =()=> {
			using (new TransactionScope())
			{
				subject.Snapshot(file_name);
				File.WriteAllText(file_name, contents);
			}
		};

		It should_not_exist =()=> File.Exists(file_name).ShouldBeFalse();
		It should_clean_up_temporary_files =()=> TempFileCount.ShouldEqual(old_temp_file_count);
	}

	class When_deleting_a_file_from_outside_the_manager_and_rolling_back : with.an_existing_file_and_some_new_content
	{
		Because I_snapshot_the_file_then_delete_it_then_roll_back =()=> {
			using (new TransactionScope(TransactionScopeOption.RequiresNew))
			{
				subject.Snapshot(file_name);
				File.Delete(file_name);
			}
		};

		It should_still_exist =()=> File.Exists(file_name).ShouldBeTrue();
		It should_clean_up_temporary_file_when_outer_transaction_is_rolled_back =()=> {
			scope1.Dispose();
			TempFileCount.ShouldEqual(old_temp_file_count);
		};
	}

	class When_changing_the_content_of_a_snapshot_file_and_rolling_back : with.an_existing_file_and_some_new_content
	{
		Because I_change_the_file_content_then_rollback_the_transaction =()=> {
			using (new TransactionScope(TransactionScopeOption.RequiresNew))
			{
				subject.Snapshot(file_name);
				File.WriteAllText(file_name, new_contents);
			}
		};

		It should_have_original_content =()=> File.ReadAllText(file_name).ShouldEqual(contents);
	}

	class When_changing_the_content_of_a_snapshot_file_and_completing : with.an_existing_file_and_some_new_content
	{
		Because I_change_the_content_then_complete_the_transaction =()=> {
			using (var t = new TransactionScope(TransactionScopeOption.RequiresNew))
			{
				subject.Snapshot(file_name);
				File.WriteAllText(file_name, new_contents);
				t.Complete();
			}
		};

		It should_have_new_contents =()=> File.ReadAllText(file_name).ShouldEqual(new_contents);
		It should_clean_up_temporary_file_when_outer_transaction_is_rolled_back =()=> {
			scope1.Dispose();
			TempFileCount.ShouldEqual(old_temp_file_count);
		};
	}

	class When_moving_a_snapshot_file_and_rolling_back : with.an_existing_file_and_a_new_path
	{
		Because I_move_the_file_then_roll_back_the_transaction =()=> {
			using (new TransactionScope(TransactionScopeOption.RequiresNew))
			{
				subject.Snapshot(file_name);
				File.Move(file_name, new_file_name);
			}
		};

		It should_have_the_original_file =()=> File.Exists(file_name).ShouldBeTrue();
		It should_have_the_original_content =()=> File.ReadAllText(file_name).ShouldEqual(contents);

		It leaves_the_new_file_in_place =()=> File.Exists(new_file_name).ShouldEqual(true);
		It should_watch_for_the_file_move_and_remove_the_new_file;
	}

	#region contexts
	namespace with {
		[Subject("with a path to a non existent file")]
		public class a_path_to_a_non_existent_file : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected static string file_name;

			Establish context = () => {
				file_name = subject.GetTempFileName();
			};
		}
		
		[Subject("with an existing file")]
		public class an_existing_file : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected static string file_name;

			Establish context = () => {
				file_name = subject.GetTempFileName();
				File.WriteAllText(file_name, contents);
			};
		}

		[Subject("with an existing file and a new path")]
		public class an_existing_file_and_a_new_path : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected static string file_name;
			protected static string new_file_name;

			Establish context = () => {
				file_name = subject.GetTempFileName();
				new_file_name = subject.GetTempFileName();
				File.WriteAllText(file_name, contents);
			};
		}

		[Subject("with an existing file and some new content")]
		public class an_existing_file_and_some_new_content : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected const string new_contents = "abc";
			protected static string file_name;

			Establish context = () => {
				file_name = subject.GetTempFileName();
				File.WriteAllText(file_name, contents);
			};
		}
	}
	#endregion
}
