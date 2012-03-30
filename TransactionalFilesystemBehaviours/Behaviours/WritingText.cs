#pragma warning disable 169 // ReSharper disable CheckNamespace, InconsistentNaming
using System.IO;
using System.Text;
using System.Transactions;
using Machine.Specifications;

namespace WritingText {
	class When_appending_text_to_a_file : with.a_non_existent_file {
		Because of = () => subject.AppendAllText(file_name, contents);
		
		It should_write_contents_to_new_file = () => File.ReadAllText(file_name).ShouldEqual(contents);
		It should_clean_up_files_when_disposing_transaction =()=> {
			scope1.Dispose();
			File.Exists(file_name).ShouldBeFalse();
		};
	}

	class When_appending_to_a_file_and_rolling_back : with.a_non_existent_file
	{
		Because I_start_a_transaction_and_don_t_commit_it =()=> {
            using (new TransactionScope()) subject.AppendAllText(file_name, contents);
		};

		It should_not_have_written_file =()=> File.Exists(file_name).ShouldBeFalse();
	}
	
	class When_writing_all_text_to_a_file : with.a_non_existent_file {
		Because of = () => subject.WriteAllText(file_name, contents);
		
		It should_write_contents_to_new_file = () => File.ReadAllText(file_name).ShouldEqual(contents);
		It should_clean_up_files_when_disposing_transaction =()=> {
			scope1.Dispose();
			File.Exists(file_name).ShouldBeFalse();
		};
	}

	class When_writing_all_text_to_a_file_and_rolling_back : with.a_non_existent_file
	{
		Because I_start_a_transaction_and_don_t_commit_it =()=> {
            using (new TransactionScope()) subject.WriteAllText(file_name, contents);
		};

		It should_not_have_written_file =()=> File.Exists(file_name).ShouldBeFalse();
	}

	class When_creating_an_empty_file : with.a_non_existent_file
	{
		Because I_create_a_blank_file =()=> subject.Create(file_name).Close();
		It should_create_a_blank_file =()=> File.Exists(file_name).ShouldBeTrue();
	}

	class When_writing_to_a_created_an_empty_file : with.a_non_existent_file
	{
		Because I_create_a_blank_file = () =>
		{
			using (var fs = subject.Create(file_name))
			{
				WriteStringToFile(fs, contents);
			}
		};


		It should_create_a_blank_file = () => File.Exists(file_name).ShouldBeTrue();
		It should_have_the_contents_I_wrote =()=> File.ReadAllText(file_name).ShouldEqual(contents);
	}
	
	class When_appending_text_to_an_open_file : with.a_file_already_open_file
	{
		Because it = ShouldFail(() => subject.AppendAllText(file_name, contents));

		It should_throw_an__IOException__ =()=> the_exception.ShouldBeOfType<IOException>();
		It should_give_message__The_process_cannot_access_the_file__ 
			=()=> the_exception.Message.ShouldStartWith("The process cannot access the file ");
	}

	class When_appending_to_an_existing_file : with.a_source_path_and_additional_content
	{
		Because I_append_text_to_the_file =()=> subject.AppendAllText(file_name, new_contents);

		It should_have_old_content_then_new_content =()=> File.ReadAllText(file_name).ShouldEqual(contents + new_contents);
	}
	
	class When_appending_to_an_existing_file_and_rolling_back : with.a_source_path_and_additional_content
	{
		Because I_append_text_to_the_file =()=> {
		    using (new TransactionScope(TransactionScopeOption.RequiresNew))
				subject.AppendAllText(file_name, new_contents);
		};

		It should_have_only_original_content =()=> File.ReadAllText(file_name).ShouldEqual(contents);
	}
	
	class When_writing_all_text_to_an_existing_file : with.a_source_path_and_additional_content
	{
		Because I_append_text_to_the_file =()=> subject.WriteAllText(file_name, new_contents);

		It should_have_only_new_content =()=> File.ReadAllText(file_name).ShouldEqual(new_contents);
	}
	
	class When_writing_all_text_to_an_existing_file_and_rolling_back : with.a_source_path_and_additional_content
	{
		Because I_append_text_to_the_file =()=> {
		    using (new TransactionScope(TransactionScopeOption.RequiresNew))
				subject.WriteAllText(file_name, new_contents);
		};

		It should_have_only_original_content =()=> File.ReadAllText(file_name).ShouldEqual(contents);
	}

	#region contexts
	namespace with
	{
		[Subject("with a non existent file")]
		public class a_non_existent_file : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected static string file_name;

			Establish context = () =>
			{
				file_name = subject.GetTempFileName();
			};

			public static void WriteStringToFile(FileStream fs, string s)
			{
				var bytes = Encoding.UTF8.GetBytes(s);
				fs.Write(bytes, 0, bytes.Length);
			}
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

		[Subject("with an existing file and additional content")]
		public class a_source_path_and_additional_content : an_open_IFileManager_transaction
		{
			protected const string contents = "123";
			protected const string new_contents = "123";
			protected static string file_name;

			Establish context = () => { 
				file_name = subject.GetTempFileName();
				subject.WriteAllText(file_name, contents);
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
