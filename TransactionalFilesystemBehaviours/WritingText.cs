#pragma warning disable 169 // ReSharper disable CheckNamespace, InconsistentNaming
using System.IO;
using Machine.Specifications;

namespace WritingText {
	class When_writing_text_to_a_file : with.a_non_existent_file {
		Because of = () => subject.AppendAllText(file_name, contents);
		
		It should_write_contents_to_new_file = () => File.ReadAllText(file_name).ShouldEqual(contents);

		It should_clean_up_temporary_files = () => {
			scope1.Dispose();
			TempFileCount.ShouldEqual(old_temp_file_count);
		};
	}

	class When_writing_text_to_an_open_file : with.a_file_already_open_file
	{
		Because it = ShouldFail(() => subject.AppendAllText(file_name, contents));

		It should_throw_an__IOException__ =()=> the_exception.ShouldBeOfType<IOException>();
		It should_give_message__The_process_cannot_access_the_file__ 
			=()=> the_exception.Message.ShouldStartWith("The process cannot access the file ");
	}

	#region contexts
	namespace with {
		[Subject("with a non existent file")]
		public class a_non_existent_file : IFileManager_tests
		{
			protected const string contents = "123";
			protected static string file_name;

			Establish context = () => {
				file_name = subject.GetTempFileName();
			};
		}

		[Subject("with a file already open")]
		public class a_file_already_open_file : IFileManager_tests
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
