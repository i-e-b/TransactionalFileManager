#pragma warning disable 169 // ReSharper disable CheckNamespace, InconsistentNaming
using System.IO;
using System.Transactions;
using Machine.Specifications;

namespace WritingText {
	class When_writing_text_to_a_file : with.a_non_existent_file {
		Because of = () => subject.AppendAllText(file_name, contents);
		
		It should_write_contents_to_new_file = () => File.ReadAllText(file_name).ShouldEqual(contents);
		Behaves_like<cleans_up_temporary_files> it_cleans_up_files;
	}
	
	class When_writing_text_to_an_open_file : with.a_file_already_open_file
	{
		Because it = ShouldFail(() => subject.AppendAllText(file_name, contents));

		It should_throw_an__IOException__ =()=> the_exception.ShouldBeOfType<IOException>();
		It should_give_message__The_process_cannot_access_the_file__ 
			=()=> the_exception.Message.ShouldStartWith("The process cannot access the file ");
	}

	class When_writing_to_a_file_and_rolling_back : with.a_non_existent_file
	{
		Because I_start_a_transaction_and_don_t_commit_it =()=> {
            using (var tr1 = new TransactionScope())
                subject.AppendAllText(file_name, contents);
		};

		It should_not_have_written_file =()=> File.Exists(file_name).ShouldBeFalse();
	}

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

	class When_creating_an_empty_file : with.a_non_existent_file
	{
		Because I_create_a_blank_file =()=> subject.Create(file_name).Close();
		It should_create_a_blank_file =()=> File.Exists(file_name).ShouldBeTrue();
	}

	#region contexts
	namespace with {
		[Subject("with a non existent file")]
		public class a_non_existent_file : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected static string file_name;

			Establish context = () => {
				file_name = subject.GetTempFileName();
			};
		}

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

		[Subject("with a file already open")]
		public class a_file_already_open_file : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected static string file_name;
			protected static FileStream fs;

			Establish context = () => {
				file_name = subject.GetTempFileName();
				fs = File.Open(file_name, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
			};

			Cleanup file_stream =()=> fs.Close();
		}
	}
	#endregion
}
