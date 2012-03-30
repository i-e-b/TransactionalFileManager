#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System;
using System.IO;
using System.Transactions;
using Machine.Specifications;
using WritingText;

namespace CreatingDirectories {
	class When_I_get_a_new_temporary_directory : with.a_path_to_a_non_existent_directory
	{
		It should_not_already_exist =()=> Directory.Exists(path).ShouldBeFalse();
	}

	class When_creating_a_directory : with.a_path_to_a_non_existent_directory {
		Because I_create_a_new_directory_and_complete_the_transaction = () => {
			using (var t = new TransactionScope())
			{
				subject.CreateDirectory(path);
				t.Complete();
			}
		};

		It should_exist =()=> Directory.Exists(path).ShouldBeTrue();
		It should_clean_up_the_directory_when_rolling_back_outer_transaction =()=> {
			scope1.Dispose();
			Directory.Exists(path).ShouldBeFalse();
		};
	}

	class When_creating_a_directory_then_rolling_back_the_transaction : with.a_path_to_a_non_existent_directory
	{
		Because I_create_a_new_directory_then_roll_back_the_transaction =()=> {
			using (new TransactionScope()) subject.CreateDirectory(path);
		};

		It should_not_exist =()=> Directory.Exists(path).ShouldBeFalse();
	}

	class When_rolling_back_from_nesting_new_directories : with.a_path_to_a_non_existent_directory
	{
		Because I_create_a_nested_directory_then_roll_back_the_transaction =()=> {
			sub_path = Path.Combine(path, "sub-path");
			using (new TransactionScope()) subject.CreateDirectory(sub_path);
		};

		It should_not_have_the_child_directory =()=> Directory.Exists(sub_path).ShouldBeFalse();
		It should_not_have_the_parent_path =()=> Directory.Exists(path).ShouldBeFalse();
	}

	#region contexts
	namespace with {
		[Subject("with a directory path")]
		public abstract class a_path_to_a_non_existent_directory : an_open_IFileManager_transaction {
			protected static string path;
			protected static string sub_path;
			Establish context = () => {
				path = Path.Combine(subject.GetTempFileName(), Guid.NewGuid().ToString());
			};
		}
	}
	#endregion
}
