#pragma warning disable 169 // ReSharper disable InconsistentNaming, CheckNamespace
using System.IO;
using System.Transactions;
using Machine.Specifications;
using Scaffold;

namespace CopyingFiles {

	class When_copying_a_file_inside_a_transaction : with.a_source_path_and_a_destination_path
	{
		Because I_copy_the_source_file_then_complete_the_transaction = () =>
		{
			using (var new_scope = new TransactionScope())
			{
				File.WriteAllText(source_file_name, contents);
				File.Copy(source_file_name, destination_file_name);
				new_scope.Complete();
			}
		};

		It should_have_source_file =()=> File.Exists(source_file_name).ShouldBeTrue();
		It should_have_destination_file =()=> File.Exists(source_file_name).ShouldBeTrue();
		It should_have_same_content_in_source_and_destination_files =()=>File.ReadAllText(source_file_name).ShouldEqual(File.ReadAllText(destination_file_name));
		
		It should_clean_up_files_in_outer_transaction =()=> {
			scope1.Dispose();
			File.Exists(source_file_name).ShouldBeFalse();
			File.Exists(destination_file_name).ShouldBeFalse();
		};
	}

	class When_copying_a_file_then_rolling_back_the_transaction : with.a_source_path_and_a_destination_path
	{
		Because I_copy_the_source_file_then_rollback_the_transaction = () =>
		{
			using (new TransactionScope())
			{
				File.WriteAllText(source_file_name, contents);
				File.Copy(source_file_name, destination_file_name);
			}
		};
		
		It should_not_have_any_source_file =()=> File.Exists(source_file_name).ShouldBeFalse();
		It should_not_have_any_destination_file =()=> File.Exists(source_file_name).ShouldBeFalse();
	}

	#region contexts
	namespace with {
		[Subject("with an existing file and a new target path")]
		public class a_source_path_and_a_destination_path : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected static string source_file_name;
			protected static string destination_file_name;

			Establish context = () => { 
				source_file_name = subject.GetTempFileName();
				destination_file_name = subject.GetTempFileName();
			};
		}
	}
	#endregion
}
